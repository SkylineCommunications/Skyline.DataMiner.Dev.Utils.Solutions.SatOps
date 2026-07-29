namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement.SlotPicker
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using SlcPropertiesIds = Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.SlcPropertiesIds;
	using PropertyInstance = Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.PropertyInstance;
	using PropertyValuesInstance = Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.PropertyValuesInstance;
	using PropertyValueSection = Skyline.DataMiner.SDM.SatOps.Common.DOM.Model.PropertyValueSection;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Profiles;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.Scheduling.Scripts.JobHandler.Enums;
	using ConfigurationContext = Skyline.DataMiner.Utils.MediaOps.Common.IOData.Workflows.Scripts.ConfigurationHandler.ConfigurationContext;
	using ConfigurationTarget = Skyline.DataMiner.Utils.MediaOps.Common.IOData.Workflows.Scripts.ConfigurationHandler.ConfigurationTarget;
	using EditConfigurationAction = Skyline.DataMiner.Utils.MediaOps.Common.IOData.Workflows.Scripts.ConfigurationHandler.EditConfigurationAction;
	using OrchestrationEvent = Skyline.DataMiner.Utils.MediaOps.Common.IOData.Scheduling.Scripts.JobHandler.OrchestrationEvent;
	using OrchestrationSettings = Skyline.DataMiner.Utils.MediaOps.Common.IOData.Scheduling.Scripts.JobHandler.OrchestrationSettings;
	using ProfileParameterValue = Skyline.DataMiner.Utils.MediaOps.Common.IOData.Scheduling.Scripts.JobHandler.ProfileParameterValue;
    using Skyline.DataMiner.SDM.SatOps.Automation.Helpers;
    using Skyline.DataMiner.SDM.SatOps.Common.API.Constants;
    using Skyline.DataMiner.Net;

    /// <summary>
    /// Provides reusable logic for reserving a frequency range on a job node via the Configuration Handler.
    /// Extracts and encapsulates the configuration action flow originally found in the
    /// <c>SAT-AS-Reserve Range On Booking</c> automation script.
    /// </summary>
    public class RangeReservationHelper
	{
		private readonly DomHelper workflowHelper;
		private readonly DomHelper propertiesHelper;
		private readonly TransponderProfileParameterHelper transponderProfileParameterHelper;
		private readonly IEngine engine;

		/// <summary>
		/// Initializes the legacy Configuration Handler range reservation helper.
		/// </summary>
		/// <param name="engine">Automation engine.</param>
		public RangeReservationHelper(IEngine engine)
		{
			this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
			workflowHelper = new DomHelper(engine.SendSLNetMessages, SlcWorkflowIds.ModuleId);
			propertiesHelper = new DomHelper(engine.SendSLNetMessages, SlcPropertiesIds.ModuleId);
			transponderProfileParameterHelper = new TransponderProfileParameterHelper(workflowHelper, new ProfileHelper(engine.SendSLNetMessages));
		}

		/// <summary>
		/// Initializes a read-only instance of the range reservation helper backed by an SLNet
		/// <see cref="IConnection"/> instead of an automation engine. Intended for callers without an
		/// <see cref="IEngine"/> (for example GQI data sources) that only need the read methods
		/// (<see cref="GetReservedRange"/>, <see cref="GetSlotName"/>, <see cref="GetFirstNodeId"/>).
		/// The write methods (range reservation and slot property storage) require the
		/// <see cref="RangeReservationHelper(IEngine)"/> constructor and are not supported on an instance
		/// created this way.
		/// </summary>
		/// <param name="connection">The SLNet connection used to read the DOM and profile modules.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="connection"/> is <c>null</c>.</exception>
		public RangeReservationHelper(IConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			workflowHelper = new DomHelper(connection.HandleMessages, SlcWorkflowIds.ModuleId);
			propertiesHelper = new DomHelper(connection.HandleMessages, SlcPropertiesIds.ModuleId);
			transponderProfileParameterHelper = new TransponderProfileParameterHelper(workflowHelper, new ProfileHelper(connection.HandleMessages));
		}

		/// <summary>
		/// Reserves a frequency range on the specified job node by building and sending an
		/// <see cref="EditConfigurationAction"/> to the Configuration Handler.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <param name="nodeId">The node identifier within the job to configure.</param>
		/// <param name="startFrequency">The relative start frequency to reserve.</param>
		/// <param name="endFrequency">The relative end frequency to reserve.</param>
		public void ReserveRange(Guid jobId, string nodeId, decimal startFrequency, decimal endFrequency)
		{
			var job = RetrieveJob(jobId);
			EnsureJobIsMutable(job);
			var transponderNodeSection = FindNodeSectionByNodeId(job, nodeId);
			var nodeConfiguration = transponderProfileParameterHelper.GetNodeConfiguration(transponderNodeSection);

			var nodeConfigInstance = transponderProfileParameterHelper.GetNodeConfigInstance(nodeConfiguration);

			var profileParameterSections = nodeConfigInstance.Sections
				.Where(x => x.SectionDefinitionID.Equals(SlcWorkflowIds.Sections.ProfileParameterValues.Id))
				.ToList();

			var overriddenValues = BuildOverriddenValues(profileParameterSections, transponderProfileParameterHelper);

			var transponderBandwidthParameter = transponderProfileParameterHelper.GetProfileParameter(NamingConstants.TransponderBandwidthCapacityName);

			AddFrequencyRangeValues(overriddenValues, transponderBandwidthParameter.ID, startFrequency, endFrequency);

			var events = new Dictionary<OrchestrationEventType, OrchestrationEvent>();

			var editConfigAction = CreateEditConfigurationAction(jobId, nodeId, nodeConfiguration, overriddenValues, events);
			editConfigAction.SendToConfigurationHandler(engine);
		}

		/// <summary>
		/// Reserves a frequency range on the specified job node by building and sending an
		/// <see cref="EditConfigurationAction"/> to the Configuration Handler.
		/// Satellite capability and bandwidth size are written directly from the supplied values,
		/// so this overload is safe to use immediately after a <c>SwapNodeAction</c> when the
		/// new node configuration instance has no pre-existing profile parameter sections.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <param name="nodeId">The node identifier within the job to configure.</param>
		/// <param name="startFrequency">The relative start frequency to reserve.</param>
		/// <param name="endFrequency">The relative end frequency to reserve.</param>
		/// <param name="satelliteName">The satellite capability discrete value, or <c>null</c> to skip setting it.</param>
		public void ReserveRange(Guid jobId, string nodeId, decimal startFrequency, decimal endFrequency, string satelliteName)
		{
			var job = RetrieveJob(jobId);
			EnsureJobIsMutable(job);
			var transponderNodeSection = FindNodeSectionByNodeId(job, nodeId);
			var nodeConfiguration = transponderProfileParameterHelper.GetNodeConfiguration(transponderNodeSection);

			var bandwidthSize = endFrequency - startFrequency;
			var overriddenValues = BuildOverriddenValuesFromInputs(transponderProfileParameterHelper, bandwidthSize, satelliteName);

			var transponderBandwidthParameter = transponderProfileParameterHelper.GetProfileParameter(NamingConstants.TransponderBandwidthCapacityName);
			AddFrequencyRangeValues(overriddenValues, transponderBandwidthParameter.ID, startFrequency, endFrequency);

			var events = new Dictionary<OrchestrationEventType, OrchestrationEvent>();
			var editConfigAction = CreateEditConfigurationAction(jobId, nodeId, nodeConfiguration, overriddenValues, events);
			editConfigAction.SendToConfigurationHandler(engine);
		}

		/// <summary>
		/// Adds or updates the slot name property in the properties DOM for the specified job and node.
		/// If a property value already exists for this job and node it is updated in-place; otherwise a new instance is created.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <param name="nodeId">The node identifier within the job.</param>
		/// <param name="slotName">The slot name value to store.</param>
		public void AddSlotNameProperty(Guid jobId, string nodeId, string slotName)
		{
			var jobIdString = Convert.ToString(jobId);

			var propertyInfoInstance = propertiesHelper.DomInstances
				.Read(DomInstanceExposers.DomDefinitionId.Equal(SlcPropertiesIds.Definitions.Property.Id))
				.Select(di => new PropertyInstance(di))
				.FirstOrDefault(pi => pi.PropertyInfo.Name == NamingConstants.PropertyInfoName);

			var propertyValueInstance = propertiesHelper.DomInstances
				.Read(DomInstanceExposers.DomDefinitionId.Equal(SlcPropertiesIds.Definitions.PropertyValues.Id))
				.Select(di => new PropertyValuesInstance(di))
				.FirstOrDefault(pvi => pvi.PropertyValueInfo.LinkedObjectID == jobIdString
					&& pvi.PropertyValue.Any(pv => pv.PropertyName == NamingConstants.PropertyInfoName));

			if (propertyValueInstance != null)
			{
				var existingEntry = propertyValueInstance.PropertyValue.FirstOrDefault(pv => pv.PropertyName == NamingConstants.PropertyInfoName);

				if (existingEntry != null)
				{
					existingEntry.Value = slotName;
					propertyValueInstance.Save(propertiesHelper);
					return;
				}
			}

			var propertyValue = new PropertyValuesInstance
			{
				PropertyValueInfo =
				{
					LinkedObjectID = jobIdString,
					Scope = NamingConstants.PropertyInfoScope,
					SubID = nodeId,
				},
			};

			propertyValue.PropertyValue.Add(new PropertyValueSection
			{
				PropertyName = NamingConstants.PropertyInfoName,
				Value = slotName,
				PropertyID = propertyInfoInstance?.ID.Id ?? Guid.Empty,
			});

			propertyValue.Save(propertiesHelper);
		}

		/// <summary>
		/// Retrieves the first node identifier from an existing job.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <returns>The node identifier string of the first node in the job.</returns>
		public string GetFirstNodeId(Guid jobId)
		{
			var job = RetrieveJob(jobId);

			var firstNode = job.Sections.Where(section => section.SectionDefinitionID.Equals(SlcWorkflowIds.Sections.Nodes.Id)).FirstOrDefault();
			if (firstNode == null)
			{
				throw new InvalidOperationException("Job does not contain any node sections.");
			}

			var nodeIdValue = firstNode.GetFieldValueById(SlcWorkflowIds.Sections.Nodes.NodeID)?.Value;

			if (nodeIdValue == null)
			{
				throw new InvalidOperationException("First node section does not contain a Node ID.");
			}

			return nodeIdValue.ToString();
		}

		/// <summary>
		/// Ensures the specified job is not in a Running or Completed state before it is mutated.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <exception cref="InvalidOperationException">Thrown when the job is Running or Completed.</exception>
		public void EnsureJobMutable(Guid jobId)
		{
			var job = RetrieveJob(jobId);
			EnsureJobIsMutable(job);
		}

		/// <summary>
		/// Reads the configured start and end time of the specified job node from the workflow module.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <param name="nodeId">The node identifier within the job.</param>
		/// <returns>A tuple containing the node start and end time.</returns>
		public (DateTime Start, DateTime End) GetNodeTiming(Guid jobId, string nodeId)
		{
			var job = RetrieveJob(jobId);
			var section = FindNodeSectionByNodeId(job, nodeId);

			var start = ReadNodeDateTime(section, SlcWorkflowIds.Sections.Nodes.NodeStartTime, "start", nodeId);
			var end = ReadNodeDateTime(section, SlcWorkflowIds.Sections.Nodes.NodeEndTime, "end", nodeId);

			return (start, end);
		}

		/// <summary>
		/// Builds the next job name placeholder from the workflow AppSettings, mirroring the legacy
		/// Job Reservation dialog: the configured job ID prefix followed by a run of <c>X</c> placeholder
		/// characters, one per configured minimum digit.
		/// </summary>
		/// <returns>The next job name placeholder (for example <c>JOB-XXXX</c>).</returns>
		/// <exception cref="InvalidOperationException">Thrown when the AppSettings DOM instance is not found.</exception>
		public string GetNextJobName()
		{
			var appSettingsFilter = DomInstanceExposers.DomDefinitionId.Equal(SlcWorkflowIds.Definitions.AppSettings.Id);
			var appSettings = workflowHelper.DomInstances.Read(appSettingsFilter).FirstOrDefault()
				?? throw new InvalidOperationException("Workflow AppSettings DOM instance was not found. Unable to generate the next job name.");

			var prefix = appSettings.GetFieldValue<string>(SlcWorkflowIds.Sections.JobSettings.Id, SlcWorkflowIds.Sections.JobSettings.JobIDPrefix)?.Value ?? string.Empty;
			var minimumDigits = appSettings.GetFieldValue<long>(SlcWorkflowIds.Sections.JobSettings.Id, SlcWorkflowIds.Sections.JobSettings.JobIDMinimumDigits)?.Value ?? 0L;

			int placeholderLength;
			if (minimumDigits <= 0)
			{
				placeholderLength = 0;
			}
			else if (minimumDigits > int.MaxValue)
			{
				placeholderLength = int.MaxValue;
			}
			else
			{
				placeholderLength = (int)minimumDigits;
			}

			return string.Concat(prefix, new string('X', placeholderLength));
		}

		/// <summary>
		/// Reads the slot name property stored for the specified job and node, or <c>null</c> when none exists.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <param name="nodeId">The node identifier within the job.</param>
		/// <returns>The stored slot name, or <c>null</c> when not set.</returns>
		public string GetSlotName(Guid jobId, string nodeId)
		{
			var jobIdString = Convert.ToString(jobId);

			var propertyValueInstance = propertiesHelper.DomInstances
				.Read(DomInstanceExposers.DomDefinitionId.Equal(SlcPropertiesIds.Definitions.PropertyValues.Id))
				.Select(di => new PropertyValuesInstance(di))
				.FirstOrDefault(pvi => pvi.PropertyValueInfo.LinkedObjectID == jobIdString
					&& pvi.PropertyValue.Any(pv => pv.PropertyName == NamingConstants.PropertyInfoName));

			var entry = propertyValueInstance?.PropertyValue.FirstOrDefault(pv => pv.PropertyName == NamingConstants.PropertyInfoName);

			return entry?.Value;
		}

		/// <summary>
		/// Reads the reserved relative frequency range (transponder bandwidth min/max) stored on the specified
		/// job node's configuration, or <c>null</c> when it has not been reserved.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <param name="nodeId">The node identifier within the job.</param>
		/// <returns>A tuple with the reserved start and end frequency, or <c>null</c> when not reserved.</returns>
		public (decimal Start, decimal End)? GetReservedRange(Guid jobId, string nodeId)
		{
			var job = RetrieveJob(jobId);
			var transponderNodeSection = FindNodeSectionByNodeId(job, nodeId);
			var nodeConfiguration = transponderProfileParameterHelper.GetNodeConfiguration(transponderNodeSection);
			var nodeConfigInstance = transponderProfileParameterHelper.GetNodeConfigInstance(nodeConfiguration);

			var profileParameterSections = nodeConfigInstance.Sections
				.Where(x => x.SectionDefinitionID.Equals(SlcWorkflowIds.Sections.ProfileParameterValues.Id))
				.ToList();

			var transponderBandwidthParameter = transponderProfileParameterHelper.GetProfileParameter(NamingConstants.TransponderBandwidthCapacityName);
			var parameterSection = transponderProfileParameterHelper.FindParameterSection(profileParameterSections, transponderBandwidthParameter.ID);
			if (parameterSection == null)
			{
				return null;
			}

			var min = parameterSection.GetValue<double>(SlcWorkflowIds.Sections.ProfileParameterValues.DoubleMinValue)?.Value;
			var max = parameterSection.GetValue<double>(SlcWorkflowIds.Sections.ProfileParameterValues.DoubleMaxValue)?.Value;

			if (min == null || max == null)
			{
				return null;
			}

			return ((decimal)min.Value, (decimal)max.Value);
		}

		private static DateTime ReadNodeDateTime(Section section, FieldDescriptorID fieldId, string label, string nodeId)
		{
			var wrapper = section.GetValue<DateTime>(fieldId)
				?? throw new InvalidOperationException($"Node {label} time is not configured for node '{nodeId}'. Please ensure the node has a valid {label} time.");

			return wrapper.Value;
		}

		/// <summary>
		/// Creates a configured <see cref="EditConfigurationAction"/> targeting the specified job node
		/// with the provided overridden parameter values and orchestration events.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <param name="nodeId">The node identifier within the job.</param>
		/// <param name="nodeConfiguration">The GUID of the DOM node configuration to be edited.</param>
		/// <param name="overriddenValues">A dictionary mapping profile parameter IDs to their new values.</param>
		/// <param name="events">A dictionary of orchestration events.</param>
		/// <returns>A fully populated <see cref="EditConfigurationAction"/>.</returns>
		private static EditConfigurationAction CreateEditConfigurationAction(
			Guid jobId,
			string nodeId,
			Guid nodeConfiguration,
			Dictionary<Guid, ProfileParameterValue> overriddenValues,
			Dictionary<OrchestrationEventType, OrchestrationEvent> events)
		{
			return new EditConfigurationAction
			{
				Context = new ConfigurationContext
				{
					Target = ConfigurationTarget.JobNode,
					DomInstanceId = jobId,
					NodeId = nodeId,
				},
				DomConfigurationId = nodeConfiguration,
				OrchestrationSettings = new OrchestrationSettings(overriddenValues, events),
			};
		}

		/// <summary>
		/// Adds or replaces a frequency range profile parameter entry in the supplied dictionary.
		/// </summary>
		/// <param name="overriddenValues">The dictionary to populate or update.</param>
		/// <param name="profileParameterId">The profile parameter ID representing transponder bandwidth capacity.</param>
		/// <param name="startFrequency">The start frequency value.</param>
		/// <param name="endFrequency">The end frequency value.</param>
		private static void AddFrequencyRangeValues(
			Dictionary<Guid, ProfileParameterValue> overriddenValues,
			Guid profileParameterId,
			decimal startFrequency,
			decimal endFrequency)
		{
			overriddenValues[profileParameterId] = new ProfileParameterValue
			{
				ProfileParameterId = profileParameterId,
				DoubleMinValue = (double)startFrequency,
				DoubleMaxValue = (double)endFrequency,
			};
		}

		/// <summary>
		/// Builds the dictionary of overridden profile parameter values based on the provided profile parameter sections.
		/// Ensures that commonly required parameters (bandwidth size and satellite) are included if present.
		/// </summary>
		/// <param name="profileParameterSections">List of profile parameter sections from the node configuration DOM instance.</param>
		/// <param name="transponderProfileParameterHelper">Helper used to locate profile parameter definitions and sections.</param>
		/// <returns>A dictionary mapping profile parameter GUIDs to their override values.</returns>
		private static Dictionary<Guid, ProfileParameterValue> BuildOverriddenValues(
			List<Section> profileParameterSections,
			TransponderProfileParameterHelper transponderProfileParameterHelper)
		{
			var overriddenValues = new Dictionary<Guid, ProfileParameterValue>();

			AddBandwidthSizeParameter(profileParameterSections, transponderProfileParameterHelper, overriddenValues);
			AddSatelliteParameter(profileParameterSections, transponderProfileParameterHelper, overriddenValues);

			return overriddenValues;
		}

		/// <summary>
		/// Builds the dictionary of overridden profile parameter values directly from the provided inputs,
		/// without reading from existing DOM sections. Use this when the node config instance is fresh
		/// (e.g. immediately after job creation or a <c>SwapNodeAction</c>) and has no pre-existing sections.
		/// </summary>
		/// <param name="transponderProfileParameterHelper">Helper used to locate profile parameter definitions.</param>
		/// <param name="bandwidthSize">The bandwidth size value (end frequency minus start frequency).</param>
		/// <param name="satelliteName">The satellite capability discrete value, or <c>null</c> to skip setting it.</param>
		/// <returns>A dictionary mapping profile parameter GUIDs to their override values.</returns>
		private static Dictionary<Guid, ProfileParameterValue> BuildOverriddenValuesFromInputs(
			TransponderProfileParameterHelper transponderProfileParameterHelper,
			decimal bandwidthSize,
			string satelliteName)
		{
			var overriddenValues = new Dictionary<Guid, ProfileParameterValue>();

			var bandwidthSizeParameter = transponderProfileParameterHelper.GetProfileParameter(NamingConstants.BandwidthSizeParameterName);
			overriddenValues[bandwidthSizeParameter.ID] = new ProfileParameterValue
			{
				ProfileParameterId = bandwidthSizeParameter.ID,
				DoubleMaxValue = (double)bandwidthSize,
			};

			if (!string.IsNullOrWhiteSpace(satelliteName))
			{
				var satelliteParameter = transponderProfileParameterHelper.GetProfileParameter(NamingConstants.SatelliteCapabilityName);
				overriddenValues[satelliteParameter.ID] = new ProfileParameterValue
				{
					ProfileParameterId = satelliteParameter.ID,
					StringValue = satelliteName,
				};
			}

			return overriddenValues;
		}

		/// <summary>
		/// Reads the satellite capability value from the profile parameter sections (if available) and adds it
		/// to the overridden values dictionary. If the parameter section is not present, the method returns
		/// without modifying the dictionary.
		/// </summary>
		/// <param name="profileParameterSections">The list of profile parameter sections to search.</param>
		/// <param name="transponderProfileParameterHelper">Helper used to read the satellite profile parameter.</param>
		/// <param name="overriddenValues">The dictionary to populate with the satellite parameter override.</param>
		private static void AddSatelliteParameter(
			List<Section> profileParameterSections,
			TransponderProfileParameterHelper transponderProfileParameterHelper,
			Dictionary<Guid, ProfileParameterValue> overriddenValues)
		{
			var satelliteParameter = transponderProfileParameterHelper.GetProfileParameter(NamingConstants.SatelliteCapabilityName);
			var parameterSection = transponderProfileParameterHelper.FindParameterSection(profileParameterSections, satelliteParameter.ID);

			if (parameterSection == null)
			{
				return;
			}

			var parameterValue = parameterSection
				.GetFieldValueById(SlcWorkflowIds.Sections.ProfileParameterValues.StringValue)
				?.Value?.ToString();

			overriddenValues[satelliteParameter.ID] = new ProfileParameterValue
			{
				ProfileParameterId = satelliteParameter.ID,
				StringValue = parameterValue,
			};
		}

		/// <summary>
		/// Reads the bandwidth size parameter value from the provided profile parameter sections and adds it
		/// to the overridden values dictionary. If the parameter section or value is missing, a default of 0.0 is used.
		/// </summary>
		/// <param name="profileParameterSections">The list of profile parameter sections to search.</param>
		/// <param name="transponderProfileParameterHelper">Helper used to read the bandwidth-size profile parameter.</param>
		/// <param name="overriddenValues">The dictionary to populate with the bandwidth size override.</param>
		private static void AddBandwidthSizeParameter(
			List<Section> profileParameterSections,
			TransponderProfileParameterHelper transponderProfileParameterHelper,
			Dictionary<Guid, ProfileParameterValue> overriddenValues)
		{
			var bandwidthSizeParameter = transponderProfileParameterHelper.GetProfileParameter(NamingConstants.BandwidthSizeParameterName);
			var parameterSection = transponderProfileParameterHelper.FindParameterSection(profileParameterSections, bandwidthSizeParameter.ID);

			if (parameterSection == null)
			{
				return;
			}

			var parameterField = parameterSection.GetFieldValueById(SlcWorkflowIds.Sections.ProfileParameterValues.DoubleMaxValue);
			var parameterValue = parameterField?.Value != null
				? Convert.ToDouble(parameterField.Value.ToString())
				: 0.0;

			overriddenValues[bandwidthSizeParameter.ID] = new ProfileParameterValue
			{
				ProfileParameterId = bandwidthSizeParameter.ID,
				DoubleMaxValue = parameterValue,
			};
		}
		private DomInstance RetrieveJob(Guid jobId)
		{
			var job = workflowHelper.DomInstances.Read(DomInstanceExposers.Id.Equal(jobId)).FirstOrDefault();
			return job ?? throw new InvalidOperationException($"Job instance with ID '{jobId}' not found in the workflow module.");
		}

		private static void EnsureJobIsMutable(DomInstance job)
		{
			if (job.StatusId == SlcWorkflowIds.Behaviors.Job_Behavior.Statuses.Running || job.StatusId == SlcWorkflowIds.Behaviors.Job_Behavior.Statuses.Completed)
			{
				throw new InvalidOperationException("Resource swap is not allowed when the Job is in Running or Completed state.");
			}
		}

		private static Section FindNodeSectionByNodeId(DomInstance job, string nodeId)
		{
			var node = job.Sections.Where(section => section.SectionDefinitionID.Equals(SlcWorkflowIds.Sections.Nodes.Id)).FirstOrDefault(section => section.GetFieldValueById(SlcWorkflowIds.Sections.Nodes.NodeID)?.Value?.ToString() == nodeId);
			return node ?? throw new InvalidOperationException($"Job node '{nodeId}' was not found.");
		}

	}
}

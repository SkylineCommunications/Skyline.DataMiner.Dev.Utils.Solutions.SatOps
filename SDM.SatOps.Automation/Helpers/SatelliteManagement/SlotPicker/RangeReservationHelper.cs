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

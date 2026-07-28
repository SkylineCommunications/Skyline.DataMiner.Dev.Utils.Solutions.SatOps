namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement.SlotPicker
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Jobs;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Profiles;
	using Skyline.DataMiner.Net.Sections;
    using Skyline.DataMiner.SDM.SatOps.Automation.Helpers;
    using Skyline.DataMiner.Utils.SatOps.Common.Constants;
	
	using Parameter = Skyline.DataMiner.Net.Profiles.Parameter;

	/// <summary>
	/// Helper class for retrieving transponder-related profile parameters and node configuration data.
	/// </summary>
	public class TransponderProfileParameterHelper
	{
		/// <summary>
		/// DOM helper used to access workflow DOM instances and related data.
		/// </summary>
		private readonly DomHelper workflowHelper;

		/// <summary>
		/// Profile helper used to read profile parameters.
		/// </summary>
		private readonly ProfileHelper profileHelper;

		/// <summary>
		/// Initializes a new instance of the <see cref="TransponderProfileParameterHelper"/> class.
		/// </summary>
		/// <param name="workflowHelper">The DOM helper for workflow DOM instances.</param>
		/// <param name="profileHelper">The profile helper for retrieving profile parameters.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="workflowHelper"/> or <paramref name="profileHelper"/> is null.</exception>
		public TransponderProfileParameterHelper(DomHelper workflowHelper, ProfileHelper profileHelper)
		{
			this.workflowHelper = workflowHelper ?? throw new ArgumentNullException(nameof(workflowHelper));
			this.profileHelper = profileHelper ?? throw new ArgumentNullException(nameof(profileHelper));
		}		

		/// <summary>
		/// Retrieves the node configuration GUID from the provided transponder node section.
		/// </summary>
		/// <param name="transponderNodeSection">The node section that contains the node configuration field.</param>
		/// <returns>The node configuration as a <see cref="Guid"/>.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the configuration field is missing or not a valid GUID.</exception>
		public Guid GetNodeConfiguration(Section transponderNodeSection)
		{
			var nodeConfigValue = transponderNodeSection.GetFieldValueById(SlcWorkflowIds.Sections.Nodes.NodeConfiguration)?.Value;
			if (nodeConfigValue == null)
			{
				throw new InvalidOperationException("Node Configuration is null in transponder node section");
			}

			if (!Guid.TryParse(nodeConfigValue.ToString(), out Guid nodeConfiguration))
			{
				throw new InvalidOperationException($"Invalid Node Configuration GUID format: {nodeConfigValue}");
			}

			return nodeConfiguration;
		}

		/// <summary>
		/// Reads the DOM instance that corresponds to the provided node configuration GUID.
		/// </summary>
		/// <param name="nodeConfiguration">The node configuration GUID.</param>
		/// <returns>The <see cref="DomInstance"/> for the given configuration.</returns>
		/// <exception cref="InvalidOperationException">Thrown when no DOM instance is found for the given GUID.</exception>
		public DomInstance GetNodeConfigInstance(Guid nodeConfiguration)
		{
			var filter = DomInstanceExposers.Id.Equal(nodeConfiguration);
			var nodeConfigInstance = workflowHelper.DomInstances
				.Read(filter)
				.SingleOrDefault();

			if (nodeConfigInstance == null)
			{
				throw new InvalidOperationException($"Node configuration instance not found for ID: {nodeConfiguration}");
			}

			return nodeConfigInstance;
		}

		/// <summary>
		/// Reads a profile parameter by its name using the profile helper.
		/// </summary>
		/// <param name="parameterName">The name of the parameter to find.</param>
		/// <returns>The matching <see cref="Parameter"/>.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the profile parameter cannot be found.</exception>
		public Parameter GetProfileParameter(string parameterName)
		{
			var parameter = profileHelper.ProfileParameters
				.Read(ParameterExposers.Name.Equal(parameterName))
				.FirstOrDefault();

			if (parameter == null)
			{
				throw new InvalidOperationException($"Profile parameter '{parameterName}' not found");
			}

			return parameter;
		}

		/// <summary>
		/// Finds the profile parameter section that matches the given parameter id within the supplied sections.
		/// </summary>
		/// <param name="profileParameterSections">The list of profile parameter sections to search.</param>
		/// <param name="parameterId">The GUID of the profile parameter to find.</param>
		/// <returns>The matching <see cref="Section"/> or null if none match.</returns>
		public Section FindParameterSection(List<Section> profileParameterSections, Guid parameterId)
		{
			return profileParameterSections.FirstOrDefault(section =>
			{
				var sectionParameterId = section
					.GetFieldValueById(SlcWorkflowIds.Sections.ProfileParameterValues.ProfileParameterID)
					?.Value?.ToString();

				return sectionParameterId == parameterId.ToString();
			});
		}		
	}
}

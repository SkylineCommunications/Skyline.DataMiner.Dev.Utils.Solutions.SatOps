namespace Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement.Scheduling
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.SDM.SatOps.Common.API.Constants;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.Scheduling.Scripts.JobHandler;
	using Skyline.DataMiner.Utils.MediaOps.Common.IOData.Scheduling.Scripts.JobHandler.Objects;
	using Skyline.DataMiner.Utils.SatOps.Common.Helpers.SatelliteManagement.SlotPicker;

	/// <summary>
	/// Orchestrates the creation and update of transponder job reservations against the Job Handler,
	/// then reserves the requested frequency range and stores the slot name property via the
	/// <see cref="RangeReservationHelper"/>.
	/// Encapsulates the flow that previously lived in the Job Reservation dialog presenter, so a caller
	/// can perform an entire reservation with a single method call.
	/// </summary>
	public class TransponderJobReservationHelper
	{
		private readonly IEngine engine;
		private readonly RangeReservationHelper rangeReservationHelper;

		/// <summary>
		/// Initializes a new instance of the <see cref="TransponderJobReservationHelper"/> class.
		/// </summary>
		/// <param name="engine">The automation engine.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="engine"/> is <c>null</c>.</exception>
		public TransponderJobReservationHelper(IEngine engine)
		{
			this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
			rangeReservationHelper = new RangeReservationHelper(engine);
		}

		/// <summary>
		/// Creates a new transponder reservation.
		/// Builds a <see cref="CreateJobAction"/> in <see cref="DesiredJobStatus.Tentative"/> with a transponder
		/// node, sends it to the Job Handler, then reserves the frequency range and stores the slot name property.
		/// </summary>
		/// <param name="request">The reservation input values.</param>
		/// <returns>The unique identifier of the created job.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <c>null</c>.</exception>
		public Guid CreateReservation(JobReservationRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			var createAction = new CreateJobAction
			{
				Name = request.Name,
				Start = request.StartTime,
				End = request.EndTime,
				DesiredJobStatus = DesiredJobStatus.Tentative,
			};

			createAction.Nodes.Add(BuildTransponderNode(request));

			var output = createAction.SendToJobHandler(engine);
			var createOutput = (CreateJobActionOutput)output.ActionOutput;

			var jobId = createOutput.DomJobId;
			var nodeId = createOutput.AddedNodeIds[0];

			ApplyRangeAndSlot(jobId, nodeId, request);

			return jobId;
		}

		/// <summary>
		/// Updates an existing transponder reservation.
		/// Sends an <see cref="EditJobAction"/> to update the job metadata, swaps the transponder node via a
		/// <see cref="SwapNodeAction"/>, then reserves the frequency range and stores the slot name property.
		/// </summary>
		/// <param name="request">The reservation input values. <see cref="JobReservationRequest.JobId"/> identifies the job to update.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <c>null</c>.</exception>
		public void UpdateReservation(JobReservationRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			var jobId = request.JobId;

			var editAction = new EditJobAction
			{
				DomJobId = jobId,
				Name = request.Name,
				Start = request.StartTime,
				End = request.EndTime,
			};

			editAction.SendToJobHandler(engine);

			var nodeId = rangeReservationHelper.GetFirstNodeId(jobId);

			var swapAction = new SwapNodeAction
			{
				DomJobId = jobId,
				NodeId = nodeId,
				Node = BuildTransponderNode(request),
			};

			swapAction.SendToJobHandler(engine);

			ApplyRangeAndSlot(jobId, nodeId, request);
		}

		/// <summary>
		/// Reserves the requested frequency range on the node and stores the slot name property.
		/// </summary>
		/// <param name="jobId">The unique identifier of the job.</param>
		/// <param name="nodeId">The node identifier within the job.</param>
		/// <param name="request">The reservation input values.</param>
		private void ApplyRangeAndSlot(Guid jobId, string nodeId, JobReservationRequest request)
		{
			rangeReservationHelper.ReserveRange(jobId, nodeId, request.RelativeStartFrequency, request.RelativeEndFrequency, request.SatelliteName);
			rangeReservationHelper.AddSlotNameProperty(jobId, nodeId, request.SlotName);
		}

		/// <summary>
		/// Builds a transponder <see cref="JobNode"/> from the request, booking the requested transponder
		/// resource for the reservation window without booking its full capacity.
		/// </summary>
		/// <param name="request">The reservation input values.</param>
		/// <returns>A populated <see cref="JobNode"/>.</returns>
		private static JobNode BuildTransponderNode(JobReservationRequest request)
		{
			return new JobNode
			{
				DomResourceId = request.TransponderResourceId,
				DomResourcePoolId = SatelliteManagementConstant.TransponderResourcePoolGuid,
				Start = request.StartTime,
				End = request.EndTime,
				BookFullCapacity = false,
				NodeConfigFilter = BuildNodeConfigFilter(request),
			};
		}

		/// <summary>
		/// Builds a <see cref="NodeConfigFilter"/> containing the bandwidth size configuration filter and,
		/// when a satellite name is supplied, the satellite capability filter.
		/// </summary>
		/// <param name="request">The reservation input values.</param>
		/// <returns>A populated <see cref="NodeConfigFilter"/>.</returns>
		private static NodeConfigFilter BuildNodeConfigFilter(JobReservationRequest request)
		{
			var bandwidthSize = request.RelativeEndFrequency - request.RelativeStartFrequency;

			var nodeConfigFilter = new NodeConfigFilter
			{
				ConfigurationFilters = new Dictionary<Guid, NodeConfigFilter.ConfigurationFilter>
				{
					{
						SatelliteManagementConstant.BandwidthSizeGuid,
						new NodeConfigFilter.ConfigurationFilter
						{
							Id = SatelliteManagementConstant.BandwidthSizeGuid,
							Value = bandwidthSize,
						}
					},
				},
			};

			if (!string.IsNullOrWhiteSpace(request.SatelliteName))
			{
				nodeConfigFilter.CapabilityFilters = new Dictionary<Guid, NodeConfigFilter.CapabilityFilter>
				{
					{
						SatelliteManagementConstant.SatelliteCapabilityGuid,
						new NodeConfigFilter.CapabilityFilter
						{
							Id = SatelliteManagementConstant.SatelliteCapabilityGuid,
							Values = new List<string> { request.SatelliteName },
						}
					},
				};
			}

			return nodeConfigFilter;
		}
	}
}

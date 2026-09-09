namespace Skyline.DataMiner.Solutions.SatOps.Common.API.Middleware
{
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Objects.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.API.Repositories.SatelliteManagement.Transponder;
    using Skyline.DataMiner.Solutions.SatOps.Common.Logging;
    using SLDataGateway.API.Types.Querying;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Enforces that transponder names are globally unique, case-insensitively.
    /// </summary>
    /// <remarks>
    /// This middleware must be registered last so that it is the outermost one and therefore runs first.
    /// Rejecting a duplicate name before <see cref="TransponderResourceCreationMiddleware"/> runs prevents
    /// a resource from being created for a transponder that is never persisted.
    /// </remarks>
    internal sealed class TransponderNameUniquenessMiddleware : IBulkRepositoryMiddleware<Transponder>
    {
        private readonly ITransponderRepository transponderRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransponderNameUniquenessMiddleware"/> class.
        /// </summary>
        /// <param name="transponderRepository">
        /// The repository used to look up the already persisted transponders that carry one of the submitted names.
        /// Only the names of the batch being validated are requested, so the whole transponder set never has to be read.
        /// This must be the repository instance without this middleware applied, to avoid re-entering validation.
        /// </param>
        public TransponderNameUniquenessMiddleware(ITransponderRepository transponderRepository)
        {
            this.transponderRepository = transponderRepository ?? throw new ArgumentNullException(nameof(transponderRepository));
        }

        public Transponder OnCreate(Transponder oToCreate, Func<Transponder, Transponder> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateUniqueNames(new[] { oToCreate });
            return next(oToCreate);
        }

        public IReadOnlyCollection<Transponder> OnCreate(IEnumerable<Transponder> oToCreate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var transpondersToCreate = oToCreate.ToList();
            ValidateUniqueNames(transpondersToCreate);
            return next(transpondersToCreate);
        }

        public Transponder OnUpdate(Transponder oToUpdate, Func<Transponder, Transponder> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            ValidateUniqueNames(new[] { oToUpdate });
            return next(oToUpdate);
        }

        public IReadOnlyCollection<Transponder> OnUpdate(IEnumerable<Transponder> oToUpdate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var transpondersToUpdate = oToUpdate.ToList();
            ValidateUniqueNames(transpondersToUpdate);
            return next(transpondersToUpdate);
        }

        public IReadOnlyCollection<Transponder> OnCreateOrUpdate(IEnumerable<Transponder> oToCreateOrUpdate, Func<IEnumerable<Transponder>, IReadOnlyCollection<Transponder>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var transpondersToCreateOrUpdate = oToCreateOrUpdate.ToList();
            ValidateUniqueNames(transpondersToCreateOrUpdate);
            return next(transpondersToCreateOrUpdate);
        }

        public long OnCount(FilterElement<Transponder> filter, Func<FilterElement<Transponder>, long> next)
        {
            return next(filter);
        }

        public long OnCount(IQuery<Transponder> query, Func<IQuery<Transponder>, long> next)
        {
            return next(query);
        }

        public void OnDelete(IEnumerable<Transponder> oToDelete, Action<IEnumerable<Transponder>> next)
        {
            next(oToDelete);
        }

        public void OnDelete(Transponder oToDelete, Action<Transponder> next)
        {
            next(oToDelete);
        }

        public IEnumerable<Transponder> OnRead(FilterElement<Transponder> filter, Func<FilterElement<Transponder>, IEnumerable<Transponder>> next)
        {
            return next(filter);
        }

        public IEnumerable<Transponder> OnRead(IQuery<Transponder> query, Func<IQuery<Transponder>, IEnumerable<Transponder>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<Transponder>> OnReadPaged(FilterElement<Transponder> filter, Func<FilterElement<Transponder>, IEnumerable<IPagedResult<Transponder>>> next)
        {
            return next(filter);
        }

        public IEnumerable<IPagedResult<Transponder>> OnReadPaged(IQuery<Transponder> query, Func<IQuery<Transponder>, IEnumerable<IPagedResult<Transponder>>> next)
        {
            return next(query);
        }

        public IEnumerable<IPagedResult<Transponder>> OnReadPaged(FilterElement<Transponder> filter, int pageSize, Func<FilterElement<Transponder>, int, IEnumerable<IPagedResult<Transponder>>> next)
        {
            return next(filter, pageSize);
        }

        public IEnumerable<IPagedResult<Transponder>> OnReadPaged(IQuery<Transponder> query, int pageSize, Func<IQuery<Transponder>, int, IEnumerable<IPagedResult<Transponder>>> next)
        {
            return next(query, pageSize);
        }

        /// <summary>
        /// Validates that transponder names are unique, both within the supplied batch and against the already persisted transponders.
        /// </summary>
        /// <param name="transponders">The transponders that are being created and/or updated.</param>
        /// <exception cref="ArgumentException">Thrown when a name is used more than once.</exception>
        private void ValidateUniqueNames(IReadOnlyCollection<Transponder> transponders)
        {
            var namedTransponders = transponders
                .Where(transponder => transponder != null && !string.IsNullOrWhiteSpace(transponder.Name))
                .ToList();

            if (namedTransponders.Count == 0)
                return;

            var duplicateInBatch = namedTransponders
                .GroupBy(transponder => transponder.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(group => group.Select(transponder => transponder.Id).Distinct().Count() > 1);

            if (duplicateInBatch != null)
                throw new ArgumentException(string.Format(ExceptionMessages.DuplicateTransponderNameDetected, duplicateInBatch.Key));

            var namesInBatch = namedTransponders
                .Select(transponder => transponder.Name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingTransponders = transponderRepository.ReadByNames(namesInBatch)?.ToList();
            if (existingTransponders == null || existingTransponders.Count == 0)
                return;

            var idsInBatch = new HashSet<Guid>(transponders.Where(transponder => transponder != null).Select(transponder => transponder.Id));
            var takenNames = new HashSet<string>(
                existingTransponders
                    .Where(existing => existing != null && !idsInBatch.Contains(existing.Id) && !string.IsNullOrWhiteSpace(existing.Name))
                    .Select(existing => existing.Name.Trim()),
                StringComparer.OrdinalIgnoreCase);

            var conflictingTransponder = namedTransponders.FirstOrDefault(transponder => takenNames.Contains(transponder.Name.Trim()));

            if (conflictingTransponder != null)
                throw new ArgumentException(string.Format(ExceptionMessages.TransponderNameAlreadyExists, conflictingTransponder.Name), nameof(transponders));
        }
    }
}

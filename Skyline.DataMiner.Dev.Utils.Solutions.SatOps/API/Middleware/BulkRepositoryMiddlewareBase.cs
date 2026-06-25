namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM;
    using SLDataGateway.API.Types.Querying;

    internal abstract class BulkRepositoryMiddlewareBase<T> : IBulkRepositoryMiddleware<T>, ICreatableMiddleware<T>, IUpdatableMiddleware<T>
        where T : class
    {
        public virtual T OnCreate(T oToCreate, Func<T, T> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(oToCreate);
        }

        public virtual T OnUpdate(T oToUpdate, Func<T, T> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(oToUpdate);
        }

        public virtual IReadOnlyCollection<T> OnCreate(IEnumerable<T> oToCreate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(oToCreate);
        }

        public virtual IReadOnlyCollection<T> OnUpdate(IEnumerable<T> oToUpdate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(oToUpdate);
        }

        public virtual IReadOnlyCollection<T> OnCreateOrUpdate(IEnumerable<T> oToCreateOrUpdate, Func<IEnumerable<T>, IReadOnlyCollection<T>> next)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(oToCreateOrUpdate);
        }

        public virtual void OnDelete(T oToDelete, Action<T> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            next(oToDelete);
        }

        public virtual void OnDelete(IEnumerable<T> oToDelete, Action<IEnumerable<T>> next)
        {
            if (oToDelete == null)
                throw new ArgumentNullException(nameof(oToDelete));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            next(oToDelete);
        }

        public virtual long OnCount(FilterElement<T> filter, Func<FilterElement<T>, long> next)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(filter);
        }

        public virtual long OnCount(IQuery<T> query, Func<IQuery<T>, long> next)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(query);
        }

        public virtual IEnumerable<T> OnRead(FilterElement<T> filter, Func<FilterElement<T>, IEnumerable<T>> next)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(filter);
        }

        public virtual IEnumerable<T> OnRead(IQuery<T> query, Func<IQuery<T>, IEnumerable<T>> next)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(query);
        }

        public virtual IEnumerable<IPagedResult<T>> OnReadPaged(FilterElement<T> filter, Func<FilterElement<T>, IEnumerable<IPagedResult<T>>> next)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(filter);
        }

        public virtual IEnumerable<IPagedResult<T>> OnReadPaged(IQuery<T> query, Func<IQuery<T>, IEnumerable<IPagedResult<T>>> next)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(query);
        }

        public virtual IEnumerable<IPagedResult<T>> OnReadPaged(FilterElement<T> filter, int pageSize, Func<FilterElement<T>, int, IEnumerable<IPagedResult<T>>> next)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(filter, pageSize);
        }

        public virtual IEnumerable<IPagedResult<T>> OnReadPaged(IQuery<T> query, int pageSize, Func<IQuery<T>, int, IEnumerable<IPagedResult<T>>> next)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            return next(query, pageSize);
        }
    }
}

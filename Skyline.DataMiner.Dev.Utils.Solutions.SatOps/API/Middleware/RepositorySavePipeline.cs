namespace Skyline.DataMiner.SDM.SatOps.Common.API.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.SDM;

    internal sealed class RepositorySavePipeline<T>
        where T : class
    {
        private readonly IReadOnlyList<IBulkRepositoryMiddleware<T>> middlewares;

        public RepositorySavePipeline(IEnumerable<IBulkRepositoryMiddleware<T>> middlewares)
        {
            if (middlewares == null)
                throw new ArgumentNullException(nameof(middlewares));

            this.middlewares = middlewares.ToList();
        }

        public T ExecuteCreate(T oToCreate, Func<T, T> terminalAction)
        {
            if (oToCreate == null)
                throw new ArgumentNullException(nameof(oToCreate));

            if (terminalAction == null)
                throw new ArgumentNullException(nameof(terminalAction));

            Func<T, T> next = terminalAction;
            for (var index = middlewares.Count - 1; index >= 0; index--)
            {
                var middleware = middlewares[index];
                if (middleware == null)
                    continue;

                var creatableMiddleware = middleware as ICreatableMiddleware<T>;
                if (creatableMiddleware == null)
                    continue;

                var currentNext = next;
                next = value => creatableMiddleware.OnCreate(value, currentNext);
            }

            return next(oToCreate);
        }

        public T ExecuteUpdate(T oToUpdate, Func<T, T> terminalAction)
        {
            if (oToUpdate == null)
                throw new ArgumentNullException(nameof(oToUpdate));

            if (terminalAction == null)
                throw new ArgumentNullException(nameof(terminalAction));

            Func<T, T> next = terminalAction;
            for (var index = middlewares.Count - 1; index >= 0; index--)
            {
                var middleware = middlewares[index];
                if (middleware == null)
                    continue;

                var updatableMiddleware = middleware as IUpdatableMiddleware<T>;
                if (updatableMiddleware == null)
                    continue;

                var currentNext = next;
                next = value => updatableMiddleware.OnUpdate(value, currentNext);
            }

            return next(oToUpdate);
        }

        public IReadOnlyCollection<T> ExecuteCreateOrUpdate(IEnumerable<T> oToCreateOrUpdate, Func<IEnumerable<T>, IReadOnlyCollection<T>> terminalAction)
        {
            if (oToCreateOrUpdate == null)
                throw new ArgumentNullException(nameof(oToCreateOrUpdate));

            if (terminalAction == null)
                throw new ArgumentNullException(nameof(terminalAction));

            Func<IEnumerable<T>, IReadOnlyCollection<T>> next = terminalAction;
            for (var index = middlewares.Count - 1; index >= 0; index--)
            {
                var middleware = middlewares[index];
                if (middleware == null)
                    continue;

                var currentNext = next;
                next = values => middleware.OnCreateOrUpdate(values, currentNext);
            }

            return next(oToCreateOrUpdate);
        }
    }
}

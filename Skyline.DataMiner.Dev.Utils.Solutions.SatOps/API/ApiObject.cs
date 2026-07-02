namespace Skyline.DataMiner.SDM.SatOps.Common.API
{
    using Skyline.DataMiner.SDM.SatOps.Common.API.Storage.Repositories;
    using System;

    public abstract class ApiObject : IIdentifiable
    {
        private protected ApiObject()
            : this(Guid.NewGuid())
        {
        }

        private protected ApiObject(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The value cannot be an empty GUID.", nameof(id));
            }

            Id = id;
        }

        public Guid Id { get; private set; }
    }
}

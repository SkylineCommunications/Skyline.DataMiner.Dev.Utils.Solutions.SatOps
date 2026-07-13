namespace Skyline.DataMiner.SDM.SatOps.Common.API
{
    using Skyline.DataMiner.SDM.SatOps.Common.API.Repositories;
    using System;

    /// <summary>
    /// Represents the base class for API objects with a unique identifier.
    /// </summary>
    public abstract class ApiObject : IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiObject"/> class with a new unique identifier.
        /// </summary>
        private protected ApiObject()
            : this(Guid.NewGuid())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiObject"/> class with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier for the API object.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is an empty GUID.</exception>
        private protected ApiObject(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The value cannot be an empty GUID.", nameof(id));
            }

            Id = id;
        }

        /// <summary>
        /// Gets the unique identifier of the API object.
        /// </summary>
        /// <value>
        /// A <see cref="Guid"/> that uniquely identifies this API object.
        /// </value>
        public Guid Id { get; private set; }
    }
}


namespace Skyline.DataMiner.Solutions.SatOps.Common.API
{
    using System;

    /// <summary>
    /// Represents an API object that has a name.
    /// </summary>
    public abstract class ApiNamedObject : ApiObject
    {
        private protected ApiNamedObject()
            : base()
        { }

        private protected ApiNamedObject(Guid id)
            : base(id)
        {
        }

        /// <summary>
        /// Gets or sets the name of the API object.
        /// </summary>
        public abstract string Name { get; set; }
    }
}

namespace Skyline.DataMiner.SDM.SatOps.Common.API.Objects.SatelliteManagement
{
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.SDM.SatOps.Common.DOM.Model;
    using System;

    public class Satellite : ApiObject<Satellite>
    {
        private readonly SatellitesInstance _domInstance;

        public Satellite() : this(new SatellitesInstance())
        {
        }

        public Satellite(Guid id) : this(new SatellitesInstance(id))
        {
        }

        internal Satellite(SatellitesInstance domInstance) : base(domInstance)
        {
            _domInstance = domInstance ?? throw new ArgumentNullException(nameof(domInstance));
        }

        internal Satellite(DomInstance domInstance) : this(new SatellitesInstance(domInstance))
        { 
        }

        internal static DomDefinitionId DomDefinition => SlcSatellite_ManagementIds.Definitions.Satellites;

        public string Name
        {
            get
            {
                return _domInstance.General.SatelliteName;
            }

            set
            {
                _domInstance.General.SatelliteName = value;
            }
        }
    }
}

namespace Skyline.DataMiner.Solutions.SatOps.Common.Storage.DOM.Helpers
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;

    internal abstract class DomModuleHelperBase
    {
        protected DomModuleHelperBase(string moduleId, IConnection connection)
        {
            DomHelper = new DomHelper(connection.HandleMessages, moduleId);
        }

        public DomHelper DomHelper { get; }
    }
}

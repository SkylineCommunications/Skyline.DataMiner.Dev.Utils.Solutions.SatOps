namespace Skyline.DataMiner.SDM.SatOps.Automation.Helpers
{
    using System;
    using System.Reflection;

    internal static class LegacyWorkflowIds
    {
        private const string TypePrefix = "Skyline.DataMiner.Utils.MediaOps.DOM.Applications.DomIds.SlcWorkflow";
        private const string AssemblyName = "Skyline.DataMiner.Utils.MediaOps.Temp.Helpers";

        public static string ModuleId => (string)Get(null, "ModuleId");

        public static class Sections
        {
            public static class Nodes
            {
                public static dynamic Id => Get("Sections+Nodes", "Id");
                public static dynamic NodeConfiguration => Get("Sections+Nodes", "NodeConfiguration");
                public static dynamic NodeID => Get("Sections+Nodes", "NodeID");
            }

            public static class ProfileParameterValues
            {
                public static dynamic Id => Get("Sections+ProfileParameterValues", "Id");
                public static dynamic DoubleMaxValue => Get("Sections+ProfileParameterValues", "DoubleMaxValue");
                public static dynamic ProfileParameterID => Get("Sections+ProfileParameterValues", "ProfileParameterID");
                public static dynamic StringValue => Get("Sections+ProfileParameterValues", "StringValue");
            }
        }

        public static class Behaviors
        {
            public static class Job_Behavior
            {
                public static class Statuses
                {
                    public static dynamic Running => Get("Behaviors+Job_Behavior+Statuses", "Running");
                    public static dynamic Completed => Get("Behaviors+Job_Behavior+Statuses", "Completed");
                }
            }
        }

        private static object Get(string nestedType, string member)
        {
            var typeName = nestedType == null ? TypePrefix : TypePrefix + "+" + nestedType;
            var type = Type.GetType(typeName + ", " + AssemblyName, throwOnError: true);
            var field = type.GetField(member, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return field?.GetValue(null) ?? type.GetProperty(member, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null)
                ?? throw new InvalidOperationException($"Workflow identifier '{typeName}.{member}' was not found.");
        }
    }
}
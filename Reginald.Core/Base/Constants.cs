using System;

namespace Reginald.Core.Base
{
    public static class Constants
    {
        // XML

        public static string LastNodeXpath = @"//Searches//Namespace[position() = last()]";
        public static string NamespacesXpath = @"//Searches//Namespace";
        public static string NamespaceIDXpathFormat = "//Searches/Namespace[@ID='{0}']";
        public static string NamespaceNameXpathFormat = "//Searches/Namespace[@Name='{0}']";

        // Applications

        public static Guid ApplicationsGuid = new("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
    }
}

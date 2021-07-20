using System;

namespace Reginald.Core.Base
{
    public static class Constants
    {
        // XML

        public const string LastNodeXpath = @"//Searches//Namespace[position() = last()]";
        public const string NamespacesXpath = @"//Searches//Namespace";
        public const string NamespaceIDXpathFormat = "//Searches/Namespace[@ID='{0}']";
        public const string NamespaceNameXpathFormat = "//Searches/Namespace[@Name='{0}']";

        // Applications

        public static Guid ApplicationsGuid = new("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
    }
}

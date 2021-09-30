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
        public const string NamespaceNameRegexPattern = @"((?<!\w){0}.*)";

        public const string SettingsNamespacesXpath = @"//Settings//Namespace";
        public const string SettingsNamespaceNameXpathFormat = "//Settings/Namespace[@Name='{0}']";

        public const string ThemesXpath = "//Themes//Theme";

        // Applications

        public static Guid ApplicationsGuid { get; } = new("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
        public static Guid RecycleBinGuid { get; } = new("{B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC}");

        // Extensions

        public const string FactorialRegexPattern = @"(?<!\.\d*)(?<!-\d*)(\d+)!";

        // APIs

        public const string StyvioStockEpFormat = "https://www.styvio.com/api/{0}";
        public const string CloudflareEp = "https://cloudflare-quic.com/b/ip";

        // Assembly

        public const string AssemblyPath = "pack://application:,,,/Reginald;component/{0}";
    }
}

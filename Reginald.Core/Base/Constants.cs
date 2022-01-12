using System;

namespace Reginald.Core.Base
{
    public static class Constants
    {
        // GUIDs

        public static Guid WindowsScriptHostShellObjectGuid { get; } = new("72c24dd5-d70a-438b-8a42-98424b88afb8");

        // Applications

        public static string ShellAppsFolder { get; } = @"shell:AppsFolder\";

        // APIs

        public static string StyvioStockEpFormat { get; } = "https://www.styvio.com/api/{0}";

        public static string CloudflareEp { get; } = "https://cloudflare-quic.com/b/ip";

        // Regex

        public static string KeywordRegexFormat { get; } = @"^{0}";

        public static string ShellItemUppercaseRegexFormat { get; } = @"(?<!^){0}";

        public static string IsMathRegexPattern { get; } = @"^[0-9\s+-/^*()><!]+$";

        public static string InvalidDecimalRegexPattern { get; } = @"\d+\.\d+\.\d*";

        public static string MalformedExponentiationRegexPattern { get; } = @"(?<!\S)\^\d+";

        public static string DivideByZeroRegexPattern { get; } = @"\/\s*(\(?[0-9\s+-/^*()><!]*\)|0)";

        public static string FactorialRegexPattern { get; } = @"(?<!\.\d*)(?<!-\d*)(\d+)!";

        public static string ExponentiationRegexPattern { get; } = @"\d*\.?\d*\^-?\d+\.?\d*";

        public static string CommandTimerSecondRegexPattern { get; } = @"(?<!\S)(\d+(\.\d+)?) ?s((ec(ond)?s?)?)?(?!\S)";

        public static string CommandTimerMinuteRegexPattern { get; } = @"(?<!\S)(\d+(\.\d+)?) ?m((in(ute)?s?)?)?(?!\S)";

        public static string CommandTimerHourRegexPattern { get; } = @"(?<!\S)(\d+(\.\d+)?) ?h((ou)?rs?)?(?!\S)";

        public static string PreciseKeywordRegexFormat { get; } = @"^\b{0}\b";

        public static string KeyphraseRegexFormat { get; } = @"\b(?<!\S){0}";

        // Miscellaneous

        public static string CommandQuitDescriptorFormat { get; } = "Quit {0}";

        public static string TimersPreciseTerm { get; } = "timers";
    }
}

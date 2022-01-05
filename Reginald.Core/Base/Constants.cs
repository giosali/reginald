﻿using System;

namespace Reginald.Core.Base
{
    public static class Constants
    {
        // Applications

        public static Guid ApplicationsGuid { get; } = new("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");

        public static Guid RecycleBinGuid { get; } = new("{B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC}");

        public static string ShellAppsFolder { get; } = @"shell:AppsFolder\";

        public static string ApplicationCaption { get; } = "Application";

        // APIs

        public static string StyvioStockEpFormat { get; } = "https://www.styvio.com/api/{0}";

        public static string CloudflareEp { get; } = "https://cloudflare-quic.com/b/ip";

        // Regex

        public static string KeywordRegexFormat { get; } = @"^{0}";

        public static string ShellItemUppercaseRegexFormat { get; } = @"(?<!^){0}";

        //public static string ApplicationRegexFormat { get; } = @".*((?<![a-z]){0}.*)";

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

        public static string ConfirmationIconPath { get; } = "pack://application:,,,/Reginald;component/Images/Results/exclamation.png";

        public static string ConfirmationCaption { get; } = "Confirmation Required - This Action Cannot Be Undone";
    }
}

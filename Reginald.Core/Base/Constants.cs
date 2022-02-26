namespace Reginald.Core.Base
{
    public static class Constants
    {
        // APIs
        public static string StyvioStockEpFormat { get; } = "https://www.styvio.com/api/{0}";

        public static string CloudflareEp { get; } = "https://cloudflare-quic.com/b/ip";

        // Regex
        public static string IsMathRegexPattern { get; } = @"^[0-9\s+-/^*()><!]+$";

        public static string InvalidDecimalRegexPattern { get; } = @"\d+\.\d+\.\d*";

        public static string MalformedExponentiationRegexPattern { get; } = @"(?<!\S)\^\d+";

        public static string DivideByZeroRegexPattern { get; } = @"\/\s*(\(?[0-9\s+-/^*()><!]*\)|0)";

        public static string FactorialRegexPattern { get; } = @"(?<!\.\d*)(?<!-\d*)(\d+)!";

        public static string ExponentiationRegexPattern { get; } = @"\d*\.?\d*\^-?\d+\.?\d*";

        // Miscellaneous
        public static string TimersPreciseTerm { get; } = "timers";
    }
}

namespace Reginald.Data.Keywords
{
    using System.Linq;
    using Reginald.Data.DisplayItems;

    /// <summary>
    /// Specifies a unit of time.
    /// </summary>
    public enum TimeUnit
    {
        /// <summary>
        /// A unit of time specifying hours.
        /// </summary>
        Hour,

        /// <summary>
        /// A unit of time specifying minutes.
        /// </summary>
        Minute,

        /// <summary>
        /// A unit of time specifying seconds.
        /// </summary>
        Second,
    }

    public partial class TimerKeyword : CommandKeyword
    {
        private const string SecondRegexPattern = @"(?<!\S)(\d+(\.\d+)?) ?s((ec(ond)?s?)?)?(?!\S)";

        private const string MinuteRegexPattern = @"(?<!\S)(\d+(\.\d+)?) ?m((in(ute)?s?)?)?(?!\S)";

        private const string HourRegexPattern = @"(?<!\S)(\d+(\.\d+)?) ?h((ou)?rs?)?(?!\S)";

        public TimerKeyword()
        {
        }

        public TimerKeyword(CommandKeywordDataModel model, string input)
            : base(model)
        {
            if (input.Length > 0)
            {
                if (TryParseTimeFromString(input))
                {
                    CanReceiveKeyboardInput = true;
                }
            }
            else
            {
                Description = string.Format(Format, Placeholder, Placeholder);
                CanReceiveKeyboardInput = false;
            }
        }

        public double Time { get; set; }

        public override void EnterKeyDown()
        {
            if (!string.IsNullOrEmpty(Completion) && Time > 0)
            {
                TimerResult result = new(this);
                result.StartTimer();
            }
        }

        /// <summary>
        /// Takes a string and returns a boolean indicating whether or not it contains a time representation and/or a description.
        /// </summary>
        /// <param name="input">The input to evaluate.</param>
        /// <returns><see langword="true"/> if the input contains a time representation and/or a description; otherwise, <see langword="false"/>.</returns>
        public bool TryParseTimeFromString(string input)
        {
            double totalTime = 0;

            //// The currentIndex is used to prevent the following from being valid:
            //// `Take out the trash 30m 30s Do the laundry`
            //// Otherwise, the description would only be set to "Do the laundry" and
            //// "Take out the trash" would be completely ignored
            //// We need the time values to start at the very beginning of the input string
            int currentIndex = 0;
            int span = 0;
            string[] timeRepresentations = new string[3];
            string[] patterns = new string[3] { HourRegexPattern, MinuteRegexPattern, SecondRegexPattern };
            for (int i = 0; i < patterns.Length; i++)
            {
                if (TryGetTime(input, patterns[i], out double time, out int start, out int end))
                {
                    if (start == currentIndex)
                    {
                        currentIndex = end + 1;

                        // We add 1 to the index of the final character to account for the space proceeding it
                        span = end + 1;
                        totalTime += ToMilliseconds(time, (TimeUnit)i, out timeRepresentations[i]);
                    }
                }
            }

            if (totalTime > 0)
            {
                string title = span > input.Length ? Placeholder : input[span..];
                if (string.IsNullOrEmpty(title))
                {
                    title = Placeholder;
                }

                string representation = string.Join(' ', timeRepresentations.Where(r => !string.IsNullOrEmpty(r)));
                Completion = title;
                Description = string.Format(Format, representation, title);
                Time = totalTime;
                return true;
            }
            else
            {
                Description = null;
                return false;
            }
        }
    }
}

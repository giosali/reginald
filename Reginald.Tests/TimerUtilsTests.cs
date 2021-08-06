using Reginald.Core.Utils;
using System.Threading.Tasks;
using Xunit;

namespace Reginald.Tests
{
    public class TimerUtilsTests
    {
        [Theory]
        [InlineData("5 hours Take out trash", "In 5 hours: Take out trash", 5 * 60 * 60)]
        [InlineData("5 hour Take out trash", "In 5 hours: Take out trash", 5 * 60 * 60)]
        [InlineData("1 hour Take out trash", "In 1 hour: Take out trash", 1 * 60 * 60)]
        [InlineData("5 h Take out trash", "In 5 hours: Take out trash", 5 * 60 * 60)]        
        public static async Task ParseTimeFromStringAsync_HourShouldReturnCorrectValues(string expression, string expectedDescription, double? expectedSeconds)
        {
            string format = "In {0} {1}: {2}";
            string split = "3";
            string defaultText = "...";
            (string description, double? seconds) = await TimerUtils.ParseTimeFromStringAsync(expression, format, split, defaultText);

            Assert.Equal(expectedDescription, description);
            Assert.Equal(expectedSeconds, seconds);
        }

        [Theory]
        [InlineData("5 minutes Take out trash", "In 5 minutes: Take out trash", 5 * 60)]
        [InlineData("5 minute Take out trash", "In 5 minutes: Take out trash", 5 * 60)]
        [InlineData("5 mins Take out trash", "In 5 minutes: Take out trash", 5 * 60)]
        [InlineData("1 minute Take out trash", "In 1 minute: Take out trash", 1 * 60)]
        [InlineData("1 min Take out trash", "In 1 minute: Take out trash", 1 * 60)]
        [InlineData("1 m Take out trash", "In 1 minute: Take out trash", 1 * 60)]
        public static async Task ParseTimeFromStringAsync_MinuteShouldReturnCorrectValues(string expression, string expectedDescription, double? expectedSeconds)
        {
            string format = "In {0} {1}: {2}";
            string split = "3";
            string defaultText = "...";
            (string description, double? seconds) = await TimerUtils.ParseTimeFromStringAsync(expression, format, split, defaultText);

            Assert.Equal(expectedDescription, description);
            Assert.Equal(expectedSeconds, seconds);
        }

        [Theory]
        [InlineData("5 seconds Take out trash", "In 5 seconds: Take out trash", 5)]
        [InlineData("5 secs Take out trash", "In 5 seconds: Take out trash", 5)]
        [InlineData("5 sec Take out trash", "In 5 seconds: Take out trash", 5)]
        [InlineData("5 s Take out trash", "In 5 seconds: Take out trash", 5)]
        [InlineData("1 second Take out trash", "In 1 second: Take out trash", 1)]
        [InlineData("1 sec Take out trash", "In 1 second: Take out trash", 1)]
        [InlineData("1 s Take out trash", "In 1 second: Take out trash", 1)]
        public static async Task ParseTimeFromStringAsync_SecondShouldReturnCorrectValues(string expression, string expectedDescription, double? expectedSeconds)
        {
            string format = "In {0} {1}: {2}";
            string split = "3";
            string defaultText = "...";
            (string description, double? seconds) = await TimerUtils.ParseTimeFromStringAsync(expression, format, split, defaultText);

            Assert.Equal(expectedDescription, description);
            Assert.Equal(expectedSeconds, seconds);
        }

        [Theory]
        [InlineData("5 Take out trash", "In 5 seconds: Take out trash", 5)]
        [InlineData("1 Take out trash", "In 1 second: Take out trash", 1)]
        public static async Task ParseTimeFromStringAsync_NoneShouldReturnCorrectValues(string expression, string expectedDescription, double? expectedSeconds)
        {
            string format = "In {0} {1}: {2}";
            string split = "3";
            string defaultText = "...";
            (string description, double? seconds) = await TimerUtils.ParseTimeFromStringAsync(expression, format, split, defaultText);

            Assert.Equal(expectedDescription, description);
            Assert.Equal(expectedSeconds, seconds);
        }

        [Theory]
        [InlineData("Take out trash", null, null)]
        [InlineData(" 5 Take out trash", null, null)]
        [InlineData(" ", null, null)]
        [InlineData("5min Take out trash", null, null)]
        [InlineData("m", null, null)]
        public static async Task ParseTimeFromStringAsync_NoDigitShouldReturnNullValues(string expression, string expectedDescription, double? expectedSeconds)
        {
            string format = "In {0} {1}: {2}";
            string split = "3";
            string defaultText = "...";
            (string description, double? seconds) = await TimerUtils.ParseTimeFromStringAsync(expression, format, split, defaultText);

            Assert.Equal(expectedDescription, description);
            Assert.Equal(expectedSeconds, seconds);
        }

        [Theory]
        [InlineData("", "In ... ...: ...", null)]
        [InlineData(null, "In ... ...: ...", null)]
        public static async Task ParseTimeFromStringAsync_NullOrEmptyShouldReturnCorrectValues(string expression, string expectedDescription, double? expectedSeconds)
        {
            string format = "In {0} {1}: {2}";
            string split = "3";
            string defaultText = "...";
            (string description, double? seconds) = await TimerUtils.ParseTimeFromStringAsync(expression, format, split, defaultText);

            Assert.Equal(expectedDescription, description);
            Assert.Equal(expectedSeconds, seconds);
        }

        [Theory]
        [InlineData("2", "In 2 seconds: ...", 2)]
        public static async Task ParseTimeFromStringAsync_PartialShouldReturnCorrectValues(string expression, string expectedDescription, double? expectedSeconds)
        {
            string format = "In {0} {1}: {2}";
            string split = "3";
            string defaultText = "...";
            (string description, double? seconds) = await TimerUtils.ParseTimeFromStringAsync(expression, format, split, defaultText);

            Assert.Equal(expectedDescription, description);
            Assert.Equal(expectedSeconds, seconds);
        }
    }
}

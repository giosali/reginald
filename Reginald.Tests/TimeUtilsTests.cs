using Reginald.Core.Utils;
using System.Threading.Tasks;
using Xunit;

namespace Reginald.Tests
{
    public class TimeUtilsTests
    {
        [Fact]
        public static async Task GetTimeAsSecondsAsync_ShouldReturnNullableDoubleAsync()
        {
            string input = "minutes";
            double time = 5;
            double? timeInSeconds = await TimeUtils.GetTimeAsSecondsAsync(input, time);
            Assert.True(timeInSeconds is double?);
        }

        [Theory]
        [InlineData("mons", 5)]
        [InlineData("5", 5)]
        public static async Task GetTimeAsSecondsAsync_ShouldReturnNullAsync(string input, double time)
        {
            double? timeInSeconds = await TimeUtils.GetTimeAsSecondsAsync(input, time);
            Assert.True(timeInSeconds is null);
        }

        [Theory]
        [InlineData("hours")]
        [InlineData("hour")]
        [InlineData("h")]
        [InlineData("minutes")]
        [InlineData("minute")]
        [InlineData("mins")]
        [InlineData("min")]
        [InlineData("m")]
        [InlineData("seconds")]
        [InlineData("second")]
        [InlineData("secs")]
        [InlineData("sec")]
        [InlineData("s")]
        public static async Task GetTimeAsSecondsAsync_ShouldReturnDoubleAsync(string input)
        {
            double time = 5;
            double? timeInSeconds = await TimeUtils.GetTimeAsSecondsAsync(input, time);
            Assert.True(timeInSeconds is not null);
        }

        [Theory]
        [InlineData("hours", 5, 5 * 60 * 60)]
        [InlineData("minutes", 5, 5 * 60)]
        [InlineData("seconds", 5, 5)]
        public static async Task GetTimeAsSecondsAsync_ShouldReturnCorrectTimeAsync(string input, double time, double expectedTimeInSeconds)
        {
            double? actualTimeInSeconds = await TimeUtils.GetTimeAsSecondsAsync(input, time);
            Assert.True(expectedTimeInSeconds == actualTimeInSeconds);
        }

        [Theory]
        [InlineData("s")]
        [InlineData("sec")]
        [InlineData("secs")]
        [InlineData("second")]
        [InlineData("seconds")]
        public static async Task IsSecond_ShouldReturnTrue(string unit)
        {
            bool isSecond = await TimeUtils.IsSecond(unit);
            Assert.True(isSecond);
        }

        [Theory]
        [InlineData("minute")]
        [InlineData("hour")]
        public static async Task IsSecond_ShouldReturnFalse(string unit)
        {
            bool isSecond = await TimeUtils.IsSecond(unit);
            Assert.False(isSecond);
        }

        [Theory]
        [InlineData("m")]
        [InlineData("min")]
        [InlineData("mins")]
        [InlineData("minute")]
        [InlineData("minutes")]
        public static async Task IsMinute_ShouldReturnTrue(string unit)
        {
            bool isMinute = await TimeUtils.IsMinute(unit);
            Assert.True(isMinute);
        }

        [Theory]
        [InlineData("second")]
        [InlineData("hour")]
        public static async Task IsMinute_ShouldReturnFalse(string unit)
        {
            bool isMinute = await TimeUtils.IsMinute(unit);
            Assert.False(isMinute);
        }

        [Theory]
        [InlineData("h")]
        [InlineData("hour")]
        [InlineData("hours")]
        public static async Task IsHour_ShouldReturnTrue(string unit)
        {
            bool isHour = await TimeUtils.IsHour(unit);
            Assert.True(isHour);
        }

        [Theory]
        [InlineData("second")]
        [InlineData("minute")]
        public static async Task IsHour_ShouldReturnFalse(string unit)
        {
            bool isHour = await TimeUtils.IsHour(unit);
            Assert.False(isHour);
        }

        [Fact]
        public static void GetTimeUnit_ShouldReturnString()
        {
            string input = "min";
            double time = 2;
            string actual = TimeUtils.GetTimeUnit(input, time);
            Assert.True(actual is string);
        }

        [Theory]
        [InlineData("s", 2, "seconds")]
        [InlineData("m", 2, "minutes")]
        [InlineData("h", 2, "hours")]
        public static void GetTimeUnit_ShouldReturnPlural(string input, double time, string expected)
        {
            string actual = TimeUtils.GetTimeUnit(input, time);
            Assert.True(expected == actual);
        }

        [Theory]
        [InlineData("s", 1, "second")]
        [InlineData("m", 1, "minute")]
        [InlineData("h", 1, "hour")]
        public static void GetTimeUnit_ShouldReturnSingular(string input, double time, string expected)
        {
            string actual = TimeUtils.GetTimeUnit(input, time);
            Assert.True(expected == actual);
        }
    }
}

namespace Reginald.Core.Utilities
{
    using System;

    /// <summary>
    /// Thread-safe equivalent of System.Random, using just static methods <see href="https://jonskeet.uk/csharp/miscutil/">by Jon Skeet</see>.
    /// </summary>
    public static class StaticRandom
    {
        private static Random _random = new();

        private static object _lock = new();

        /// <summary>
        /// Returns a random number within the <see cref="int"/> range.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to <see cref="int.MinValue"/> and less than <see cref="int.MaxValue"/>.
        /// </returns>>
        public static int Next()
        {
            lock (_lock)
            {
                return _random.Next(int.MinValue, int.MaxValue);
            }
        }
    }
}

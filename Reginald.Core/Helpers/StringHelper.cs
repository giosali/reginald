namespace Reginald.Core.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Receives a string and returns a new string with all unmatched or unterminated characters escaped.
        /// </summary>
        /// <param name="expression">The string to clean.</param>
        /// <returns>A new string with all unmatched or unterminated characters in <paramref name="expression"/> escaped.</returns>
        public static string RegexClean(string expression)
        {
            string[] characters = new string[] { @"\", "[", "(", ")", ".", "+" };
            for (int i = 0; i < characters.Length; i++)
            {
                string character = characters[i];
                expression = expression.Replace(character, @$"\{character}");
            }
            return expression;
        }
    }
}

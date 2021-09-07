namespace Reginald.Core.Helpers
{
    public static class StringHelpers
    {
        public static string RegexClean(string expression)
        {
            string[] characters = new string[] { @"\", "[", "(", ")" };
            for (int i = 0; i < characters.Length; i++)
            {
                string character = characters[i];
                expression = expression.Replace(character, @$"\{character}");
            }
            return expression;
        }

        public static string RegexOrSplit(string expression, out int count)
        {
            string[] substrings = expression.Split(' ');
            string concatenation = string.Join("|", substrings);
            count = substrings.Length;
            return concatenation;
        }

        public static string RegexOrBoundarySplit(string expression, out int count)
        {
            string[] substrings = expression.Split(' ');
            string concatenation = string.Empty;
            for (int i = 0; i < substrings.Length; i++)
            {
                string delimiter = i == substrings.Length - 1 ? string.Empty : "|";
                string substring = substrings[i];
                if (substring != string.Empty)
                {
                    concatenation += @"\b" + substring + delimiter;
                }
                else
                {
                    concatenation += substring;
                }
            }
            count = substrings.Length;
            return concatenation;
        }
    }
}

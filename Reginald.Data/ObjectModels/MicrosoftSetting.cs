namespace Reginald.Data.ObjectModels
{
    using Reginald.Data.Producers;
    using Reginald.Data.Products;
    using Newtonsoft.Json;
    using System;
    using Reginald.Data.Inputs;
    using Reginald.Services.Utilities;
    using Reginald.Core.Extensions;

    public class MicrosoftSetting : ObjectModel, ISingleProducer<SearchResult>
    {
        [JsonProperty("containsAmpersand")]
        public bool ContainsAmpersand { get; set; }

        [JsonProperty("containsHyphen")]
        public bool ContainsHyphen { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        public bool Check(string input)
        {
            if (input.Length < 3 || input.Length > Key.Length)
            {
                return false;
            }

            if (ContainsAmpersand)
            {
                input = input.Replace("&", "and");
            }

            if (ContainsHyphen)
            {
                char firstCh = input[0];
                int firstChIndex = 0;
                while (true)
                {
                    firstChIndex = Key.IndexOf(firstCh.ToString(), firstChIndex, StringComparison.OrdinalIgnoreCase);
                    if (firstChIndex == -1)
                    {
                        break;
                    }

                    if (firstChIndex != 0 && Key[firstChIndex - 1] != ' ')
                    {
                        firstChIndex++;
                        continue;
                    }

                    bool match = true;
                    for (int i = 0, j = 0; i < input.Length; i++, j++)
                    {
                        char inputCh = input.TryElementAt(i);
                        if (inputCh == default(char))
                        {
                            return false;
                        }

                        if (inputCh == '-')
                        {
                            inputCh = ' ';
                        }

                        char keyCh = Key.TryElementAt(j + firstChIndex);
                        if (keyCh == default(char))
                        {
                            return false;
                        }

                        if (char.ToUpperInvariant(inputCh) == char.ToUpperInvariant(keyCh))
                        {
                            continue;
                        }

                        if (keyCh == ' ')
                        {
                            // Handles situations where the input is
                            // "signin options" and the key is 
                            // "sign-in options".
                            i--;
                            continue;
                        }

                        match = false;
                        break;
                    }

                    if (match)
                    {
                        return true;
                    }

                    firstChIndex++;
                }

                return false;
            }

            int index = Key.IndexOf(input, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                return false;
            }

            return index == 0 || Key[index - 1] == ' ';
        }

        public SearchResult Produce()
        {
            SearchResult result = new(Caption, Icon, Description);
            result.EnterKeyPressed += OnEnterKeyPressed;
            return result;
        }

        private void OnEnterKeyPressed(object sender, InputProcessingEventArgs e)
        {
            ProcessUtility.GoTo(Uri);
            e.Handled = true;
        }
    }
}
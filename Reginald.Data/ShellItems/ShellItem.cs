namespace Reginald.Data.ShellItems
{
    public abstract class ShellItem : Item
    {
        protected const string NameRegexFormat = "^{0}";

        protected const string ShellItemUppercaseRegexFormat = @"(?<!^){0}";

        public override bool IsAltKeyDown { get; set; }

        public string Path { get; set; }
    }
}

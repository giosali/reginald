namespace Reginald.Core.Extensions
{
    public static class Int32Extensions
    {
        public static string Quantify(this int number, string unit, string plural = null)
        {
            return number == 1 ? $"{number} {unit}" : $"{number} {(plural is null ? unit + "s" : plural)}";
        }
    }
}

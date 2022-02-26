namespace Reginald.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static string Quantify(this double number, string unit, string plural = null)
        {
            return number == 1 ? $"{number} {unit}" : $"{number} {(plural is null ? unit + "s" : plural)}";
        }
    }
}

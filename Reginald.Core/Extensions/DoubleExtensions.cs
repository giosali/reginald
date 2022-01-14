namespace Reginald.Core.Extensions
{
    using Reginald.Core.Products;

    public static class DoubleExtensions
    {
        public static string Quantify(this double number, string unit, string plural = null)
        {
            return number == 1 ? $"{number} {unit}" : $"{number} {(plural is null ? unit + "s" : plural)}";
        }

        public static double ToMilliseconds(this double number, TimeUnit unit, out string representation)
        {
            double time = number * 1000;
            representation = null;

            switch (unit)
            {
                case TimeUnit.Second:
                    representation = number.Quantify("sec");
                    break;

                case TimeUnit.Minute:
                    time *= 60;
                    representation = number.Quantify("min");
                    break;

                case TimeUnit.Hour:
                    time *= 60 * 60;
                    representation = number.Quantify("hr");
                    break;

                default:
                    break;
            }

            return time;
        }
    }
}

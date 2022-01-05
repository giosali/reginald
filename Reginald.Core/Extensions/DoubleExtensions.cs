using Reginald.Core.Enums;

namespace Reginald.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static string Quantify(this double number, string unit, string plural = null)
        {
            return number == 1 ? $"{number} {unit}" : $"{number} {(plural is null ? unit + "s" : plural)}";
        }

        public static double ToMilliseconds(this double number, Unit unit, out string representation)
        {
            double time = number * 1000;
            representation = null;

            switch (unit)
            {
                case Unit.Second:
                    representation = number.Quantify("sec");
                    break;

                case Unit.Minute:
                    time *= 60;
                    representation = number.Quantify("min");
                    break;

                case Unit.Hour:
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

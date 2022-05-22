namespace Reginald.Core.Math
{
    internal static class Operator
    {
        internal static bool IsOperator(char ch) => ch switch
        {
            '^' or '*' or '/' or '÷' or '+' or '-' or '−' => true,
            _ => false,
        };

        internal static int GetPrecedence(char op) => op switch
        {
            '^' => 2,
            '*' or '/' or '÷' => 1,
            '+' or '-' or '−' => 0,
            _ => -1,
        };

        internal static bool IsLeftAssociative(char op) => op switch
        {
            '*' or '/' or '÷' or '+' or '-' or '−' => true,
            _ => false,
        };
    }
}

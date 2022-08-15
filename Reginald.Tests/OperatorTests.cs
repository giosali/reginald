namespace Reginald.Tests
{
    using Reginald.Core.Math;
    using Xunit;

    public class OperatorTests
    {
        [Theory]
        [InlineData('-')]
        [InlineData('+')]
        [InlineData('/')]
        [InlineData('*')]
        [InlineData('^')]
        public void IsOperator_WhenGivenOperator_ShouldReturnTrue(char ch)
        {
            Assert.True(Operator.IsOperator(ch));
        }

        [Theory]
        [InlineData('(')]
        [InlineData(')')]
        [InlineData('[')]
        [InlineData(']')]
        [InlineData('{')]
        [InlineData('}')]
        [InlineData('|')]
        [InlineData('a')]
        [InlineData('\0')]
        public void IsOperator_WhenNotGivenOperator_ShouldReturnFalse(char ch)
        {
            Assert.False(Operator.IsOperator(ch));
        }

        [Theory]
        [InlineData('^', '*')]
        [InlineData('^', '/')]
        [InlineData('^', '+')]
        [InlineData('^', '-')]
        [InlineData('*', '+')]
        [InlineData('*', '-')]
        [InlineData('/', '+')]
        [InlineData('/', '-')]
        public void GetPrecedence_CompareGreaterOperators(char op1, char op2)
        {
            Assert.True(Operator.GetPrecedence(op1) > Operator.GetPrecedence(op2));
        }

        [Theory]
        [InlineData('*', '/')]
        [InlineData('+', '-')]
        public void GetPrecedence_CompareEqualOperators(char op1, char op2)
        {
            Assert.Equal(Operator.GetPrecedence(op1), Operator.GetPrecedence(op2));
        }

        [Theory]
        [InlineData('-')]
        [InlineData('+')]
        [InlineData('/')]
        [InlineData('*')]
        public void IsLeftAssociative_WhenGivenLeftAssociativeOperator_ShouldReturnTrue(char op)
        {
            Assert.True(Operator.IsLeftAssociative(op));
        }

        [Theory]
        [InlineData('^')]
        [InlineData('(')]
        [InlineData(')')]
        public void IsLeftAssociative_WhenGivenRightAssociativeOperator_ShouldReturnFalse(char op)
        {
            Assert.False(Operator.IsLeftAssociative(op));
        }
    }
}

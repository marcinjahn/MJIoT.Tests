using MjIot.EventsHandler.ValueModifiers;
using MjIot.Storage.Models.EF6Db;
using System;
using System.Globalization;
using System.Threading;
using Xunit;

namespace MjIot.EventsHandler.Tests
{
    public class CalculationTests
    {
        private Calculation _calculation;

        public CalculationTests()
        {
            _calculation = new Calculation();
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Theory]
        [InlineData("0", "1", "1")]
        [InlineData("1", "0", "1")]
        [InlineData("2", "1", "3")]
        [InlineData("1.1", "2,2", "3.3")]
        [InlineData("7", "-8", "-1")]
        public void Modify_AdditionCalculationUsed_ReturnCorrectValue(string input, string calculationValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionCalculation.Addition, calculationValue);

            var result = _calculation.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        public void Modify_AdditionCalculationUsed_ExceptionThrown(string input, string calculationValue)
        {
            var connection = GenerateConnection(ConnectionCalculation.Subtraction, calculationValue);

            Assert.Throws<NotSupportedException>(() => _calculation.Modify(input, connection));
        }

        [Theory]
        [InlineData("0", "1", "-1")]
        [InlineData("1", "0", "1")]
        [InlineData("2", "1", "1")]
        [InlineData("1.1", "2,2", "-1.1")]
        [InlineData("7", "-8", "15")]
        public void Modify_SubtractionCalculationUsed_ReturnCorrectValue(string input, string calculationValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionCalculation.Subtraction, calculationValue);

            var result = _calculation.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        public void Modify_SubtractionCalculationUsed_ExceptionThrown(string input, string calculationValue)
        {
            var connection = GenerateConnection(ConnectionCalculation.Subtraction, calculationValue);

            Assert.Throws<NotSupportedException>(() => _calculation.Modify(input, connection));
        }

        [Theory]
        [InlineData("0", "1", "0")]
        [InlineData("1", "0", "0")]
        [InlineData("2", "1", "2")]
        [InlineData("1,1", "2.2", "2.42")]
        [InlineData("7", "-8", "-56")]
        public void Modify_ProductCalculationUsed_ReturnCorrectValue(string input, string calculationValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionCalculation.Product, calculationValue);

            var result = _calculation.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        public void Modify_ProductCalculationUsed_ExceptionThrown(string input, string calculationValue)
        {
            var connection = GenerateConnection(ConnectionCalculation.Product, calculationValue);

            Assert.Throws<NotSupportedException>(() => _calculation.Modify(input, connection));
        }

        [Theory]
        [InlineData("0", "1", "0")]
        [InlineData("2", "1", "2")]
        [InlineData("1,1", "2.2", "0.5")]
        [InlineData("7", "-2", "-3.5")]
        public void Modify_DivisionCalculationUsed_ReturnCorrectValue(string input, string calculationValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionCalculation.Division, calculationValue);

            var result = _calculation.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("2", "0")]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        public void Modify_DivisionCalculationUsed_ExceptionThrown(string input, string calculationValue)
        {
            var connection = GenerateConnection(ConnectionCalculation.Division, calculationValue);

            Assert.Throws<NotSupportedException>(() => _calculation.Modify(input, connection));
        }

        [Theory]
        [InlineData("true", "true", "true")]
        [InlineData("true", "false", "false")]
        [InlineData("false", "true", "false")]
        [InlineData("false", "false", "false")]
        public void Modify_BooleanAndCalculationUsed_ReturnCorrectValue(string input, string calculationValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionCalculation.BooleanAnd, calculationValue);

            var result = _calculation.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("2", "0")]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        [InlineData("true", "qwerty")]
        public void Modify_BooleanAndCalculationUsed_ExceptionThrown(string input, string calculationValue)
        {
            var connection = GenerateConnection(ConnectionCalculation.BooleanAnd, calculationValue);

            Assert.Throws<NotSupportedException>(() => _calculation.Modify(input, connection));
        }

        [Theory]
        [InlineData("true", "true", "false")]
        [InlineData("true", "false", "false")]
        [InlineData("false", "true", "true")]
        [InlineData("false", "false", "true")]
        [InlineData("false", null, "true")]
        public void Modify_BooleanNotCalculationUsed_ReturnCorrectValue(string input, string calculationValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionCalculation.BooleanNot, calculationValue);

            var result = _calculation.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("2", "0")]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        public void Modify_BooleanNotCalculationUsed_ExceptionThrown(string input, string calculationValue)
        {
            var connection = GenerateConnection(ConnectionCalculation.BooleanNot, calculationValue);

            Assert.Throws<NotSupportedException>(() => _calculation.Modify(input, connection));
        }

        [Theory]
        [InlineData("true", "true", "true")]
        [InlineData("true", "false", "true")]
        [InlineData("false", "true", "true")]
        [InlineData("false", "false", "false")]
        public void Modify_BooleanOrCalculationUsed_ReturnCorrectValue(string input, string calculationValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionCalculation.BooleanOr, calculationValue);

            var result = _calculation.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("2", "0")]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        [InlineData("true", "qwerty")]
        public void Modify_BooleanOrCalculationUsed_ExceptionThrown(string input, string calculationValue)
        {
            var connection = GenerateConnection(ConnectionCalculation.BooleanOr, calculationValue);

            Assert.Throws<NotSupportedException>(() => _calculation.Modify(input, connection));
        }

        [Theory]
        [InlineData("true", "true", "true")]
        [InlineData("true", "false", "true")]
        [InlineData("false", "true", "false")]
        [InlineData("false", "false", "false")]
        [InlineData("qwerty", "0", "qwerty")]
        [InlineData("0", "1", "0")]
        [InlineData("2", "1", "2")]
        [InlineData("4.57", "1", "4.57")]
        public void Modify_NoneCalculationUsed_ReturnCorrectValue(string input, string calculationValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionCalculation.None, calculationValue);

            var result = _calculation.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("2", "x+4", "6")]
        [InlineData("2", "x*7-5", "9")]
        [InlineData("true", "x&false", "false")]
        [InlineData("true", "x&true&(5-4 == 1)", "true")]
        [InlineData("9", "Math.Sqrt(x)", "3")]  //klasa Math
        [InlineData("9", "Math.Sqrt(x) + x", "12")] //dwukrotne użycie input
        [InlineData("qwerty", "char[] charArray = x.ToCharArray(); Array.Reverse( charArray ); return new string( charArray );", "ytrewq")] //complex operation
        [InlineData("qwerty", "123456", "123456")] //zamiana stringa wejściowego na zupełnie inny
        public void Modify_CustomCalculationUsed_ReturnCorrectValue(string input, string calculationScript, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionCalculation.Custom, calculationScript);

            var result = _calculation.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }


        [Theory]
        [InlineData("3", "qwerty+7")]
        [InlineData("qwerty", "(x+7)/20")]
        public void Modify_CustomCalculationUsedWithWrongData_ExceptionThrown(string input, string calculationScript)
        {
            var connection = GenerateConnection(ConnectionCalculation.Custom, calculationScript);

            Assert.Throws<NotSupportedException>(() => _calculation.Modify(input, connection));
        }






        private Connection GenerateConnection(ConnectionCalculation calculationType, string calculationValue)
        {
            return new Connection
            {
                Calculation = calculationType,
                CalculationValue = calculationValue
            };
        }
    }
}
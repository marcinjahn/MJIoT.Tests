using MjIot.EventsHandler.ValueModifiers;
using MjIot.Storage.Models.EF6Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MjIot.EventsHandler.Tests
{
    public class CalculationTests
    {
        Calculation _calculation;

        public CalculationTests()
        {
            _calculation = new Calculation();
        }

        [Theory]
        [InlineData("0", "1", "1")]
        [InlineData("1", "0", "1")]
        [InlineData("2", "1", "3")]
        [InlineData("1.1", "2.2", "3,3")]
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
        [InlineData("1.1", "2.2", "-1,1")]
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
        [InlineData("1.1", "2.2", "2,42")]
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
        [InlineData("1.1", "2.2", "0,5")]
        [InlineData("7", "-2", "-3,5")]
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
        [InlineData("true", "qwerty")]
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

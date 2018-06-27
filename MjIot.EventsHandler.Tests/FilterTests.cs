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
    public class FilterTests
    {
        private Filter _filter;

        public FilterTests()
        {
            _filter = new Filter();
        }

        [Theory]
        [InlineData("1", "1", "1")]
        [InlineData("1", "2", null)]
        [InlineData("some text", "some text", "some text")]
        [InlineData("some text", "some text1", null)]
        public void Modify_EqualFilterUsed_ReturnCorrectValue(string input, string filterValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionFilter.Equal, filterValue);

            var result = _filter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("1", "0", "1")]
        [InlineData("2", "1", "2")]
        [InlineData("14.32", "14.1", "14.32")]
        [InlineData("14,32", "14,1", "14,32")]
        [InlineData("7", "-8", "7")]
        [InlineData("-7", "-8", "-7")]
        [InlineData("14.32", "14.33", null)]
        [InlineData("14,32", "14,33", null)]
        [InlineData("5", "5", null)]
        [InlineData("12.67", "12.67", null)]
        public void Modify_GreaterFilterUsed_ReturnCorrectValue(string input, string filterValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionFilter.Greater, filterValue);

            var result = _filter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        public void Modify_GreaterFilterUsed_ExceptionThrown(string input, string filterValue)
        {
            var connection = GenerateConnection(ConnectionFilter.Greater, filterValue);

            Assert.Throws<NotSupportedException>(() => _filter.Modify(input, connection));
        }

        [Theory]
        [InlineData("1", "0", "1")]
        [InlineData("2", "1", "2")]
        [InlineData("14.32", "14.1", "14.32")]
        [InlineData("14,32", "14,1", "14,32")]
        [InlineData("7", "-8", "7")]
        [InlineData("-7", "-8", "-7")]
        [InlineData("14.32", "14.33", null)]
        [InlineData("14,32", "14,33", null)]
        [InlineData("5", "5", "5")]
        [InlineData("12.67", "12.67", "12.67")]
        public void Modify_GreaterOrEqualFilterUsed_ReturnCorrectValue(string input, string filterValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionFilter.GreaterOrEqual, filterValue);

            var result = _filter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        public void Modify_GreaterOrEqualFilterUsed_ExceptionThrown(string input, string filterValue)
        {
            var connection = GenerateConnection(ConnectionFilter.GreaterOrEqual, filterValue);

            Assert.Throws<NotSupportedException>(() => _filter.Modify(input, connection));
        }

        [Theory]
        [InlineData("0", "1", "0")]
        [InlineData("1", "0", null)]
        [InlineData("2", "1", null)]
        [InlineData("14.32", "14.1", null)]
        [InlineData("14,32", "14,1", null)]
        [InlineData("7", "-8", null)]
        [InlineData("-8", "-7", "-8")]
        [InlineData("14.32", "14.33", "14.32")]
        [InlineData("14,32", "14,33", "14,32")]
        [InlineData("5", "5", null)]
        [InlineData("12.67", "12.67", null)]
        public void Modify_LessFilterUsed_ReturnCorrectValue(string input, string filterValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionFilter.Less, filterValue);

            var result = _filter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        public void Modify_LessFilterUsed_ExceptionThrown(string input, string filterValue)
        {
            var connection = GenerateConnection(ConnectionFilter.Less, filterValue);

            Assert.Throws<NotSupportedException>(() => _filter.Modify(input, connection));
        }

        [Theory]
        [InlineData("0", "1", "0")]
        [InlineData("1", "0", null)]
        [InlineData("2", "1", null)]
        [InlineData("14.32", "14.1", null)]
        [InlineData("14,32", "14,1", null)]
        [InlineData("7", "-8", null)]
        [InlineData("-8", "-7", "-8")]
        [InlineData("14.32", "14.33", "14.32")]
        [InlineData("14,32", "14,33", "14,32")]
        [InlineData("5", "5", "5")]
        [InlineData("12.67", "12.67", "12.67")]
        public void Modify_LessOrEqualFilterUsed_ReturnCorrectValue(string input, string filterValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionFilter.LessOrEqual, filterValue);

            var result = _filter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty", "0")]
        [InlineData("2", "qwerty")]
        public void Modify_LessOrEqualFilterUsed_ExceptionThrown(string input, string filterValue)
        {
            var connection = GenerateConnection(ConnectionFilter.LessOrEqual, filterValue);

            Assert.Throws<NotSupportedException>(() => _filter.Modify(input, connection));
        }

        [Theory]
        [InlineData("0", "1", "0")]
        [InlineData("1", "0", "1")]
        [InlineData("2", "1", "2")]
        [InlineData("14.32", "14.1", "14.32")]
        [InlineData("14,32", "14,1", "14,32")]
        [InlineData("7", "-8", "7")]
        [InlineData("-8", "-7", "-8")]
        [InlineData("14.32", "14.33", "14.32")]
        [InlineData("14,32", "14,33", "14,32")]
        [InlineData("5", "5", "5")]
        [InlineData("12.67", "12.67", "12.67")]
        public void Modify_NoneFilterUsed_ReturnCorrectValue(string input, string filterValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionFilter.None, filterValue);

            var result = _filter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("0", "1", "0")]
        [InlineData("1", "0", "1")]
        [InlineData("2", "1", "2")]
        [InlineData("14.32", "14.1", "14.32")]
        [InlineData("14,32", "14,1", "14,32")]
        [InlineData("7", "-8", "7")]
        [InlineData("-8", "-7", "-8")]
        [InlineData("14.32", "14.33", "14.32")]
        [InlineData("14,32", "14,33", "14,32")]
        [InlineData("5", "5", null)]
        [InlineData("12.67", "12.67", null)]
        [InlineData("some text", "some text", null)]
        [InlineData("some text", "some text1", "some text")]
        public void Modify_NotEqualFilterUsed_ReturnCorrectValue(string input, string filterValue, string expectedResult)
        {
            var connection = GenerateConnection(ConnectionFilter.NotEqual, filterValue);

            var result = _filter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        private Connection GenerateConnection(ConnectionFilter filterType, string filterValue)
        {
            return new Connection
            {
                Filter = filterType,
                FilterValue = filterValue
            };
        }
    }
}

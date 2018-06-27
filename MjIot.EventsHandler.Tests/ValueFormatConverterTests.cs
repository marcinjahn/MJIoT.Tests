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
    public class ValueFormatConverterTests
    {
        ValueFormatConverter _converter;

        public ValueFormatConverterTests()
        {
            _converter = new ValueFormatConverter();
        }

        [Theory]
        [InlineData("1", "1")]
        [InlineData("0", "0")]
        [InlineData("2.4", "2,4")]
        [InlineData("16.54", "16,54")]
        [InlineData("-44", "-44")]
        public void Modify_NumberToNumber_SameValueReturned(string input, string expectedResult)
        {
            var connection = GenerateConnection(PropertyFormat.Number, PropertyFormat.Number);

            var result = _converter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("true")]
        public void Modify_NumberToNumber_ExceptionThrown(string input)
        {
            var connection = GenerateConnection(PropertyFormat.Number, PropertyFormat.Number);

            Assert.Throws<NotSupportedException>(() => _converter.Modify(input, connection));
        }

        [Theory]
        [InlineData("1", "1")]
        [InlineData("0", "0")]
        [InlineData("2.4", "2,4")]
        [InlineData("16.54", "16,54")]
        [InlineData("-44", "-44")]
        public void Modify_NumberToString_SameValueReturned(string input, string expectedResult)
        {
            var connection = GenerateConnection(PropertyFormat.Number, PropertyFormat.String);

            var result = _converter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("true")]
        public void Modify_NumberToString_ExceptionThrown(string input)
        {
            var connection = GenerateConnection(PropertyFormat.Number, PropertyFormat.String);

            Assert.Throws<NotSupportedException>(() => _converter.Modify(input, connection));
        }

        [Theory]
        [InlineData("1", "true")]
        [InlineData("0", "false")]
        [InlineData("2.4", "true")]
        [InlineData("16.54", "true")]
        [InlineData("-3", "false")]
        public void Modify_NumberToBoolean_SameValueReturned(string input, string expectedResult)
        {
            var connection = GenerateConnection(PropertyFormat.Number, PropertyFormat.Boolean);

            var result = _converter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("true")]
        public void Modify_NumberToBoolean_ExceptionThrown(string input)
        {
            var connection = GenerateConnection(PropertyFormat.Number, PropertyFormat.Boolean);

            Assert.Throws<NotSupportedException>(() => _converter.Modify(input, connection));
        }

        [Theory]
        [InlineData("1", "1")]
        [InlineData("0", "0")]
        [InlineData("2.4", "2,4")]
        [InlineData("16.54", "16,54")]
        [InlineData("-44", "-44")]
        public void Modify_StringToNumber_SameValueReturned(string input, string expectedResult)
        {
            var connection = GenerateConnection(PropertyFormat.String, PropertyFormat.Number);

            var result = _converter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("true")]
        public void Modify_StringToNumber_ExceptionThrown(string input)
        {
            var connection = GenerateConnection(PropertyFormat.String, PropertyFormat.Number);

            Assert.Throws<NotSupportedException>(() => _converter.Modify(input, connection));
        }

        [Theory]
        [InlineData("1", "1")]
        [InlineData("0", "0")]
        [InlineData("2.4", "2.4")]
        [InlineData("16.54", "16.54")]
        [InlineData("-44", "-44")]
        [InlineData("some text", "some text")]
        [InlineData("true", "true")]
        public void Modify_StringToString_SameValueReturned(string input, string expectedResult)
        {
            var connection = GenerateConnection(PropertyFormat.String, PropertyFormat.String);

            var result = _converter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        [InlineData("on", "true")]
        [InlineData("TRUE", "true")]
        [InlineData("off", "false")]
        [InlineData("FALSE", "false")]
        public void Modify_StringToBoolean_SameValueReturned(string input, string expectedResult)
        {
            var connection = GenerateConnection(PropertyFormat.String, PropertyFormat.Boolean);

            var result = _converter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("12")]
        public void Modify_StringToBoolean_ExceptionThrown(string input)
        {
            var connection = GenerateConnection(PropertyFormat.String, PropertyFormat.Boolean);

            Assert.Throws<NotSupportedException>(() => _converter.Modify(input, connection));
        }

        [Theory]
        [InlineData("true", "1")]
        [InlineData("false", "0")]
        public void Modify_BooleanToNumber_SameValueReturned(string input, string expectedResult)
        {
            var connection = GenerateConnection(PropertyFormat.Boolean, PropertyFormat.Number);

            var result = _converter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("12")]
        public void Modify_BooleanToNumber_ExceptionThrown(string input)
        {
            var connection = GenerateConnection(PropertyFormat.Boolean, PropertyFormat.Number);

            Assert.Throws<NotSupportedException>(() => _converter.Modify(input, connection));
        }

        [Theory]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        public void Modify_BooleanToString_SameValueReturned(string input, string expectedResult)
        {
            var connection = GenerateConnection(PropertyFormat.Boolean, PropertyFormat.String);

            var result = _converter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("12")]
        public void Modify_BooleanToString_ExceptionThrown(string input)
        {
            var connection = GenerateConnection(PropertyFormat.Boolean, PropertyFormat.String);

            Assert.Throws<NotSupportedException>(() => _converter.Modify(input, connection));
        }

        [Theory]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        public void Modify_BooleanToBoolean_SameValueReturned(string input, string expectedResult)
        {
            var connection = GenerateConnection(PropertyFormat.Boolean, PropertyFormat.Boolean);

            var result = _converter.Modify(input, connection);

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("qwerty")]
        [InlineData("12")]
        public void Modify_BooleanToBoolean_ExceptionThrown(string input)
        {
            var connection = GenerateConnection(PropertyFormat.Boolean, PropertyFormat.Boolean);

            Assert.Throws<NotSupportedException>(() => _converter.Modify(input, connection));
        }

        private Connection GenerateConnection(PropertyFormat senderFormat, PropertyFormat listenerFormat)
        {
            return new Connection
            {
                SenderProperty = new PropertyType { Format = senderFormat },
                ListenerProperty = new PropertyType { Format = listenerFormat }
            };
        }
    }
}

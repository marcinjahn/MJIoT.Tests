using MjIot.EventsHandler.Models;
using System;
using Xunit;

namespace MjIot.EventsHandler.Tests
{
    public class IncomingMessageTests
    {
        [Theory]
        [InlineData("some text")]
        [InlineData(4)]
        [InlineData(true)]
        public void Constructor_StringAsInput_CreatesCorrectObject(object value)
        {
            var stringInput = GetStringMessage(1, "Property1", value.ToString());
            var obj = new IncomingMessage(stringInput);

            Assert.Equal(1, obj.DeviceId);
            Assert.Equal("Property1", obj.PropertyName);
            Assert.Equal(value.ToString(), obj.PropertyValue);
        }

        [Theory]
        [InlineData("some text")]
        [InlineData(4)]
        [InlineData(true)]
        public void Constructor_EachElementAsInput_CreatesCorrectObject(object value)
        {
            var obj = new IncomingMessage(1, "Property1", value.ToString());

            Assert.Equal(1, obj.DeviceId);
            Assert.Equal("Property1", obj.PropertyName);
            Assert.Equal(value.ToString(), obj.PropertyValue);
        }

        [Theory]
        [InlineData(@"{{DeviceId1: ""1"",PropertyName: ""Property1"",PropertyValue: ""4""}}")]
        [InlineData(@"{{PropertyName: ""Property1"",PropertyValue: ""4""}}")]
        [InlineData(@"{{DeviceId: ""abc"",PropertyName: ""Property1"",PropertyValue: ""4""}}")]
        public void Constructor_IncorrectStringAsInput_ThrowsException(string stringInput)
        {
            Assert.Throws<Exception>(() => new IncomingMessage(stringInput));
        }

        private string GetStringMessage(int deviceId, string propertyName, string value)
        {
            return $@"{{DeviceId: ""{deviceId}"",PropertyName: ""{propertyName}"",PropertyValue: ""{value}""}}";
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("abc", null)]
        [InlineData(null, "4")]
        public void Constructor_IncorrectElementsAsInput_ThrowsException(string propertyName, string value)
        {
            Assert.Throws<ArgumentNullException>(() => new IncomingMessage(1, propertyName, value));
        }
    }
}
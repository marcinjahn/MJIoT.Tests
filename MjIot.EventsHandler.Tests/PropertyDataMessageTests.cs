using MjIot.EventsHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MjIot.EventsHandler.Tests
{
    public class PropertyDataMessageTests
    {
        [Theory]
        [InlineData("some text")]
        [InlineData(4)]
        [InlineData(true)]
        public void Constructor_StringAsInput_CreatesCorrectObject(object value)
        {
            var stringInput = GetStringMessage(1, "Property1", value.ToString());
            var obj = new PropertyDataMessage(stringInput);

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
            var obj = new PropertyDataMessage(1, "Property1", value.ToString());

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
            Assert.Throws<Exception>(() => new PropertyDataMessage(stringInput));
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
            Assert.Throws<ArgumentNullException>(() => new PropertyDataMessage(1, propertyName, value));
        }
    }
}

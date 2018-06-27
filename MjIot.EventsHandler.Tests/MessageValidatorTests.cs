using MjIot.EventsHandler.Models;
using MjIot.EventsHandler.Services;
using MjIot.Storage.Models.EF6Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MjIot.EventsHandler.Tests
{
    public class MessageValidatorTests
    {
        [Theory]
        [MemberData(nameof(GetIsMessageValidTestData))]
        public void IsMessageValid_VariousExamplesProvided_ReturnCorrectResult(PropertyDataMessage message, PropertyFormat format, bool expectedResult)
        {
            var result = MessageValidator.IsMessageValid(message, format);

            Assert.Equal(expectedResult, result);
        }

        public static IEnumerable<object[]> GetIsMessageValidTestData()
        {
            yield return new object[] { new PropertyDataMessage { PropertyValue = "1"}, PropertyFormat.Number, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "12,4" }, PropertyFormat.Number, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "12.4" }, PropertyFormat.Number, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "-18.97" }, PropertyFormat.Number, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "true" }, PropertyFormat.Number, false };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "false" }, PropertyFormat.Number, false };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "some text" }, PropertyFormat.Number, false };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "..." }, PropertyFormat.Number, false };

            yield return new object[] { new PropertyDataMessage { PropertyValue = "true" }, PropertyFormat.Boolean, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "false" }, PropertyFormat.Boolean, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "1" }, PropertyFormat.Boolean, false };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "some text" }, PropertyFormat.Boolean, false };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "..." }, PropertyFormat.Boolean, false };

            yield return new object[] { new PropertyDataMessage { PropertyValue = "true" }, PropertyFormat.String, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "false" }, PropertyFormat.String, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "1" }, PropertyFormat.String, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "-14.876" }, PropertyFormat.String, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "some text" }, PropertyFormat.String, true };
            yield return new object[] { new PropertyDataMessage { PropertyValue = "..." }, PropertyFormat.String, true };
        }
    }
}

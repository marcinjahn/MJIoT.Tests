using MjIot.EventsHandler.Models;
using MjIot.EventsHandler.Services;
using MjIot.Storage.Models.EF6Db;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Xunit;

namespace MjIot.EventsHandler.Tests
{
    public class MessageValidatorTests
    {
        public MessageValidatorTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Theory]
        [MemberData(nameof(GetIsMessageValidTestData))]
        public void IsMessageValid_VariousExamplesProvided_ReturnCorrectResult(IncomingMessage message, PropertyFormat format, bool expectedResult)
        {
            var result = MessageValidator.IsMessageValid(message, format);

            Assert.Equal(expectedResult, result);
        }

        public static IEnumerable<object[]> GetIsMessageValidTestData()
        {
            yield return new object[] { new IncomingMessage { PropertyValue = "1" }, PropertyFormat.Number, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "12,4" }, PropertyFormat.Number, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "12.4" }, PropertyFormat.Number, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "-18.97" }, PropertyFormat.Number, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "true" }, PropertyFormat.Number, false };
            yield return new object[] { new IncomingMessage { PropertyValue = "false" }, PropertyFormat.Number, false };
            yield return new object[] { new IncomingMessage { PropertyValue = "some text" }, PropertyFormat.Number, false };
            yield return new object[] { new IncomingMessage { PropertyValue = "..." }, PropertyFormat.Number, false };

            yield return new object[] { new IncomingMessage { PropertyValue = "true" }, PropertyFormat.Boolean, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "false" }, PropertyFormat.Boolean, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "1" }, PropertyFormat.Boolean, false };
            yield return new object[] { new IncomingMessage { PropertyValue = "some text" }, PropertyFormat.Boolean, false };
            yield return new object[] { new IncomingMessage { PropertyValue = "..." }, PropertyFormat.Boolean, false };

            yield return new object[] { new IncomingMessage { PropertyValue = "true" }, PropertyFormat.String, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "false" }, PropertyFormat.String, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "1" }, PropertyFormat.String, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "-14.876" }, PropertyFormat.String, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "some text" }, PropertyFormat.String, true };
            yield return new object[] { new IncomingMessage { PropertyValue = "..." }, PropertyFormat.String, true };
        }
    }
}
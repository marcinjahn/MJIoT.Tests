using MjIot.EventsHandler.Models;
using MjIot.EventsHandler.Services;
using MjIot.Storage.Models.EF6Db;
using MJIot.Storage.Models;
using MJIot.Storage.Models.Repositiories;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MjIot.EventsHandler.Tests
{
    public class EventHandlerTests
    {
        private Mock<IUnitOfWork> UnitOfWorkMock { get; set; }
        private Mock<IIotService> IotServiceMock { get; set; }
        private Mock<IDeviceRepository> DeviceRepositoryMock { get; set; }
        private Mock<IPropertyTypeRepository> PropertyTypeRepositoryMock { get; set; }
        private Mock<IConnectionRepository> ConnectionRepositoryMock { get; set; }
        private Mock<ILogger> LoggerMock { get; set; }

        private EventHandlerFixture _fixture;

        public EventHandlerTests()
        {
            UnitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            IotServiceMock = new Mock<IIotService>(MockBehavior.Strict);
            DeviceRepositoryMock = new Mock<IDeviceRepository>(MockBehavior.Strict);
            PropertyTypeRepositoryMock = new Mock<IPropertyTypeRepository>(MockBehavior.Strict);
            ConnectionRepositoryMock = new Mock<IConnectionRepository>(MockBehavior.Strict);
            LoggerMock = new Mock<ILogger>();

            _fixture = new EventHandlerFixture();
        }

        [Fact]
        public void Constructor_SenderDeviceExists_NoExceptionIsThrown()
        {
            var message = new IncomingMessage(1, "Property1", "false");

            PropertyTypeRepositoryMock = new Mock<IPropertyTypeRepository>(MockBehavior.Loose);
            SetupDevicesRepositoryForSender();

            UnitOfWorkMock.Setup(n => n.PropertyTypes).Returns(PropertyTypeRepositoryMock.Object);

            try
            {
                var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, null);
            }
            catch (Exception e)
            {
                if (e.Message != "Sender property not found")
                    Assert.True(false);
            }
        }

        [Fact]
        public void Constructor_SenderPropertyExists_NoExceptionIsThrown()
        {
            var message = new IncomingMessage(1, "Property1", "false");

            SetupDevicesRepositoryForSender();
            SetupPropertyTypesRepositoryForSender();

            var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, null);
        }

        [Fact]
        public void Constructor_MessageFromNonExistingDevice_ThrowsException()
        {
            var message = new IncomingMessage(5, "Property1", "false");

            DeviceRepositoryMock.Setup(n => n.Get(5)).Returns((Device)null);
            UnitOfWorkMock.Setup(n => n.Devices).Returns(DeviceRepositoryMock.Object);

            try
            {
                var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, null);
            }
            catch (Exception e)
            {
                if (e.Message != "Sender device not found")
                    Assert.True(false);
            }
        }

        [Fact]
        public void Constructor_MessageFromNonExistingProperty_ThrowsException()
        {
            var message = new IncomingMessage(1, "NonExistingProperty", "false");

            SetupDevicesRepositoryForSender();
            SetupPropertyTypesRepositoryForSender();

            try
            {
                var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, null);
            }
            catch (Exception e)
            {
                if (e.Message != "Sender property not found")
                    Assert.True(false);
            }
        }

        [Theory]
        [MemberData(nameof(GetWrongConstructorParameters))]
        public void Constructor_NullParametersGiven_ThrowsArgumentNullException(IncomingMessage message, IUnitOfWork unitOfWork, IIotService iotService)
        {
            Assert.Throws<ArgumentNullException>(() => new EventHandler(message, unitOfWork, iotService, null));
        }

        public static IEnumerable<object[]> GetWrongConstructorParameters()
        {
            yield return new object[] { null, null, null };
            yield return new object[] { null, new Mock<IUnitOfWork>().Object, null };
            yield return new object[] { new IncomingMessage(), new Mock<IUnitOfWork>().Object, null };
            yield return new object[] { null, new Mock<IUnitOfWork>().Object, new Mock<IIotService>().Object };
            yield return new object[] { null, new Mock<IUnitOfWork>().Object, null };
            yield return new object[] { new IncomingMessage(), null, null };
        }

        [Theory]
        [InlineData(PropertyFormat.Number, "false")]
        [InlineData(PropertyFormat.Number, "some text")]
        [InlineData(PropertyFormat.Boolean, "some text")]
        [InlineData(PropertyFormat.Boolean, "1")]
        public async void HandleMessage_IncorrectMessageValueFormatGiven_ThrowsException(PropertyFormat expectedFormat, string incomingValue)
        {
            var message = new IncomingMessage(1, "Property1", incomingValue);

            SetupDevicesRepositoryForSender();
            SetupPropertyTypesRepositoryForSender();
            _fixture.PropertyTypeOfSender.Format = expectedFormat;

            var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, null);

            try
            {
                await handler.HandleMessage();
            }
            catch (Exception e)
            {
                if (e.Message != $"Received value does not match the declared type of sender property! (DeviceId: { message.DeviceId }, Property: { message.PropertyName }, Value: { message.PropertyValue })")
                    Assert.True(false);
            }
        }

        [Fact]
        public async void HandleMessage_MessageSentIsNotFromSenderProperty_ReturnsFalse()
        {
            var message = new IncomingMessage(1, "Property1", "4");

            SetupDevicesRepositoryForSender();
            SetupPropertyTypesRepositoryForSender();
            _fixture.PropertyTypeOfSender.IsSenderProperty = false;

            var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, null);
            var result = await handler.HandleMessage();

            Assert.False(result);
        }

        [Fact]
        public async void HandleMessage_MessageSentIsFromSenderProperty_ReturnsTrue()
        {
            var message = new IncomingMessage(1, "Property1", "4");

            SetupDevicesRepositoryForSender();
            SetupPropertyTypesRepositoryForSender();
            SetupConnectionsWithSender(ConnectionsToSetup.None);

            _fixture.PropertyTypeOfSender.IsSenderProperty = true;

            var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, null);
            var result = await handler.HandleMessage();

            Assert.True(result);
        }

        [Fact]
        public async void HandleMessage_ListenerCanOnlyReceiveMessagesWhileBeingOnlineAndItIsOffline_MessageSendingIsNotExecuted()
        {
            var message = new IncomingMessage(1, "Property1", "4");

            SetupDevicesRepositoryForSender();
            SetupPropertyTypesRepositoryForSender();
            SetupConnectionsWithSender(ConnectionsToSetup.OnlyOfflineDisabled);
            SetUpIotServiceOnlineStatusForOfflineDisabledListener(false);

            var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, LoggerMock.Object);

            await handler.HandleMessage();

            LoggerMock.Verify(n => n.Log($"EventHandler - Online check failed for listener of ID = {_fixture.ConnectionWithOffilineDisabled.ListenerDevice.Id}. Message will not be sent."));
            IotServiceMock.Verify(n => n.SendToListenerAsync(It.IsAny<MessageForListener>()), Times.Never());
        }

        [Fact]
        public async void HandleMessage_ListenerCanOnlyReceiveMessagesWhileBeingOnlineAndItIsOnline_MessageSendingIsExecuted()
        {
            var message = new IncomingMessage(1, "Property1", "4");

            SetupDevicesRepositoryForSender();
            SetupPropertyTypesRepositoryForSender();
            SetupConnectionsWithSender(ConnectionsToSetup.OnlyOfflineDisabled);

            IotServiceMock = new Mock<IIotService>(MockBehavior.Loose);
            SetUpIotServiceOnlineStatusForOfflineDisabledListener(true);

            var iotMessage = new MessageForListener(_fixture.OfflineDisabledListenerDevice.Id.ToString(), _fixture.PropertyTypeOfOfflineDisabledListener.Name, "4");

            var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, LoggerMock.Object);

            await handler.HandleMessage();

            LoggerMock.Verify(n => n.Log($"EventHandler - Online check passed for listener of ID = {_fixture.ConnectionWithOffilineDisabled.ListenerDevice.Id}. Message will be sent."), Times.Once());
            IotServiceMock.Verify(n => n.SendToListenerAsync(iotMessage), Times.Once());
        }

        [Fact]
        public async void HandleMessage_TwoListenersAreAttachedAndOnlyOneIsCapableToReceiveMessages_MessageSentToOneAndNotSentToTheOther()
        {
            var message = new IncomingMessage(1, "Property1", "4");

            SetupDevicesRepositoryForSender();
            SetupPropertyTypesRepositoryForSender();
            SetupConnectionsWithSender(ConnectionsToSetup.BothOfflineAndOnline);

            IotServiceMock = new Mock<IIotService>(MockBehavior.Loose);
            SetUpIotServiceOnlineStatusForOfflineDisabledListener(false);

            var iotMessageForOfflineDisabled = new MessageForListener(_fixture.OfflineDisabledListenerDevice.Id.ToString(), _fixture.PropertyTypeOfOfflineDisabledListener.Name, "4");
            var iotMessageForOfflineEnabled = new MessageForListener(_fixture.OfflineEnabledListenerDevice.Id.ToString(), _fixture.PropertyTypeOfOfflineEnabledListener.Name, "true");

            var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, LoggerMock.Object);

            await handler.HandleMessage();

            LoggerMock.Verify(n => n.Log($"EventHandler - Online check failed for listener of ID = {_fixture.ConnectionWithOffilineDisabled.ListenerDevice.Id}. Message will not be sent."), Times.Once());
            IotServiceMock.Verify(n => n.SendToListenerAsync(iotMessageForOfflineDisabled), Times.Never());
            IotServiceMock.Verify(n => n.SendToListenerAsync(iotMessageForOfflineEnabled), Times.Once());
            IotServiceMock.Verify(n => n.SendToListenerAsync(It.IsAny<MessageForListener>()), Times.Once());
        }

        [Fact]
        public async void HandleMesage_TwoListenersAttachedAndBothAreCapableToReceiveMessage_MessageIsSentToBoth()
        {
            var message = new IncomingMessage(1, "Property1", "4");

            SetupDevicesRepositoryForSender();
            SetupPropertyTypesRepositoryForSender();
            SetupConnectionsWithSender(ConnectionsToSetup.BothOfflineAndOnline);

            IotServiceMock = new Mock<IIotService>(MockBehavior.Loose);
            SetUpIotServiceOnlineStatusForOfflineDisabledListener(true);

            var iotMessageForOfflineDisabled = new MessageForListener(_fixture.OfflineDisabledListenerDevice.Id.ToString(), _fixture.PropertyTypeOfOfflineDisabledListener.Name, "4");
            var iotMessageForOfflineEnabled = new MessageForListener(_fixture.OfflineEnabledListenerDevice.Id.ToString(), _fixture.PropertyTypeOfOfflineEnabledListener.Name, "true");

            var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object, LoggerMock.Object);

            await handler.HandleMessage();

            LoggerMock.Verify(n => n.Log($"EventHandler - Online check passed for listener of ID = {_fixture.ConnectionWithOffilineDisabled.ListenerDevice.Id}. Message will be sent."), Times.Once());
            IotServiceMock.Verify(n => n.SendToListenerAsync(iotMessageForOfflineDisabled), Times.Once());
            IotServiceMock.Verify(n => n.SendToListenerAsync(iotMessageForOfflineEnabled), Times.Once());
            IotServiceMock.Verify(n => n.SendToListenerAsync(It.IsAny<MessageForListener>()), Times.Exactly(2));
        }

        private void SetUpIotServiceOnlineStatusForOfflineDisabledListener(bool isOnline)
        {
            var taskSource = new TaskCompletionSource<bool>();
            taskSource.SetResult(isOnline);

            IotServiceMock.Setup(n => n.IsDeviceOnline(_fixture.OfflineDisabledListenerDevice.Id.ToString()))
                .Returns(taskSource.Task);
        }

        private void SetupConnectionsWithSender(ConnectionsToSetup connectionsToSetup)
        {
            List<Connection> availableConnections = new List<Connection>();
            if (connectionsToSetup == ConnectionsToSetup.BothOfflineAndOnline)
                availableConnections = new List<Connection> { _fixture.ConnectionWithOffilineEnabled, _fixture.ConnectionWithOffilineDisabled };
            else if (connectionsToSetup == ConnectionsToSetup.OnlyOfflineEnabled)
                availableConnections = new List<Connection> { _fixture.ConnectionWithOffilineEnabled };
            else if (connectionsToSetup == ConnectionsToSetup.OnlyOfflineDisabled)
                availableConnections = new List<Connection> { _fixture.ConnectionWithOffilineDisabled };

            ConnectionRepositoryMock.Setup(n => n.GetDeviceConnections(_fixture.SenderDeviceWithoutProps))
                .Returns(availableConnections);
            UnitOfWorkMock.Setup(n => n.Connections).Returns(ConnectionRepositoryMock.Object);
        }

        private void SetupPropertyTypesRepositoryForSender()
        {
            UnitOfWorkMock.Setup(n => n.PropertyTypes).Returns(PropertyTypeRepositoryMock.Object);
            PropertyTypeRepositoryMock.Setup(n => n.GetPropertiesOfDevice(_fixture.SenderDeviceType))
                .Returns(_fixture.PropertyTypes);
        }

        private void SetupDevicesRepositoryForSender()
        {
            DeviceRepositoryMock.Setup(n => n.Get(1)).Returns(_fixture.SenderDeviceWithoutProps);
            DeviceRepositoryMock.Setup(n => n.GetDeviceType(_fixture.SenderDeviceWithProps.Id)).Returns(_fixture.SenderDeviceWithProps.DeviceType);
            UnitOfWorkMock.Setup(n => n.Devices).Returns(DeviceRepositoryMock.Object);
        }

        public enum ConnectionsToSetup
        {
            None,
            OnlyOfflineEnabled,
            OnlyOfflineDisabled,
            BothOfflineAndOnline
        }
    }
}
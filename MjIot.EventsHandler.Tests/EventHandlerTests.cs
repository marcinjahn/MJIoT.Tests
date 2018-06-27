using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using MJIot.Storage.Models;
using MjIot.EventsHandler.Services;
using MJIot.Storage.Models.Repositiories;
using MjIot.Storage.Models.EF6Db;
using MjIot.EventsHandler.Models;

namespace MjIot.EventsHandler.Tests
{
    public class EventHandlerTests
    {
        private Mock<IUnitOfWork> UnitOfWorkMock { get; set; }
        private Mock<IIotService> IotServiceMock { get; set; }
        private Mock<IDeviceRepository> DeviceRepositoryMock { get; set; }
        private Mock<IPropertyTypeRepository> PropertyTypeRepositoryMock { get; set; }
        private Mock<IConnectionRepository> ConnectionRepositoryMock { get; set; }

        public EventHandlerTests()
        {
            UnitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            IotServiceMock = new Mock<IIotService>(MockBehavior.Strict);
            DeviceRepositoryMock = new Mock<IDeviceRepository>(MockBehavior.Strict);
            PropertyTypeRepositoryMock = new Mock<IPropertyTypeRepository>(MockBehavior.Strict);
            ConnectionRepositoryMock = new Mock<IConnectionRepository>(MockBehavior.Strict);
        }

        [Fact]
        public void Constructor_SenderDeviceExists_NoExceptionIsThrown()
        {
            var message = new PropertyDataMessage(@"{DeviceId: ""1"",PropertyName: ""Property1"",PropertyValue: ""false""}");

            PropertyTypeRepositoryMock = new Mock<IPropertyTypeRepository>(MockBehavior.Loose);
            var mockedDevice = new Device();

            DeviceRepositoryMock.Setup(n => n.Get(1)).Returns(mockedDevice);
            UnitOfWorkMock.Setup(n => n.Devices).Returns(DeviceRepositoryMock.Object);
            UnitOfWorkMock.Setup(n => n.PropertyTypes).Returns(PropertyTypeRepositoryMock.Object);

            try
            {
                var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object);
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
            var message = new PropertyDataMessage(@"{DeviceId: ""1"",PropertyName: ""Property1"",PropertyValue: ""false""}");
            var mockedDeviceType = new DeviceType();
            var mockedDevice = new Device { DeviceType = mockedDeviceType };
            var mockedPropertyTypes = new PropertyType[]
            {
                new PropertyType {Name = "Property1"},
                new PropertyType {Name = "Property2"},
                new PropertyType {Name = "Property3"}
            };

            DeviceRepositoryMock.Setup(n => n.Get(1)).Returns(mockedDevice);
            PropertyTypeRepositoryMock.Setup(n => n.GetPropertiesOfDevice(mockedDeviceType)).Returns(mockedPropertyTypes);
            UnitOfWorkMock.Setup(n => n.Devices).Returns(DeviceRepositoryMock.Object);
            UnitOfWorkMock.Setup(n => n.PropertyTypes).Returns(PropertyTypeRepositoryMock.Object);

            var handler = new EventHandler(message, UnitOfWorkMock.Object, IotServiceMock.Object);
        }
    }
}

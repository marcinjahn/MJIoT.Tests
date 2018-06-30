using MjIot.Storage.Models.EF6Db;

namespace MjIot.EventsHandler.Tests
{
    public class EventHandlerFixture
    {
        public User User { get; private set; }
        public DeviceType SenderDeviceType { get; private set; }
        public DeviceType OfflineEnabledListenerDeviceType { get; private set; }
        public DeviceType OfflineDisabledListenerDeviceType { get; private set; }
        public Device SenderDeviceWithProps { get; private set; }
        public Device SenderDeviceWithoutProps { get; private set; }
        public Device OfflineEnabledListenerDevice { get; private set; }
        public Device OfflineDisabledListenerDevice { get; private set; }
        public PropertyType PropertyTypeOfSender { get; private set; }
        public PropertyType PropertyTypeOfOfflineEnabledListener { get; private set; }
        public PropertyType PropertyTypeOfOfflineDisabledListener { get; private set; }
        public PropertyType[] PropertyTypes { get; private set; }
        public Connection ConnectionWithOffilineEnabled { get; set; }
        public Connection ConnectionWithOffilineDisabled { get; set; }

        public EventHandlerFixture()
        {
            User = new User { Id = 2, Login = "user1", Password = "pass1" };

            SenderDeviceType = new DeviceType
            {
                Id = 3,
                IsAbstract = false,
                Name = "DeviceType1",
                OfflineMessagesEnabled = true
                //BaseDeviceType =
            };
            OfflineEnabledListenerDeviceType = new DeviceType
            {
                Id = 4,
                IsAbstract = false,
                Name = "DeviceType2",
                OfflineMessagesEnabled = true
            };

            OfflineDisabledListenerDeviceType = new DeviceType
            {
                Id = 5,
                IsAbstract = false,
                Name = "DeviceType3",
                OfflineMessagesEnabled = false
            };

            SenderDeviceWithProps = new Device
            {
                Id = 1,
                IoTHubKey = "123",
                User = User,
                DeviceType = SenderDeviceType
            };

            SenderDeviceWithoutProps = new Device
            {
                Id = 1,
                IoTHubKey = "123"
            };

            OfflineEnabledListenerDevice = new Device
            {
                Id = 2,
                IoTHubKey = "456",
                User = User,
                DeviceType = OfflineEnabledListenerDeviceType
            };

            OfflineDisabledListenerDevice = new Device
            {
                Id = 3,
                IoTHubKey = "789",
                User = User,
                DeviceType = OfflineDisabledListenerDeviceType
            };

            PropertyTypeOfSender = new PropertyType
            {
                Id = 1,
                Name = "Property1",
                DeviceType = SenderDeviceType,
                Format = PropertyFormat.Number,
                IsListenerProperty = false,
                IsSenderProperty = true,
                UIConfigurable = false
            };

            PropertyTypeOfOfflineEnabledListener = new PropertyType
            {
                Id = 2,
                Name = "Property2",
                DeviceType = OfflineEnabledListenerDeviceType,
                Format = PropertyFormat.Boolean,
                IsListenerProperty = true,
                IsSenderProperty = false,
                UIConfigurable = false
            };

            PropertyTypeOfOfflineDisabledListener = new PropertyType
            {
                Id = 3,
                Name = "Property3",
                DeviceType = OfflineDisabledListenerDeviceType,
                Format = PropertyFormat.String,
                IsListenerProperty = true,
                IsSenderProperty = true,
                UIConfigurable = false
            };

            ConnectionWithOffilineEnabled = new Connection
            {
                Id = 1,
                User = User,
                SenderDevice = SenderDeviceWithProps,
                SenderProperty = PropertyTypeOfSender,
                ListenerDevice = OfflineEnabledListenerDevice,
                ListenerProperty = PropertyTypeOfOfflineEnabledListener,
                Filter = ConnectionFilter.None,
                Calculation = ConnectionCalculation.None
            };

            ConnectionWithOffilineDisabled = new Connection
            {
                Id = 2,
                User = User,
                SenderDevice = SenderDeviceWithProps,
                SenderProperty = PropertyTypeOfSender,
                ListenerDevice = OfflineDisabledListenerDevice,
                ListenerProperty = PropertyTypeOfOfflineDisabledListener,
                Filter = ConnectionFilter.None,
                Calculation = ConnectionCalculation.None
            };

            PropertyTypes = new PropertyType[] { PropertyTypeOfSender, PropertyTypeOfOfflineEnabledListener, PropertyTypeOfOfflineDisabledListener };
        }
    }
}
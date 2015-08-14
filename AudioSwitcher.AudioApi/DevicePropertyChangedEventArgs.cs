using System;
using System.Linq.Expressions;

namespace AudioSwitcher.AudioApi
{
    public class DevicePropertyChangedEventArgs : DeviceChangedEventArgs
    {
        public DevicePropertyChangedEventArgs(IDevice dev, string propertyName = null)
            : base(dev, AudioDeviceEventType.PropertyChanged)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }

        private static string GetName(Expression<Func<IDevice, object>> exp)
        {
            var body = exp.Body as MemberExpression;

            if (body == null)
            {
                var ubody = (UnaryExpression) exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }

        public static DevicePropertyChangedEventArgs FromExpression(IDevice dev,
            Expression<Func<IDevice, object>> propertyNameExpression)
        {
            return new DevicePropertyChangedEventArgs(dev, GetName(propertyNameExpression));
        }
    }
}
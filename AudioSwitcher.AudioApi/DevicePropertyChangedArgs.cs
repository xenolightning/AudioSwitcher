using System;
using System.Linq.Expressions;

namespace AudioSwitcher.AudioApi
{
    public class DevicePropertyChangedArgs : DeviceChangedArgs
    {
        public DevicePropertyChangedArgs(IDevice dev, string propertyName = null)
            : base(dev, DeviceChangedType.PropertyChanged)
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

        public static DevicePropertyChangedArgs FromExpression(IDevice dev,
            Expression<Func<IDevice, object>> propertyNameExpression)
        {
            return new DevicePropertyChangedArgs(dev, GetName(propertyNameExpression));
        }
    }
}
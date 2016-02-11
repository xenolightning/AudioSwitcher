using System;
using System.Linq.Expressions;

namespace AudioSwitcher.AudioApi
{
    public class DevicePropertyChangedArgs : DeviceChangedArgs
    {
        public string PropertyName { get; private set; }

        public DevicePropertyChangedArgs(IDevice dev, string propertyName = null)
            : base(dev, DeviceChangedType.PropertyChanged)
        {
            PropertyName = propertyName;
        }

        private static string GetName(Expression<Func<IDevice, object>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Invalid Expression", nameof(propertyExpression));
            }
            return memberExpression.Member.Name;
        }


        public static DevicePropertyChangedArgs FromExpression(IDevice dev, Expression<Func<IDevice, object>> propertyNameExpression)
        {
            return new DevicePropertyChangedArgs(dev, GetName(propertyNameExpression));
        }
    }
}
using System;
using System.Threading.Tasks;

namespace AudioSwitcher.AudioApi.Sandbox
{
    public class SandboxDevice : Device
    {
        public string fullName;
        public DeviceIcon icon;
        public string iconPath;
        public Guid id;
        public string interfaceName;
        public bool isMuted;
        public string name;
        public DeviceState state;
        public DeviceType type;

        public override Guid Id
        {
            get
            {
                return id;
            }
        }

        public override string InterfaceName
        {
            get
            {
                return interfaceName;
            }
        }

        public override string Name
        {
            get
            {
                return name;
            }
            set
            {
            }
        }

        public override string FullName
        {
            get
            {
                return fullName;
            }
        }

        public override DeviceIcon Icon
        {
            get
            {
                return icon;
            }
        }

        public override string IconPath
        {
            get
            {
                return iconPath;
            }
        }

        public override DeviceState State
        {
            get
            {
                return state;
            }
        }

        public override DeviceType DeviceType
        {
            get
            {
                return type;
            }
        }

        public override bool IsMuted
        {
            get
            {
                return isMuted;
            }
        }

        public override double Volume { get; set; }

        public SandboxDevice(IAudioController controller)
            : base(controller)
        {
        }

        public override bool Mute(bool mute)
        {
            return isMuted = mute;
        }
    }
}
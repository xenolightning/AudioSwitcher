using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioSwitcher.AudioApi
{
    public interface IControllerPlugin
    {

        string Name
        {
            get;
        }

        Controller Controller
        {
            get;
            set;
        }

        void OnRegister();

        void OnUnregister();

    }
}

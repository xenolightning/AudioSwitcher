using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EasyHook;

namespace HookingSample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //Config.Register("Audio Switcher Default Device Hook", "AudioSwitcher.AudioApi.Hooking.dll");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

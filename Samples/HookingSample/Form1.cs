using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Hooking;
using AudioSwitcher.AudioApi.Hooking.ComObjects;

namespace HookingSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private DefaultDeviceHook ddHook;

        private void button1_Click(object sender, EventArgs e)
        {
            if (ddHook != null)
            {
                ddHook.UnHook();
                ddHook = null;
                return;
            }

            int pid = int.Parse(textBox1.Text);
            ddHook = new DefaultDeviceHook(pid, SystemDeviceId);
            ddHook.OnError += DdHookOnOnError;
            ddHook.Hook();
            //new DefaultDeviceHook.EntryPoint(null, null).Run(null, null);
        }

        private void DdHookOnOnError(int processId, Exception exception)
        {
            MessageBox.Show(exception.ToString());
        }

        private string SystemDeviceId(DataFlow dataFlow, Role role)
        {
            //return null;
            return new CoreAudioController().GetPlaybackDevices().First(x => x.ShortName == "Speakers" && x.Description.Contains("High")).SystemId;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var x = new CoreAudioController().DefaultPlaybackDevice;
            label1.Text = x.ShortName;
        }
    }
}

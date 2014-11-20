using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

namespace MemoryLeakTester
{
    public partial class MainForm : Form
    {
        private CoreAudioController _controller;
        private List<IDevice> _list;

        public MainForm()
        {
            InitializeComponent();
            _controller = new CoreAudioController();
            _list = new List<IDevice>();
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            var d = (await _controller.GetPlaybackDevicesAsync()).First(x => x.IsDefaultDevice);
            var nd = (await _controller.GetPlaybackDevicesAsync()).First(x => !x.IsDefaultDevice);
            var vd = (await _controller.GetPlaybackDevicesAsync()).First(x => x.Volume > 0);

            d.SetAsDefault();

            label1.Text = vd.Volume.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _list.Clear();
        }
    }
}

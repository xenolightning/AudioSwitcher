using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Hooking;
using AudioSwitcher.AudioApi.Session;
using Role = AudioSwitcher.AudioApi.Hooking.ComObjects.Role;

namespace HookingSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DefaultDeviceHook _hook;
        private Process _selectedProcess;
        private CoreAudioDevice _selectedAudioDevice;

        public ObservableCollection<Process> Processes
        {
            get;
            private set;
        }

        public ObservableCollection<IDevice> AudioDevices
        {
            get;
            private set;
        }

        public Process SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                OnPropertyChanged("SelectedProcess");
            }
        }

        public CoreAudioDevice SelectedAudioDevice
        {
            get { return _selectedAudioDevice; }
            set
            {
                _selectedAudioDevice = value;
                OnPropertyChanged("SelectedAudioDevice");
            }
        }

        public bool IsHookSet => Hook != null;

        public CoreAudioController Controller
        {
            get;
            private set;
        }

        public DefaultDeviceHook Hook
        {
            get { return _hook; }
            private set
            {
                _hook = value;
                OnPropertyChanged("Hook");
                OnPropertyChanged("IsHookSet");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Processes = new ObservableCollection<Process>();
            AudioDevices = new ObservableCollection<IDevice>();

            Controller = new CoreAudioController();

            DataContext = this;

            Controller.AudioDeviceChanged.Subscribe(x =>
            {
                Console.WriteLine("{0} - {1}", x.Device.Name, x.ChangedType.ToString());
            });

            Controller.DefaultPlaybackDevice.SetAsDefault();

            Controller.DefaultPlaybackDevice.GetCapability<IAudioSessionController>();

            Controller.DefaultCaptureDevice.PeakValueChanged.Subscribe(x =>
            {
                Console.WriteLine(x.PeakValue);
            });

            Controller.DefaultPlaybackDevice.PeakValueChanged.Subscribe(x =>
            {
                //Console.WriteLine(x.PeakValue);
            });

            Thread.Sleep(100);

            foreach (var audioSession in Controller.DefaultPlaybackDevice.GetCapability<IAudioSessionController>())
            {
                Console.WriteLine(audioSession.Id);
            }

            //foreach (var audioSession in Controller.DefaultPlaybackDevice.SessionController.All())
            //{
            //    audioSession.VolumeChanged.Subscribe(v =>
            //    {
            //        Console.WriteLine("{0} - {1}", v.Session.DisplayName, v.PeakValue);
            //    });

            //    audioSession.PeakValueChanged.Throttle(TimeSpan.FromMilliseconds(10)).Subscribe(v =>
            //    {
            //        Console.WriteLine("{0} - {1}", v.Session.DisplayName, v.PeakValue);
            //    });

            //    audioSession.MuteChanged.Subscribe(m =>
            //    {
            //        Console.WriteLine("{0} - {1}", m.Session.DisplayName, m.IsMuted);
            //    });
            //}

            Controller.DefaultPlaybackDevice.GetCapability<IAudioSessionController>()?.SessionCreated.Subscribe(x =>
            {
                Console.WriteLine("{0} - {1}", x.DisplayName, x.Volume);
                //x.VolumeChanged.Subscribe(v =>
                //{
                //});
            });

            Controller.DefaultPlaybackDevice.GetCapability<IAudioSessionController>()?.SessionDisconnected.Subscribe(x =>
            {
                Console.WriteLine(x);

                foreach (var session in Controller.DefaultPlaybackDevice.GetCapability<IAudioSessionController>())
                {
                    Console.WriteLine("{0} - {1}", session.DisplayName, session.Volume);
                }

            });
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Hook != null)
            {
                UnHook();
            }
        }

        private void RefreshProcesses(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                var processlist = Process.GetProcesses();
                Processes.Clear();
                foreach (var p in processlist.OrderBy(x => x.ProcessName))
                {
                    Processes.Add(p);
                }
            }));
        }

        private void HookProcess(object sender, RoutedEventArgs e)
        {
            Controller.DefaultCaptureDevice.SetAsDefaultAsync();

            if (Hook != null)
            {
                UnHook();
                return;
            }

            if (SelectedProcess == null || SelectedAudioDevice == null)
                return;

            var sId = SelectedAudioDevice.RealId;

            Hook = new DefaultDeviceHook((dataFlow, role) =>
            {
                if (role != Role.Communications)
                    return sId;

                return null;
            });

            if (Hook.Hook(SelectedProcess.Id))
            {
                Hook.Complete += pid =>
                {
                    UnHook();
                };
            }
            else
            {
                Hook = null;
                MessageBox.Show(this, "Could not hook process");
            }
        }

        private void UnHook()
        {
            if (Hook != null)
            {
                Hook.Dispose();
                Hook = null;
            }
        }

        private void RefreshAudioDevices(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                var devices = Controller.GetPlaybackDevices(DeviceState.Active);
                AudioDevices.Clear();
                foreach (var d in devices.OrderBy(x => x.Name))
                {
                    AudioDevices.Add(d);
                }
            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

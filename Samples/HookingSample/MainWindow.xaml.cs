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
        private Timer _hookCheckTimer;

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

        public bool IsHookSet
        {
            get
            {
                return Hook != null;
            }
        }

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

            Controller.DefaultPlaybackCommunicationsDevice.SetAsDefault();

            DataContext = this;

            _hookCheckTimer = new Timer(CheckHook, null, 0, 1000);

            foreach (var audioSession in Controller.DefaultPlaybackDevice.SessionController.All())
            {
                audioSession.VolumeChanged.Subscribe(v =>
                {
                    Console.WriteLine("{0} - {1}", v.Session.DisplayName, v.Volume);
                });

                audioSession.MuteChanged.Subscribe(m =>
                {
                    Console.WriteLine("{0} - {1}", m.Session.DisplayName, m.IsMuted);
                });
            }

            Controller.DefaultPlaybackDevice.SessionController.SessionChanged.Subscribe(x =>
            {
                if (x.ChangeType == AudioSessionChangedType.Created)
                {
                    var newSession = Controller.DefaultPlaybackDevice.SessionController.First(y => y.Id == x.SessionId);
                    newSession.VolumeChanged.Subscribe(v =>
                    {
                        Console.WriteLine("{0} - {1}", v.Session.DisplayName, v.Volume);
                    });
                }

                Console.WriteLine("{0} - {1}", x.ChangeType, x.SessionId);

                foreach (var session in Controller.DefaultPlaybackDevice.SessionController)
                {
                    Console.WriteLine("{0} - {1} - {2}", session.ProcessId, session.DisplayName, session.Volume);
                }
            });
        }

        private void CheckHook(object state)
        {
            //if (Hook != null && Hook.Status == EHookStatus.Inactive)
            //    UnHook();
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
            if (Hook != null)
            {
                UnHook();
                return;
            }

            if (SelectedProcess == null || SelectedAudioDevice == null)
                return;

            var sId = SelectedAudioDevice.RealId;

            Hook = new DefaultDeviceHook((dataFlow, role) => sId);

            if (Hook.Hook(SelectedProcess.Id))
            {
                Hook.OnComplete += () =>
                {
                    UnHook();
                    Controller.SetDefaultDevice(Controller.DefaultPlaybackDevice);
                };

                Controller.SetDefaultDevice(Controller.DefaultPlaybackDevice);
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

                Thread.Sleep(1000);

                Controller.SetDefaultDevice(Controller.DefaultPlaybackDevice);
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

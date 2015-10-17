using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Observables;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    [Collection("CoreAudio")]
    public class ControllerTests
    {
        private IAudioController CreateTestController()
        {
            return new CoreAudioController();
        }

        [Fact]
        public void CoreAudio_NumberOfHandlesAreWithinAcceptableRange()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            Debug.WriteLine("Handles Before: " + originalHandles);
            var controller = CreateTestController();

            for (var i = 0; i < 50; i++)
            {
                controller.GetDevices();
                var isDefault = controller.DefaultPlaybackDevice.SetAsDefault();
                Assert.True(isDefault);
                controller.GetPlaybackDevices();
            }

            var newHandles = Process.GetCurrentProcess().HandleCount;
            Debug.WriteLine("Handles After: " + newHandles);

            //*15 for each device and the handles it requires
            //*3 because that should cater for at least 2 copies of each device
            var maxHandles = controller.GetDevices().Count() * 15 * 3;

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
        }

        [Fact]
        public async Task CoreAudio_NumberOfHandlesAreWithinAcceptableRange_Async()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            Debug.WriteLine("Handles Before: " + originalHandles);
            var controller = CreateTestController();

            for (var i = 0; i < 50; i++)
            {
                await controller.GetCaptureDevicesAsync();
                var isDefault = await controller.DefaultPlaybackDevice.SetAsDefaultAsync();
                Assert.True(isDefault);
                await controller.GetPlaybackDevicesAsync();
            }

            var newHandles = Process.GetCurrentProcess().HandleCount;
            Debug.WriteLine("Handles After: " + newHandles);

            //*15 for each device and the handles it requires
            //*3 because that should cater for at least 2 copies of each device
            var maxHandles = controller.GetDevices().Count() * 15 * 3;

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
        }

        /// <summary>
        /// This test isn't overly reliable because it uses signals from the system.
        /// But it's almost impossible to test COM disposal properly
        /// </summary>
        [Fact]
        public async void CoreAudio_Controller_Dispose_EventPropagation()
        {
            var count = 0;
            var ev = new TaskCompletionSource<bool>();
            using (var outerController = new CoreAudioController())
            {
                using (var controller = new CoreAudioController())
                {

                    controller.AudioDeviceChanged.Subscribe(args =>
                    {
                        if (args.ChangedType != DeviceChangedType.DefaultDevice)
                            return;

                        count++;
                        ev.TrySetResult(true);
                    });

                    controller.DefaultPlaybackDevice.SetAsDefault();
                }

                outerController.DefaultPlaybackDevice.SetAsDefault();
            }

            await ev.Task;

            //The event should only fire once because the inner controller is disposed before the second fire
            Assert.Equal(1, count);
        }

    }
}

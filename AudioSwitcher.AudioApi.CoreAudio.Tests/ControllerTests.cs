using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.Observables;
using Xunit;

namespace AudioSwitcher.AudioApi.CoreAudio.Tests
{
    [Collection("CoreAudio_Controller")]
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
            int maxHandles, newHandles;
            Debug.WriteLine("Handles Before: " + originalHandles);
            using (var controller = CreateTestController())
            {

                for (var i = 0; i < 50; i++)
                {
                    controller.GetDevices();
                    var isDefault = controller.DefaultPlaybackDevice.SetAsDefault();
                    controller.GetPlaybackDevices();
                }

                newHandles = Process.GetCurrentProcess().HandleCount;
                Debug.WriteLine("Handles After: " + newHandles);

                //*15 for each device and the handles it requires
                //*3 because that should cater for at least 2 copies of each device
                maxHandles = controller.GetDevices(DeviceState.All).Count() * 8 * 2;
            }

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
        }

        [Fact]
        public async Task CoreAudio_NumberOfHandlesAreWithinAcceptableRange_Async()
        {
            var originalHandles = Process.GetCurrentProcess().HandleCount;
            int maxHandles, newHandles;
            Debug.WriteLine("Handles Before: " + originalHandles);
            using (var controller = CreateTestController())
            {

                for (var i = 0; i < 50; i++)
                {
                    await controller.GetCaptureDevicesAsync();

                    var isDefault = await controller.DefaultPlaybackDevice.SetAsDefaultAsync();
                    await controller.GetPlaybackDevicesAsync();
                }

                newHandles = Process.GetCurrentProcess().HandleCount;
                Debug.WriteLine("Handles After: " + newHandles);

                //*15 for each device and the handles it requires
                //*3 because that should cater for at least 2 copies of each device
                maxHandles = controller.GetDevices(DeviceState.All).Count() * 8 * 2;
            }

            //Ensure it doesn't blow out the handles
            Assert.True(newHandles - originalHandles < maxHandles);
        }

        /// <summary>
        /// This test isn't overly reliable because it uses signals from the system.
        /// But it's almost impossible to test COM disposal properly
        /// </summary>
        [Fact]
        public async Task CoreAudio_Controller_Dispose_EventPropagation()
        {
            var count = 0;
            var ev = new TaskCompletionSource<bool>();
            var complete = new TaskCompletionSource<bool>();

            using (var outerController = new CoreAudioController())
            {
                using (var controller = new CoreAudioController())
                {

                    controller.AudioDeviceChanged.Subscribe(args =>
                    {
                        if (args.ChangedType != DeviceChangedType.DefaultChanged)
                            return;

                        count++;
                        ev.TrySetResult(true);
                    }, () =>
                    {
                        complete.TrySetResult(true);
                    });

                    controller.DefaultPlaybackDevice.SetAsDefault();

                    await ev.Task;
                }

                outerController.DefaultPlaybackDevice.SetAsDefault();
            }

            await complete.Task;

            //The event should only fire once because the inner controller is disposed before the second fire
            Assert.Equal(1, count);
        }

    }
}

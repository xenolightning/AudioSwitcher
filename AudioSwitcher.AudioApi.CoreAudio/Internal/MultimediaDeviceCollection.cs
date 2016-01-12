using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal class MultimediaDeviceCollection : IEnumerable<IMultimediaDevice>, IDisposable
    {
        private IMultimediaDeviceCollection _multimediaDeviceCollection;

        internal MultimediaDeviceCollection(IMultimediaDeviceCollection parent)
        {
            ComThread.Assert();

            _multimediaDeviceCollection = parent;
        }

        public IEnumerator<IMultimediaDevice> GetEnumerator()
        {
            for (var index = 0; index < Count; index++)
            {
                yield return this[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Device count
        /// </summary>
        public int Count
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    uint result;
                    Marshal.ThrowExceptionForHR(_multimediaDeviceCollection.GetCount(out result));
                    return Convert.ToInt32(result);
                });
            }
        }

        /// <summary>
        ///     Get device by index
        /// </summary>
        /// <param name="index">Device index</param>
        /// <returns>Device at the specified index</returns>
        public IMultimediaDevice this[int index]
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    IMultimediaDevice result;
                    _multimediaDeviceCollection.Item(Convert.ToUInt32(index), out result);
                    return result;
                });
            }
        }

        public void Dispose()
        {
            if (_multimediaDeviceCollection != null)
            {
                Marshal.FinalReleaseComObject(_multimediaDeviceCollection);
                _multimediaDeviceCollection = null;
            }
        }
    }
}
/*
  LICENSE
  -------
  Copyright (C) 2007 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/
// this version modified for Audio Switcher

using System;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    /// <summary>
    ///     Property Store class, only supports reading properties at the moment.
    /// </summary>
    internal class PropertyStore
    {
        private readonly Mode _accessMode;
        private readonly IPropertyStore _storeInterface;

        /// <summary>
        ///     Creates a new property store
        /// </summary>
        /// <param name="store">IPropertyStore COM interface</param>
        /// <param name="accessMode">The mode to open the propertystore in. Read/Write</param>
        internal PropertyStore(IPropertyStore store, Mode accessMode)
        {
            _storeInterface = store;
            _accessMode = accessMode;
        }

        public Mode AccessMode
        {
            get { return _accessMode; }
        }

        /// <summary>
        ///     Property Count
        /// </summary>
        public int Count
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    uint result;
                    Marshal.ThrowExceptionForHR(_storeInterface.GetCount(out result));

                    //Will error if the count returns > max value
                    return Convert.ToInt32(result);
                });
            }
        }

        /// <summary>
        ///     Gets property by index
        /// </summary>
        /// <param name="index">Property index</param>
        /// <returns>The property</returns>
        public PropertyStoreProperty this[int index]
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    PropVariant result;
                    PropertyKey key = Get(index);
                    Marshal.ThrowExceptionForHR(_storeInterface.GetValue(ref key, out result));
                    return new PropertyStoreProperty(key, result);
                });
            }
        }

        /// <summary>
        ///     Indexer by guid
        /// </summary>
        /// <param name="key">Property Key</param>
        /// <returns>Property or null if not found</returns>
        public PropertyStoreProperty this[PropertyKey key]
        {
            get
            {
                return ComThread.Invoke(() =>
                {
                    for (int i = 0; i < Count; i++)
                    {
                        PropertyKey ikey = Get(i);
                        if ((ikey.FormatId == key.FormatId) && (ikey.PropertyId == key.PropertyId))
                        {
                            PropVariant result;
                            Marshal.ThrowExceptionForHR(_storeInterface.GetValue(ref ikey, out result));
                            return new PropertyStoreProperty(ikey, result);
                        }
                    }
                    return null;
                });
            }
            set
            {
                if (AccessMode == Mode.Read)
                    return;

                for (int i = 0; i < Count; i++)
                {
                    PropertyKey ikey = Get(i);
                    if ((ikey.FormatId == key.FormatId) && (ikey.PropertyId == key.PropertyId))
                    {
                        SetValue(ikey, value.Value);
                    }
                }
            }
        }

        /// <summary>
        ///     Contains property guid
        /// </summary>
        /// <param name="key">Looks for a specific key</param>
        /// <returns>True if found</returns>
        public bool Contains(PropertyKey key)
        {
            return ComThread.Invoke(() =>
            {
                for (int i = 0; i < Count; i++)
                {
                    PropertyKey ikey = Get(i);
                    if ((ikey.FormatId == key.FormatId) && (ikey.PropertyId == key.PropertyId))
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        /// <summary>
        ///     Gets property key at sepecified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Property key</returns>
        public PropertyKey Get(int index)
        {
            return ComThread.Invoke(() =>
            {
                PropertyKey key;
                Marshal.ThrowExceptionForHR(_storeInterface.GetAt((uint)index, out key));
                return key;
            });
        }

        /// <summary>
        ///     Gets property value at specified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Property value</returns>
        public PropVariant GetValue(int index)
        {
            return ComThread.Invoke(() =>
            {
                PropVariant result;
                PropertyKey key = Get(index);
                Marshal.ThrowExceptionForHR(_storeInterface.GetValue(ref key, out result));
                return result;
            });
        }

        /// <summary>
        ///     Sets property value of the property
        /// </summary>
        /// <returns>Property value</returns>
        public void SetValue(PropertyKey key, object value)
        {
            ComThread.Invoke(() =>
            {
                if (AccessMode == Mode.Read)
                    return;

                if (!Contains(key))
                    return;

                PropVariant result;
                Marshal.ThrowExceptionForHR(_storeInterface.GetValue(ref key, out result));
                result.Value = value;
                Marshal.ThrowExceptionForHR(_storeInterface.SetValue(ref key, ref result));
                _storeInterface.Commit();
            });
        }

        internal enum Mode
        {
            Read,
            ReadWrite,
            Write
        }
    }
}
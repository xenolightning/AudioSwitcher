using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;
using AudioSwitcher.AudioApi.CoreAudio.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    internal class CachedPropertyDictionary : IPropertyDictionary
    {
        private Dictionary<PropertyKey, object> _properties;
        private IPropertyStore _propertyStoreInteface;

        public CachedPropertyDictionary()
        {
            ComThread.Assert();

            _properties = new Dictionary<PropertyKey, object>();
        }

        public AccessMode Mode { get; private set; }

        public int Count => _properties?.Count ?? 0;

        public object this[PropertyKey key]
        {
            get
            {
                if (_properties.ContainsKey(key))
                    return _properties[key];

                return null;
            }
            set
            {
                if (Mode == AccessMode.Read)
                    return;

                SetValue(key, value);
            }
        }

        public bool Contains(PropertyKey key)
        {
            return _properties.ContainsKey(key);
        }

        public void Dispose()
        {
            _properties = null;
            ComThread.BeginInvoke(() => { _propertyStoreInteface = null; });
        }

        /// <summary>
        /// Will attempt to load the properties from the MMDevice. If it can't open, or the device is in 
        /// an invalid state it will continue to use it's current internal property cache
        /// </summary>
        /// <param name="device"></param>
        public void TryLoadFrom(IMultimediaDevice device)
        {
            var properties = GetProperties(device);

            if (properties.Count > 0)
                _properties = properties;
        }

        private Dictionary<PropertyKey, object> GetProperties(IMultimediaDevice device)
        {
            var properties = new Dictionary<PropertyKey, object>();
            //Opening in write mode, can cause exceptions to be thrown when not run as admin.
            //This tries to open in write mode if available
            try
            {
                device.OpenPropertyStore(StorageAccessMode.ReadWrite, out _propertyStoreInteface);
                Mode = AccessMode.ReadWrite;
            }
            catch
            {
                Debug.WriteLine("Cannot open property store in write mode");
            }

            if (_propertyStoreInteface == null)
            {
                Marshal.ThrowExceptionForHR(device.OpenPropertyStore(StorageAccessMode.Read, out _propertyStoreInteface));
                Mode = AccessMode.Read;
            }
            try
            {
                uint count;
                _propertyStoreInteface.GetCount(out count);
                for (uint i = 0; i < count; i++)
                {
                    PropertyKey key;
                    PropVariant variant;
                    _propertyStoreInteface.GetAt(i, out key);

                    _propertyStoreInteface.GetValue(ref key, out variant);

                    if (variant.IsSupported())
                        properties.Add(key, variant.Value);
                }
            }
            catch(Exception)
            {
                Debug.WriteLine("Cannot get property values");
                return new Dictionary<PropertyKey, object>();
            }

            return properties;
        }

        /// <summary>
        ///     Sets property value of the property
        /// </summary>
        /// <returns>Property value</returns>
        public void SetValue(PropertyKey key, object value)
        {
            ComThread.Assert();

            if (Mode == AccessMode.Read)
                return;

            if (!Contains(key))
                return;

            Marshal.ThrowExceptionForHR(_propertyStoreInteface.SetValue(ref key, ref value));
            _propertyStoreInteface.Commit();
        }
    }
}
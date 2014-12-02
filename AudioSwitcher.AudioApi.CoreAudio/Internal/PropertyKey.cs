using System;

namespace AudioSwitcher.AudioApi.CoreAudio
{
    /// <summary>
    ///     PROPERTYKEY is defined in wtypes.h
    /// </summary>
    public struct PropertyKey
    {
        /// <summary>
        ///     Format ID
        /// </summary>
        public readonly Guid FormatId;

        /// <summary>
        ///     Property ID
        /// </summary>
        public readonly int PropertyId;

        /// <summary>
        ///     <param name="formatId"></param>
        ///     <param name="propertyId"></param>
        /// </summary>
        public PropertyKey(Guid formatId, int propertyId)
        {
            FormatId = formatId;
            PropertyId = propertyId;
        }
    }
}
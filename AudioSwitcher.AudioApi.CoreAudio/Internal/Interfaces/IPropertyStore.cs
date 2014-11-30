using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.CoreAudio.Interfaces
{
    [Guid(ComIIds.PROPERTY_STORE_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyStore
    {
        [PreserveSig]
        int GetCount(
            [Out] [MarshalAs(UnmanagedType.U4)] out int propertyCount);

        [PreserveSig]
        int GetAt(
            [In] [MarshalAs(UnmanagedType.U4)] int propertyIndex,
            [Out] out PropertyKey propertyKey);

        [PreserveSig]
        int GetValue(
            [In] ref PropertyKey propertyKey,
            [Out] out PropVariant value);

        [PreserveSig]
        int SetValue(
            [In] ref PropertyKey propertyKey,
            [In] ref PropVariant value);

        [PreserveSig]
        int Commit();
    }
}

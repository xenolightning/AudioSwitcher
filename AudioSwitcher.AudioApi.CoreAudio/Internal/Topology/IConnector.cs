using System.Runtime.InteropServices;
using AudioSwitcher.AudioApi.CoreAudio.Interfaces;

namespace AudioSwitcher.AudioApi.CoreAudio.Topology
{
    [Guid(ComInterfaceIds.CONNECTOR_IID)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IConnector
    {
        [PreserveSig]
        int GetType([Out] [MarshalAs(UnmanagedType.I4)] out ConnectorType connectorType);

        [PreserveSig]
        int GetDataFlow([Out] [MarshalAs(UnmanagedType.I4)] out EDataFlow dataFlow);

        [PreserveSig]
        int ConnectTo([In] [MarshalAs(UnmanagedType.Interface)] IConnector connector);

        [PreserveSig]
        int Disconnect();

        [PreserveSig]
        int IsConnected([Out] [MarshalAs(UnmanagedType.Bool)] out bool isConnected);

        [PreserveSig]
        int GetConnectedTo([Out] [MarshalAs(UnmanagedType.Interface)] out IConnector connector);

        [PreserveSig]
        int GetConnectorIdConnectedTo([Out] [MarshalAs(UnmanagedType.LPWStr)] out string connectorId);

        [PreserveSig]
        int GetDeviceIdConnectedTo([Out] [MarshalAs(UnmanagedType.LPWStr)] out string deviceId);
    }
}

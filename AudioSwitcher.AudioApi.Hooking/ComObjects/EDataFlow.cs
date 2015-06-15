using System;

namespace AudioSwitcher.AudioApi.Hooking.ComObjects
{
    [Flags]
    public enum EDataFlow
    {
        Render,
        Capture,
        All
    };
}
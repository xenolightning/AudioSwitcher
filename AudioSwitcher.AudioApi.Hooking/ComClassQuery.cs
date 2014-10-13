using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace AudioSwitcher.AudioApi.Hooking
{
    [ComImport]
    [Guid("00000001-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IClassFactory
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int CreateInstance(
            [In, MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
            ref Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out object obj);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int LockServer(
            [In] bool fLock);
    }

    [ComImport]
    [Guid("B196B28F-BAB4-101A-B69C-00AA00341D07")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IClassFactory2
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        Object CreateInstance(
            [In, MarshalAs(UnmanagedType.Interface)] Object unused,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid);

        void LockServer(Int32 fLock);

        IntPtr GetLicInfo(); // TODO : an enum called LICINFO

        [return: MarshalAs(UnmanagedType.BStr)]
        String RequestLicKey(
            [In, MarshalAs(UnmanagedType.U4)] int reserved);

        [return: MarshalAs(UnmanagedType.Interface)]
        Object CreateInstanceLic(
            [In, MarshalAs(UnmanagedType.Interface)] object pUnkOuter,
            [In, MarshalAs(UnmanagedType.Interface)] object pUnkReserved,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid,
            [In, MarshalAs(UnmanagedType.BStr)] string bstrKey);
    }

    internal class ComClassQuery
    {
        public static unsafe void Query(ComClassInfo cci)
        {
            Guid classguid = cci.ClassType.GUID;
            Guid interfguid = cci.InterfaceType.GUID;
            Guid classfactoryguid = typeof (IClassFactory).GUID;
            Guid classfactory2guid = typeof (IClassFactory2).GUID;
            object classinstance = null;

#if false
    // create an instance via .NET built-in functionality
    // vtable might be hijacked by rpcrt4.dll
            classinstance = cci.ClassType.InvokeMember("", BindingFlags.CreateInstance, null, null, null);
#endif
#if false
    // create via ole-convenience-function
    // vtable might be hijacked by rpcrt4.dll
            OLE32.CoCreateInstance(ref classguid, null, 1 + 4, ref interfguid, out classinstance);
#endif
#if false
    // create via ole-functions
    // vtable might be hijacked by rpcrt4.dll
            try
            {
                if (classinstance == null)
                {
                    object classfactoryO;
                    OLE32.CoGetClassObject(ref classguid, 1 + 4, 0, ref classfactoryguid, out classfactoryO);
                    IClassFactory classfactory = (IClassFactory)classfactoryO;
                    classfactory.CreateInstance(null, ref interfguid, out classinstance);
                    Marshal.FinalReleaseComObject(classfactory);
                }
            }
            catch { }
            try
            {
                if (classinstance == null)
                {
                    object classfactoryO;
                    OLE32.CoGetClassObject(ref classguid, 1 + 4, 0, ref classfactoryguid, out classfactoryO);
                    IClassFactory2 classfactory = (IClassFactory2)classfactoryO;
                    classinstance = classfactory.CreateInstance(null, interfguid);
                    Marshal.FinalReleaseComObject(classfactory);
                }
            }
            catch { }
            if (classinstance == null)
            {
                // Error...
            }
#endif
#if true
            // create via raw dll functions
            // no chance for other people to hijack the vtable
            try
            {
                do
                {
                    RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\{" + classguid + "}\\InprocServer32");
                    if (rk == null)
                        break;
                    string classdllname = rk.GetValue(null).ToString();
                    IntPtr libH = KERNEL32.LoadLibrary(classdllname);
                    if (libH == IntPtr.Zero)
                        break;
                    IntPtr factoryFunc = KERNEL32.GetProcAddress(libH, "DllGetClassObject");
                    if (factoryFunc == IntPtr.Zero)
                        break;
                    var factoryDel =
                        (DllGetClassObjectDelegate)
                            Marshal.GetDelegateForFunctionPointer(factoryFunc, typeof (DllGetClassObjectDelegate));
                    object classfactoryO;
                    factoryDel(ref classguid, ref classfactoryguid, out classfactoryO);
                    if (classfactoryO == null)
                        break;
                    var classfactory = (IClassFactory) classfactoryO;
                    classfactory.CreateInstance(null, ref interfguid, out classinstance);
                    Marshal.FinalReleaseComObject(classfactory);
                } while (false);
            }
            catch
            {
            }
            try
            {
                if (classinstance == null)
                {
                    do
                    {
                        RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\{" + classguid + "}\\InprocServer32");
                        if (rk == null)
                            break;
                        string classdllname = rk.GetValue(null).ToString();
                        IntPtr libH = KERNEL32.LoadLibrary(classdllname);
                        if (libH == IntPtr.Zero)
                            break;
                        IntPtr factoryFunc = KERNEL32.GetProcAddress(libH, "DllGetClassObject");
                        if (factoryFunc == IntPtr.Zero)
                            break;
                        var factoryDel =
                            (DllGetClassObjectDelegate)
                                Marshal.GetDelegateForFunctionPointer(factoryFunc, typeof (DllGetClassObjectDelegate));
                        object classfactoryO;
                        factoryDel(ref classguid, ref classfactory2guid, out classfactoryO);
                        if (classfactoryO == null)
                            break;
                        var classfactory = (IClassFactory2) classfactoryO;
                        classinstance = classfactory.CreateInstance(null, interfguid);
                        Marshal.FinalReleaseComObject(classfactory);
                    } while (false);
                }
            }
            catch
            {
            }
            if (classinstance == null)
            {
                // Error...
            }
#endif

            IntPtr interfaceIntPtr = Marshal.GetComInterfaceForObject(classinstance, cci.InterfaceType);
            var interfaceRawPtr = (int***) interfaceIntPtr.ToPointer();
            // get vtable
            int** vTable = *interfaceRawPtr;
            // get com-slot-number (vtable-index) of function X
            // get function-address from vtable
            int mi_vto = Marshal.GetComSlotForMethodInfo(cci.Method);
            int* faddr = vTable[mi_vto];
            cci.MFunctionPointer = new IntPtr(faddr);
            // release intptr
            Marshal.Release(interfaceIntPtr);
            Marshal.FinalReleaseComObject(classinstance);
        }

        public class ComClassInfo : IDisposable
        {
            private readonly Type _mClassType;

            private readonly Type _mInterfaceType;

            private readonly MethodInfo _mMethodInfo;
            internal IntPtr MFunctionPointer = IntPtr.Zero;
            internal ProcessModule m_ModuleHandle;

            public ComClassInfo(Type classtype, Type interfacetype, string functionname)
            {
                _mClassType = classtype;
                _mInterfaceType = interfacetype;
                _mMethodInfo = _mInterfaceType.GetMethod(functionname);
            }

            public Type ClassType
            {
                get { return _mClassType; }
            }

            public Type InterfaceType
            {
                get { return _mInterfaceType; }
            }

            public string Functionname
            {
                get { return _mMethodInfo.Name; }
            }

            public MethodInfo Method
            {
                get { return _mMethodInfo; }
            }

            public IntPtr FunctionPointer
            {
                get { return MFunctionPointer; }
            }

            public ProcessModule LibraryOfFunction
            {
                get
                {
                    if (m_ModuleHandle == null)
                    {
                        DetermineModuleHandle();
                    }
                    return m_ModuleHandle;
                }
            }

            public void Dispose()
            {
                Marshal.Release(MFunctionPointer);
            }

            internal void DetermineModuleHandle()
            {
                Process pr = Process.GetCurrentProcess();
                foreach (ProcessModule pm in pr.Modules)
                {
                    if (MFunctionPointer.ToInt64() >= pm.BaseAddress.ToInt64() &&
                        MFunctionPointer.ToInt64() <= pm.BaseAddress.ToInt64() + pm.ModuleMemorySize)
                    {
                        m_ModuleHandle = pm;
                        return;
                    }
                }
                m_ModuleHandle = null;
            }
        }

        private delegate int DllGetClassObjectDelegate(
            ref Guid ClassId, ref Guid InterfaceId, [Out, MarshalAs(UnmanagedType.Interface)] out object ppunk);
    }
}
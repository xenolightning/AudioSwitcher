using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AudioSwitcher.AudioApi.Hooking
{
    [ComImport()]
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

    [ComImport()]
    [Guid("B196B28F-BAB4-101A-B69C-00AA00341D07")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IClassFactory2
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

    class ComClassQuery
    {
        public class ComClassInfo : IDisposable
        {
            public ComClassInfo(Type classtype, Type interfacetype, string functionname)
            {
                m_ClassType = classtype;
                m_InterfaceType = interfacetype;
                m_MethodInfo = m_InterfaceType.GetMethod(functionname);
            }

            Type m_ClassType;
            public Type ClassType { get { return m_ClassType; } }

            Type m_InterfaceType;
            public Type InterfaceType { get { return m_InterfaceType; } }

            MethodInfo m_MethodInfo;
            public string Functionname { get { return m_MethodInfo.Name; } }
            public MethodInfo Method { get { return m_MethodInfo; } }

            internal IntPtr m_FunctionPointer = IntPtr.Zero;
            public IntPtr FunctionPointer { get { return m_FunctionPointer; } }

            internal System.Diagnostics.ProcessModule m_ModuleHandle;
            public System.Diagnostics.ProcessModule LibraryOfFunction
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

            internal void DetermineModuleHandle()
            {
                System.Diagnostics.Process pr = System.Diagnostics.Process.GetCurrentProcess();
                foreach (System.Diagnostics.ProcessModule pm in pr.Modules)
                {
                    if (m_FunctionPointer.ToInt64() >= pm.BaseAddress.ToInt64() &&
                        m_FunctionPointer.ToInt64() <= pm.BaseAddress.ToInt64() + pm.ModuleMemorySize)
                    {
                        m_ModuleHandle = pm;
                        return;
                    }
                }
                m_ModuleHandle = null;
            }

            public void Dispose()
            {
                Marshal.Release(m_FunctionPointer);
            }
        }

        private delegate int DllGetClassObjectDelegate(ref Guid ClassId, ref Guid InterfaceId, [Out, MarshalAs(UnmanagedType.Interface)] out object ppunk);

        public static unsafe void Query(ComClassInfo cci)
        {
            Guid classguid = cci.ClassType.GUID;
            Guid interfguid = cci.InterfaceType.GUID;
            Guid classfactoryguid = typeof(IClassFactory).GUID;
            Guid classfactory2guid = typeof(IClassFactory2).GUID;
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
                if (classinstance == null)
                {
                    do
                    {
                        Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("CLSID\\{" + classguid.ToString() + "}\\InprocServer32");
                        if (rk == null)
                            break;
                        string classdllname = rk.GetValue(null).ToString();
                        IntPtr libH = KERNEL32.LoadLibrary(classdllname);
                        if (libH == IntPtr.Zero)
                            break;
                        IntPtr factoryFunc = KERNEL32.GetProcAddress(libH, "DllGetClassObject");
                        if (factoryFunc == IntPtr.Zero)
                            break;
                        DllGetClassObjectDelegate factoryDel = (DllGetClassObjectDelegate)Marshal.GetDelegateForFunctionPointer(factoryFunc, typeof(DllGetClassObjectDelegate));
                        object classfactoryO;
                        factoryDel(ref classguid, ref classfactoryguid, out classfactoryO);
                        if (classfactoryO == null)
                            break;
                        IClassFactory classfactory = (IClassFactory)classfactoryO;
                        classfactory.CreateInstance(null, ref interfguid, out classinstance);
                        Marshal.FinalReleaseComObject(classfactory);
                    } while (false);
                }
            }
            catch { }
            try
            {
                if (classinstance == null)
                {
                    do
                    {
                        Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("CLSID\\{" + classguid.ToString() + "}\\InprocServer32");
                        if (rk == null)
                            break;
                        string classdllname = rk.GetValue(null).ToString();
                        IntPtr libH = KERNEL32.LoadLibrary(classdllname);
                        if (libH == IntPtr.Zero)
                            break;
                        IntPtr factoryFunc = KERNEL32.GetProcAddress(libH, "DllGetClassObject");
                        if (factoryFunc == IntPtr.Zero)
                            break;
                        DllGetClassObjectDelegate factoryDel = (DllGetClassObjectDelegate)Marshal.GetDelegateForFunctionPointer(factoryFunc, typeof(DllGetClassObjectDelegate));
                        object classfactoryO;
                        factoryDel(ref classguid, ref classfactory2guid, out classfactoryO);
                        if (classfactoryO == null)
                            break;
                        IClassFactory2 classfactory = (IClassFactory2)classfactoryO;
                        classinstance = classfactory.CreateInstance(null, interfguid);
                        Marshal.FinalReleaseComObject(classfactory);
                    } while (false);
                }
            }
            catch { }
            if (classinstance == null)
            {
                // Error...
            }
#endif

            IntPtr interfaceIntPtr = Marshal.GetComInterfaceForObject(classinstance, cci.InterfaceType);
            int*** interfaceRawPtr = (int***)interfaceIntPtr.ToPointer();
            // get vtable
            int** vTable = *interfaceRawPtr;
            // get com-slot-number (vtable-index) of function X
            // get function-address from vtable
            int mi_vto = Marshal.GetComSlotForMethodInfo(cci.Method);
            int* faddr = vTable[mi_vto];
            cci.m_FunctionPointer = new IntPtr(faddr);
            // release intptr
            Marshal.Release(interfaceIntPtr);
            Marshal.FinalReleaseComObject(classinstance);
        }
    }

}
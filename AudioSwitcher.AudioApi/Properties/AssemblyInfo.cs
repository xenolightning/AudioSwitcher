using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]

#if DEBUG

[assembly: InternalsVisibleTo("AudioSwitcher.AudioApi.Tests")]
#endif
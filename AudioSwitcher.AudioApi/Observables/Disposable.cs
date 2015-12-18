using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioSwitcher.AudioApi.Observables
{
    public static class Disposable
    {

        public static IDisposable Empty
        {
            get { return DefaultDisposable.Instance; }
        }

    }
}

using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class CoreLibrary : ObjectInstance, IJavaScriptLibrary
    {
        public CoreLibrary(ScriptEngine engine)
            : base(engine)
        {
            PopulateFields();
            PopulateFunctions();
        }

        public string Name
        {
            get { return "Core"; }
        }

        public int Version
        {
            get { return 1; }
        }

        public void Add(ScriptEngine engine)
        {
            if (engine.GetGlobalValue(Name) == Undefined.Value)
                engine.SetGlobalValue(Name, this);
        }

        public void Remove(ScriptEngine engine)
        {
            if (engine.GetGlobalValue(Name) != Undefined.Value)
                engine.Global.Delete(Name, false);
        }
    }
}
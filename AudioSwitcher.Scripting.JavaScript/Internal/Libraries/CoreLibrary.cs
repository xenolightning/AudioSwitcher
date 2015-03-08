using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.Scripting.JavaScript.Internal.Libraries
{
    internal sealed partial class CoreLibrary : ObjectInstance, IScriptLibrary
    {
        public CoreLibrary(ScriptEngine engine)
            : base(engine)
        {
            PopulateFields();
            PopulateFunctions();
        }

    }
}
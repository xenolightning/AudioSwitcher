using Jurassic;

namespace AudioSwitcher.Scripting
{
    public interface IJavaScriptLibrary
    {
        string Name
        {
            get;
        }

        int Version
        {
            get;
        }

        void Add(ScriptEngine engine);

        void Remove(ScriptEngine engine);
    }
}
namespace AudioSwitcher.Scripting.JavaScript
{
    public class JSScript : IScript
    {
        public JSScript()
        {
            ScriptInfo = JSScriptInfo.Instance;
        }

        public string Name
        {
            get;
            set;
        }

        public IScriptSource Source
        {
            get;
            set;
        }

        public ScriptInfo ScriptInfo
        {
            get;
            private set;
        }
    }
}
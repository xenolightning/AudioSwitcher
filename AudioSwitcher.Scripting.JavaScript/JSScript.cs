namespace AudioSwitcher.Scripting.JavaScript
{
    public class JSScript : IScript
    {
        public JSScript()
        {
            ScriptInfo = JSScriptInfo.Instance;
        }

        public string Name { get; set; }

        public string Content { get; set; }

        public IScriptInfo ScriptInfo { get; private set; }
    }
}
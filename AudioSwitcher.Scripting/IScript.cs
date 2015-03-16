namespace AudioSwitcher.Scripting
{
    public interface IScript
    {
        string Name
        {
            get;
        }

        IScriptSource Source
        {
            get;
        }

        ScriptInfo ScriptInfo
        {
            get;
        }
    }
}
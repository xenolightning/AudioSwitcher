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

        IScriptInfo ScriptInfo
        {
            get;
        }

        IScript Clone();

    }
}
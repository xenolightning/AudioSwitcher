namespace AudioSwitcher.Scripting
{
    public interface IScript
    {
        string Name
        {
            get;
        }

        string Content
        {
            get;
        }

        IScriptInfo ScriptInfo
        {
            get;
        }
    }
}
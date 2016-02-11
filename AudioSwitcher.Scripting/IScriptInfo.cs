namespace AudioSwitcher.Scripting
{
    public interface IScriptInfo
    {

        string CommonName
        {
            get;
        }

        string OfficialName
        {
            get;
        }

        string VersionString
        {
            get;
        }

        string MediaType
        {
            get;
        }

        string SyntaxHighlightingCode
        {
            get;
        }

        string FileExtension
        {
            get;
        }

    }
}
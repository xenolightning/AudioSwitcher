namespace AudioSwitcher.Scripting.JavaScript
{
    public class JSScriptInfo : IScriptInfo
    {
        public static readonly JSScriptInfo Instance = new JSScriptInfo();

        private JSScriptInfo()
        {
            CommonName = "JavaScript";
            OfficialName = "ECMAScript";
            VersionString = "5.1";
            SyntaxHighlightingCode = "JavaScript";
            FileExtension = "js";
        }

        public string CommonName
        {
            get;
            private set;
        }

        public string OfficialName
        {
            get;
            private set;
        }

        public string VersionString
        {
            get;
            private set;
        }

        public string SyntaxHighlightingCode
        {
            get;
            private set;
        }

        public string FileExtension
        {
            get;
            private set;
        }
    }
}
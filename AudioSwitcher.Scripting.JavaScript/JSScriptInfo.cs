namespace AudioSwitcher.Scripting.JavaScript
{
    public class JSScriptInfo : ScriptInfo
    {
        public static readonly JSScriptInfo Instance = new JSScriptInfo();
        private readonly string _commonName;
        private readonly string _versionString;
        private readonly string _officialName;
        private readonly string _syntaxHighlightingCode;
        private readonly string _fileExtension;

        private JSScriptInfo()
        {
            _commonName = "JavaScript";
            _officialName = "ECMAScript";
            _versionString = "5.1";
            _syntaxHighlightingCode = "JavaScript";
            _fileExtension = "js";
        }

        public override string CommonName
        {
            get { return _commonName; }
        }

        public override string OfficialName
        {
            get { return _officialName; }
        }

        public override string VersionString
        {
            get { return _versionString; }
        }

        public override string SyntaxHighlightingCode
        {
            get { return _syntaxHighlightingCode; }
        }

        public override string FileExtension
        {
            get { return _fileExtension; }
        }
    }
}
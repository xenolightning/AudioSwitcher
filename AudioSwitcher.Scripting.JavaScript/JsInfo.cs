namespace AudioSwitcher.Scripting.JavaScript
{
    public class JsInfo : IScriptInfo
    {
        public static readonly JsInfo Instance = new JsInfo();

        private readonly string _commonName;
        private readonly string _versionString;
        private readonly string _officialName;
        private readonly string _syntaxHighlightingCode;
        private readonly string _fileExtension;
        private readonly string _mediaType;

        private JsInfo()
        {
            _commonName = "JavaScript";
            _officialName = "ECMAScript";
            _versionString = "5.1";
            _syntaxHighlightingCode = "JavaScript";
            _fileExtension = "js";
            _mediaType = "text/javascript";
        }

        public string CommonName => _commonName;

        public string OfficialName => _officialName;

        public string VersionString => _versionString;

        public string MediaType => _mediaType;

        public string SyntaxHighlightingCode => _syntaxHighlightingCode;

        public string FileExtension => _fileExtension;
    }
}
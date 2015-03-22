using System;

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

        private JsInfo()
        {
            _commonName = "JavaScript";
            _officialName = "ECMAScript";
            _versionString = "5.1";
            _syntaxHighlightingCode = "JavaScript";
            _fileExtension = "js";
        }

        public string CommonName
        {
            get { return _commonName; }
        }

        public string OfficialName
        {
            get { return _officialName; }
        }

        public string VersionString
        {
            get { return _versionString; }
        }

        public string SyntaxHighlightingCode
        {
            get { return _syntaxHighlightingCode; }
        }

        public Type ScriptEngineType
        {
            get { return typeof(JsEngine); }
        }

        public string FileExtension
        {
            get { return _fileExtension; }
        }
    }
}
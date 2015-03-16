using System;

namespace AudioSwitcher.Scripting
{
    [Serializable]
    public abstract class ScriptInfo
    {

        public abstract string CommonName
        {
            get;
        }

        public abstract string OfficialName
        {
            get;
        }

        public abstract string VersionString
        {
            get;
        }

        public abstract string SyntaxHighlightingCode
        {
            get;
        }

        public abstract string FileExtension
        {
            get;
        }

        public override bool Equals(object obj)
        {
            var scriptInfo = obj as ScriptInfo;

            if (scriptInfo == null)
                return base.Equals(obj);

            return Equals(scriptInfo);
        }
        protected bool Equals(ScriptInfo other)
        {
            return OfficialName == other.OfficialName && VersionString == other.VersionString;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ CommonName.GetHashCode();
                result = (result * 397) ^ OfficialName.GetHashCode();
                result = (result * 397) ^ VersionString.GetHashCode();
                result = (result * 397) ^ SyntaxHighlightingCode.GetHashCode();
                result = (result * 397) ^ FileExtension.GetHashCode();
                return result;
            }
        }
    }
}
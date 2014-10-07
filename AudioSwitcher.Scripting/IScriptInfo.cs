using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioSwitcher.Scripting
{
    public interface IScriptInfo
    {

        string CommonName { get; }

        string OfficialName { get; }

        string VersionString { get; }

        string SyntaxHighlightingCode { get; }

        string FileExtension { get; }
    }
}

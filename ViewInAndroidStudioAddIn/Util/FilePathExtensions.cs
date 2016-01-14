using System;
using MonoDevelop.Core;

namespace Taiste.ViewInAndroidStudio.Util
{
    public static class FilePathExtensions
    {
        public static string GetPathPartAfterDirectory (this FilePath path, string dirName)
        {
            return path.ToString ()
                .Split (new []{ dirName }, StringSplitOptions.None) [1]
                .Substring (1);
        }
    }
}


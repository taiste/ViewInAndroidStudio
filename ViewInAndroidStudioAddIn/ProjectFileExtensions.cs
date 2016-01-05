using System;
using MonoDevelop.Projects;
using System.Linq;

namespace ViewInAndroidStudio
{
    public static class ProjectFileExtensions
    {
        public static bool IsResource(this ProjectFile file) 
        {
            if (file == null)
                return false;
            return file.BuildAction == "AndroidResource";
        }


        public static bool IsResourceXmlFile(this ProjectFile file){
            if (file == null)
                return false;

            bool isXml = file.Name.Split ('.').Last ().ToLowerInvariant().EndsWith("xml");

            return isXml && IsResource(file);
        }
    }
}


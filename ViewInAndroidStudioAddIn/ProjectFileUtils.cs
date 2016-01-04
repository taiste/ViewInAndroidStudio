using System;
using MonoDevelop.Projects;
using System.Linq;

namespace ViewInAndroidStudio
{
    public static class ProjectFileUtils
    {
        public static bool IsResource(ProjectFile file) 
        {
            if (file == null)
                return false;
            return file.BuildAction == "AndroidResource";
        }


        public static bool IsResourceXmlFile(ProjectFile file){
            if (file == null)
                return false;

            bool isXml = file.Name.Split ('.').Last ().ToLowerInvariant().EndsWith("xml");

            return isXml && IsResource(file);
        }
    }
}


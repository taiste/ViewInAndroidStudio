using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Projects;

namespace Taiste.ViewInAndroidStudio.Util
{
    public static class ProjectFileExtensions
    {
        public static bool IsResource (this ProjectFile file)
        {
            if (file == null)
                return false;
            return file.BuildAction == "AndroidResource";
        }


        public static bool IsResourceXmlFile (this ProjectFile file)
        {
            if (file == null)
                return false;

            bool isXml = file.Name.Split ('.').Last ().ToLowerInvariant ().EndsWith ("xml");

            return isXml && file.IsResource ();
        }



        public static FilePath GetAndroidStudioFilePath (this ProjectFile file)
        {
            if (!file.IsResource ()) {
                return null;
            }

            return file.Project.GetAndroidStudioProjectResourceDirectoryPath ()
                   .Combine (file.FilePath.GetPathPartAfterDirectory ("Resources"));

        }
    }
}


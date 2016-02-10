using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Gtk;
using Mono.Unix.Native;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Projects;
using Taiste.ViewInAndroidStudio.Commands;
using Taiste.ViewInAndroidStudio.Preferences;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Taiste.ViewInAndroidStudio.Util
{
    public static class ProjectExtensions
    {
        const string ArchiveName = "AndroidTemplate.zip";


        static readonly string[] SupportLibs = {
            "com.android.support:appcompat-v7",
            "com.android.support:design",
            "com.android.support:support-v4",
            "com.android.support:cardview-v7",
            "com.android.support:gridlayout-v7",
            "com.android.support:recyclerview-v7",
            "com.android.support:preference-v7",
            "com.android.support:support-v13",
            "com.android.support:preference-v14",
            "com.android.support:support-annotations",
            "com.android.support:design",
            "com.android.support:percent",
        };

        public static void CreateAndroidStudioProject (this Project p)
        {
            FilePath androidStudioProjectPath = GetAndroidStudioProjectPath (p);

            try {
                if (Directory.Exists (androidStudioProjectPath)) {
                    Directory.Delete (androidStudioProjectPath, true);
                }
                Directory.CreateDirectory (androidStudioProjectPath);
            } catch (IOException e) {
                GtkHelpers.ShowDialog (String.Format ("Could not (re)create project directory: {0}", e.Message), MessageType.Error);
                return;
            }

            p.CreateAndroidStudioProjectStructure ();

            ViewHandler.OpenFileInAndroidStudio (androidStudioProjectPath.Combine ("build.gradle"));
        }

        static void CreateAndroidStudioProjectStructure (this Project p)
        {
            
            FilePath archivePath = 
                new FilePath (Assembly.GetExecutingAssembly ().Location)
                    .ParentDirectory.Combine (ArchiveName);

            var xamarinResourcePath = p.GetResourceDirectoryPath ();
            var androidStudioResourcePath = p.GetAndroidStudioProjectResourceDirectoryPath ();
            try {
                ZipFile.ExtractToDirectory(archivePath,p.GetAndroidStudioProjectPath());

                LinkResourceFiles(xamarinResourcePath, androidStudioResourcePath);

                p.MakeAliasFile();
                p.FinishGradleFile();

            } catch (IOException e) {
                GtkHelpers.ShowDialog (String.Format("Could not (re)create project structure: {0}", e.Message), MessageType.Error);
            }

        }

        static void LinkResourceFiles(FilePath oldPath, FilePath newPath){
            if (Environment.OSVersion.Platform != PlatformID.Unix &&
                Environment.OSVersion.Platform != PlatformID.MacOSX) {
                throw new NotImplementedException ("Not Implemented for current platform");
            }

            foreach (var dirPath in Directory.EnumerateDirectories(oldPath)){
                var directoryName = new FilePath (dirPath).FileName;
                bool isLayout = directoryName.StartsWith ("layout");
                var newDirPath = newPath.Combine(directoryName);
                Directory.CreateDirectory(newDirPath);
               
                foreach (var fileName in Directory.EnumerateFiles(dirPath)){
                    var fName = new FilePath (fileName).FileName;
                    var oldFilePath = new FilePath(dirPath).Combine(fName);

                    var newFilePath = newDirPath.Combine(TransformFileName(fName, isLayout));
                    Syscall.symlink(oldFilePath, newFilePath);
                }
            }

        }

        static string ConvertCharToUniqueString(char c){
            if (Char.IsDigit(c) ||
                (c >= 'a' && c <= 'z')||
                (c >= 'A' && c <= 'Z')||
                c == '_' ||
                c == '.') {
                return ""+c;
            }
            string res = "";
            while (c > ((char)0)) {
                c--;
                res = (char)('a' + c % 26) + res;
                c = (char)(c / 26);
            }
            return "_" + res;
        }

        static string TransformFileName(string fileName, bool isLayout){
            if (fileName.EndsWith ("axml")) {
                fileName = String.Join (".", fileName.Substring (0, fileName.LastIndexOf ('.')), "xml");
            }

            fileName = Regex.Replace (fileName, "([A-Z])", (match) => ((match.Index != 0)? "_":"") + match.Value.ToLower ());
            fileName = String.Join ("", fileName.Select (c => ConvertCharToUniqueString (c)));
            return (isLayout?"_res_":"") + fileName;
        }

        static IEnumerable<ProjectFile> SelectDistinctFilesWithCapital(this IEnumerable<ProjectFile> filePaths){
            return filePaths.Where(f => new FilePath(f.Name).FileName.Any(Char.IsUpper)).Distinct();
        }

        static List<ProjectFile> GetResourceFilesByType(this Project p, string fileType ){
            var dirs = Directory.EnumerateDirectories(p.GetResourceDirectoryPath())
                .Where(d => new FilePath(d).FileName.StartsWith(fileType));
            return  p.Files.Where (f => 
                dirs.Any (d => f.FilePath.IsChildPathOf (d)))
                    .SelectDistinctFilesWithCapital ()
                    .ToList ();
            
        }

        static readonly string[] AliasedFileTypes = {
            "layout", "drawable", "mipmap"
        };

        static void MakeAliasFile(this Project p){

            var oldPath = p.GetResourceDirectoryPath ();
            var newPath = p.GetAndroidStudioProjectResourceDirectoryPath ();
            var valuesDir = newPath.Combine("values");

            if (!Directory.Exists(valuesDir)){
                Directory.CreateDirectory(valuesDir);
            }

            var aliasFile = valuesDir.Combine("__aliases_.xml");
            XElement aliasRoot = null;
            if (File.Exists(aliasFile)){
                try {
                    using (var input = File.OpenRead(aliasFile)){
                        aliasRoot = XElement.Load(new XmlTextReader(input));
                    }
                } catch (Exception e){
                    System.Diagnostics.Debug.WriteLine("Could not read layout alias file");                        
                }
                File.Delete(aliasFile);
            }

            if (aliasRoot == null){
                aliasRoot = new XElement(XName.Get("resources"));
            }

            foreach (var fileType in AliasedFileTypes) {
                AddAliases (fileType, p.GetResourceFilesByType(fileType), aliasRoot);
              }

            using (var writer = new XmlTextWriter(aliasFile, System.Text.Encoding.UTF8)){
                writer.Formatting = Formatting.Indented;
                aliasRoot.WriteTo(writer);
            }

        }

        static void AddAliases (string resourceType, List<ProjectFile> files, XElement aliasRoot)
        {
            foreach (var file in files) {
                var fileName = Directory.GetFiles(file.FilePath.ParentDirectory).FirstOrDefault(name => new FilePath(name).FileName.ToLower() == file.FilePath.FileName.ToLower());
                fileName = new FilePath(fileName).FileName;
                fileName = fileName.Substring (0, fileName.LastIndexOf ('.'));

                var layoutName = "@"+resourceType+"/" + TransformFileName (fileName, resourceType == "layout"); //TODO smarter check

                var aliasName = new FilePath(file.Name).FileName;
                aliasName = aliasName.Substring (0, aliasName.LastIndexOf ('.'));

                XElement aliasElement = new XElement (XName.Get ("item"));
                aliasElement.Add (new XAttribute (XName.Get ("name"), aliasName));
                aliasElement.Add (new XAttribute (XName.Get ("type"), resourceType));
                aliasElement.Value = layoutName;
                aliasRoot.Add (aliasElement);
            }
        }

        static void FinishGradleFile(this Project p) {
            FilePath pathToBuildGradle = p.GetAndroidStudioProjectPath().Combine("app").Combine ("build.gradle");
            string contents = null;
            using (StreamReader reader = new StreamReader (File.OpenRead (pathToBuildGradle))) {
                contents = reader.ReadToEnd ();
            }

            Regex.Replace (contents, "compileSdkVersion ([0-9]+)", (match) => 
                "compileSdkVersion "+AddInPreferences.CompileSdkVersion
            );

            Regex.Replace (contents, "buildToolsVersion \"([0-9.]+)\"", (match) => 
                "buildToolsVersion \""+AddInPreferences.BuildToolsVersion+"\""
            );

            Regex.Replace (contents, "minSdkVersion ([0-9]+)", (match) => 
                "minSdkVersion "+AddInPreferences.MinSdkVersion
            );

            Regex.Replace (contents, "targetSdkVersion ([0-9]+)", (match) => 
                "targetSdkVersion "+AddInPreferences.CompileSdkVersion
            );

            StringBuilder fileContents = new StringBuilder (contents);

            fileContents.AppendLine ("dependencies {");
            fileContents.AppendLine ("    compile fileTree(dir: 'libs', include: ['*.jar'])");
            fileContents.AppendLine ("    testCompile 'junit:junit:4.12'");

            foreach (var package in SupportLibs) {
                fileContents.AppendLine ("    compile '" + package + ":" + AddInPreferences.SupportVersion + "'");
            }
            fileContents.AppendLine ("}");

            using (StreamWriter writer = new StreamWriter(File.OpenWrite(pathToBuildGradle))){
                writer.Write (fileContents);
            }



        }

        public static FilePath GetAndroidStudioProjectPath (this Project p)
        {
            return AddInPreferences.ProjectsDirectoryPath.Combine (p.Name);
        }

        public static FilePath GetAndroidStudioProjectResourceDirectoryPath (this Project p)
        {
            return GetAndroidStudioProjectPath (p).Combine ("app").Combine ("src").Combine ("main").Combine ("res");
        }

        public static FilePath GetResourceDirectoryPath (this Project p)
        {
            return p.BaseDirectory.Combine ("Resources");
        }

        public static bool IsAndroidProject (this Project p)
        {
            return p.GetProjectTypes ().Contains ("MonoDroid");
        }
    }
}


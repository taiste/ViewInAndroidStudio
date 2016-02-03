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

namespace Taiste.ViewInAndroidStudio.Util
{
    public static class ProjectExtensions
    {
        const string ArchiveName = "TaisteAndroid.zip";

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

                MakeLayoutAliasFile(xamarinResourcePath, androidStudioResourcePath);

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

        static string TransformFileName(string fileName, bool isLayout){
            if (fileName.EndsWith ("axml")) {
                fileName = String.Join (".", fileName.Substring (0, fileName.LastIndexOf ('.')), "xml");
            }

            fileName = Regex.Replace (fileName, "([A-Z])", (match) => ((match.Index != 0)? "_":"") + match.Value.ToLower ());

            return (isLayout?"_res_":"") + fileName;
        }

        static void MakeLayoutAliasFile(FilePath oldPath, FilePath newPath){
            var layoutDirs = Directory.EnumerateDirectories(oldPath)
                .Where(d => new FilePath(d).FileName.StartsWith("layout")).ToList();

            var valuesDir = newPath.Combine("values");

            var layoutFiles = layoutDirs.Select(d => Directory.EnumerateFiles(oldPath.Combine(d)))
                .Aggregate(new HashSet<string>(),(acc, fnames) => {
                    foreach (var f in fnames){
                        acc.Add(new FilePath(f).FileName);
                    }
                    return acc;
                }).Where(f => f.EndsWith("xml") && Char.IsUpper(f[0])).ToList();

            if (!layoutFiles.Any ()) {
                return;
            }

            if (!Directory.Exists(valuesDir)){
                Directory.CreateDirectory(valuesDir);
            }
            var layoutAliasFile = valuesDir.Combine("layout.xml");
            XElement layoutAliasRoot = null;
            if (File.Exists(layoutAliasFile)){
                try {
                    using (var input = File.OpenRead(layoutAliasFile)){
                        layoutAliasRoot = XElement.Load(new XmlTextReader(input));
                    }
                } catch (Exception e){
                    System.Diagnostics.Debug.WriteLine("Could not read layout alias file");                        
                }
                File.Delete(layoutAliasFile);
            }

            if (layoutAliasRoot == null){
                layoutAliasRoot = new XElement(XName.Get("resources"));
            }

           

            foreach (var file in layoutFiles){
                var fileName = file.Substring(0, file.LastIndexOf('.'));
                var layoutName = "@layout/" + TransformFileName(fileName, true);
                var aliasName = fileName;
                XElement aliasElement = new XElement(XName.Get("item"));
                aliasElement.Add(new XAttribute(XName.Get("name"), aliasName));
                aliasElement.Add(new XAttribute(XName.Get("type"), "layout"));
                aliasElement.Value = layoutName;
                layoutAliasRoot.Add(aliasElement);
            }

            using (var writer = new XmlTextWriter(layoutAliasFile, System.Text.Encoding.UTF8)){
                writer.Formatting = Formatting.Indented;
                layoutAliasRoot.WriteTo(writer);
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


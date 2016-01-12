using System;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using System.IO;
using Gtk;
using MonoDevelop.Ide;
using System.Diagnostics;
using MonoDevelop.Ide.Gui.Dialogs;
using System.Threading.Tasks;

namespace ViewInAndroidStudio
{
    public static class ProjectHandler
    {
        const string ProjectsDirectory = ".viewinandroidstudio";

        public static bool IsProjectCreated (Project p)
        {
            var path = GetProjectFilePath (p).FullPath;
            return Directory.Exists (path);
        }

        public static void CreateProject (Project p)
        {
            FilePath fp = GetProjectFilePath (p);

            try {
                if (Directory.Exists (fp)) {
                    Directory.Delete (fp, true);
                }
                Directory.CreateDirectory (fp);
            } catch (IOException e) {
                MessageDialog md = new MessageDialog (IdeApp.Workbench.RootWindow,
                                       DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Ok,
                                       "Could not (re)create project directory: {0}", e.Message);
                md.Run ();
                md.Destroy ();
                return;
            }

            ProgressDialog pd = new ProgressDialog (IdeApp.Workbench.RootWindow,false, false);
            pd.Progress = 1;
            pd.Message = "Linking project " + p.Name + "...";
            pd.Deletable = false;
            pd.SetPosition (WindowPosition.CenterOnParent);
            pd.Show ();
            var link = Process.Start ("android_xamarin_linker.sh", "\"" + fp.FullPath + "/\" \"" + p.BaseDirectory.Combine ("Resources").FullPath + "/\"");
            link.WaitForExit ();
            pd.Hide ();
            pd.Respond(ResponseType.Accept);
            pd.Destroy ();

            ViewHandler.OpenFileInAndroidStudio (GetProjectFilePath(p).Combine ("TaisteAndroid").Combine("build.gradle"));
        }

        public static FilePath GetProjectFilePath (Project p)
        {
            return GetProjectsDirectoryPath ().Combine (p.Name);
        }

        public static FilePath GetAndroidStudioProjectPath (Project p)
        {
            return GetProjectFilePath (p).Combine ("TaisteAndroid");
        }

        public static FilePath GetProjectsDirectoryPath ()
        {
            string homePath = 
                (Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable ("HOME")
                : Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%");
            FilePath projectsPath = new FilePath (homePath);
            return projectsPath.Combine (ProjectsDirectory);        
        }

        public static FilePath GetAndroidStudioProjectResourceDirectoryPath(Project p) {
            return GetAndroidStudioProjectPath(p).Combine("app").Combine("src").Combine("main").Combine("res");
        }
    }
}


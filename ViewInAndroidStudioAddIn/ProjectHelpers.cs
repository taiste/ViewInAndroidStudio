using System;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using System.IO;
using Gtk;
using MonoDevelop.Ide;
using System.Reflection;
using Mono.Unix.Native;
using MonoDevelop.Core.Execution;


namespace Taiste.ViewInAndroidStudio
{
    public static class ProjectHelpers
    {
        const string ProjectsDirectory = ".viewinandroidstudio";
        const string ScriptFileName = "android_xamarin_linker.sh";

        public static bool IsAndroidStudioProjectCreated (Project p)
        {
            var path = GetTempProjectPath (p).FullPath;
            return Directory.Exists (path);
        }

        public static void CreateAndroidStudioProject (Project p)
        {
            FilePath fp = GetTempProjectPath (p);

            try {
                if (Directory.Exists (fp)) {
                    Directory.Delete (fp, true);
                }
                Directory.CreateDirectory (fp);
            } catch (IOException e) {
                var md = new MessageDialog (IdeApp.Workbench.RootWindow,
                             DialogFlags.DestroyWithParent, 
                             MessageType.Error,
                             ButtonsType.Ok,
                             "Could not (re)create project directory: {0}", e.Message);
                md.Run ();
                md.Destroy ();
                return;
            }

            FilePath scriptPath = new FilePath (Assembly.GetExecutingAssembly ().Location);
            scriptPath = scriptPath.ParentDirectory.Combine (ScriptFileName);

            Syscall.chmod (scriptPath, FilePermissions.S_IRWXU | (FilePermissions.S_IRWXG ^ FilePermissions.S_IWGRP) | (FilePermissions.S_IRWXO ^ FilePermissions.S_IWOTH));

            var scriptArguments = "\"" + fp.FullPath + Path.DirectorySeparatorChar +
                                  "\" \"" + p.BaseDirectory.Combine ("Resources").FullPath + Path.DirectorySeparatorChar + "\"";
            
            var process = 
                Runtime.ProcessService.StartProcess ("bash", "-c '\"" + scriptPath + "\" " + scriptArguments + "'", scriptPath.ParentDirectory, null);
            process.WaitForExit ();

            ViewHandler.OpenFileInAndroidStudio (GetAndroidStudioProjectPath (p).Combine ("build.gradle"));
        }

        public static FilePath GetTempProjectPath (Project p)
        {
            return GetTempProjectsDirectoryPath ().Combine (p.Name);
        }

        public static FilePath GetAndroidStudioProjectPath (Project p)
        {
            return GetTempProjectPath (p).Combine ("TaisteAndroid");
        }

        private static FilePath GetUserHomePath ()
        {
            return new FilePath (
                (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable ("HOME")
                : Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%")
            );
        }

        public static FilePath GetTempProjectsDirectoryPath ()
        {
            return GetUserHomePath ().Combine (ProjectsDirectory);        
        }

        public static FilePath GetAndroidStudioProjectResourceDirectoryPath (Project p)
        {
            return GetAndroidStudioProjectPath (p).Combine ("app").Combine ("src").Combine ("main").Combine ("res");
        }
    }
}


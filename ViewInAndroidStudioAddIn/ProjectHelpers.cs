using System;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using System.IO;
using Gtk;
using MonoDevelop.Ide;
using System.Reflection;
using Mono.Unix.Native;
using MonoDevelop.Core.Execution;
using Taiste.ViewInAndroidStudio.Util;


namespace Taiste.ViewInAndroidStudio
{
    public static class ProjectHelpers
    {
        const string ProjectsDirectory = ".viewinandroidstudio";
        const string ScriptFileName = "android_xamarin_linker.sh";

        public static bool IsAndroidStudioProjectCreated (Project p)
        {
            var path = GetAndroidStudioProjectPath (p).FullPath;
            return Directory.Exists (path);
        }

        public static void CreateAndroidStudioProject (Project p)
        {
            FilePath androidStudioProjectPath = GetAndroidStudioProjectPath (p);

            try {
                if (Directory.Exists (androidStudioProjectPath)) {
                    Directory.Delete (androidStudioProjectPath, true);
                }
                Directory.CreateDirectory (androidStudioProjectPath);
            } catch (IOException e) {
                GtkHelpers.ShowDialog(String.Format("Could not (re)create project directory: {0}", e.Message), MessageType.Error);
                return;
            }

            FilePath scriptPath = new FilePath (Assembly.GetExecutingAssembly ().Location);
            scriptPath = scriptPath.ParentDirectory.Combine (ScriptFileName);

            Syscall.chmod (scriptPath, FilePermissions.S_IRWXU | (FilePermissions.S_IRWXG ^ FilePermissions.S_IWGRP) | (FilePermissions.S_IRWXO ^ FilePermissions.S_IWOTH));

            var scriptArguments = 
                (androidStudioProjectPath + Path.DirectorySeparatorChar).Quote()
                + " " 
                + (p.BaseDirectory.Combine ("Resources") + Path.DirectorySeparatorChar).Quote();
            
            var process = 
                Runtime.ProcessService.StartProcess ("bash", "-c '" + scriptPath.ToString().Quote() + " " + scriptArguments + "'", scriptPath.ParentDirectory, null);
            process.WaitForExit ();

            ViewHandler.OpenFileInAndroidStudio (androidStudioProjectPath.Combine ("build.gradle"));
        }

        public static FilePath GetAndroidStudioProjectPath (Project p)
        {
            return GetProjectsDirectoryPath ().Combine (p.Name);
        }
            

        private static FilePath GetUserHomePath ()
        {
            return new FilePath (
                (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable ("HOME")
                : Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%")
            );
        }

        public static FilePath GetProjectsDirectoryPath ()
        {
            return GetUserHomePath ().Combine (ProjectsDirectory);        
        }

        public static FilePath GetAndroidStudioProjectResourceDirectoryPath (Project p)
        {
            return GetAndroidStudioProjectPath (p).Combine ("app").Combine ("src").Combine ("main").Combine ("res");
        }
    }
}


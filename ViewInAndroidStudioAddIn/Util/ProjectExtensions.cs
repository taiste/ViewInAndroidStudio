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

namespace Taiste.ViewInAndroidStudio.Util
{
    public static class ProjectExtensions
    {
        const string ScriptFileName = "android_xamarin_linker.sh";

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

        static void CreateAndroidStudioProjectStructure(this Project p) 
        {
            FilePath scriptPath = 
                new FilePath (Assembly.GetExecutingAssembly ().Location)
                    .ParentDirectory.Combine (ScriptFileName);

            Syscall.chmod (scriptPath, FilePermissions.S_IRWXU | (FilePermissions.S_IRWXG ^ FilePermissions.S_IWGRP) | (FilePermissions.S_IRWXO ^ FilePermissions.S_IWOTH));

            var scriptArguments = 
                (p.GetAndroidStudioProjectPath() + Path.DirectorySeparatorChar).Quote ()
                + " "
                + (p.BaseDirectory.Combine ("Resources") + Path.DirectorySeparatorChar).Quote ();

            var process = 
                Runtime.ProcessService.StartProcess ("bash", "-c '" + scriptPath.ToString ().Quote () + " " + scriptArguments + "'", scriptPath.ParentDirectory, null);
            process.WaitForExit ();   
        }

        public static FilePath GetAndroidStudioProjectPath (this Project p)
        {
            return AddInPreferences.ProjectsDirectoryPath.Combine (p.Name);
        }

        public static FilePath GetAndroidStudioProjectResourceDirectoryPath (this Project p)
        {
            return GetAndroidStudioProjectPath (p).Combine ("app").Combine ("src").Combine ("main").Combine ("res");
        }

        public static bool IsAndroidProject (this Project p)
        {
            return p.GetProjectTypes ().Contains ("MonoDroid");
        }
    }
}


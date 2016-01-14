using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System.Linq;
using MonoDevelop.Ide.Tasks;
using System.Collections.Generic;
using MonoDevelop.Core;
using System.Diagnostics;
using System.IO;
using Gtk;
using Taiste.ViewInAndroidStudio.Util;
using Taiste.ViewInAndroidStudio.Preferences;

namespace Taiste.ViewInAndroidStudio.Commands
{
    public class ViewHandler : CommandHandler
    {
      

        protected override void Run ()
        {
            base.Run ();

            var xamarinFileToOpen = IdeApp.ProjectOperations.CurrentSelectedItem as ProjectFile;
            var xamarinProject = xamarinFileToOpen.Project;

            var androidStudioFilePath = xamarinProject.GetAndroidStudioProjectResourceDirectoryPath ();

            androidStudioFilePath = androidStudioFilePath.Combine (
                xamarinFileToOpen.FilePath.FullPath.ToString ()
                .Split (new string[]{ "Resources" }, StringSplitOptions.None) [1]
                .Substring (1)
            );

            if (!File.Exists (androidStudioFilePath)) {
                GtkHelpers.ShowDialog ("The file does not exist in a current Android Studio project. The Android Studio project will be (re)created.", MessageType.Info);
                xamarinProject.CreateAndroidStudioProject ();
                return;
            }

            OpenFileInAndroidStudio (xamarinProject.GetAndroidStudioProjectPath (), androidStudioFilePath);
        }

        public static void OpenFileInAndroidStudio (params string[] filePaths)
        {
            if (!File.Exists (AddInPreferences.AndroidStudioLocation)) {
                GtkHelpers.ShowDialog (
                    String.Format ("Android Studio executable not found at {0}, please locate the executable in Preferences.", AddInPreferences.AndroidStudioLocation),
                    MessageType.Error);
                return;
            }

            string args = filePaths
                .Select (StringExtensions.Quote)
                .Aggregate ("", StringExtensions.JoinWithSpace);
            Process.Start (new ProcessStartInfo (AddInPreferences.AndroidStudioLocation, args));
        }



        protected override void Update (CommandInfo info)
        {
            base.Update (info);
            var file = IdeApp.ProjectOperations.CurrentSelectedItem as ProjectFile;
            info.Visible = file != null && file.IsResourceXmlFile ();
        }

      
    }
}


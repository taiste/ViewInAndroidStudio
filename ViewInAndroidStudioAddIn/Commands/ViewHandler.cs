using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System.Linq;
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

            var androidStudioFilePath = xamarinFileToOpen.GetAndroidStudioFilePath ();

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


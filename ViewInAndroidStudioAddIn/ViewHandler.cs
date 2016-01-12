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

namespace ViewInAndroidStudio
{
    public class ViewHandler : CommandHandler
    {
      

       protected override void Run ()
        {
            base.Run ();


            var fileToOpen =  IdeApp.ProjectOperations.CurrentSelectedItem as ProjectFile;

            if (!ProjectHandler.IsProjectCreated (fileToOpen.Project)) {
                ProjectHandler.CreateProject (fileToOpen.Project);
            }

            var androidStudioFilePath = ProjectHandler.GetProjectResourceDirectoryPath (fileToOpen.Project);
            androidStudioFilePath = androidStudioFilePath.Combine( 
                fileToOpen.FilePath.FullPath.ToString ()
                .Split (new string[]{"Resources"}, StringSplitOptions.None)[1]
                .Substring(1)
            );
            
            OpenFileInAndroidStudio (ProjectHandler.GetAndroidStudioProjectPath(fileToOpen.Project), androidStudioFilePath);
        }
            
        public static void OpenFileInAndroidStudio(params string[] filePath)
        {
            if (!File.Exists(Preferences.AndroidStudioLocation)) {
                MessageDialog dialog = new MessageDialog(IdeApp.Workbench.RootWindow,
                    DialogFlags.DestroyWithParent, 
                    MessageType.Error, 
                    ButtonsType.Ok,
                    "Android Studio executable not found at {0}, please locate the executable in Preferences.",
                    Preferences.AndroidStudioLocation);
                dialog.Run();
                dialog.Destroy();
                return;
            }

            string args = filePath.Select( s=> "\"" + s + "\"").Aggregate("",(a,s)=>a + " " + s);
            Process.Start (new ProcessStartInfo(Preferences.AndroidStudioLocation, args));
        }
                                     
        protected override void Update (CommandInfo info)
        {
            base.Update (info);
            var file = IdeApp.ProjectOperations.CurrentSelectedItem as ProjectFile;
            info.Visible = file != null && file.IsResourceXmlFile();
        }

      
    }
}


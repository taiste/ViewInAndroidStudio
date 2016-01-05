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

namespace ViewInAndroidStudio
{
    public class ViewHandler : CommandHandler
    {
      

       protected override void Run ()
        {
            base.Run ();
            var fileToOpen =  IdeApp.ProjectOperations.CurrentSelectedItem as ProjectFile;
            string args = "-a \""+ Preferences.AndroidStudioLocation + "\" " + fileToOpen.FilePath.FullPath.ToString().Replace(" ", "\\ ");
            Process.Start (new ProcessStartInfo("open", args){UseShellExecute = false});
        }
            

        protected override void Update (CommandInfo info)
        {
            base.Update (info);
            var file = IdeApp.ProjectOperations.CurrentSelectedItem as ProjectFile;
            info.Visible = file != null && file.IsResourceXmlFile();
        }

      
    }
}


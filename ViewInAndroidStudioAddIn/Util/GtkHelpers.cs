using System;
using Gtk;
using MonoDevelop.Ide;

namespace Taiste.ViewInAndroidStudio.Util
{
    public static class GtkHelpers
    {
        public static void ShowDialog(string message, MessageType type)
        {
            MessageDialog dialog = new MessageDialog(
                IdeApp.Workbench.RootWindow,
                DialogFlags.DestroyWithParent,
                type,
                ButtonsType.Ok,
                message
            );
            dialog.Run ();
            dialog.Destroy ();
        }
    }
}


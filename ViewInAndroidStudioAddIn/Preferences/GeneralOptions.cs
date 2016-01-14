using MonoDevelop.Components;
using MonoDevelop.Ide.Gui.Dialogs;

namespace Taiste.ViewInAndroidStudio.Preferences
{
    public class GeneralOptions : OptionsPanel
    {
        FileEntry entry;

        public override Gtk.Widget CreatePanelWidget ()
        {
            Gtk.VBox box = new Gtk.VBox ();
            box.Spacing = 6;

            Gtk.HBox labelBox = new Gtk.HBox ();

            Gtk.Label label = new Gtk.Label ("Android studio executable location:");
            label.ModifyFont (new Pango.FontDescription { Weight = Pango.Weight.Bold });

            labelBox.PackStart (label, false, false, 0);

            entry = new FileEntry ();
            entry.Path = AddInPreferences.AndroidStudioLocation;

            box.PackStart (labelBox, false, false, 0);
            box.PackStart (entry, false, false, 0);
            box.ShowAll ();
            return box;

        }

        public override void ApplyChanges ()
        {
            AddInPreferences.AndroidStudioLocation = entry.Path;
            AddInPreferences.SaveConfig ();
        }
    }
}


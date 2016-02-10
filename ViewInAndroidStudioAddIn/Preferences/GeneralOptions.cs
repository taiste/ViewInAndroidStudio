using MonoDevelop.Components;
using MonoDevelop.Ide.Gui.Dialogs;

namespace Taiste.ViewInAndroidStudio.Preferences
{
    public class GeneralOptions : OptionsPanel
    {
        FileEntry entry;
        Gtk.Entry MinSdkEntry;
        Gtk.Entry TargetSdkEntry;
        Gtk.Entry SupportVersionEntry;
        Gtk.Entry BuildToolsVersionEntry;

        public override Gtk.Widget CreatePanelWidget ()
        {
            Gtk.VBox box = new Gtk.VBox ();
            box.Spacing = 6;

            Gtk.HBox labelBox = new Gtk.HBox ();

            Gtk.Label label = CreateLabel ("Android studio executable location:");

            labelBox.PackStart (label, false, false, 0);

            entry = new FileEntry ();
            entry.Path = AddInPreferences.AndroidStudioLocation;


            Gtk.HBox sdkVersions = new Gtk.HBox ();
            Gtk.Label minSdkLabel = CreateLabel ("Min SDK version:");
            MinSdkEntry = new Gtk.Entry (AddInPreferences.MinSdkVersion);
            Gtk.Label targetSdkLabel = CreateLabel ("Target/Compile SDK version:");
            TargetSdkEntry = new Gtk.Entry (AddInPreferences.CompileSdkVersion);


            sdkVersions.PackStart (minSdkLabel, false, false, 0);
            sdkVersions.PackStart (MinSdkEntry, true, false, 0);
            sdkVersions.PackStart (targetSdkLabel, false, false, 0);
            sdkVersions.PackStart (TargetSdkEntry, true, false, 0);


            Gtk.HBox otherVersions = new Gtk.HBox ();
            Gtk.Label supportVersionLabel = CreateLabel ("Support library versions:");
            SupportVersionEntry = new Gtk.Entry (AddInPreferences.SupportVersion);
            Gtk.Label buildToolsVersionLabel = CreateLabel ("Build tools version:");
            BuildToolsVersionEntry = new Gtk.Entry (AddInPreferences.BuildToolsVersion);


            otherVersions.PackStart (supportVersionLabel, false, false, 0);
            otherVersions.PackStart (SupportVersionEntry, true, false, 0);
            otherVersions.PackStart (buildToolsVersionLabel, false, false, 0);
            otherVersions.PackStart (BuildToolsVersionEntry, true, false, 0);

            box.PackStart (labelBox, false, false, 0);
            box.PackStart (entry, false, false, 0);
            box.PackStart (sdkVersions, false, false, 0);
            box.PackStart (otherVersions, false, false, 0);
            box.ShowAll ();
            return box;

        }

        static Gtk.Label CreateLabel(string text){
            var label = new Gtk.Label (text);
            label.ModifyFont (new Pango.FontDescription { Weight = Pango.Weight.Bold });
            return label;
        }

        public override void ApplyChanges ()
        {
            AddInPreferences.AndroidStudioLocation = entry.Path;
            AddInPreferences.MinSdkVersion = MinSdkEntry.Text;
            AddInPreferences.CompileSdkVersion = TargetSdkEntry.Text;
            AddInPreferences.SupportVersion = SupportVersionEntry.Text;
            AddInPreferences.BuildToolsVersion = BuildToolsVersionEntry.Text;
            AddInPreferences.SaveConfig ();
        }
    }
}


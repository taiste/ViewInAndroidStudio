using System;
using MonoDevelop.Core;
using System.IO;
using MonoDevelop.Core.Serialization;
using System.Xml;

namespace Taiste.ViewInAndroidStudio
{
    class AddinConfig
    {
        [ItemProperty]
        public string AndroidStudioLocation = "/Applications/Android Studio.app/Contents/MacOS/studio";
    }


    public static class Preferences
    {
        public static string AndroidStudioLocation {
            get { return GetConfig ().AndroidStudioLocation; }
            set { GetConfig ().AndroidStudioLocation = value; }
        }

        private static AddinConfig configuration;
        private static readonly DataContext dataContext = new DataContext ();

        static string ConfigFile {
            get { return UserProfile.Current.ConfigDir.Combine ("ViewInAndroidStudio.xml"); }
        }


        public static void SaveConfig ()
        {
            if (configuration != null) {
                XmlDataSerializer s = new XmlDataSerializer (dataContext);
                using (XmlTextWriter wr = new XmlTextWriter (File.CreateText (ConfigFile))) {
                    wr.Formatting = Formatting.Indented;
                    s.Serialize (wr, configuration, typeof(AddinConfig)); 
                }
            }
        }

        private static AddinConfig GetConfig ()
        {
            if (configuration != null) {
                return configuration;
            }
            if (File.Exists (ConfigFile)) {
                try {
                    XmlDataSerializer s = new XmlDataSerializer (dataContext);
                    using (var reader = File.OpenText (ConfigFile)) {
                        configuration = (AddinConfig)s.Deserialize (reader, typeof(AddinConfig));
                    }
                } catch {
                    ((FilePath)ConfigFile).Delete ();
                }
            }
            if (configuration == null) {
                configuration = new AddinConfig ();
            }
            return configuration;
        }
    }
}


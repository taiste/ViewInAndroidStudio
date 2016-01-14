using System;
using System.IO;
using System.Xml;
using MonoDevelop.Core;
using MonoDevelop.Core.Serialization;

namespace Taiste.ViewInAndroidStudio.Preferences
{
    class AddinConfig
    {
        [ItemProperty]
        public string AndroidStudioLocation = "/Applications/Android Studio.app/Contents/MacOS/studio";
    }


    public static class AddInPreferences
    {
        const string ProjectsDirectory = ".viewinandroidstudio";

        public static string AndroidStudioLocation {
            get { return GetConfig ().AndroidStudioLocation; }
            set { GetConfig ().AndroidStudioLocation = value; }
        }

        private static AddinConfig configuration;
        private static readonly DataContext dataContext = new DataContext ();

        static string ConfigFile {
            get { return UserProfile.Current.ConfigDir.Combine ("ViewInAndroidStudio.xml"); }
        }

        private static FilePath GetUserHomePath ()
        {
            return new FilePath (
                (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                ? Environment.GetEnvironmentVariable ("HOME")
                : Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%")
            );
        }

        public static FilePath ProjectsDirectoryPath {
            get {
                return GetUserHomePath ().Combine (ProjectsDirectory);
            }        
        }

        public static void SaveConfig ()
        {
            if (configuration != null) {
                XmlDataSerializer s = new XmlDataSerializer (dataContext);
                using (var wr = new XmlTextWriter (File.CreateText (ConfigFile))) {
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


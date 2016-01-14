namespace Taiste.ViewInAndroidStudio.Util
{
    public static class StringExtensions
    {
        public static string Quote (this string s)
        {
            return "\"" + s + "\"";
        }

        public static string JoinWithSpace (this string a, string s)
        {
            return a + " " + s;
        }
    }
}


using System.Windows.Forms;

namespace Fighter
{
    public class Paths
    {
        public static string Root
        {
            get
            {
                return Application.StartupPath + @"\Plugins\Fighter";
            }
        }

        public static string RootFolder
        {
            get
            {
                return Root + @"\";
            }
        }


        public static string Settings
        {
            get
            {
                return RootFolder + "Settings";
            }
        }

        public static string SettingsFolder
        {
            get
            {
                return Settings + @"\";
            }
        }


        public static string Logs
        {
            get
            {
                return RootFolder + "Logs";
            }
        }

        public static string LogsFolder
        {
            get
            {
                return Logs + @"\";
            }
        }


        public static string Navigation
        {
            get
            {
                return RootFolder + "Navigation";
            }
        }

        public static string NavigationFolder
        {
            get
            {
                return Navigation + @"\";
            }
        }


        public static string Templates
        {
            get
            {
                return RootFolder + @"Templates";
            }
        }

        public static string TemplatesFolder
        {
            get
            {
                return Templates + @"\";
            }
        }
    }
}

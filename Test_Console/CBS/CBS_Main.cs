using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace CBS
{
    class CBS_Main
    {
        private static bool FileWatcherEnabled = true;
        // WINDOWS
        //
        // Will get changed to LINUX paths on Initialise if APP is running
        // on LINUX !!!
        private static string Source_Path = @"C:\var\EFD\";
        private static string flights_Path = @"C:\var\cbs\prediction\flights\";
        private static string App_Settings_Path = @"C:\var\cbs\settings\EFD\";
        private static string System_Status_Path = @"C:\var\cbs\prediction\systemStatus\";
        private static string Main_Status_Path = @"C:\var\cbs\prediction\status\";
        private static string AIRAC_Data_Path = @"C:\var\cbs\settings\AIRAC\";
        private static string Tmp_Directory =  @"C:\tmp";

        // Common
        private static string HEART_BEAT = "" + DateTime.UtcNow.Year + DateTime.UtcNow.Month + DateTime.UtcNow.Day + DateTime.UtcNow.Hour + DateTime.UtcNow.Minute + DateTime.UtcNow.Second;

        // Timer used to periodically save off last time application was alive
        private static System.Timers.Timer Cold_Start_Timer;
        // The number of minutes after applicaton will power
        // up in "Cold Power Up" Status. This imples that
        // upon power only newly arrived data will be processed
        // and old data will immediatly be deleted.
        private static int Cold_Start_Timeout_Min = 10;

        // Timer to periodically create EFD_Status.xml file
        // as status indication of the module. 
        private static System.Timers.Timer System_Status_Timer;
        // Holds System Status periodic rate in seconds
        private static int System_Status_Update_Rate_Sec = 10;

        /////////////////////////////////////////////////
        // No EFD data reception timout in minutes
        private static int No_EFD_Data_Timout = 10;
        private static DateTime Last_EFD_Messsage_Reception_Time = DateTime.UtcNow;

        // Called by the 
        public static void Notify_EFD_Message_Recived()
        {
            Last_EFD_Messsage_Reception_Time = DateTime.UtcNow;
        }

        public enum Host_OS { WIN, LINUX };
        public static Host_OS Get_Host_OS()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return Host_OS.LINUX;
            else
                return Host_OS.WIN;
        }

        public static void SetSourceAndDestinationPaths(string SOURCE, string DESTINATION)
        {
            Source_Path = SOURCE;
            flights_Path = DESTINATION;
        }

        // Returns power off time (+/- 59 sec)
        public static DateTime Get_Power_OFF_Time()
        {
            return GetDate_Time_From_YYYYMMDDHHMMSS(HEART_BEAT);
        }

        public static DateTime GetDate_Time_From_YYYYMMDDHHMMSS(string DATETIME)
        {
            int Year = int.Parse(DATETIME.Substring(0, 4));
            int Month = int.Parse(DATETIME.Substring(4, 2));
            int Day = int.Parse(DATETIME.Substring(6, 2));
            int Hour = int.Parse(DATETIME.Substring(8, 2));
            int Minute = int.Parse(DATETIME.Substring(10, 2));
            int Sec = int.Parse(DATETIME.Substring(12, 2));
            return new DateTime(Year, Month, Day, Hour, Minute, Sec);
        }

        public static string GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime Time_In)
        {
            return Time_In.Year.ToString("0000") + Time_In.Month.ToString("00") + Time_In.Day.ToString("00") + Time_In.Hour.ToString("00") + Time_In.Minute.ToString("00") + Time_In.Second.ToString("00");
        }

        public static string Get_DATE_AS_YYMMDD(DateTime Time_In)
        {
            return Time_In.Year.ToString("00") + Time_In.Month.ToString("00") + Time_In.Day.ToString("00");
        }

        public static string Get_TIME_AS_HHMM(DateTime Time_In)
        {
            return Time_In.Hour.ToString("00") + Time_In.Minute.ToString("00");
        }

        public static string Get_Source_Dir()
        {

            return Source_Path;

        }

        public static string Get_Destination_Dir()
        {

            return flights_Path;

        }

        public static string Get_Temp_Dir()
        {
            return Tmp_Directory;
        }

        public static string Get_AIRAC_Dir()
        {
            return AIRAC_Data_Path;
        }

        public static string Get_System_Status_Dir()
        {
            return System_Status_Path;
        }

        public static string Get_APP_Settings_Path()
        {
            return App_Settings_Path;
        }

        public static void Initialize()
        {
            /////////////////////////////////////////////////////////////////
            // First check where we are running and prepare app
            //
            if (Get_Host_OS() == Host_OS.LINUX)
            {
                // Linux
                Source_Path = "/var/EFD/";
                flights_Path = "/var/cbs/prediction/flights/";
                System_Status_Path = "/var/cbs/prediction/systemStatus/";
                Main_Status_Path = "/var/cbs/prediction/status/";
                App_Settings_Path = "/var/cbs/settings/EFD/";
                AIRAC_Data_Path = "/var/cbs/settings/AIRAC/";
                Tmp_Directory = "/tmp/";
            }

            // Now make sure that proper directory structure 
            // is set up on the host machine
            if (Directory.Exists(Source_Path) == false)
                Directory.CreateDirectory(Source_Path);
            if (Directory.Exists(flights_Path) == false)
                Directory.CreateDirectory(flights_Path);
            if (Directory.Exists(App_Settings_Path) == false)
                Directory.CreateDirectory(App_Settings_Path);
            if (Directory.Exists(System_Status_Path) == false)
                Directory.CreateDirectory(System_Status_Path);
            if (Directory.Exists(Main_Status_Path) == false)
                Directory.CreateDirectory(Main_Status_Path);
            if (Directory.Exists(AIRAC_Data_Path) == false)
                Directory.CreateDirectory(AIRAC_Data_Path);
            if (Directory.Exists(Tmp_Directory) == false)
                Directory.CreateDirectory(Tmp_Directory);

            // Check if cbs_config.txt exists, if so load settings
            // data saved from the previous session
            string Settings_Data;
            string FileName = Path.Combine(App_Settings_Path, "cbs_config.txt");
            char[] delimiterChars = { ' ' };
            StreamReader MyStreamReader;
            if (File.Exists(FileName))
            {
                // Lets read in settings from the file
                MyStreamReader = System.IO.File.OpenText(FileName);
                while (MyStreamReader.Peek() >= 0)
                {
                    Settings_Data = MyStreamReader.ReadLine();
                    if (Settings_Data[0] != '#')
                    {
                        string[] words = Settings_Data.Split(delimiterChars);

                        switch (words[0])
                        {
                            case "SOURCE_DIR":
                                Source_Path = words[1];
                                break;
                            case "FLIGHTS_DIR":
                                flights_Path = words[1];
                                break;
                            case "SYSTEM_STATUS_DIR":
                                System_Status_Path = words[1];
                                break;
                            case "MAIN_STATUS_DIR":
                                Main_Status_Path = words[1];
                                break;
                            case "HEART_BEAT":
                                HEART_BEAT = words[1];
                                break;
                            case "COLD_POWER_UP":
                                Cold_Start_Timeout_Min = int.Parse(words[1]);
                                break;
                            case "SYS_STATUS_UPDATE_RATE":
                                System_Status_Update_Rate_Sec = int.Parse(words[1]);
                                break;
                            case "NO_EFD_DATA_TIMEOUT":
                                No_EFD_Data_Timout = int.Parse(words[1]);
                                break;
                            case "AIRAC_DATA_SOURCE":
                                AIRAC_Data_Path = words[1];
                                break;
                            default:
                                break;
                        }
                    }
                }

                MyStreamReader.Close();
                MyStreamReader.Dispose();

                // Here check if there has been more than parameter since 
                // application has been down
                TimeSpan TenMin = new TimeSpan(0, Cold_Start_Timeout_Min, 0);
                TimeSpan AppDown = DateTime.UtcNow - Get_Power_OFF_Time();

                // Now check if the application has been down for more than
                // 10 minutes. If so then clear the directory
                if (AppDown > TenMin)
                    ClearSourceDirectory();
                else
                {
                    // Call routine to process all files that might have
                    // arrived in the last 10 minutes or less.
                }

                // Lets save once so HART BEAT gets saved right away
                SaveSettings();
            }
            else
            {
                // Lets then create one with default setting
                SaveSettings();

                // Since we had no idea when the application was last powered off
                // we assume it has been more than timuout parameter, sop lets delete all files
                ClearSourceDirectory();
            }

            // Now start heart beat timer.
            Cold_Start_Timer = new System.Timers.Timer(10000); // Set up the timer for 1minute
            Cold_Start_Timer.Elapsed += new ElapsedEventHandler(_HEART_BEAT_timer_Elapsed);
            Cold_Start_Timer.Enabled = true;

            // Start file watcher to process incomming data
            if (FileWatcherEnabled)
                FileWatcher.CreateWatcher(Source_Path);

            //////////////////////////////////////////////////////
            // Start periodic timer that will drive system status 
            // update logic
            // Now start heart beat timer.
            System_Status_Timer = new System.Timers.Timer((System_Status_Update_Rate_Sec * 100)); // Set up the timer for 1minute
            System_Status_Timer.Elapsed += new ElapsedEventHandler(System_Status_Periodic_Update);
            System_Status_Timer.Enabled = true;
        }

        public static void Restart_Watcher()
        {
            if (FileWatcherEnabled)
            {
                FileWatcher.StopWatcher();
                FileWatcher.CreateWatcher(Source_Path);
            }
        }

        private static void _HEART_BEAT_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveSettings();
        }

        // Periodically call System Status Handler
        private static void System_Status_Periodic_Update(object sender, ElapsedEventArgs e)
        {
            TimeSpan No_EFD_Data_Time = (DateTime.UtcNow - Last_EFD_Messsage_Reception_Time);
            TimeSpan Timeout = new TimeSpan(0, No_EFD_Data_Timout, 0);
            System_Status.Generate((No_EFD_Data_Time > Timeout));
        }

        // Deletes all files from the source directory
        public static void ClearSourceDirectory()
        {
            foreach (string directories in Directory.GetDirectories(Source_Path))
                Directory.Delete(directories, true);
        }

        public static void SaveSettings()
        {
            string FileName = Path.Combine(App_Settings_Path, "cbs_config.txt");
            string Settings_Data = "";

            //////////////////////////////////////////////////////////////////////////////////////
            // Do not chanage the order of calls
            Settings_Data = Settings_Data + "# This is CBS configuration file" + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "# Root directory of the incomming EFD messages" + Environment.NewLine;
            Settings_Data = Settings_Data + "SOURCE_DIR" + " " + Source_Path + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "# Destination of the output prediction data" + Environment.NewLine;
            Settings_Data = Settings_Data + "FLIGHTS_DIR" + " " + flights_Path + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "# Destination of the system status data" + Environment.NewLine;
            Settings_Data = Settings_Data + "SYSTEM_STATUS_DIR" + " " + System_Status_Path + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "# System Status update rate in seconds" + Environment.NewLine;
            Settings_Data = Settings_Data + "SYS_STATUS_UPDATE_RATE" + " " + System_Status_Update_Rate_Sec + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "# Destination of the main status data" + Environment.NewLine;
            Settings_Data = Settings_Data + "MAIN_STATUS_DIR" + " " + Main_Status_Path + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "# Do not touch this one !!! App internal parameter" + Environment.NewLine;
            Settings_Data = Settings_Data + "HEART_BEAT" + " " + GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.UtcNow) + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "# Number of min after app will power up in cold power up mode" + Environment.NewLine;
            Settings_Data = Settings_Data + "COLD_POWER_UP" + " " + Cold_Start_Timeout_Min.ToString() + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "# No EFD data recived status reporting timout in minutes" + Environment.NewLine;
            Settings_Data = Settings_Data + "NO_EFD_DATA_TIMEOUT" + " " + No_EFD_Data_Timout.ToString() + Environment.NewLine;
            Settings_Data = Settings_Data + "#" + Environment.NewLine;
            Settings_Data = Settings_Data + "# Destination of the AIRAC data" + Environment.NewLine;
            Settings_Data = Settings_Data + "AIRAC_DATA_SOURCE" + " " + AIRAC_Data_Path.ToString() + Environment.NewLine;
            //////////////////////////////////////////////////////////////////////////////////////

            // create a writer and open the file
            TextWriter tw = new StreamWriter(FileName);

            try
            {
                // write a line of text to the file
                tw.Write(Settings_Data);
            }
            catch
            {
            }

            // close the stream
            tw.Close();
            tw.Dispose();
        }
    }
}

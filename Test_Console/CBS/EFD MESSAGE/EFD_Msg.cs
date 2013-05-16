using System;
using System.IO;
using System.Collections.Generic;

namespace CBS
{
    public class EFD_Msg
    {
        // These are derived 
        // from the EFD messages 
        public string IFPLID;
        public string ACID;
        public string ADEP;
        public string ADES;
        public string EOBT;
        public string EOBD;
        public string ARCTYP;
        public string FLTSTATE;
        public string FL_STATUS = "Unknown";
        public string[] Waypoints;

        public class Waypoint
        {
            public enum Wpt_Type
            {
                Basic,
                Entry,
                Exit
            };

            public string Name = "N/A";
            public GeoCordSystemDegMinSecUtilities.LatLongClass Position;
            public string Flight_Level = "N/A";
            public Wpt_Type Type = Wpt_Type.Basic;
        }

        public class Sector_Type
        {
            public string ID;
            public DateTime SECTOR_ENTRY_TIME;
            public DateTime SECTOR_EXIT_TIME;
        }

        // These are calculated data
        public GeoCordSystemDegMinSecUtilities.LatLongClass ENTRY_AOI_POINT = new GeoCordSystemDegMinSecUtilities.LatLongClass();
        public GeoCordSystemDegMinSecUtilities.LatLongClass EXIT_AOI_POINT = new GeoCordSystemDegMinSecUtilities.LatLongClass();
        public DateTime ENTRY_AOI_TIME;
        public DateTime EXIT_AOI_TIME;
        public string Entry_FL = "N/A";
        public string Exit_FL = "N/A";
        public List<Waypoint> TrajectoryPoints = new List<Waypoint>();
        public List<Sector_Type> Sector_List = new List<Sector_Type>();

        public EFD_Msg(StreamReader Reader)
        {
            string OneLine;
            char[] delimiterChars = { ' ' };

            // Parse the file and extract all data needed by
            // EFD
            while (Reader.Peek() >= 0)
            {
                OneLine = Reader.ReadLine();
                string[] Words = OneLine.Split(delimiterChars);

                try
                {
                    switch (Words[0])
                    {
                        case "-IFPLID":
                            IFPLID = Words[1];
                            break;
                        case "-ARCID":
                            ACID = Words[1];
                            break;
                        case "-FLTSTATE":
                            FLTSTATE = Words[1];
                            break;
                        case "-ARCTYP":
                            ARCTYP = Words[1];
                            break;
                        case "-ADEP":
                            ADEP = Words[1];
                            break;
                        case "-ADES":
                            ADES = Words[1];
                            break;
                        case "-EOBT":
                            EOBT = Words[1];
                            break;
                        case "-EOBD":
                            EOBD = Words[1];
                            break;
                        case "-BEGIN":
                            if (Words[1] == "RTEPTS")
                            {

                            }
                            else if (Words[1] == "ASPLIST")
                            {

                            }
                            else
                            {

                            }
                            break;
                        // Maastricht UAC Entry and Exit Times
                        // -ASP -AIRSPDES EDYYAOI -ETI 130404091206 -XTI 130404095840
                        case "":
                            if (Words.Length == 8)
                            {
                                if (Words[1] == "-ASP")
                                {
                                    // Always extract UAC Entry and Exit Times
                                    if ((Words[2] == "-AIRSPDES") && (Words[3] == "EDYYAOI"))
                                    {
                                        ENTRY_AOI_TIME = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + Words[5]);
                                        EXIT_AOI_TIME = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + Words[7]);
                                    }

                                    // Now extract all MUAC sectors and sector entry/exit times
                                    // Always extract UAC Entry and Exit Times
                                    if ((Words[2] == "-AIRSPDES") && (Words[3].Substring(0, 4) == "EDYY"))
                                    {
                                        string Sector_ID = Words[3].Substring(4, (Words[3].Length - 4));
                                        // FOX,FOX1,FOX2,UAC,UACX,AOI
                                        if (Sector_ID != "FOX" && Sector_ID != "FOX1" && Sector_ID != "FOX2" &&
                                            Sector_ID != "UAC" && Sector_ID != "UACX" && Sector_ID != "AOI")
                                        {
                                            Sector_Type Sector = new Sector_Type();
                                            Sector.ID = Sector_ID;
                                            Sector.SECTOR_ENTRY_TIME = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + Words[5]);
                                            Sector.SECTOR_EXIT_TIME = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + Words[7]);
                                            Sector_List.Add(Sector);
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("Error in EFD input file...");
                }
            }

            Reader.Close();
            Reader.Dispose();
        }

        public bool Is_New_Data_Set()
        {
            bool New_Data_Set = false;
            string FileName = Get_Dir_By_ACID_AND_IFPLID(ACID, IFPLID);
            char[] delimiterChars = { ' ' };
            StreamReader MyStreamReader;
            string Data_Set;
            // Lets read in settings from the file
            MyStreamReader = System.IO.File.OpenText(FileName);
            while (MyStreamReader.Peek() >= 0)
            {
                Data_Set = MyStreamReader.ReadLine();
                if (Data_Set[0] != '#')
                {
                    string[] words = Data_Set.Split(delimiterChars);

                    switch (words[0])
                    {
                        case "ADEP":
                            if (ADEP != words[1])
                                New_Data_Set = true;
                            break;
                        case "ADES":
                            if (ADES != words[1])
                                New_Data_Set = true;
                            break;
                        case "EOBT":
                            if (EOBT != words[1])
                                New_Data_Set = true;
                            break;
                        case "EOBD":
                            if (EOBD != words[1])
                                New_Data_Set = true;
                            break;

                        default:
                            break;
                    }
                }
            }

            MyStreamReader.Close();
            MyStreamReader.Dispose();

            return New_Data_Set;
        }

        public void SaveDataSet()
        {
            string FileName = Get_Dir_By_ACID_AND_IFPLID(ACID, IFPLID);
            string Settings_Data = "";

            //////////////////////////////////////////////////////////////////////////////////////
            // Do not chanage the order of calls
            Settings_Data = "ADEP " + ADEP + Environment.NewLine;
            Settings_Data = Settings_Data + "ADES " + ADES + Environment.NewLine;
            Settings_Data = Settings_Data + "EOBT " + EOBT + Environment.NewLine;
            Settings_Data = Settings_Data + "EOBD " + EOBD + Environment.NewLine;
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

        private static string Get_Dir_By_ACID_AND_IFPLID(string ACID, string IFPLID)
        {
            string DIR = "";
            // First check if directory already exists
            string IFPLID_DIR_NAME = ACID + "_" + IFPLID + "_*";
            string[] DestDirectory = Directory.GetDirectories(CBS_Main.Get_Destination_Dir(), IFPLID_DIR_NAME);
            if (DestDirectory.Length == 1)
            {
                DIR = DestDirectory[0];
                DIR = Path.Combine(DIR, ".dataset");
            }
            return DIR;
        }
    }
}


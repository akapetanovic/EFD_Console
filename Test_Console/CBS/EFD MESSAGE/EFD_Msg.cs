using System;
using System.IO;

namespace CBS
{
	public class EFD_Msg
	{
		// These are derived data from the
		// EFD messages 
		public string IFPLID;
		public string ACID;
		public string ADEP;
		public string ADES;
		public string EOBT;
		public string EOBD;
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
			private GeoCordSystemDegMinSecUtilities.LatLongClass Position;
			public string Flight_Level = "N/A";
			public Wpt_Type Type = Wpt_Type.Basic;
		}

		// These are calculated data
		private GeoCordSystemDegMinSecUtilities.LatLongClass  ENTRY_AOI_POINT;
		private GeoCordSystemDegMinSecUtilities.LatLongClass  EXIT_AOI_POINT;
		public DateTime ENTRY_AOI_TIME;
		public DateTime EXIT_AOI_TIME;
		public Waypoint[] TrajectoryPoints;

		public EFD_Msg (StreamReader Reader)
		{
			string OneLine;
			char[] delimiterChars = { ' ' };

			// Parse the file and extract all data needed by
			// EFD
			while (Reader.Peek() >= 0) 
			{
				OneLine = Reader.ReadLine ();
				string[] Words = OneLine.Split (delimiterChars);

				switch (Words [0]) {
				case "-IFPLID":
					IFPLID = Words [1];
					break;
				case "-ARCID":
					ACID = Words [1];
					break;
				case "-ADEP":
					ADEP = Words [1];
					break;
				case "-ADES":
					ADES = Words [1];
					break;
				case "-EOBT":
					EOBT = Words [1];
					break;
				case "-EOBD":
					EOBD = Words [1];
					break;
				default:
					break;
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


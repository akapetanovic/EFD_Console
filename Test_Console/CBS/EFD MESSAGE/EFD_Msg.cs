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
		public string[] Waypoints;

		public class Waypoint
		{
			public enum Wpt_Type
			{
				Basic,
				Entry,
				Exit
			}
			;

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
			// CBS
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
	}
}


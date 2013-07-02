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
        public string TSTARTTIME;
        public string TENDTIME;

        public class Waypoint
        {
            public string Name = "N/A";
            public GeoCordSystemDegMinSecUtilities.LatLongClass Position = new GeoCordSystemDegMinSecUtilities.LatLongClass();
            public bool Position_Determined = false;
            public string Flight_Level = "N/A";
            public string ETO = "N/A";
        }

        //public class Sector
        //{
        //    public string ID = "N/A";
        //    public DateTime SECTOR_ENTRY_TIME = DateTime.Now;
        //    public DateTime SECTOR_EXIT_TIME = DateTime.Now;
        //    public string EFL = "N/A";
        //    public string XFL = "N/A";
        //}

        //public List<Sector> Sector_List = new List<Sector>();
        public List<Waypoint> GEO_Artifical_List = new List<Waypoint>();


        // These are calculated data
        public GeoCordSystemDegMinSecUtilities.LatLongClass ENTRY_AOI_POINT = new GeoCordSystemDegMinSecUtilities.LatLongClass();
        public GeoCordSystemDegMinSecUtilities.LatLongClass EXIT_AOI_POINT = new GeoCordSystemDegMinSecUtilities.LatLongClass();
        public DateTime AOI_ENTRY_TIME = DateTime.Now;
        public DateTime AOI_EXIT_TIME = DateTime.Now;
        public string AOI_ENTRY_TIME_YYMMDDHHMMSS = "N/A";
        public string AOI_EXIT_TIME_YYMMDDHHMMSS = "N/A";
        public string AOI_ENTRY_FL = "N/A";
        public string AOI_EXIT_FL = "N/A";
        public List<Waypoint> TrajectoryPoints = new List<Waypoint>();

        public EFD_Msg(StreamReader Reader)
        {
            string OneLine;
            char[] delimiterChars = { ' ' };

            // Parse the file and extract all data needed by EFD
            while (Reader.Peek() >= 0)
            {
                OneLine = Reader.ReadLine();
                string[] Words = OneLine.Split(delimiterChars);
                Waypoint WPT = new Waypoint();

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
                        case "-GEO":
                            try
                            {
                                string LAT = Words[4];
                                string LON = Words[6];
                                // -LATTD 550302N -LONGTD 1070037W
                                int LAT_DEG = int.Parse(LAT.Substring(0, 2));
                                int LAT_MIN = int.Parse(LAT.Substring(2, 2));
                                int LAT_SEC = int.Parse(LAT.Substring(4, 2));
                                GeoCordSystemDegMinSecUtilities.LatLongPrefix LAT_Prefix = GeoCordSystemDegMinSecUtilities.LatLongPrefix.Not_Valid;
                                if (LAT[6] == 'N')
                                    LAT_Prefix = GeoCordSystemDegMinSecUtilities.LatLongPrefix.N;
                                else
                                    LAT_Prefix = GeoCordSystemDegMinSecUtilities.LatLongPrefix.S;

                                int LON_DEG = int.Parse(LON.Substring(0, 3));
                                int LON_MIN = int.Parse(LON.Substring(3, 2));
                                int LON_SEC = int.Parse(LON.Substring(5, 2));
                                GeoCordSystemDegMinSecUtilities.LatLongPrefix LON_Prefix = GeoCordSystemDegMinSecUtilities.LatLongPrefix.Not_Valid;
                                if (LON[7] == 'W')
                                    LON_Prefix = GeoCordSystemDegMinSecUtilities.LatLongPrefix.W;
                                else
                                    LON_Prefix = GeoCordSystemDegMinSecUtilities.LatLongPrefix.E;

                                WPT.Name = Words[2];
                                WPT.Position = new GeoCordSystemDegMinSecUtilities.LatLongClass(LAT_DEG, LAT_MIN, LAT_SEC, LAT_Prefix, LON_DEG, LON_MIN, LON_SEC, LON_Prefix);
                                WPT.Position_Determined = true;
                                GEO_Artifical_List.Add(WPT);
                            }
                            catch (Exception e)
                            {
                                CBS_Main.WriteToLogFile("Exception in EFD_Msg.cs, -GEP Parsing: " + e.Message);
                            }
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
                            if ((Words.Length == 8) && (Words[1] == "-ASP"))
                            {
                                // Always extract UAC Entry and Exit Times
                                if ((Words[2] == "-AIRSPDES") && (Words[3] == "EDYYAOI"))
                                {
                                    AOI_ENTRY_TIME = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + Words[5]);
                                    AOI_ENTRY_TIME_YYMMDDHHMMSS = "20" + Words[5];
                                    AOI_EXIT_TIME = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + Words[7]);
                                    AOI_EXIT_TIME_YYMMDDHHMMSS = "20" + Words[7];
                                }

                                //// Now extract all MUAC sectors and sector entry/exit times
                                //// Always extract UAC Entry and Exit Times
                                //if ((Words[2] == "-AIRSPDES") && (Words[3].Substring(0, 4) == "EDYY"))
                                //{
                                //    string Sector_ID = Words[3].Substring(4, (Words[3].Length - 4));
                                //    // FOX,FOX1,FOX2,UAC,UACX,AOI
                                //    if (Sector_ID != "FOX" && Sector_ID != "FOX1" && Sector_ID != "FOX2" &&
                                //        Sector_ID != "UAC" && Sector_ID != "UACX" && Sector_ID != "AOI")
                                //    {
                                //        Sector Sector = new Sector();
                                //        Sector.ID = Sector_ID;
                                //        Sector.SECTOR_ENTRY_TIME = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + Words[5]);
                                //        Sector.SECTOR_EXIT_TIME = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + Words[7]);
                                //        Sector_List.Add(Sector);
                                //    }
                                //}
                            }
                            else if (Words.Length > 1)
                            {
                                if (Words[1] == "-AD" || Words[1] == "-VEC" || Words[1] == "-PT")
                                {
                                    FIX_TO_LATLNG.FIXPOINT_TYPE FIX = new FIX_TO_LATLNG.FIXPOINT_TYPE();

                                    switch (Words[1])
                                    {
                                        // -AD  -ADID OMDB -ETO 130404033500 -PTRTE DCT
                                        case "-AD":

                                            break;
                                        // -VEC -RELDIST 01 -FL F010 -ETO 130404034715
                                        case "-VEC":

                                            // Extract data from -PT line
                                            WPT.Name = "-VEC" + Words[3];
                                            WPT.Flight_Level = Words[5].Substring(1); // Remove F at the beggining.
                                            WPT.ETO = Words[7];

                                            if (TrajectoryPoints.Count > 0)
                                            {
                                                // Add a new point to the list
                                                // Only add position that are known 
                                                // as defined in the fixpoint table.
                                                TrajectoryPoints.Add(WPT);
                                            }

                                            break;
                                        // -PT  -PTID GEO01 -FL F300 -ETO 130404041754
                                        case "-PT":

                                            // Extract data from -PT line
                                            WPT.Name = Words[4];
                                            WPT.Flight_Level = Words[6].Substring(1); // Remove F at the beggining.
                                            WPT.ETO = Words[8];

                                            FIX = FIX_TO_LATLNG.Get_LATLNG(WPT.Name);
                                            if (FIX.Is_Found == true)
                                            {
                                                WPT.Position = FIX.Position;
                                                WPT.Position_Determined = true;

                                                // Add a new point to the list
                                                // Only add position that are known 
                                                // as defined in the fixpoint table.
                                                TrajectoryPoints.Add(WPT);
                                            }
                                            else
                                            {
                                                // Lets check if the point is in the artifical EFD message
                                                // provided list. If so extract the position and assign it to
                                                // the main trajectory WPT list
                                                foreach (Waypoint GEO_WPT in GEO_Artifical_List)
                                                {
                                                    if (WPT.Name == GEO_WPT.Name)
                                                    {
                                                        WPT.Position = new GeoCordSystemDegMinSecUtilities.LatLongClass(GEO_WPT.Position.GetLatLongDecimal().LatitudeDecimal, GEO_WPT.Position.GetLatLongDecimal().LongitudeDecimal);
                                                        WPT.Position_Determined = true;
                                                        TrajectoryPoints.Add(WPT);
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    CBS_Main.WriteToLogFile("Exception in EFD_Msg.cs, Instantiation: " + e.Message);
                }
            }

            /////////////////////////////////////////////////////////////////////////
            // DO NOT CHANGE THE ORDER OF THE FOLLOWING CALLS
            /////////////////////////////////////////////////////////////////////////

            // Here parse the list and
            // 1. Remove all "-VEC points from the end of the list
            // 2. Determine Lon/Lat of each -VEC point
            ParseTrajectoryList(ref TrajectoryPoints);

            // Iterate through the sector lists and
            // calculate sector entry/exit levels based on the
            // extrapolation data calculated from the main trajectory
            // WPT list
            // CalculateSectorEntry_Exit_Times(ref Sector_List);

            /////////////////////////////////
            // Now set AOI Entry/Exit Points
            CalculateAOI_Entry_Exit_Times();

            Reader.Close();
            Reader.Dispose();
            ///////////////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////
        }

        // This method is to be called once TrajectoryPoints
        // list has been populated and parsed so only known
        // points are left in the list (The route has been extracted)
        private void CalculateAOI_Entry_Exit_Times()
        {

            // First take care of the sector entry FL
            int Start_Index = 0;
            int End_Index = 0;
            bool Start_End_WPT_Search_Status = false;

            // FL
            int Start_FL;
            int End_FL;
            int FL_DIFFERENCE;
            TimeSpan Start_To_Sector;
            TimeSpan Start_To_End;
            double Time_Factor;
            double Sector_Crossing_FL;

            // select a reference elllipsoid
            Ellipsoid reference = Ellipsoid.WGS84;
            // instantiate the calculator
            GeodeticCalculator geoCalc = new GeodeticCalculator();

            // Get indexes of WPT before and after AOI ENTRY crossing crossing border
            Get_Start_End_WPT_Index(AOI_ENTRY_TIME, out Start_Index, out End_Index, out Start_End_WPT_Search_Status);
            if (Start_End_WPT_Search_Status == true)
            {
                // First take care of the FL
                Start_FL = int.Parse(TrajectoryPoints[Start_Index].Flight_Level);
                End_FL = int.Parse(TrajectoryPoints[End_Index].Flight_Level);
                FL_DIFFERENCE = End_FL - Start_FL;

                if (Start_Index != End_Index)
                {

                    Start_To_Sector = AOI_ENTRY_TIME - CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[Start_Index].ETO);
                    Start_To_End = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[End_Index].ETO) - CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[Start_Index].ETO);
                    Time_Factor = Start_To_Sector.TotalSeconds / Start_To_End.TotalSeconds;
                    Sector_Crossing_FL = Start_FL + (FL_DIFFERENCE * Time_Factor);
                }
                else
                {
                    Sector_Crossing_FL = Start_FL;
                    Time_Factor = 0.0;
                }
                AOI_ENTRY_FL = Math.Round(Sector_Crossing_FL).ToString();

                GlobalPosition Start_Pos = new GlobalPosition(new GlobalCoordinates(TrajectoryPoints[Start_Index].Position.GetLatLongDecimal().LatitudeDecimal, TrajectoryPoints[Start_Index].Position.GetLatLongDecimal().LongitudeDecimal));
                GlobalPosition End_Pos = new GlobalPosition(new GlobalCoordinates(TrajectoryPoints[End_Index].Position.GetLatLongDecimal().LatitudeDecimal, TrajectoryPoints[End_Index].Position.GetLatLongDecimal().LongitudeDecimal));

                // Get the distance and conver it to NM
                double distance = geoCalc.CalculateGeodeticMeasurement(reference, Start_Pos, End_Pos).PointToPointDistance;
                if (distance > 0)
                {
                    distance = (distance / 100.0) * (double)Time_Factor;
                    distance = 0.00053996 * distance;

                    ////////////////////////////////////////////////////////////

                    // Calculate the azimuth between two known points
                    Angle Azimuth = geoCalc.CalculateGeodeticMeasurement(reference, Start_Pos, End_Pos).Azimuth;

                    // Calculate new position
                    GeoCordSystemDegMinSecUtilities.LatLongClass New_Position =
                    GeoCordSystemDegMinSecUtilities.CalculateNewPosition(new GeoCordSystemDegMinSecUtilities.LatLongClass(Start_Pos.Latitude.Degrees, Start_Pos.Longitude.Degrees), distance, Azimuth.Degrees);
                    ENTRY_AOI_POINT = new GeoCordSystemDegMinSecUtilities.LatLongClass(New_Position.GetLatLongDecimal().LatitudeDecimal, New_Position.GetLatLongDecimal().LongitudeDecimal);
                }
                else
                {
                    ENTRY_AOI_POINT = new GeoCordSystemDegMinSecUtilities.LatLongClass(Start_Pos.Latitude.Radians, Start_Pos.Longitude.Radians);
                }
            }

            // Get indexes of WPT before and after AOI EXIT crossing crossing border
            Get_Start_End_WPT_Index(AOI_EXIT_TIME, out Start_Index, out End_Index, out Start_End_WPT_Search_Status);
            if (Start_End_WPT_Search_Status == true)
            {
                // First take care of the FL
                Start_FL = int.Parse(TrajectoryPoints[Start_Index].Flight_Level);
                End_FL = int.Parse(TrajectoryPoints[End_Index].Flight_Level);
                FL_DIFFERENCE = End_FL - Start_FL;

                Start_To_Sector = AOI_EXIT_TIME - CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[Start_Index].ETO);
                Start_To_End = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[End_Index].ETO) - CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[Start_Index].ETO);
                Time_Factor = Start_To_Sector.TotalSeconds / Start_To_End.TotalSeconds;
                Sector_Crossing_FL = Start_FL + (FL_DIFFERENCE * Time_Factor);
                AOI_EXIT_FL = Math.Round(Sector_Crossing_FL).ToString();

                GlobalPosition Start_Pos = new GlobalPosition(new GlobalCoordinates(TrajectoryPoints[Start_Index].Position.GetLatLongDecimal().LatitudeDecimal, TrajectoryPoints[Start_Index].Position.GetLatLongDecimal().LongitudeDecimal));
                GlobalPosition End_Pos = new GlobalPosition(new GlobalCoordinates(TrajectoryPoints[End_Index].Position.GetLatLongDecimal().LatitudeDecimal, TrajectoryPoints[End_Index].Position.GetLatLongDecimal().LongitudeDecimal));

                // Get the distance and conver it to NM
                double distance = geoCalc.CalculateGeodeticMeasurement(reference, Start_Pos, End_Pos).PointToPointDistance;
                distance = (distance / 100.0) * (double)Time_Factor;
                distance = 0.00053996 * distance;
                ////////////////////////////////////////////////////////////

                // Calculate the azimuth between two known points
                Angle Azimuth = geoCalc.CalculateGeodeticMeasurement(reference, Start_Pos, End_Pos).Azimuth;

                // Calculate new position
                GeoCordSystemDegMinSecUtilities.LatLongClass New_Position =
                GeoCordSystemDegMinSecUtilities.CalculateNewPosition(new GeoCordSystemDegMinSecUtilities.LatLongClass(Start_Pos.Latitude.Degrees, Start_Pos.Longitude.Degrees), distance, Azimuth.Degrees);
                EXIT_AOI_POINT = new GeoCordSystemDegMinSecUtilities.LatLongClass(New_Position.GetLatLongDecimal().LatitudeDecimal, New_Position.GetLatLongDecimal().LongitudeDecimal);
            }
        }

        //// This method is to be called once Sector and Trajectory
        //// lists are populated (upon comleting EFD message extraction)
        //public void CalculateSectorEntry_Exit_Times(ref List<Sector> Sector_List)
        //{
        //    // Loop through the sector list and calculate
        //    // entry/exit levels
        //    for (int i = 0; i < Sector_List.Count; i++)
        //    {
        //        // First take care of the sector entry FL
        //        int Start_Index = 0;
        //        int End_Index = 0;
        //        bool Start_End_WPT_Search_Status = false;

        //        int Start_FL;
        //        int End_FL;
        //        int FL_DIFFERENCE;
        //        TimeSpan Start_To_Sector;
        //        TimeSpan Start_To_End;
        //        double Time_Factor;
        //        double Sector_Crossing_FL;

        //        // Get indexes of WPT before and after sector crossing border
        //        Get_Start_End_WPT_Index(Sector_List[i].SECTOR_ENTRY_TIME, out Start_Index, out End_Index, out Start_End_WPT_Search_Status);
        //        if (Start_End_WPT_Search_Status == true)
        //        {
        //            Start_FL = int.Parse(TrajectoryPoints[Start_Index].Flight_Level);
        //            End_FL = int.Parse(TrajectoryPoints[End_Index].Flight_Level);
        //            FL_DIFFERENCE = End_FL - Start_FL;

        //            Start_To_Sector = Sector_List[i].SECTOR_ENTRY_TIME - CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[Start_Index].ETO);
        //            Start_To_End = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[End_Index].ETO) - CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[Start_Index].ETO);
        //            Time_Factor = Start_To_Sector.TotalSeconds / Start_To_End.TotalSeconds;
        //            Sector_Crossing_FL = Start_FL + (FL_DIFFERENCE * Time_Factor);
        //            Sector_List[i].EFL = Math.Round(Sector_Crossing_FL).ToString();
        //        }

        //        // Now calculate sector exit FL
        //        // Get indexes of WPT before and after sector crossing border
        //        Get_Start_End_WPT_Index(Sector_List[i].SECTOR_EXIT_TIME, out Start_Index, out End_Index, out Start_End_WPT_Search_Status);
        //        if (Start_End_WPT_Search_Status == true)
        //        {
        //            Start_FL = int.Parse(TrajectoryPoints[Start_Index].Flight_Level);
        //            End_FL = int.Parse(TrajectoryPoints[End_Index].Flight_Level);
        //            FL_DIFFERENCE = End_FL - Start_FL;

        //            Start_To_Sector = Sector_List[i].SECTOR_EXIT_TIME - CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[Start_Index].ETO);
        //            Start_To_End = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[End_Index].ETO) - CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[Start_Index].ETO);
        //            Time_Factor = Start_To_Sector.TotalSeconds / Start_To_End.TotalSeconds;
        //            Sector_Crossing_FL = Start_FL + (FL_DIFFERENCE * Time_Factor);
        //            Sector_List[i].XFL = Math.Round(Sector_Crossing_FL).ToString();
        //        }
        //    }
        //}

        // This method returns the indexes of the two WPT points (before and after) given time.
        // Intended use is to obtain points before and after expected sector crossing. The points
        // have expected FL and times and based on that it is possible to calculate sector crossing FL
        private void Get_Start_End_WPT_Index(DateTime TimeAtPoint, out int Start, out int End, out bool Succefull)
        {
            Start = 0;
            End = 0;
            Succefull = false;
            for (int i = 0; i < TrajectoryPoints.Count; i++)
            {
                TimeSpan Time = CBS_Main.GetDate_Time_From_YYYYMMDDHHMMSS("20" + TrajectoryPoints[i].ETO) - TimeAtPoint;
                if (Time.TotalSeconds > 0)
                {
                    Start = i - 1;
                    End = i;
                    // Check for the special case when the first points in the
                    // list is also sector entry point.
                    if (Start < 0)
                        Start = 0;

                    Succefull = true;
                    break;
                }
            }


        }

        // 1. Remove all "-VEC points from the end of the list
        // 2. Determine Lon/Lat of each -VEC point
        public void ParseTrajectoryList(ref List<Waypoint> T_List)
        {
            ////////////////////////////////////////////////////////////////////////////////////////
            // 1. Remove all "-VEC points from the end of the list
            ////////////////////////////////////////////////////////////////////////////////////////
            int LastKnownPointIndex = -1;
            for (int i = 0; i < T_List.Count; i++)
            {
                if (T_List[i].Name.Length == 6 && T_List[i].Name.Substring(0, 4) == "-VEC")
                {

                }
                else
                {
                    LastKnownPointIndex = i;
                }
            }

            if (LastKnownPointIndex < (T_List.Count - 1))
            {
                try
                {
                    T_List.RemoveRange(LastKnownPointIndex + 1, T_List.Count - (LastKnownPointIndex + 1));
                }
                catch (Exception e)
                {
                    CBS_Main.WriteToLogFile("Exception removing VEC range: " + e.Message);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////
            // 2. Determine Lon/Lat of each -VEC point
            ////////////////////////////////////////////////////////////////////////////////////////////
            for (int i = 0; i < T_List.Count; i++)
            {
                // Check if this is a -VEC point
                if (T_List[i].Name.Length == 6 && T_List[i].Name.Substring(0, 4) == "-VEC")
                {
                    // 1. YES, then LastKnownPointIndex is the index of the previous last known point
                    // 2. Now determine the next known point
                    int next_known_point = GetNextKnownPointIndex(i + 1, ref T_List);

                    // Now we have two known points, so lets compute a new position representing
                    // the percentage of the distance between two known points specifed in the -VEC
                    // point.
                    int percentage = int.Parse(T_List[i].Name.Substring(4, 2));

                    // select a reference elllipsoid
                    Ellipsoid reference = Ellipsoid.WGS84;
                    // instantiate the calculator
                    GeodeticCalculator geoCalc = new GeodeticCalculator();

                    GlobalPosition Start_Pos = new GlobalPosition(new GlobalCoordinates(T_List[LastKnownPointIndex].Position.GetLatLongDecimal().LatitudeDecimal, T_List[LastKnownPointIndex].Position.GetLatLongDecimal().LongitudeDecimal));
                    GlobalPosition End_Pos = new GlobalPosition(new GlobalCoordinates(T_List[next_known_point].Position.GetLatLongDecimal().LatitudeDecimal, T_List[next_known_point].Position.GetLatLongDecimal().LongitudeDecimal));

                    // Get the distance and conver it to NM
                    double distance = geoCalc.CalculateGeodeticMeasurement(reference, Start_Pos, End_Pos).PointToPointDistance;
                    distance = (distance / 100.0) * (double)percentage;
                    distance = 0.00053996 * distance;
                    ////////////////////////////////////////////////////////////

                    // Calculate the azimuth between two known points
                    Angle Azimuth = geoCalc.CalculateGeodeticMeasurement(reference, Start_Pos, End_Pos).Azimuth;

                    // Calculate new position
                    GeoCordSystemDegMinSecUtilities.LatLongClass New_Position =
                    GeoCordSystemDegMinSecUtilities.CalculateNewPosition(new GeoCordSystemDegMinSecUtilities.LatLongClass(Start_Pos.Latitude.Degrees, Start_Pos.Longitude.Degrees), distance, Azimuth.Degrees);

                    // Assign new position to the -VEC point
                    T_List[i].Position = new GeoCordSystemDegMinSecUtilities.LatLongClass(New_Position.GetLatLongDecimal().LatitudeDecimal, New_Position.GetLatLongDecimal().LongitudeDecimal);
                }
                else
                {
                    LastKnownPointIndex = i;
                }
            }
        }

        public int GetNextKnownPointIndex(int cur_index, ref List<Waypoint> T_List)
        {
            int return_index = 0;
            for (int i = cur_index; i < T_List.Count; i++)
            {
                if (T_List[i].Name.Length == 6 && T_List[i].Name.Substring(0, 4) == "-VEC")
                {
                }
                else
                {
                    return_index = i;
                    break;
                }
            }
            return return_index;
        }

        public bool Is_New_Data_Set()
        {
            bool New_Data_Set = false;
            string FileName = Get_Dir_By_ACID_AND_IFPLID(ACID, IFPLID);
            char[] delimiterChars = { ' ' };
            StreamReader MyStreamReader = null;
            string Data_Set;

            try
            {
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
            }
            catch (Exception e)
            {
                CBS_Main.WriteToLogFile("Error in EFD Message " + e.Message);
                MyStreamReader.Close();
                MyStreamReader.Dispose();
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
                CBS_Main.WriteToLogFile("Exception in EFD_Msg.cs, Saving " + FileName);
                // close the stream
                tw.Close();
                tw.Dispose();
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


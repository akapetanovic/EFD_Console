using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CBS
{
    public static class MySqlWriter
    {
        private static string connString;
        private static MySqlConnection MySQLconn;

        public static class MySQLConnetionString
        {
            public static string login_name = "root";
            public static string server_name = "localhost";
            public static string database = "flights";
            public static string table_name = "tms3";
        }

        public static void CloseConnection()
        {
                MySQLconn.Close();
        }

        // ARCID
        // IFPLID 
        // FLTSTATE  
        // ADEP 
        // ADES 
        // ARCTYP 
        // ETI 
        // XTI 
        // LASTUPD 
        // TSTARTTIME
        // TENDTIME
        // TPOINTS
        // POINTS        
        public static void Write_One_Message(EFD_Msg Message)
        {
            //// Lets build SEQMUAC string
            //// FORMAT:
            //// //RH1,1515,1522//RH2,1515,1522
            //string SEQMUAC = "";
            //foreach (EFD_Msg.Sector Msg in Message.Sector_List)
            //{
            //    SEQMUAC = SEQMUAC + "//" + Msg.ID + ',' + GetTimeAS_HHMM(Msg.SECTOR_ENTRY_TIME) + ',' + GetTimeAS_HHMM(Msg.SECTOR_EXIT_TIME) + ',' + Msg.EFL + ',' + Msg.XFL;
            //}

            // Lets build TPOINTS string
            //tpoints (LON,LAT,FL,broj sekundi u odnosu na tstarttime) u formatu:
            //51.28083,6.75722,0,0;51.59056,6.69417,11900,431

            string TPOINTS = "";
            int Index = 0;
            TimeSpan TS = new TimeSpan(0);
            foreach (EFD_Msg.Waypoint WPT in Message.TrajectoryPoints)
            {
                if (Index > 0)
                {
                    TS = CBS_Main.GetDate_Time_From_YYMMDDHHMMSS(WPT.ETO) - CBS_Main.GetDate_Time_From_YYMMDDHHMMSS(Message.TrajectoryPoints[0].ETO);
                }
               
                TPOINTS = TPOINTS + string.Format("{0:0.0000}", WPT.Position.GetLatLongDecimal().LongitudeDecimal) + ',' + string.Format("{0:0.0000}", WPT.Position.GetLatLongDecimal().LatitudeDecimal)
                    + ',' + WPT.Flight_Level + ',' + TS.TotalSeconds.ToString() + ";";

                Index++;
            }

            // LASTUPD
            DateTime T_Now = DateTime.UtcNow;
            string LASTUPD = T_Now.Year.ToString("0000") + T_Now.Month.ToString("00") + T_Now.Day.ToString("00") +
                T_Now.Hour.ToString("00") + T_Now.Minute.ToString("00") + T_Now.Second.ToString("00");

            string TSTARTTIME = ConvertToUNIXTimestamp(CBS_Main.GetDate_Time_From_YYMMDDHHMMSS(Message.TrajectoryPoints[0].ETO)).ToString();
            string TENDTIME = ConvertToUNIXTimestamp(CBS_Main.GetDate_Time_From_YYMMDDHHMMSS(Message.TrajectoryPoints[Message.TrajectoryPoints.Count - 1].ETO)).ToString();
            
            string query = "INSERT INTO " + MySQLConnetionString.table_name +
                " (IFPLID, ARCID, FLTSTATE, ADEP, ADES, ARCTYP, ETI, XTI, LASTUPD, TSTARTTIME, TENDTIME, TPOINTS) " +
                " VALUES ("  + Get_With_Quitation(Message.IFPLID) + "," +
                             Get_With_Quitation(Message.ACID) + "," +
                             Get_With_Quitation(Message.FLTSTATE) + "," +
                             Get_With_Quitation(Message.ADEP) + "," +
                             Get_With_Quitation(Message.ADES) + "," +
                             Get_With_Quitation(Message.ARCTYP) + "," +
                             Get_With_Quitation(CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(Message.AOI_ENTRY_TIME)) + "," +
                             Get_With_Quitation(CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(Message.AOI_EXIT_TIME)) + "," +
                             Get_With_Quitation(LASTUPD) + "," +
                             Get_With_Quitation(TSTARTTIME) + "," +
                             Get_With_Quitation(TENDTIME) + "," +
                             Get_With_Quitation(TPOINTS) + ")";

            
            
            // First delete the data for the flight (if already exists)
            string delete_query = "DELETE FROM " + MySQLConnetionString.table_name + " WHERE IFPLID= " + Get_With_Quitation(Message.IFPLID) + "AND ARCID= " + Get_With_Quitation(Message.ACID);
            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(delete_query, MySQLconn);

            try
            {
                //Execute command
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //create command and assign the query and connection from the constructor
            cmd = new MySqlCommand(query, MySQLconn);

            try
            {
                //Execute command
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private static double ConvertToUNIXTimestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (double)span.TotalSeconds;
        }

        private static string Get_With_Quitation(string String_IN)
        {
            return "'" + String_IN + "'";
        }

        private static string GetTimeAS_HHMM(DateTime Time_In)
        {
            return Time_In.Hour.ToString("00") + Time_In.Minute.ToString("00");
        }

        public static void Initialise()
        {
            // Set the connection string
            connString = "server=" + MySQLConnetionString.server_name +
               ";User Id=" + MySQLConnetionString.login_name + ";database=" + MySQLConnetionString.database;

            // Open up the connection
            MySQLconn = new MySqlConnection(connString);

            try
            {
                MySQLconn.Open();
                CBS_Main.WriteToLogFile("Opened up MySQL connection " + connString);
            }
            catch (Exception e)
            {
                CBS_Main.WriteToLogFile("My SQL Error: " + e.Message);
            }
        }
    }
}

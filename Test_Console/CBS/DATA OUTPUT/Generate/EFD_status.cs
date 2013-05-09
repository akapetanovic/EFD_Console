using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CBS
{
    class EFD_Status
    {
        ///var/cbs/prediction/flights/ACID_IFPLID_DATETIME/status/EFD_{Status}_{DATETIME}.kml 
        //<?xml version="1.0" encoding="UTF-8"?>
        //<kml xmlns="http://www.opengis.net/kml/2.2">
        //<Document>
        //<Placemark>
        //    <name>EFD Status {Inflight}</name>
        //    <TimeStamp> <!-- required TimeStamp or TimeSpan block -->
        //        <when>2013-02-20T00:05:20Z</when>
        //    </TimeStamp>
        //    <ExtendedData>
        //      <Data name="markerType">
        //          <value> timelineItem</value>
        //      </Data>
        //     <Data name="dataSourceName">
        //          <value>EFD</value>
        //      </Data>
        //    </ExtendedData>
        //</Placemark>
        //</Document>
        //</kml>
        public static void Generate_Output(EFD_Msg Message_Data)
        {
            string Time_Stamp = KML_Common.Get_KML_Time_Stamp();
            string KML_File_Content =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine +
                    "<kml xmlns=\"http://www.opengis.net/kml/2.2\">" + Environment.NewLine +
                    "<Document>" + Environment.NewLine +
                    "<Placemark>" + Environment.NewLine +
                        "<name>EFD Status " + Message_Data.FL_STATUS + "</name>" + Environment.NewLine +
                        "<TimeStamp> <!-- required TimeStamp or TimeSpan block -->" + Environment.NewLine +
                            "<when>" + Time_Stamp + "</when>" + Environment.NewLine +
                        "</TimeStamp>" + Environment.NewLine +
                        "<ExtendedData>" + Environment.NewLine +
                            "<Data name=\"markerType\">" + Environment.NewLine +
                                "<value> timelineItem</value>" + Environment.NewLine +
                            "</Data>" + Environment.NewLine +
                            "<Data name=\"dataSourceName\">" + Environment.NewLine +
                                "<value>EFD</value>" + Environment.NewLine +
                            "</Data>" + Environment.NewLine +
                        "</ExtendedData>" + Environment.NewLine +
                    "</Placemark>" + Environment.NewLine +
                    "</Document>" + Environment.NewLine +
                    "</kml>";

            // Get the final data path
            string File_Path = Get_Dir_By_ACID_AND_IFPLID(Message_Data.ACID, Message_Data.IFPLID);
            File_Path = Path.Combine(File_Path, ("EFD_" + Message_Data.FL_STATUS + '_' + CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.UtcNow) + ".kml"));

            // Save data in the tmp directory first
            string Tmp = Path.Combine(CBS_Main.Get_Temp_Dir(), ("EFD_" + Message_Data.FL_STATUS + '_' + CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.UtcNow) + ".kml"));

            // create a writer and open the file
            TextWriter tw = new StreamWriter(Tmp);

            try
            {
                // write a line of text to the file
                tw.Write(KML_File_Content);
            }
            catch
            {

            }

            // close the stream
            tw.Close();

            // Now move it to the final destination
            File.Move(Tmp, File_Path);
        }
        public static string Get_Dir_By_ACID_AND_IFPLID(string ACID, string IFPLID)
        {
            string DIR = "";
            // First check if directory already exists
            string IFPLID_DIR_NAME = ACID + "_" + IFPLID + "_*";
            string[] DestDirectory = Directory.GetDirectories(CBS_Main.Get_Destination_Dir(), IFPLID_DIR_NAME);
            if (DestDirectory.Length == 1)
            {
                DIR = DestDirectory[0];
                DIR = Path.Combine(DIR, "status");
            }
            return DIR;
        }
    }
}

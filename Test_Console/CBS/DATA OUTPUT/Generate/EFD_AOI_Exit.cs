using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CBS
{
    class EFD_AOI_Exit
    {
        // /var/cbs/prediction/flights/ACID_IFPLID_DATETIME/EFD/EFD_AOI_Exit_DATETIME.kml

            //<?xml version="1.0" encoding="UTF-8"?>
            //<kml xmlns="http://www.opengis.net/kml/2.2">
            //<Document>
            //    <Placemark>
            //        <name>EFD AOI Exit</name>
            //        <TimeStamp>
            //            <when>2013-02-20T00:05:20Z</when>
            //        </TimeStamp>
            //        <ExtendedData>
            //            <Data name="dataSourceName">
            //                <value>EFD</value>
            //            </Data>
            //            <Data name="markerType">
            //                <value>customMarker</value>
            //            </Data>
            //            <Data name="customIcon">
            //                <value>imageGoogleYellow.png</value>
            //            </Data>
            //            <Data name="popupLine1">
            //                <value>Time:{TIME}</value>
            //            </Data>
            //            <Data name="popupLine2">
            //                <value>Point:{LON,LAT}</value>
            //            </Data>
            //            <Data name="popupLine3">
            //                <value>Altitude:{Altitude}</value>
            //            </Data>
            //            <Data name="fileLocation">
            //                <value>flights/ACID_IFPLID_DATETIME/EFD/EFD_AOI_Exit_DATETIME.kml</value>
            //            </Data>
            //        </ExtendedData>
            //        <Point>
            //            <coordinates>1.33410,52.34636,10965,20130305013800</coordinates>
            //        </Point>
            //    </Placemark>
            //</Document>
            //</kml>
        public static void Generate_Output (EFD_Msg Message_Data)
        {
            string TIME_AS_YYYYMMDDHHMMSS = CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.UtcNow);
            string Time_Stamp = KML_Common.Get_KML_Time_Stamp();
            string Exit_LON, Exit_LAT;
            Message_Data.ENTRY_AOI_POINT.GetDegMinSecStringFormat(out Exit_LAT, out Exit_LON);

            string KML_File_Content =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine +
                    "<kml xmlns=\"http://www.opengis.net/kml/2.2\">" + Environment.NewLine +
                    "<Document>" + Environment.NewLine +
                "<Placemark>" + Environment.NewLine +
                    "<name>EFD AOI Exit</name>" + Environment.NewLine +
                    "<TimeStamp>" + Environment.NewLine +
                        "<when>" + Time_Stamp + "</when>" + Environment.NewLine +
                    "</TimeStamp>" + Environment.NewLine +
                    "<ExtendedData>" + Environment.NewLine +
                    "<Data name=\"dataSourceName\">" + Environment.NewLine +
                            "<value>EFD</value>" + Environment.NewLine +
                        "</Data>" + Environment.NewLine +
                        "<Data name=\"markerType\">" + Environment.NewLine +
                            "<value>customMarker</value>" + Environment.NewLine +
                        "</Data>" + Environment.NewLine +
                        "<Data name=\"customIcon\">" + Environment.NewLine +
                            "<value>imageGoogleYellow.png</value>" + Environment.NewLine +
                        "</Data>" + Environment.NewLine +

                        "<Data name=\"popupLine1\">" + Environment.NewLine +
                            "<value>Time:" + Message_Data.EXIT_AOI_TIME.ToShortDateString() + "/" + Message_Data.EXIT_AOI_TIME.ToShortTimeString() + "</value>" + Environment.NewLine +
                        "</Data>" + Environment.NewLine +

                        "<Data name=\"popupLine2\">" + Environment.NewLine +
                            "<value>Point:" + Exit_LON + "'" + Exit_LAT + "</value>" + Environment.NewLine +
                        "</Data>" + Environment.NewLine +

                        "<Data name=\"popupLine3\">" + Environment.NewLine +
                            "<value>Altitude:" + Message_Data.Exit_FL + "</value>" + Environment.NewLine +
                        "</Data>" + Environment.NewLine +

                        "<Data name=\"fileLocation\">" + Environment.NewLine +
                            "<value>flights/ACID_IFPLID_DATETIME/EFD/EFD_AOI_Exit_" + TIME_AS_YYYYMMDDHHMMSS + ".kml" + "</value>" + Environment.NewLine +
                        "</Data>" + Environment.NewLine +

                        "</ExtendedData>" + Environment.NewLine +
                    "<Point>" + Environment.NewLine +
                        "<coordinates>12.09607,51.41915,1201,20130305003900</coordinates>" + Environment.NewLine +
                    "</Point>" + Environment.NewLine +

                    "</Placemark>" + Environment.NewLine +
            "</Document>" + Environment.NewLine +
            "</kml>";

            // Get the final data path
            string File_Path = Get_Dir_By_ACID_AND_IFPLID(Message_Data.ACID, Message_Data.IFPLID);
            File_Path = Path.Combine(File_Path, ("EFD_AOI_Exit_" + TIME_AS_YYYYMMDDHHMMSS + ".kml"));

            // Save data in the tmp directory first
            string Tmp = Path.Combine(CBS_Main.Get_Temp_Dir(), ("EFD_AOI_Exit_" + TIME_AS_YYYYMMDDHHMMSS + ".kml"));

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
                DIR = Path.Combine(DIR, "EFD");
            }
            return DIR;
        }
    }
}

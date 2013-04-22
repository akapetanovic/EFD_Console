using System;
using System.Collections.Generic;
using System.Text;
using Google.KML;

namespace CBS
{
    class EFD_AOI_Entry
    {
        // /var/cbs/prediction/flights/ACID_IFPLID_DATETIME/EFD/EFD_AOI_Entry_DATETIME.kml

        //<?xml version="1.0" encoding="UTF-8"?>
        //<kml xmlns="http://www.opengis.net/kml/2.2">
        //<Document>
        //    <Placemark>
        //        <name>EFD AOI Entry</name>
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
        //                <value>flights/ACID_IFPLID_DATETIME/EFD/EFD_AOI_Entry_DATETIME.kml</value>
        //            </Data>
        //        </ExtendedData>
        //        <Point>
        //            <coordinates>12.09607,51.41915,1201,20130305003900</coordinates>
        //        </Point>
        //    </Placemark>
        //</Document>
        //</kml>
        public static void Generate_Output (EFD_Msg Message_Data)
        {
           
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;

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
        public static void Generate_Output (EFD_Msg Message_Data)
        {

        }
    }
}

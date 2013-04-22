using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace CBS
{
    class Common
    {
        //<FPL_data>
        //        <CLS>BAW905U</CLS> 
        //        <ADEP>EDDF</ADEP> 
        //        <ADES>EGLL</ADES> 
        //        <EOBT>HHMM</EOBT> 
        //        <EOBD>YYMMDD</EOBD> 
        //        <IFPLID>AA92346961</IFPLID> 
        //</FPL_data>

        public static void Generate_Output (EFD_Msg Message_Data)
        {
            XmlDocument XDoc = new XmlDocument();

            // Create root node.
            XmlElement XElemRoot = XDoc.CreateElement("FPL_Data");
            //Add the node to the document.
            XDoc.AppendChild(XElemRoot);

            //////////////////////////////////////////////////
            // Create Nodes
            //
            XmlElement XTemp = XDoc.CreateElement("CLS");
            XTemp.InnerText = Message_Data.ACID;
            XElemRoot.AppendChild(XTemp);

            XTemp = XDoc.CreateElement("ADEP");
            XTemp.InnerText = Message_Data.ADEP;
            XElemRoot.AppendChild(XTemp);

            XTemp = XDoc.CreateElement("ADES");
            XTemp.InnerText = Message_Data.ADES;
            XElemRoot.AppendChild(XTemp);

            XTemp = XDoc.CreateElement("EOBT");
            XTemp.InnerText = Message_Data.EOBT;
            XElemRoot.AppendChild(XTemp);

            XTemp = XDoc.CreateElement("EOBD");
            XTemp.InnerText = Message_Data.EOBD;
            XElemRoot.AppendChild(XTemp);

            XTemp = XDoc.CreateElement("IFPLID");
            XTemp.InnerText = Message_Data.IFPLID;
            XElemRoot.AppendChild(XTemp);

            string File_Path = Get_Dir_By_ACID_AND_IFPLID(Message_Data.ACID, Message_Data.IFPLID);
            File_Path = Path.Combine(File_Path, ("Flight_Data_EFD_" + CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.UtcNow) + ".xml"));

            string Tmp = Path.Combine(CBS_Main.Get_Temp_Dir(), ("Flight_Data_EFD_" + CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.UtcNow) + ".xml"));
            XDoc.Save(Tmp);
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
                DIR = Path.Combine(DIR, "Common");
            }

            return DIR;
        }
    }
}

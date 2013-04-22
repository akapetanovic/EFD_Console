using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace CBS
{
    class System_Status
    {
        // Periodically create empty file called /var/cbs/prediction/systemStatus/EFD_Status.xml
        // The file is created empty unless no EFD data is recived after N minutes. In that case
        // the following file is created:
        //
        // <EFDStatus>
        //      <Status>ERROR</Status>
        //      <Message>No Incoming EFD Data</Message>
        //      <TimeStamp>DateTime</TimeStamp>
        // </EFDStatus>
        //
        private static bool No_EFD_DATA_Flag_Last_Cycle = false;

        public static void Generate(bool No_EFD_DATA_Flag)
        {
            if ((No_EFD_DATA_Flag == true) && (No_EFD_DATA_Flag_Last_Cycle == false))
            {
                XmlDocument XDoc = new XmlDocument();
               

                // Create root node.
                XmlElement XElemRoot = XDoc.CreateElement("EFDStatus");
                //Add the node to the document.
                XDoc.AppendChild(XElemRoot);

                //////////////////////////////////////////////////
                // Create Nodes
                //
                XmlElement XTemp = XDoc.CreateElement("Status");
                XTemp.InnerText = "ERROR";
                XElemRoot.AppendChild(XTemp);

                XTemp = XDoc.CreateElement("Message");
                XTemp.InnerText = "No Incoming EFD Data";
                XElemRoot.AppendChild(XTemp);

                XTemp = XDoc.CreateElement("TimeStamp");
                XTemp.InnerText = CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.Now);
                XElemRoot.AppendChild(XTemp);

                string File_Path = Path.Combine(CBS_Main.Get_System_Status_Dir(), ("EFD_Status.xml"));
                string Tmp = Path.Combine(CBS_Main.Get_Temp_Dir(), ("EFD_Status.xml"));
                XDoc.Save(Tmp);
                File.Move(Tmp, File_Path);
            }
            else if (No_EFD_DATA_Flag == false)
            {
                string File_Path = Path.Combine(CBS_Main.Get_System_Status_Dir(), ("EFD_Status.xml"));
                if (File.Exists(File_Path))
                    File.Delete(File_Path);

                FileStream fs = File.Create(File_Path);
                fs.Close();
                fs.Dispose();

            }

            No_EFD_DATA_Flag_Last_Cycle = No_EFD_DATA_Flag;
        }
    }
}

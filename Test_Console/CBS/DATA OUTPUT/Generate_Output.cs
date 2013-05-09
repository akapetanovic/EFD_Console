using System;
using System.IO;

namespace CBS
{
	public static class Generate_Output
	{
		public static void Generate(EFD_Msg Message_Data)
		{
            // First check if directory already exists
            string IFPLID_DIR_NAME = Message_Data.ACID + "_" + Message_Data.IFPLID + "_*";
            string[] DestDirectory = Directory.GetDirectories(CBS_Main.Get_Destination_Dir(), IFPLID_DIR_NAME);
            if (DestDirectory.Length == 0)
            {
                // This must be a new flight, so lets create applicable directory
                IFPLID_DIR_NAME = Message_Data.ACID + "_" + Message_Data.IFPLID + "_";
                IFPLID_DIR_NAME = IFPLID_DIR_NAME + CBS_Main.GetDate_Time_AS_YYYYMMDDHHMMSS(DateTime.UtcNow);
                Directory.CreateDirectory(CBS_Main.Get_Destination_Dir() + IFPLID_DIR_NAME);
                // Now when it is created, lets get it again so it can be used
                DestDirectory = Directory.GetDirectories(CBS_Main.Get_Destination_Dir(), IFPLID_DIR_NAME);

                // Now create subdirectories within new directory
                Directory.CreateDirectory(Path.Combine(DestDirectory[0], "common"));
                Directory.CreateDirectory(Path.Combine(DestDirectory[0], "EFD"));
                Directory.CreateDirectory(Path.Combine(DestDirectory[0], "status"));
            }

            Common.Generate_Output(Message_Data);
            EFD.Generate_Output(Message_Data);
            Main_Status.Generate_Output(Message_Data);
            EFD_Status.Generate_Output(Message_Data);
		}
	}
}


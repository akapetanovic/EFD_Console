
namespace CBS
{
    class EFD
    {
        // EFD_AOI_Entry_DATETIME.kml
        // EFD_AOI_Exit_DATETIME.kml
        // EFD_Trajetory_DATETIME.kml
        public static void Generate_Output (EFD_Msg Message_Data)
        {
            EFD_AOI_Entry.Generate_Output(Message_Data);
            EFD_AOI_Exit.Generate_Output(Message_Data);
            EFD_Trajectory.Generate_Output(Message_Data);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CBS
{
    class Main_Status
    {
        // /var/cbs/prediction/status/MAIN_{Status}_{DATETIME}.kml 

        // If the Status=closed, no new files are written for this flight. 
        // ( This part is not fully designed yet.  The idea is to inform other modules that
        // a flight has left AOI and the MAIN is going to close the directory.  
        // It is not so important for EFD, since EFD has the IFPLID as the key.  
        // ADS-B module, Message modul can see this flight hours after it leaves AOI.)
        public static void Generate_Output (EFD_Msg Message_Data)
        {

        }
    }
}

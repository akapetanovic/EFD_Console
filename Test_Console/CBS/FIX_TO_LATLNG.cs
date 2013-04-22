using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CBS
{
    public class FIX_TO_LATLNG
    {
        public class FIXPOINT_TYPE
        {
            public bool Is_Found = false;
            public string Name;
            private GeoCordSystemDegMinSecUtilities.LatLongClass Position;

            public void SetPosition(double LAT, double LNG)
            {
                Position = new GeoCordSystemDegMinSecUtilities.LatLongClass(LAT, LNG);
            }
        }

        public static FIXPOINT_TYPE Get_LATLNG(string FIX)
        {
            FIXPOINT_TYPE Return_FIX = new FIXPOINT_TYPE();
            string FIXPOINT_Data;
            string FileName = Path.Combine(CBS_Main.Get_AIRAC_Dir(), "fixpoints");
            char[] delimiterChars = { ';' };
            StreamReader MyStreamReader;
            DateTime StartTime = DateTime.UtcNow;
            if (File.Exists(FileName))
            {
                // Lets read in settings from the file
                MyStreamReader = System.IO.File.OpenText(FileName);
               
                while (MyStreamReader.Peek() >= 0)
                {
                    FIXPOINT_Data = MyStreamReader.ReadLine();
                    string[] words = FIXPOINT_Data.Split(delimiterChars);

                    if (words[0] == FIX)
                    {
                        Return_FIX.Is_Found = true;
                        Return_FIX.Name = FIX;
                        
                        string sLAT = words[4];
                        if (words[4][0] == '.')
                            sLAT = '0' + words[4];

                        string sLON = words[5];
                        if (words[5][0] == '.')
                            sLON = '0' + words[5];

                        double LAT = double.Parse(sLAT);
                        double LNG = double.Parse(sLON);
                        Return_FIX.SetPosition(LAT, LNG);
                        break;
                    }
                }
            }
            return Return_FIX;
        }
    }
}

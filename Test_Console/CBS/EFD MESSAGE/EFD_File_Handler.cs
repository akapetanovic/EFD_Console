using System;
using System.IO;
using System.Threading;
using System.Timers;

namespace CBS
{
    public static class EFD_File_Handler
    {
        private static StreamReader MyStreamReader;
        // Timer to periodically create EFD_Status.xml file
        // as status indication of the module. 

        private static System.Timers.Timer System_Status_Timer;

        public static void Initialise()
        {
            // Now start heart beat timer.
            System_Status_Timer = new System.Timers.Timer(1000); // Set up the timer for 1sec
            System_Status_Timer.Elapsed += new ElapsedEventHandler(System_Status_Periodic_Update);
            System_Status_Timer.Enabled = true;

            CBS_Main.WriteToLogFile("Starting 1sec timer to check for incomming EFD messages");
        }

        // Periodically call System Status Handler
        private static void System_Status_Periodic_Update(object sender, ElapsedEventArgs e)
        {
            System_Status_Timer.Enabled = false;
            Handle_New_File();
            System_Status_Timer.Enabled = true;
        }

        public static void Handle_New_File()
        {
            string[] filePaths = Directory.GetFiles(CBS_Main.Get_Source_Data_Path(), "*.log*",
                                         SearchOption.AllDirectories);

            foreach (string Path in filePaths)
            {
                try
                {
                    using (MyStreamReader = System.IO.File.OpenText(Path))
                    {
                        if (MyStreamReader != null)
                        {
                            //// Pass in stream reader and initialise new
                            //// EFD message. 
                            EFD_Msg EDF_MESSAGE = new EFD_Msg(MyStreamReader);

                            MyStreamReader.Close();
                            MyStreamReader.Dispose();

                            try
                            {
                                //// Generate output
                                Generate_Output.Generate(EDF_MESSAGE);
                            }
                            catch (Exception e1)
                            {
                                CBS_Main.WriteToLogFile("Error in Generate_Output.Generate " + e1.Message);
                            }

                            try
                            {
                                // Write data to the MySqlDatabase
                                MySqlWriter.Write_One_Message(EDF_MESSAGE);
                            }

                            catch (Exception e2)
                            {
                                CBS_Main.WriteToLogFile("Error in MySqlWriter.Write_One_Message " + e2.Message);
                            }

                            // Let the status handler know that the
                            // message has arrived...
                            try
                            {
                                CBS_Main.Notify_EFD_Message_Recived();
                            }
                            catch (Exception e3)
                            {
                                CBS_Main.WriteToLogFile("Error in CBS_Main.Notify_EFD_Message_Recived " + e3.Message);
                            }

                            //// Once done with the file, 
                            //// lets delete it as we do not
                            //// needed it any more
                            try
                            {
                                System.IO.File.Delete(Path);
                            }
                            catch
                            {
                                CBS_Main.WriteToLogFile("Error in EFD_File_Handler, can't delete file " + Path);
                            }

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CBS_Main.WriteToLogFile("Exception EFD_Handler: " + ex.Message);
                }      
            }
        }
    }
}


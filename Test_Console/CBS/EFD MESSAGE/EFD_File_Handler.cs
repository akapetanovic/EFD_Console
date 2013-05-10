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
            //////////////////////////////////////////////////////
            // Start periodic timer that will drive system status 
            // update logic
            // Now start heart beat timer.
            System_Status_Timer = new System.Timers.Timer((100)); // Set up the timer for 1minute
            System_Status_Timer.Elapsed += new ElapsedEventHandler(System_Status_Periodic_Update);
            System_Status_Timer.Enabled = true;
        }

        // Periodically call System Status Handler
        private static void System_Status_Periodic_Update(object sender, ElapsedEventArgs e)
        {
            Handle_New_File();
        }

        public static void Handle_New_File()
        {
            string[] filePaths = Directory.GetFiles(CBS_Main.Get_Source_Data_Path(), "*.log",
                                         SearchOption.TopDirectoryOnly);

            foreach (string Path in filePaths)
            {
                while (true)
                {
                    try
                    {
                        using (MyStreamReader = System.IO.File.OpenText(Path))
                        {
                            if (MyStreamReader != null)
                            {
                                Thread.Sleep(100);
                                //// Pass in stream reader and initialise new
                                //// EFD message. 
                                EFD_Msg EDF_MESSAGE = new EFD_Msg(MyStreamReader);

                                MyStreamReader.Close();
                                MyStreamReader.Dispose();

                                //// Generate output
                                Generate_Output.Generate(EDF_MESSAGE);

                                // Let the status handler know that the
                                // message has arrived...
                                CBS_Main.Notify_EFD_Message_Recived();

                                //// Once done with the file, 
                                //// lets delete it as we do not
                                //// needed it any more
                                try
                                {
                                    System.IO.File.Delete(Path);
                                }
                                catch
                                {
                                    Console.WriteLine("Error in EFD_File_Handle");
                                }

                                break;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        string T = ex.Message;
                    }
                    Thread.Sleep(500);
                }
            }

        }
    }
}


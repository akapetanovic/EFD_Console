using System;

namespace CBS
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool Run = true;

			CBS.CBS_Main.Initialize();

			Console.WriteLine("Starting EFD 1.2, to abort type stop");

			while (Run) {

				string STR = Console.ReadLine();

				switch (STR)
				{

				case "stop":
					Run = false;
					break;
				default:
					Console.WriteLine("Type stop to abort execution");
					break;
				}
			}
		}
	}
}

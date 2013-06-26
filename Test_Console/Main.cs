using System;

namespace CBS
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool Run = true;

			Console.WriteLine("Starting EFD 1.5, to abort type stop");

			CBS.CBS_Main.Initialize();

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

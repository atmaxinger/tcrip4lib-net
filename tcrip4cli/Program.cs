using System;
using tcrip4lib;


namespace tcrip4cli
{
	class MainClass
	{
		public static void PrintStatus(TCRIP4 tcrip4)
		{
			var status = tcrip4.GetStatus().Result;

			Console.WriteLine("Current Status:");
			foreach (var rel in status.Relais)
			{
				Console.WriteLine($"\t{ rel.Name }: { rel.On }");
			}
			Console.WriteLine();
			Console.WriteLine($"\tPot0: { status.Pot }");
			Console.WriteLine();

		}

		public static void Main(string[] args)
		{
			TCRIP4 tcrip4 = new TCRIP4();
			bool finished = false;

			Console.WriteLine("Welcome to TCRIP 4 CLI");
			Console.WriteLine("======================");
			Console.WriteLine();

			PrintStatus(tcrip4);

			int numberOfRelais = tcrip4.GetNumberOfRelais().Result;

			do
			{
				Console.Write("> ");
				var command = Console.ReadLine().Trim().ToLower();

				string[] split = command.Split(' ');

				if (split[0].StartsWith("q"))
				{
					finished = true;
				}
				else if (split.Length == 2)
				{
					try
					{
						int relay = int.Parse(split[1]);

						if (relay > 0 && relay <= numberOfRelais)
						{

							if (split[0].StartsWith("t"))
							{
								var now = tcrip4.ToggleRelay(relay).Result;
							}
							else if (split[0].StartsWith("on"))
							{
								var now = tcrip4.SwitchOn(relay).Result;
							}
							else if (split[0].StartsWith("off"))
							{
								var now = tcrip4.SwitchOff(relay).Result;
							}
						}
						else
						{
							throw new FormatException();
						}
					}
					catch (FormatException fe)
					{
						Console.WriteLine($"Not a valid relay number! (1 - { numberOfRelais })");
					}
				}
				else
				{
					Console.WriteLine("Unknown Command");
				}

				PrintStatus(tcrip4);
			} while (!finished);


			Console.WriteLine("Hello World!");
		}
	}
}

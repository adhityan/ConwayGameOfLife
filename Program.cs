using System;

namespace ConwayGameOfLife
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Conway c = new Conway("B3/S23", new string[] {"       ", "       ", "   #   ", "  ###  ", "   #   ", "       ", "       " });

			for (int i = 0; i < 4; i++)
			{
				c.nextIteration();
				Console.WriteLine(c);
			}

			Console.ReadKey();
		}
	}
}

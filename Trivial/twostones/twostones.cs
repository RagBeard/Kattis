using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kattis.IO;

namespace Kattis.Trivial.twostones
{
	class TwoStones
	{
		static void Main(string[] args)
		{
			Scanner scan = new Scanner();


			int n = scan.NextInt();

			int half = n / 2;

			if ( (half * 2) == n)
			{
				Console.WriteLine("Bob");
			}
			else
			{
				Console.WriteLine("Alice");
			}			
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kattis.IO;

namespace Kattis.Medium.woodcutting
{
	public class WoodCutting
	{

		static void Main(string[] args)
		{
			Scanner scan = new Scanner();

			int tests = scan.NextInt();

			for(int i = 0; i < tests; i++)
			{
				int customers = scan.NextInt();

				int[] customerSums = new int[customers];


				for (int k = 0; k < customers; k++)
				{
					int pieces = scan.NextInt();


					for (int p = 0; p < pieces; p++)
						customerSums[k] += scan.NextInt();
				}

				var ordered = customerSums.OrderBy(v => v).ToList();
				

				List<int> result = new List<int>();

				//inefficient, this will Sum increasingly large chunks of ordered each loop...
				//keep previous loop sum

				int sum = 0;

				for (int s = 0; s < ordered.Count; s++)
				{
					sum += ordered[s];
					result.Add(sum);
				}

				Console.WriteLine(result.Average());

			}

			
		}
	}
}

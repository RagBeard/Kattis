using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kattis.IO;
using System.Globalization;

namespace Kattis.Hard.sellingspatulas
{
	class brutespatula
	{
		const float CostPerMinute = 0.08f;

		static float GetProfit(Dictionary<int, float> minuteProfits)
		{
			int first = minuteProfits.First().Key;
			int last = minuteProfits.Last().Key + 1; //count the last minute


			float profit = minuteProfits.Sum(x => x.Value) - (last - first) * CostPerMinute;

			return profit;
		}


		static void Mainasdf(string[] args)
		{
			Scanner scan = new Scanner();

			while (scan.HasNext())
			{
				var n = scan.NextInt();

				if (n == 0)
					break;

				var salesMinutes = new Dictionary<int, float>();

				for (int i = 0; i < n; ++i)
				{
					var min = scan.NextInt();
					var sale = scan.Next();
					var fsale = float.Parse(sale, CultureInfo.InvariantCulture);
					salesMinutes.Add(min, fsale);
				}

				if (salesMinutes.Count < 1)
				{
					Console.WriteLine("no profit");
					continue;
				}

				Cluster bestCluster = new Cluster();

				//BRUTE FORCE
				for (int k = 0; k < salesMinutes.Count; ++k)
				{
					Dictionary<int, float> subSM = new Dictionary<int, float>(salesMinutes);
					subSM = subSM.Skip(k).ToDictionary(kk => kk.Key, vv => vv.Value);

					for (int i = 1; i < 1439; ++i)
					{
						var clusters = new List<Cluster>();

						Cluster current = new Cluster();
						int clusterStart = -1;

						int minutesPerCluster = i;

						bool finished = false;
						int index = 0;

						while (!finished)
						{
							var now = subSM.ElementAt(index);

							float sale = now.Value;
							int minute = now.Key;

							current = new Cluster();
							clusterStart = minute;

							while ((minute - clusterStart) < minutesPerCluster)
							{
								current.points.Add(minute, sale);

								index++;

								if (index >= subSM.Count)
								{
									finished = true;
									break;
								}

								var next = subSM.ElementAt(index);

								minute = next.Key;
								sale = next.Value;
							}

							clusters.Add(current);
						}

						var top = clusters.OrderBy(v => v.Profit).Reverse().Take(1).First();

						if (top.Profit > bestCluster.Profit)
						{
							bestCluster = top;
						}
					}
				}


				float bestProfit = bestCluster.Profit;


				if (bestProfit <= CostPerMinute)
				{
					Console.WriteLine("no profit");
				}
				else
				{
					string profitString = bestProfit.ToString("0.00", CultureInfo.InvariantCulture);
					var fromMinute = bestCluster.points.First().Key;
					var toMinute = bestCluster.points.Last().Key;

					Console.WriteLine(profitString + " " + fromMinute + " " + toMinute);
				}
			}
		}

	}
}

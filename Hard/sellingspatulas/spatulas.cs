using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kattis.IO;
using System.Globalization;

namespace Kattis.Hard.sellingspatulas
{
	struct Cluster
	{
		public int startMinute;
		public int endMinute;
		public float profit;
	}

	class spatulas
	{
		static float CostPerMinute = 0.08f;

		static float GetProfit(Dictionary<int, float> minuteProfits)
		{
			if (minuteProfits.Count == 0)
				return 0.0f;

			int first = minuteProfits.First().Key;
			int last = minuteProfits.Last().Key + 1; //count the last minute


			float profit = minuteProfits.Sum(x => x.Value) - (last - first) * CostPerMinute;

			return profit;
		}

		static int LocalMaxFind(Dictionary<int, float> data, int index)
		{
			int leftIndex = index - 1;
			int rightIndex = index + 1;

			//check out of bounds
			if (rightIndex > (data.Count-1))
			{
				return Math.Min(data.Count-1, index);

			}
			else if (leftIndex < 0)
			{
				return Math.Max(0, index);
			}

			var mid = data.ElementAt(index);
			var left = data.ElementAt(leftIndex);
			var right = data.ElementAt(rightIndex);

			
			if (left.Value > mid.Value)
			{
				return LocalMaxFind(data, leftIndex);
			}
			else if (right.Value > mid.Value)
			{
				return LocalMaxFind(data, rightIndex);
			}
			
			return index;			
		}

		static int LocalMinFind(Dictionary<int, float> data, int index)
		{
			int leftIndex = index - 1;
			int rightIndex = index + 1;

			//check out of bounds
			if (rightIndex > (data.Count - 1))
			{
				return Math.Min(data.Count - 1, index);

			}
			else if (leftIndex < 0)
			{
				return Math.Max(0, index);
			}

			var mid = data.ElementAt(index);
			var left = data.ElementAt(leftIndex);
			var right = data.ElementAt(rightIndex);
			

			if (left.Value < mid.Value)
			{
				return LocalMaxFind(data, leftIndex);
			}
			else if (right.Value < mid.Value)
			{
				return LocalMaxFind(data, rightIndex);
			}
			

			return index;
		}

		static Dictionary<int, float> GetTopValues(Dictionary<int, float> data, int amount)
		{
			return data.OrderBy(v => v.Value).Reverse().Take(amount).ToDictionary(kv => kv.Key, kv => kv.Value);

		}
		

		static int GetReach(float sale, int fromMinute = 0)
		{
			//minus one, to ensure a profit
			int result = (int)(sale / CostPerMinute) - 1;

			return result + fromMinute;
		}


		static float GetRemaining(int minute, float sale, int nextMinute)
		{
			float cost = (nextMinute - minute + 1) * CostPerMinute;

			float remainSale = sale - cost;

			return remainSale;
		}


		static void Main(string[] args)
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

					//only sales 0.08 or higher
					if (fsale > CostPerMinute)
						salesMinutes.Add(min, fsale);
				}
								

				float bestProfit = 0;

				//enter middle of salesMinutes
				// check left element, if its higher than middle, go there, check left again until it's not longer true.
				//split set into two chunks, get profit of each chunk, look at the highest profit one

				//int midPoint = salesMinutes.Count / 2;
				//int startMid = midPoint;

				//int peakIndex = LocalMaxFind(salesMinutes, midPoint);

				//int leftMin = LocalMinFind(salesMinutes, peakIndex - 1);
				//int rightMin = LocalMinFind(salesMinutes, peakIndex + 1);


				//var midPeak = salesMinutes.ElementAt(peakIndex);

				//var duration = salesMinutes.Last().Key;
				//float place = (float) midPeak.Key / duration;

				//if (place > 0.55f)
				//{
				//	//mid Peak is found to
				//}
				//else if (place < 0.45f)
				//{

				//}

				//var leftSales = salesMinutes.Take(leftMin).ToDictionary(kv => kv.Key, kv => kv.Value);
				//var rightSales = salesMinutes.Skip(leftMin).ToDictionary(kv => kv.Key, kv => kv.Value);

				//var toplist = GetTopValues(salesMinutes, 3);

				var currentIndex = 0;

				int clusterStart = 0;
				int clusterEnd = 0;
				float clusterProfit = 0.0f;

				List<Cluster> clusters = new List<Cluster>();
				
				while ( (currentIndex+1) < salesMinutes.Count)
				{
					int minute = salesMinutes.ElementAt(currentIndex).Key;
					float sale = salesMinutes.ElementAt(currentIndex).Value;

					if (clusterStart == 0)
						clusterStart = minute;

					int nextMinute = salesMinutes.ElementAt(currentIndex+1).Key;
					float nextSale = salesMinutes.ElementAt(currentIndex+1).Value;

					float remainSale = GetRemaining(minute, sale, nextMinute);

					if (remainSale < 0.0f)
					{
						//cluster ends at this minute
						clusterEnd = minute;

						// get profit
						var values = new Dictionary<int, float>();
						
						values = salesMinutes.Where(v => (v.Key >= clusterStart) && (v.Key <= clusterEnd)).ToDictionary(kv => kv.Key, kv => kv.Value);
						clusterProfit = GetProfit(values);
				
						//now save this 
						clusters.Add(new Cluster
						{
							startMinute = clusterStart,
							endMinute = clusterEnd,
							profit = clusterProfit,
						});

						//reset to start next cluster
						clusterStart = 0;
						clusterProfit = 0.0f;
					}					

					currentIndex++;
				}

				//handle last cluster, if clusterStart isnt 0, theres an unfinished cluster
				if (clusterStart > 0 && clusterEnd > 0)
				{
					var values = salesMinutes.Where(v => (v.Key >= clusterStart) && (v.Key <= clusterEnd)).ToDictionary(kv => kv.Key, kv => kv.Value);
					clusterProfit = GetProfit(values);
					clusters.Add(new Cluster
					{
						startMinute = clusterStart,
						endMinute = clusterEnd,
						profit = clusterProfit,
					});
				}

				Cluster bestCluster = new Cluster();

				//handle salesMinutes arrays size 1 or smaller
				if (salesMinutes.Count < 2 && salesMinutes.Count > 0)
				{
					bestCluster = new Cluster
					{
						profit = salesMinutes.ElementAt(0).Value,
						startMinute = salesMinutes.ElementAt(0).Key,
						endMinute = salesMinutes.ElementAt(0).Key,
					};
				}
				else if (clusters.Count >= 1)
				{
					bestCluster = clusters.OrderBy(v => v.profit).Reverse().ToList().First();
				}
												
				bestProfit = bestCluster.profit;


				if (bestProfit <= CostPerMinute)
				{

					Console.WriteLine("no profit");

				}
				else
				{
					string profitString = bestProfit.ToString("0.00", CultureInfo.InvariantCulture);
					var fromMinute = bestCluster.startMinute;
					var toMinute = bestCluster.endMinute;
					
					Console.WriteLine(profitString + " " + fromMinute + " " + toMinute);

				}

			}
		}
	}
}

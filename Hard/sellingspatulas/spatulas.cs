using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kattis.IO;
using System.Globalization;

namespace Kattis.Hard.sellingspatulas
{
	class Window : CPM
	{
		public List<Cluster> clusters;

		private float profit;
		public float Profit
		{
			get
			{
				if (profit == 0.0f)
					CalcProfit();

				return profit;
			}
		}

		private Cluster center = new Cluster();
		public Cluster Center
		{
			get
			{
				if (center.points.Count == 0)
				{
					center = clusters.ElementAt(clusters.Count / 2);
				}

				return center;
			}
			set
			{
				center = value;
			}
		}

		public Window()
		{
			clusters = new List<Cluster>();
		}

		private void CalcProfit()
		{
			profit = 0.0f;

			for (int i = 0; i < clusters.Count; ++i)
			{
				var cur = clusters.ElementAt(i);
				float dCost = 0.0f;

				if (i+1 < clusters.Count)
				{
					var next = clusters.ElementAt(i + 1);
					
					// - 1 to avoid double-counting a cost minute
					// because each Cluster already includes that minute in its profit
					dCost = (next.points.First().Key - cur.points.Last().Key - 1) * CostPerMinute;
				}			

				profit += cur.Profit - dCost;
			}					
		}

	}


	class spatulas : CPM
	{
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

		//gets the euclidean distance between two points, where minute is x and sale is y
		static float GetDistance(int minute1, float sale1, int minute2, float sale2)
		{
			float dexp = (minute1 - minute2) * (minute1 - minute2) + (sale1 - sale2) * (sale1 - sale2);

			return (float)Math.Sqrt(dexp);
		}

		static Window GetLeftWindow(int size, Cluster pivot, List<Cluster> clusters)
		{
			Window leftWindow = new Window();

			int pivotMinute = pivot.points.First().Key;

			int wMin = pivotMinute - size;
			int wMax = pivotMinute + 1; //make sure we actually include pivot

			var clustersInReach = clusters.Where(v => v.points.First().Key > wMin && v.points.First().Key < wMax).ToList();

			leftWindow.clusters = clustersInReach;

			return leftWindow;
		}		

		static Window GetCenterWindow(int size, Cluster pivot, List<Cluster> clusters)
		{
			Window centerWindow = new Window();
			int pivotMinute = pivot.points.First().Key;

			int wMin = pivotMinute - (size / 2);
			int wMax = pivotMinute + (size / 2);
			
			var clustersInReach = clusters.Where(v => v.points.First().Key > wMin && v.points.First().Key < wMax).ToList();

			centerWindow.clusters = clustersInReach;

			return centerWindow;
		}

		static Window GetRightWindow(int size, Cluster pivot, List<Cluster> clusters)
		{
			Window rightWindow = new Window();

			int pivotMinute = pivot.points.First().Key;

			int wMin = pivotMinute - 1; // make sure we include pivot also.
			int wMax = pivotMinute +size;

			var clustersInReach = clusters.Where(v => v.points.First().Key > wMin && v.points.First().Key < wMax).ToList();

			rightWindow.clusters = clustersInReach;

			return rightWindow;
		}


		static void Mainasdfas(string[] args)
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

				// create clusters of datapoints with <10 minute distance from eachother

				var clusters = new List<Cluster>();
				
				Cluster current = new Cluster();
				int clusterStart = -1;

				int minutesPerCluster = 1;

				bool finished = false;
				int index = 0;
				
				while (!finished)
				{
					var now = salesMinutes.ElementAt(index);

					float sale = now.Value;
					int minute = now.Key;

					current = new Cluster();
					clusterStart = minute;

					while ((minute - clusterStart) < minutesPerCluster)
					{
						current.points.Add(minute, sale);

						index++;

						if (index >= salesMinutes.Count)
						{
							finished = true;
							break;
						}

						var next = salesMinutes.ElementAt(index);

						minute = next.Key;
						sale = next.Value;
					}

					clusters.Add(current);					
				}
								
				//Now I have everything divided into Clusters
				//So the smallest point is 10 minutes long

				int sizeOfTopList = 10;				
				var topList = clusters.OrderBy(v => v.Profit).Reverse().Take(sizeOfTopList).ToList();
				topList = topList.TakeWhile(v => v.Profit > 0.09f).ToList();

				List<Window> foundWindows = new List<Window>();

				foreach (var cluster in topList)
				{
					//if (cluster.Profit < 0.09f)
					//	continue;

					//windowSize in minutes
					int windowSize = (int)((cluster.Profit / 0.08f) - 0.08f);

					Window bestWindow = new Window();
					bestWindow.clusters.Add(cluster);

					//now find best window between three positions: left, center, right
					
					bool done = false;

					int leftSize = windowSize;

					// 5 - 1a.
					Window leftWindow = GetLeftWindow(leftSize, cluster, clusters);
	
					while (!done)
					{
						// 5 - 1b.
						if (leftWindow.Profit <= bestWindow.Profit)
						{
							// go to next position
							done = true;
							continue;
						}
						else // 5 - 1c.
						{
							//leftWindow has higher profit than bestWindow!
							bestWindow = leftWindow;
							leftSize = (int)((leftWindow.Profit / 0.08f) - 0.08f);
						}

						// 5 - 1a mod.
						leftWindow = GetCenterWindow(leftSize, bestWindow.Center, clusters);
					}

					done = false;

					int centerSize = windowSize;
					Window centerWindow = GetCenterWindow(centerSize, cluster, clusters);

					while (!done)
					{
						// 5 - 1b.
						if (centerWindow.Profit <= bestWindow.Profit)
						{
							// go to next position
							done = true;
							continue;
						}
						else // 5 - 1c.
						{
							//centerWindow has higher profit than bestWindow!
							bestWindow = centerWindow;
							centerSize = (int)((centerWindow.Profit / 0.08f) - 0.08f);
						}

						// 5 - 1a mod.
						centerWindow = GetCenterWindow(centerSize, bestWindow.Center, clusters);
					}

					done = false;

					int rightSize = windowSize;
					Window rightWindow = GetRightWindow(rightSize, cluster, clusters);

					while (!done)
					{
						// 5 - 1b.
						if (rightWindow.Profit <= bestWindow.Profit)
						{
							done = true;
							continue;
						}
						else // 5 - 1c.
						{
							//rightWindow has higher profit than bestWindow!
							bestWindow = rightWindow;
							rightSize = (int)((rightWindow.Profit / 0.08f) - 0.08f);
						}

						// 5 - 1a mod.
						rightWindow = GetCenterWindow(rightSize, bestWindow.Center, clusters);
					}

					foundWindows.Add(bestWindow);
				}

				Window answer = new Window();

				// if topList contained NO clusters, profit is impossible.
				// UNLESS a profit is clustered together with a loss of the same size.
				// so this statement possibly takes care of it.
				if (topList.Count == 0 && salesMinutes.Count > 0)
				{
					// sort salesMinutes by largest, pick largest, 
					// if that value is > 0.09f, add it to a Cluster

					float bestVal = -1000.0f;
					int bestIdx = 0;

					for (int i = 0; i < salesMinutes.Count; ++i)
					{
						var val = salesMinutes.ElementAt(i).Value;
						if (val > bestVal)
						{
							bestIdx = i;
							bestVal = val;
						}
					}

					var bestElem = salesMinutes.ElementAt(bestIdx);

					Cluster finalCluster = new Cluster();					

					if (bestVal > 0.09f)
					{
						finalCluster.points.Add(bestElem.Key, bestElem.Value);

						int reach = (int)((bestVal / 0.08f) - 0.08f);

						if (bestIdx > 0)
						{
							var prev = salesMinutes.ElementAt(bestIdx - 1);

							bool inReach = prev.Key > (bestElem.Key - reach);

							if (prev.Value > 0.09f && inReach)
							{
								finalCluster.points.Add(prev.Key, prev.Value);
							}
						}

						if (finalCluster.points.Count < 2)
						{
							if (bestIdx < salesMinutes.Count - 1)
							{
								var next = salesMinutes.ElementAt(bestIdx + 1);

								bool inReach = next.Key < (bestElem.Key + reach);

								if (next.Value > 0.09f && inReach)
								{
									finalCluster.points.Add(next.Key, next.Value);

								}
							}
						}
					}

					answer.clusters.Add(finalCluster);
					
				}

				var topWindows = foundWindows.OrderBy(v => v.Profit).Reverse().Take(5).ToList();

				if (topWindows.Count > 0)
					answer = topWindows.First();
								
				if (answer.Profit <= 0.0f)
				{
					Console.WriteLine("no profit");
				}
				else
				{
					string profitString = answer.Profit.ToString("0.00", CultureInfo.InvariantCulture);
					var fromMinute = answer.clusters.First().points.First().Key;
					var toMinute = answer.clusters.Last().points.Last().Key;
					
					Console.WriteLine(profitString + " " + fromMinute + " " + toMinute);
				}
			}
		}
	}
}

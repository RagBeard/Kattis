using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kattis.IO;
using System.Globalization;

namespace Kattis.Hard.sellingspatulas
{
	class CPM
	{
		public const float CostPerMinute = 0.08f;
		public const int MinuteLimit = 1439;
	}

	class Cluster : CPM
	{
		public Dictionary<int, float> points;

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

		private int length; // length in minutes
		public int Length
		{
			get
			{
				if (length == 0)
				{
					if (points.Count > 0)
						length = points.Last().Key - points.First().Key;
				}

				return length;
			}
		}

		public Cluster()
		{
			points = new Dictionary<int, float>();
		}

		public void CalcProfit()
		{
			if (points.Count == 0)
			{
				profit = 0.0f;
			}
			else
			{
				int first = points.First().Key;
				int last = points.Last().Key + 1; //count the last minute

				profit = points.Sum(x => x.Value) - (last - first) * CostPerMinute;

			}
		}
	}

	class DistinctClusterComparer : IEqualityComparer<Cluster>
	{
		public bool Equals(Cluster x, Cluster y)
		{
			return x.Profit == y.Profit &&
				x.points.First().Key == y.points.First().Key &&
				x.points.Last().Key == y.points.Last().Key;
		}

		public int GetHashCode(Cluster obj)
		{
			return obj.Profit.GetHashCode() ^ obj.points.GetHashCode();
		}
	}

	class spatulas2 : CPM
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

		static Cluster LeftWindow(int size, Cluster refCluster, Dictionary<int, float> allPoints)
		{
			Cluster outCluster = new Cluster();

			int clusterLast = refCluster.points.Last().Key;
			int wMin = clusterLast - size;
			int wMax = clusterLast + 1; //make sure we actually include refCluster

			var pointsInReach = allPoints.Where(v => v.Key > wMin && v.Key < wMax).ToDictionary(x => x.Key, x => x.Value);

			outCluster.points = pointsInReach;

			return outCluster;
		}

		static Cluster CenterWindow(int size, Cluster refCluster, Dictionary<int, float> allPoints)
		{
			Cluster outCluster = new Cluster();

			int clusterFirst = refCluster.points.First().Key;
			int clusterLast = refCluster.points.Last().Key;

			int clusterCenter = clusterFirst + ((clusterLast - clusterFirst) / 2);
			
			int wMin = clusterCenter - (size / 2);
			int wMax = clusterCenter + (size / 2);

			var pointsInReach = allPoints.Where(v => v.Key > wMin && v.Key < wMax).ToDictionary(x => x.Key, x => x.Value);

			outCluster.points = pointsInReach;

			return outCluster;
		}

		static Cluster RightWindow(int size, Cluster refCluster, Dictionary<int, float> allPoints)
		{
			Cluster outCluster = new Cluster();

			int clusterFirst = refCluster.points.First().Key;
			int wMin = clusterFirst - 1; //make sure we actually include refCluster
			int wMax = clusterFirst + size; 

			var pointsInReach = allPoints.Where(v => v.Key > wMin && v.Key < wMax).ToDictionary(x => x.Key, x => x.Value);

			outCluster.points = pointsInReach;

			return outCluster;
		}


		static Cluster Trim(Cluster cluster, float minProfit = 0.09f)
		{
			int trimStart = 0;
			int trimEnd = 0;

			for(int i = 0; i < cluster.points.Count; ++i)
			{
				var pt = cluster.points.ElementAt(i);

				if (pt.Value < minProfit)
					trimStart++;
				else
					break;
			}

			cluster.points = cluster.points.Skip(trimStart).ToDictionary(x => x.Key, x => x.Value);

			for (int i = cluster.points.Count-1; i >= 0; --i)
			{
				var pt = cluster.points.ElementAt(i);

				if (pt.Value < minProfit)
				{
					trimEnd++;
				}
				else
					break;
			}

			int nToTake = cluster.points.Count - trimEnd;

			cluster.points = cluster.points.Take(nToTake).ToDictionary(x => x.Key, x => x.Value);

			return cluster;
		}

		static List<Cluster> ClusterPoints(Dictionary<int, float> points, int sizeMinutes)
		{
			List<Cluster> result = new List<Cluster>();

			Cluster current = new Cluster();
			int clusterStart = -1;

			bool finished = false;
			int index = 0;

			while (!finished)
			{
				var now = points.ElementAt(index);

				float sale = now.Value;
				int minute = now.Key;

				current = new Cluster();
				clusterStart = minute;

				while ((minute - clusterStart) < sizeMinutes)
				{
					current.points.Add(minute, sale);

					index++;

					if (index >= points.Count)
					{
						finished = true;
						break;
					}

					var next = points.ElementAt(index);

					minute = next.Key;
					sale = next.Value;
				}

				//trim before adding
				result.Add(Trim(current));
			}

			return result;
		}

		static void Mainasdfasd(string[] args)
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


				int totalMinutesInSet = salesMinutes.Last().Key - salesMinutes.First().Key;

				// create clusters of datapoints with <x minutes distance from eachother

				var clusters5min = ClusterPoints(salesMinutes, 5);
				var clusters10min = ClusterPoints(salesMinutes, 10);
				var clusters20min = ClusterPoints(salesMinutes, 20);

				var clusters = new List<Cluster>();
				clusters.AddRange(clusters5min);
				clusters.AddRange(clusters10min);
				clusters.AddRange(clusters20min);

				//int rangeGrowth = 10;

				int rangeGrowth = totalMinutesInSet / 10;

				int sizeOfTopList = 3;				
				var topList = clusters.OrderBy(v => v.Profit).Reverse().Take(sizeOfTopList).ToList();
				topList = topList.TakeWhile(v => v.Profit > 0.09f).ToList();

				List<Cluster> bestClusters = new List<Cluster>();

				foreach (var cluster in topList)
				{
					bool done = false;
					int span = cluster.points.Last().Key - cluster.points.First().Key;
					float bestProfit = cluster.Profit;
					int rangeLimit = (int)(bestProfit / CostPerMinute);

					int windowRange = 0;

					Cluster bestCluster = cluster;
					
					while (!done)
					{
						//[START]

						//trim ends
						Trim(bestCluster);

						//update values						
						span = bestCluster.points.Last().Key - bestCluster.points.First().Key;
						bestProfit = bestCluster.Profit;
						rangeLimit = (int)(bestProfit / CostPerMinute);

						// [SET_SPAN]
						//windowRange += Math.Min(span + minutesPerCluster, rangeLimit);
						windowRange += rangeGrowth;

						//make sure window is at least as large as bestCluster span :)
						windowRange = Math.Max(windowRange, span + rangeGrowth);


						// [LEFT]
						Cluster leftCluster = LeftWindow(windowRange, bestCluster, salesMinutes);
						Trim(leftCluster);

						float leftProfit = leftCluster.Profit;
						if (leftProfit > bestProfit)
						{
							bestCluster = leftCluster;
							continue;
						}
						else
						{
							//[CENTER]
							Cluster centerCluster = CenterWindow(windowRange, bestCluster, salesMinutes);
							Trim(centerCluster);

							float centerProfit = centerCluster.Profit;
							if (centerProfit > bestProfit)
							{
								bestCluster = centerCluster;
								continue;

							}
							else
							{
								//[RIGHT]
								Cluster rightCluster = RightWindow(windowRange, bestCluster, salesMinutes);
								Trim(rightCluster);

								float rightProfit = rightCluster.Profit;
								if (rightProfit > bestProfit)
								{
									bestCluster = rightCluster;
									continue;
								}
								else
								{
									// set done = true if
									// windowRange > (rangeLimit + minutesPerCluster)
									// windowRange >= totalMinutesInSet

									if (windowRange > (rangeLimit + rangeGrowth))
										done = true;
									if (windowRange >= totalMinutesInSet)
										done = true;
								}
							}
						}
					}
					

					bestClusters.Add(bestCluster);

				}

				//TODO:
				//remove duplicates from toplist (i.e. same start and end points)
				//sort by profit
				//pick first

				if (topList.Count == 0)
				{
					Console.WriteLine("no profit");
				}
				else
				{
					var distinctBest = bestClusters.Distinct(new DistinctClusterComparer());

					bestClusters = distinctBest.OrderBy(x => x.Profit).Reverse().ToList();

					var answer = bestClusters.First();

					var first = bestClusters.First();
					var second = bestClusters.ElementAt(1);

					if (first.Profit == second.Profit)
					{
						//get shortest
						if (first.Length < second.Length)
							answer = first;
						else if (first.Length > second.Length)
							answer = second;
						else
						{
							//they are equal length, get the one that starts earliest
							answer = first.points.First().Key < second.points.First().Key ? first : second;
						}
					}
					

					string profitString = answer.Profit.ToString("0.00", CultureInfo.InvariantCulture);
					var fromMinute = answer.points.First().Key;
					var toMinute = answer.points.Last().Key;

					Console.WriteLine(profitString + " " + fromMinute + " " + toMinute);
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kattis.IO;

namespace Kattis.Medium.natrij
{
	public class natrij
	{
		static int H = 0;
		static int M = 1;
		static int S = 2;


		//Natrij [solved]

		public static string BoomCalc(string [] hmsNow, string [] hmsBoom)
		{
			int hourNow = int.Parse(hmsNow[H]);
			int minNow = int.Parse(hmsNow[M]);
			int secNow = int.Parse(hmsNow[S]);

			int hourBoom = int.Parse(hmsBoom[H]);
			int minBoom = int.Parse(hmsBoom[M]);
			int secBoom = int.Parse(hmsBoom[S]);

			bool sameHour = hourNow == hourBoom;
			bool sameMin = minNow == minBoom;
			bool sameSec = secNow == secBoom;


			//hour, min, sec diffs
			int hd = 0;
			int md = 0;
			int sd = 0;

			if (hourBoom < hourNow)
			{
				//next day
				hd += (24 - hourNow);
				hd += hourBoom;
			}
			else if (hourBoom > hourNow)
			{
				//same day
				hd += hourBoom - hourNow;
			}
			

			if (minBoom < minNow)
			{
				//next hour
				md += (60 - minNow);
				md += minBoom;

				if (sameHour)
				{
					//actually next day
					hd = 23;
				}
				else
					hd--;
			}
			else if (minBoom > minNow)
			{
				//same hour
				md += minBoom - minNow;
			}
			else
			{
				md = 0;
			}

			if (secBoom < secNow)
			{
				//next minute
				sd += (60 - secNow);
				sd += secBoom;

				if (sameMin && sameHour)
				{
					md = 59;
					hd = 23;
				}
				else
					md--;
			}
			else if (secBoom > secNow)
				sd += secBoom - secNow;

			
			if (sd < 0)
			{
				md--;
				sd = 59;
			}

			if (md < 0)
			{
				hd--;
				md = 59;
			}

			if (sameHour && sameMin && sameSec)
			{
				hd = 24;
				md = 0;
				sd = 0;
			}

			string res = string.Format("{0:00}:{1:00}:{2:00}", hd, md, sd);

			return res;
		}

		static void Main(string[] args)
		{
			Scanner scan = new Scanner();

			var hmsNow = scan.Next().Split(':');
			var hmsBoom = scan.Next().Split(':');
			
			string result = BoomCalc(hmsNow, hmsBoom);

			Console.WriteLine(result);
			
		}
	}
}

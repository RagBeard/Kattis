using Kattis.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kattis.Hard.sellingspatulas
{

	struct Minute
	{
		public int minute; //minute since store opening
		public float netProfit; //net profit this minute ( sale - CostPerMinute)
		public float bankValue; //net profit/loss this minute (accumulated since store opening)
		public float prevBankValue; //net profit/loss previous minute (accumulated since store opening)
	};

	struct InputSale
	{
		public int minute;
		public float amount;
	};

	//struct Peak
	//{
	//	public float profit;
	//	public int from;
	//	public int to;
	//};


	//Spatulas 3:
	//heavily based on Kristoffers C++ code
	// yields Wrong Answer on Kattis Test #2

	//has been tested for cases:
	// three peaks of same profit and duration (equalpeaks_order.txt)
	// two peaks of same profit but second shorter duration (equalpeaks_duration.txt)


	class Spatulas3
	{
		public static float Trunc(float value, int digits)
		{
			double mult = Math.Pow(10.0, digits);
			double result = Math.Truncate(mult * value) / mult;
			return (float)result;
		}

		static void Main(string [] args)
		{
			float CostPerMinute = 0.08f;
			int MinuteLimit = 1440;

			Scanner scan = new Scanner();

			while (scan.HasNext())
			{
				var n = scan.NextInt();

				if (n == 0)
					break;

				List<InputSale> inputMinutes = new List<InputSale>(MinuteLimit);
				List<Minute> openMinutes = new List<Minute>(MinuteLimit);
				
				
				for (int i = 0; i < n; ++i)
				{
					var minute = scan.NextInt();
					var sale = scan.Next();

					var fsale = float.Parse(sale, CultureInfo.InvariantCulture);

					inputMinutes.Add(new InputSale()
					{
						minute = minute,
						amount = fsale,
					});
				}

				int tmin = inputMinutes.First().minute;
				int tmax = inputMinutes.Last().minute;

				int next_sale = tmin;

				float bank = 0.0f;
				
				for (int i = 0, saleIdx = 0; i <= tmax; ++i)
				{
					float net = -CostPerMinute;

					if (i == next_sale)
					{
						net += inputMinutes[saleIdx++].amount;

						if (saleIdx < inputMinutes.Count)
							next_sale = inputMinutes[saleIdx].minute;
					}

					Minute minute = new Minute();

					minute.minute = i;
					minute.netProfit = net;
					minute.bankValue = bank + net;
					minute.prevBankValue = bank;

					openMinutes.Add(minute);

					bank += net;
				}


				int peakStart = -1;
				int peakEnd = -1;

				float nowValue = 0.0f;
				float bestValue = 0.0f;

				int bestDist = 9999999;
				int bestMinute = -1;

				float peakOffset = 0;
				
				
				foreach (var minute in openMinutes)
				{
					if (minute.netProfit > 0)
					{
						if (minute.netProfit > nowValue && nowValue < 0)
						{
							peakStart = minute.minute;
							peakOffset = minute.prevBankValue;
						}	

					}

					peakEnd = minute.minute;

					int currentDist = peakEnd - peakStart;

					//net profit at this minute, relative to peak start
					// = (bank value from store opening) - bank value from start of peak
					float value = minute.bankValue - peakOffset;

					value = (float)Math.Round(value, 2);

					if ( value > bestValue || (value >= bestValue && currentDist < bestDist))
					{
						bestMinute = peakStart;
						bestDist = currentDist;
						bestValue = value;							 
					}

					nowValue = value;
				}


				if (bestMinute == -1)
				{
					Console.WriteLine("no profit");
				}
				else
				{
					string profitString = bestValue.ToString("0.00", CultureInfo.InvariantCulture);
					var fromMinute = bestMinute;
					var toMinute = bestMinute + bestDist;

					Console.WriteLine(profitString + " " + fromMinute + " " + toMinute);
				}

			}
		}


	}
}

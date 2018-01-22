using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Kattis.Hard.sellingspatulas
{
	// generate input data according to the limits set by selling spatulas

	public class Limits
	{
		public const int StoresMax = 150;

		public const int SalesMin = 1;
		public const int SalesMax = 1000;

		public const float SaleValueMin = -100.0f;
		public const float SaleValueMax = 100.0f;

		public const int MinuteMax = 1439;
	}

	class spatula_generation : Limits
	{

		static void Mainasd(string[] args)
		{
			Generate();
		}

		static void Generate()
		{
			Random rand = new Random(DateTime.Now.Millisecond);

			List<string> outStrings = new List<string>();

			int stores = rand.Next(StoresMax);

			for (int store = 0; store < stores; ++store)
			{
				int sales = rand.Next(SalesMin, SalesMax);


				int currentMinute = 0;

				List<string> salesStrings = new List<string>();

				for (int sale = 0; sale < sales; ++sale)
				{
					currentMinute += rand.Next(MinuteMax / (sales/2) ) + 1;

					if (currentMinute >= MinuteMax)
					{
						sales = sale;
						break;
					}

					double mult = ( (rand.Next(0, 2) == 0) ? SaleValueMin : SaleValueMax);
					float saleValue = (float) (rand.NextDouble() * mult);


					string saleString = saleValue.ToString("0.00", CultureInfo.InvariantCulture);

					salesStrings.Add(currentMinute.ToString() + " " + saleString);
				}

				outStrings.Add(sales.ToString());
				outStrings.AddRange(salesStrings);
				
			}

			outStrings.Add("0");

			System.IO.File.WriteAllLines(@"F:\Projects\GitRepos\Kattis\Hard\sellingspatulas\genInput.txt", outStrings.ToArray());

		}



	}
}

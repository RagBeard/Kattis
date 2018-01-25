using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kattis.IO;

namespace Kattis.Easy.variablearithmetic
{
	public class Variable
	{
		public string name = "";
		public int value = -1;
	}

	class VariableArithmetic
	{
		static List<Variable> variables = new List<Variable>();

		static string DoAddition(List<string> lineIn)
		{
			int num = -1;
			int sum = 0;

			string result = "";

			List<string> buffer = new List<string>();

			foreach (var str in lineIn)
			{
				bool numeric = int.TryParse(str, out num);

				if (numeric)
				{
					sum += num;
				}
				else
				{
					// try finding it in variables
					// if found WITH VALUE, add to sum
					// else, add to buffer

					var found = variables.Where(v => v.name == str).FirstOrDefault();

					if (found == null)
						buffer.Add(str);
					else
					{
						if (found.value > -1)
							sum += found.value;
						else
							buffer.Add(str);
					}
				}
			}

			if (sum > 0)
			{
				result = sum.ToString();

				foreach(var str in buffer)
					result += " + " + str;
			}
			else
			{
				result = lineIn[0];

				for (int i = 1; i < lineIn.Count; i++)
					result += " + " + lineIn.ElementAt(i);

			}

			return result;
		}

		static void Mainasdfasdf(string[] args)
		{
			Scanner scan = new Scanner();

			//because scan doesnt give me newlines
			// detect newline from statement pattern: variable operation variable [NEWLINE] variable operation variable



			bool newLine = false;
			bool op = false;
			bool prevOp = false;

			int i = 0;

			bool definition = false;

			List<string> line = new List<string>();

			while (scan.HasNext())
			{
				
				var token = scan.Next();

				op = (token == "+") || (token == "=");

				if (!definition)
					definition = (token == "=");

				newLine = (!prevOp && !op);

				if (newLine && i != 0)
				{
					//current token is on a newline
					//previous line is finished
					//process the line

					if (definition)
					{
						//find line[0] name in variables
						// if it doesnt exist, add it to variables
						// set that variable value to line[1]

						var found = variables.Where(v => v.name == line[0]).FirstOrDefault();
						
						if (found == null)
						{
							Variable newVar = new Variable();
							newVar.name = line[0];
							newVar.value = int.Parse(line[1]);
							variables.Add(newVar);
						}
						else
							found.value = int.Parse(line[1]);


						definition = false;
					}
					else
					{
						//addition

						//output it's result

						string result = DoAddition(line);

						Console.WriteLine(result);
					}
					
					line.Clear();
					line.Add(token);
				}
				else
				{
					if (!op)
						line.Add(token);
				}




				prevOp = op;
				i++;
			}



		}
	}
}

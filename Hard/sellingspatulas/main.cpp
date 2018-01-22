
#include "stdafx.h"
#include <fstream>
#include <vector>
#include <iostream>

struct Node
{
	int minute;
	float value;
	float diff;
};


int main()
{

	std::ifstream input("input.txt");
	std::vector<Node> input_nodes;
	std::vector<Node> nodes;

	input_nodes.reserve(1000);
	nodes.reserve(1440);

	int count = 0;
	input >> count;

	do
	{
		input_nodes.clear();
		nodes.clear();

		//std::cout << count << std::endl;

		for (int i = 0; i < count; ++i)
		{
			Node node;
			input >> node.minute >> node.value;

			input_nodes.push_back(node);

			//std::cout << node.minute << " " << node.value << std::endl;
		}

		// create nodes
		int tmin = input_nodes.front().minute;
		int tmax = input_nodes.back().minute;

		int input_next = tmin;
		float bank = 0;

		Node * prev = NULL;

		for (int i = 0, input_i = 0; i < tmax; ++i)
		{
			float add = -0.08f;

			if (i == input_next)
			{
				add += input_nodes[input_i++].value;
				if (input_nodes.size() > input_i)
				{
					input_next = input_nodes[input_i].minute;
				}
			}
						
			Node node;
			
			node.minute = i;
			node.diff = add;
			node.value = bank + add;
			
			nodes.push_back(node);

			prev = &nodes.back();
			bank += add;
		}

		//std::cout << "processed..." << std::endl << std::endl;


		int i1 = -1;
		int i2 = -1;

		float avg_best = 0;
		float avg_now = 0;
		int best_dist = 9999999;
		int best_minute = -1;

		float offset = 0;

		// iterate on our set of data and save the best peak where average income by time was the highest

		for (Node node : nodes)
		{
			if (node.diff > 0)
			{
				if (node.diff > avg_now && avg_now < 0)
				{
					i1 = node.minute;
					float d = node.value - node.diff;
					offset = d;
				}
			}

			i2 = node.minute;

			int current_dist = i2 - i1;
			float avg = (node.value - offset);

			if (avg > avg_best || (avg >= avg_best &&  current_dist < best_dist))
			{
				best_minute = i1;
				best_dist = current_dist;
				avg_best = avg;
			}

			avg_now = avg;
		}

		if (best_minute == -1)
		{
			std::cout << "no profit" << std::endl;
		}
		else
		{
			std::cout << avg_best << " " << best_minute << " " << best_minute + best_dist << std::endl;
		}

		input >> count;

	} while (count > 0);
	

	system("pause");

	return 0;
}
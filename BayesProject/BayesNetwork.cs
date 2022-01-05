using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BayesProject
{
    public class BayesNetwork
    {
        private Graph networkGraph;

        public BayesNetwork(string filePath)
        {
            List<string> lines = System.IO.File.ReadLines(filePath).ToList();

            int noNodes = Int32.Parse(lines[0].Split(':')[1].Trim());
            networkGraph = new Graph(noNodes);
            int pos = 2;
            //number of nodes
            for (int i = 0; i < noNodes; ++i)
            {
                //start from 3rd line
                //read first line with nodes
                var evidences = lines[pos].TrimStart('[').TrimEnd(']').Replace(" ", "").Split(',');
                var temp = lines[++pos].Split(':');
                var nodeID = temp[0].Trim();
                var parents = Regex.Replace(temp[1], @"[{} \t]", "").Split(',');
                var node = new Node(nodeID);

                foreach (var e in evidences)
                {
                    node.AddEvidenceToDomain(e);
                }

                if (parents.Length == 1 && string.IsNullOrEmpty(parents[0]))
                {
                    networkGraph.AddNodes(node, null);
                    networkGraph.SetProbabilities(nodeID, lines.GetRange(pos + 1, 1));
                    pos += 3;
                }
                else
                {
                    networkGraph.AddNodes(node, parents);
                    networkGraph.SetProbabilities(nodeID, lines.GetRange(pos + 1, Convert.ToInt32(Math.Pow(2, parents.Length))));
                    pos += Convert.ToInt32(Math.Pow(2, parents.Length)) + 2;
                }
            }
            //don't forget to enable console output
            networkGraph.PrintNodesProbabilities();
            //Console.WriteLine(networkGraph.GetProbabilityOfNode("Fever", "Yes", "No No"));
        }

        public Graph getNetworkGraph
        {
            get { return networkGraph; }
        }
    }
}

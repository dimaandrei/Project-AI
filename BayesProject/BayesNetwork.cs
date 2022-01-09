using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BayesProject
{
    /// <summary>
    /// BayesNetwork class
    /// </summary>
    public class BayesNetwork
    {
        // Private members
        private Graph _networkGraph;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filePath">The path of file which containts the parameters of the bayesian network</param>
        public BayesNetwork(string filePath)
        {
            List<string> lines = System.IO.File.ReadLines(filePath).ToList();

            int noNodes = Int32.Parse(lines[0].Split(':')[1].Trim());
            _networkGraph = new Graph(noNodes);
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
                    _networkGraph.AddNodes(node, null);
                    _networkGraph.SetProbabilities(nodeID, lines.GetRange(pos + 1, 1));
                    pos += 3;
                }
                else
                {
                    _networkGraph.AddNodes(node, parents);
                    var noOfProb = 1;

                    foreach (var parent in parents)
                    {
                        noOfProb *= _networkGraph.GetNode(parent).GetEvidencesNumber();
                    }

                    _networkGraph.SetProbabilities(nodeID, lines.GetRange(pos + 1, noOfProb));
                    pos += noOfProb + 2;
                }
            }

            _networkGraph.PrintNodesProbabilities();
        }

        /// <summary>
        /// Getter of the graph
        /// </summary>
        public Graph NetworkGraph
        {
            get { return _networkGraph; }
        }
    }
}

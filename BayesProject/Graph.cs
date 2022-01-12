using System;
using System.Collections.Generic;
using System.Linq;

namespace BayesProject
{
    /// <summary>
    /// Graph class
    /// </summary>
    public class Graph
    {
        // Private members
        private readonly List<Node> _nodes;
        private int _noNodes;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Graph()
        {
            _nodes = new List<Node>();
        }

        /// <summary>
        /// Add nodes
        /// </summary>
        /// <param name="node">The node</param>
        /// <param name="parents">The parents of node</param>
        public void AddNodes(Node node, string[] parents)
        {
            if (parents != null)
            {
                foreach (var parent in parents)
                {
                    node.AddParent(parent);
                }
            }
            _nodes.Add(node);
        }

        /// <summary>
        /// Set probabilities
        /// </summary>
        /// <param name="nodeID">The node</param>
        /// <param name="values">The values</param>
        public void SetProbabilities(string nodeID, List<string> values)
        {
            _nodes.Where(node => node.NodeID == nodeID).ToList().ForEach(node => node.SetProbabilities(values));
        }

        /// <summary>
        /// Print probabilities of nodes
        /// </summary>
        public void PrintNodesProbabilities()
        {
            foreach (var node in _nodes)
            {
                node.PrintProbabilites();
            }
        }

        /// <summary>
        /// Getter
        /// </summary>
        /// <param name="id">The id of node</param>
        /// <returns>A node</returns>
        public Node GetNode(String id)
        {
            try
            {
                return _nodes.Where(node => node.NodeID == id).ToList().First();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Getter of the list of nodes
        /// </summary>
        public List<Node> Nodes
        {
            get { return _nodes; }
        }

        /// <summary>
        /// Set the evidence of a node
        /// </summary>
        /// <param name="nodeID">The id of node</param>
        /// <param name="evidence">The evidence</param>
        public void SetNodeEvidence(string nodeID, String evidence)
        {
            _nodes.Where(node => node.NodeID == nodeID).ToList().ForEach(node => node.Evidence = evidence);
        }

        /// <summary>
        /// Kahn Sorting - Topological sort
        /// </summary>
        /// <returns>List of nodes</returns>
        public List<Node> KahnSorting()
        {
            List<Node> L = new List<Node>();
            List<Node> S = new List<Node>();

            int[] indegree=new int[this._nodes.Count];

            for(int i = 0; i < this._nodes.Count; ++i)

            {
                indegree[i] = _nodes[i].ParentsNumber;
            }

            foreach (var node in _nodes)
            {
                if (node.HasParent == false)
                {
                    S.Add(node);
                }
            }

            int cnt = 0;
            while (S.Count != 0)
            {
                var tempNode = S.First();
                L.Add(tempNode);
                S.RemoveAt(0);

                var aux = _nodes.Where(node => node.IsChidOf(tempNode.NodeID) == true).ToList();
                foreach (var node in aux)
                {
                    if (--indegree[_nodes.IndexOf(node)] == 0)
                    {
                        S.Add(node);
                    }
                }
                cnt += 1;
            }

            if (cnt != _nodes.Count)
                throw new Exception("The graph has cycles!");

            return L;
        }

        /// <summary>
        /// Getter
        /// </summary>
        /// <param name="nodeID">The id of node</param>
        /// <param name="typeOfProbability">The type of probability</param>
        /// <param name="parentsEvidence">The evidence of parents</param>
        /// <returns>The probability of node</returns>
        public double GetProbabilityOfNode(string nodeID, String typeOfProbability, string parentsEvidence) //e.g. parentsEvidence = "Yes Yes" || parentsEvidence = "p"
        {

            foreach (var node in _nodes)
            {
                if (node.NodeID == nodeID)
                {

                    var p = node.GetProbabilities(parentsEvidence);
                    foreach (var evidence in node.GetEvidenceDomain())
                    {
                        if (evidence == typeOfProbability)
                            return p[node.GetEvidenceDomain().IndexOf(evidence)];

                    }
                }
            }
            throw new Exception("Probability not found!");
        }
    }
}

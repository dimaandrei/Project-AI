using System;
using System.Collections.Generic;
using System.Linq;

namespace BayesProject
{
    public class Graph
    {
        private readonly List<Node> _nodes;
        private int _noNodes;

        public Graph(int noNodes)
        {
            _noNodes = noNodes;
            _nodes = new List<Node>();
        }

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

        public void SetProbabilities(string nodeID, List<string> values)
        {
            _nodes.Where(node => node.NodeID == nodeID).ToList().ForEach(node => node.SetProbabilities(values));
        }

        public void PrintNodesProbabilities()
        {
            foreach (var node in _nodes)
            {
                node.PrintProbabilites();
            }
        }

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

        public List<Node> Nodes
        {
            get { return _nodes; }
        }

        public void SetNodeEvidence(string nodeID, String evidence)
        {
            _nodes.Where(node => node.NodeID == nodeID).ToList().ForEach(node => node.Evidence = evidence);
        }

        public List<Node> KahnSorting()
        {
            List<Node> L = new List<Node>();
            List<Node> S = new List<Node>();
            int[] indegree = new int[_noNodes];

            for (int i = 0; i < _noNodes; ++i)
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

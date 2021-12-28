using System;
using System.Collections.Generic;
using System.Linq;

namespace BayesProject
{
    public class Graph
    {
        private readonly List<Node> nodes;
        private int noNodes;

        public Graph(int _noNodes)
        {
            this.noNodes = _noNodes;
            this.nodes = new List<Node>();
        }

        public void AddNodes(string id, string []parents)
        {
            var temp = new Node(id);
            if (parents!=null)
            {
                foreach (var parent in parents)
                {
                    temp.AddParent(parent);
                }
            }
            nodes.Add(temp);
        }

        public void SetProbabilities(string nodeID, List<string> values)
        {
            nodes.Where(node => node.NodeID == nodeID).ToList().ForEach(node => node.SetProbabilities(values));
        }

        public void PrintNodesProbabilities()
        {
            foreach(var node in nodes)
            {
                node.PrintProbabilites();
            }
        }

        public List<Node> GetNodes
        {
            get { return nodes; }
        }

        public void SetNodeEvidence(string nodeID, TypeOfEvidence evidence)
        {
            nodes.Where(node => node.NodeID == nodeID).ToList().ForEach(node => node.Evidence=evidence);
        }

        public List<Node> KahnSorting()
        {
            List<Node> L = new List<Node>();
            List<Node> S = new List<Node>();
            int[] indegree=new int[noNodes];

            for(int i=0;i<noNodes;++i)
            {
                indegree[i] = nodes[i].ParentsNumber;
            }

            foreach(var node in nodes)
            {
                if(node.HasParent == false)
                {
                    S.Add(node);
                }
            }

            int cnt = 0;

            while(S.Count!=0)
            {
                var tempNode = S.First();
                L.Add(tempNode);
                S.RemoveAt(0);

                var aux = nodes.Where(node => node.IsChidOf(tempNode.NodeID)==true).ToList();
                foreach(var node in aux)
                {
                    if(--indegree[nodes.IndexOf(node)] ==0)
                    {
                        S.Add(node);
                    }
                }
                cnt += 1;
            }

            if (cnt != nodes.Count)
                throw new Exception("The graph has cycles!");

            return L;
        }
    }
}

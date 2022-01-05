using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace BayesProject
{
    public class Node
    {
        public readonly static string NOT_PRESENT = "NotPresent";
        private readonly String VertexId;
        private readonly HashSet<String> ParentAdjacencySet;
        private String evidence;
        private List<String> evidenceDomain;
        private Dictionary<string, List<double>> probabilitiesMap;

        public Node(String _vertexId)
        {
            this.VertexId = _vertexId;
            this.ParentAdjacencySet = new HashSet<String>();
            probabilitiesMap = new Dictionary<string, List<double>>();
            evidenceDomain = new List<string>();
            evidence = NOT_PRESENT;
        }

        public Node CloneNode()
        {
            Node newNode = (Node)this.MemberwiseClone();
            return newNode;
        }

        public bool IsChidOf(String id)
        {
            return this.ParentAdjacencySet.Contains(id);
        }

        public void AddParent(String _vertex)
        {
            if (this.VertexId == _vertex)
            {
                throw new ArgumentException("The vertex cannot be adjacent to itself!");
            }
            this.ParentAdjacencySet.Add(_vertex);
        }

        public HashSet<String> GetAdjacentVertices()
        {
            return this.ParentAdjacencySet;
        }

        public void AddEvidenceToDomain(String evidence)
        {
            evidenceDomain.Add(evidence);
        }

        public int GetEvidencesNumber()
        {
            return evidenceDomain.Count;
        }

        public List<String> GetEvidenceDomain()
        {
            return new List<String>(evidenceDomain);
        }

        public String Evidence
        {
            get { return evidence; }
            set { evidence = value; }
        }

        public void SetProbabilities(List<string> values)
        {
           /* if (values.Count != Math.Pow(2, ParentAdjacencySet.Count))
                throw new ArgumentException("Number of lines doesn't match."); //think more about this one*/
            if (values.Count == 1)
            {
                var val = values.First<string>().Split(' ').Select(p => Double.Parse(p)).ToList();
                probabilitiesMap.Add("p", val);
            }
            else
            {
                foreach (string line in values)
                {
                    var aux = line.Split(' ');
                    var key = aux.Take(ParentAdjacencySet.Count).Aggregate("", (acumulator, partial) => acumulator += partial + " ").Trim();
                    var probs = aux.Reverse().Take(evidenceDomain.Count).Select(p => Double.Parse(p)).ToList();
                    probs.Reverse();
                    probabilitiesMap.Add(key, probs);
                }
            }
        }

        public List<double> GetProbabilities(string key)
        {
            return probabilitiesMap[key];
        }

        public string ParentsToString()
        {
            string aux = "";
            foreach (var i in ParentAdjacencySet)
            {
                aux += i + " ";
            }
            return aux.Trim();
        }
        public String NodeID
        {
            get { return VertexId; }
        }

        public void PrintProbabilites()
        {
            Console.WriteLine("Probabilities for \"" + VertexId + "\" with parents [" + ParentsToString() + "]:");
            foreach (var p in probabilitiesMap)
            {
                Console.Write("\t" + p.Key + ": " );
                foreach(var prob in p.Value)
                {
                    Console.Write(prob + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool HasParent
        {
            get { return ParentAdjacencySet.Count != 0; }
        }

        public int ParentsNumber
        {
            get { return ParentAdjacencySet.Count; }
        }
    }
}

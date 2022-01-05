using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace BayesProject
{
    public enum TypeOfEvidence
    {
        Yes,
        No,
        NotPresent
    }

    public class Node
    {
        private readonly String VertexId;
        private readonly HashSet<String> ParentAdjacencySet;
        private TypeOfEvidence evidence = TypeOfEvidence.NotPresent;
        private  Dictionary<string, Tuple<double, double>> probabilitiesMap;

        public Node(String _vertexId)
        {
            this.VertexId = _vertexId;
            this.ParentAdjacencySet = new HashSet<String>();
            probabilitiesMap = new Dictionary<string, Tuple<double, double>>();
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

        public TypeOfEvidence Evidence{
            get { return evidence; }
            set { evidence = value;  }
        }

        public void SetProbabilities(List<string> values)
        {
            if (values.Count != Math.Pow(2, ParentAdjacencySet.Count))
                throw new ArgumentException("Number of lines doesn't match."); //think more about this one
            if(values.Count == 1)
            {
                var val = values.First<string>().Split(' ').Select(p => Double.Parse(p)).ToList();
                probabilitiesMap.Add("p", Tuple.Create(val[0], val[1]));
            }
            else
            {
                foreach (string line in values)
                {
                    var aux = line.Split(' ');
                    var key = aux.Take(ParentAdjacencySet.Count).Aggregate("", (acumulator, partial) => acumulator += partial + " ").Trim();
                    var probs=aux.Reverse().Take(2).Select(p => Double.Parse(p)).ToList();

                    probabilitiesMap.Add(key, Tuple.Create(probs[1], probs[0]));
                }
            }
        }

        public Tuple<double, double> GetProbabilities(string key)
        {
            return probabilitiesMap[key];
        }

        public string ParentsToString()
        {
            string aux = "";
            foreach(var i in ParentAdjacencySet)
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
            foreach(var p in probabilitiesMap)
            {
                Console.WriteLine("\t" + p.Key + ": " + p.Value);
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

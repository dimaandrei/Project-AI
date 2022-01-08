using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BayesProject
{
    public class Node
    {
        public readonly static string NOT_PRESENT = "NotPresent";
        private readonly String _vertexId;
        private readonly HashSet<String> _parentAdjacencySet;
        private String _evidence;
        private List<String> _evidenceDomain;
        private Dictionary<string, List<double>> _probabilitiesMap;

        public Node(String vertexId)
        {
            this._vertexId = vertexId;
            this._parentAdjacencySet = new HashSet<String>();
            _probabilitiesMap = new Dictionary<string, List<double>>();
            _evidenceDomain = new List<string>();
            _evidence = NOT_PRESENT;
        }

        public Node CloneNode()
        {
            Node newNode = (Node)MemberwiseClone();
            return newNode;
        }

        public bool IsChidOf(String id)
        {
            return _parentAdjacencySet.Contains(id);
        }

        public void AddParent(String vertex)
        {
            if (_vertexId == vertex)
            {
                throw new ArgumentException("The vertex cannot be adjacent to itself!");
            }
            _parentAdjacencySet.Add(vertex);
        }

        public HashSet<String> GetAdjacentVertices()
        {
            return _parentAdjacencySet;
        }

        public void AddEvidenceToDomain(String evidence)
        {
            _evidenceDomain.Add(evidence);
        }

        public int GetEvidencesNumber()
        {
            return _evidenceDomain.Count;
        }

        public List<String> GetEvidenceDomain()
        {
            return new List<String>(_evidenceDomain);
        }

        public String Evidence
        {
            get { return _evidence; }
            set { _evidence = value; }
        }

        public void SetProbabilities(List<string> values)
        {
            /* if (values.Count != Math.Pow(2, ParentAdjacencySet.Count))
                 throw new ArgumentException("Number of lines doesn't match."); //think more about this one*/
            if (values.Count == 1)
            {
                var val = values.First<string>().Split(' ').Select(p => Double.Parse(p)).ToList();
                _probabilitiesMap.Add("p", val);
            }
            else
            {
                foreach (string line in values)
                {
                    var aux = line.Split(' ');
                    var key = aux.Take(_parentAdjacencySet.Count).Aggregate("", (acumulator, partial) => acumulator += partial + " ").Trim();
                    var probs = aux.Reverse().Take(_evidenceDomain.Count).Select(p => Double.Parse(p)).ToList();
                    probs.Reverse();
                    _probabilitiesMap.Add(key, probs);
                }
            }
        }

        public List<double> GetProbabilities(string key)
        {
            return _probabilitiesMap[key];
        }

        public string ParentsToString()
        {
            string aux = "";
            foreach (var i in _parentAdjacencySet)
            {
                aux += i + " ";
            }
            return aux.Trim();
        }
        public String NodeID
        {
            get { return _vertexId; }
        }

        public void PrintProbabilites()
        {
            Console.WriteLine("Probabilities for \"" + _vertexId + "\" with parents [" + ParentsToString() + "]:");
            foreach (var p in _probabilitiesMap)
            {
                Console.Write("\t" + p.Key + ": ");
                foreach (var prob in p.Value)
                {
                    Console.Write(prob + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public bool HasParent
        {
            get { return _parentAdjacencySet.Count != 0; }
        }

        public int ParentsNumber
        {
            get { return _parentAdjacencySet.Count; }
        }
    }
}

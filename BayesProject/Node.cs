using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BayesProject
{
    /// <summary>
    /// Node class
    /// </summary>
    public class Node
    {
        // Private members
        private readonly String _vertexId;
        private readonly HashSet<String> _parentAdjacencySet;
        private String _evidence;
        private List<String> _evidenceDomain;
        private Dictionary<string, List<double>> _probabilitiesMap;

        // Public members
        public readonly static string NOT_PRESENT = "NotPresent";

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="vertexId">Id of the vertex</param>
        public Node(String vertexId)
        {
            this._vertexId = vertexId;
            this._parentAdjacencySet = new HashSet<String>();
            _probabilitiesMap = new Dictionary<string, List<double>>();
            _evidenceDomain = new List<string>();
            _evidence = NOT_PRESENT;
        }

        /// <summary>
        /// Method which clone a node
        /// </summary>
        /// <returns>A new cloned node</returns>
        public Node CloneNode()
        {
            Node newNode = (Node)MemberwiseClone();
            return newNode;
        }

        /// <summary>
        /// Method which check if a node is a child of another one
        /// </summary>
        /// <param name="id">Id of the vertex</param>
        /// <returns>True, if the node is a child of the node which have the id
        ///          False, otherwise </returns>

        public bool IsChidOf(String id)
        {
            return _parentAdjacencySet.Contains(id);
        }

        /// <summary>
        /// Add a parent node
        /// </summary>
        /// <param name="vertex">The vertex</param>
        public void AddParent(String vertex)
        {
            if (_vertexId == vertex)
            {
                throw new ArgumentException("The vertex cannot be adjacent to itself!");
            }
            _parentAdjacencySet.Add(vertex);
        }

        /// <summary>
        /// Getter
        /// </summary>
        /// <returns>_parentAdjacencySet</returns>
        public HashSet<String> GetAdjacentVertices()
        {
            return _parentAdjacencySet;
        }

        /// <summary>
        /// Add evidence to list
        /// </summary>
        /// <param name="evidence">The name of evidence</param>
        public void AddEvidenceToDomain(String evidence)
        {
            _evidenceDomain.Add(evidence);
        }

        /// <summary>
        /// Getter
        /// </summary>
        /// <returns>The number of evidences</returns>
        public int GetEvidencesNumber()
        {
            return _evidenceDomain.Count;
        }

        /// <summary>
        /// Getter
        /// </summary>
        /// <returns>The list of evidences</returns>
        public List<String> GetEvidenceDomain()
        {
            return new List<String>(_evidenceDomain);
        }

        /// <summary>
        /// Getter/Setter of evidence
        /// </summary>
        public String Evidence
        {
            get { return _evidence; }
            set { _evidence = value; }
        }

        /// <summary>
        /// Method which set the probability
        /// </summary>
        /// <param name="values">The values</param>
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

        /// <summary>
        /// Getter
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>List of probabilities</returns>
        public List<double> GetProbabilities(string key)
        {
            return _probabilitiesMap[key];
        }

        /// <summary>
        /// Method which convert to string
        /// </summary>
        /// <returns>Parents in string format</returns>
        public string ParentsToString()
        {
            string aux = "";
            foreach (var i in _parentAdjacencySet)
            {
                aux += i + " ";
            }
            return aux.Trim();
        }

        /// <summary>
        /// Getter of node
        /// </summary>
        public String NodeID
        {
            get { return _vertexId; }
        }

        /// <summary>
        /// Print probabilities
        /// </summary>
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

        /// <summary>
        /// Check is a node has parent
        /// </summary>
        public bool HasParent
        {
            get { return _parentAdjacencySet.Count != 0; }
        }

        /// <summary>
        /// Getter of the number of parents
        /// </summary>
        public int ParentsNumber
        {
            get { return _parentAdjacencySet.Count; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BayesProject
{
    /// <summary>
    ///  Calculate probability using inference by enumeration
    /// </summary>
    public class InferenceByEnumeration
    {
        private BayesNetwork _bayesNetwork;

        /// <summary>
        /// Constructor of class
        /// </summary>
        /// <param name="bayesNetwork">Bayesian network</param>
        public InferenceByEnumeration(BayesNetwork bayesNetwork)
        {
            _bayesNetwork = bayesNetwork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryVariable">Query node</param>
        /// <returns>An array of probabilities for query variable</returns>
        public double[] EnumerationAsk(String queryVariable)
        {
            var nodes = _bayesNetwork.getNetworkGraph.KahnSorting();
            TypeOfEvidence[] evidences = (TypeOfEvidence[])Enum.GetValues(typeof(TypeOfEvidence));

            double[] Q = new double[evidences.Length - 1];

            foreach (var node in nodes)
            {
                if (node.NodeID == queryVariable && node.Evidence != TypeOfEvidence.NotPresent)
                {
                    int j = 0;
                    foreach (var typeOfEvidence in evidences)
                    {
                        if (typeOfEvidence == TypeOfEvidence.NotPresent)
                            continue;

                        if (node.Evidence == typeOfEvidence)
                        {
                            Q[j] = 1.0;
                        }
                        else
                        {
                            Q[j] = 0.0;
                        }
                        ++j;
                    }
                    return Q;
                }
            }

            
            int i = 0;

            foreach (var typeOfEvidence in evidences)
            {
                if (typeOfEvidence == TypeOfEvidence.NotPresent)
                    continue;
                var vars = new List< Node > ();
                foreach(var node in nodes)
                {
                    Node aux = node.CloneNode();
                    if(aux.NodeID == queryVariable)
                    {
                        aux.Evidence = typeOfEvidence;
                    }
                    vars.Add(aux);
                }

                _bayesNetwork.getNetworkGraph.SetNodeEvidence(queryVariable, typeOfEvidence);
                
                Q[i] = EnumerateAll(vars, typeOfEvidence); 
                
                _bayesNetwork.getNetworkGraph.SetNodeEvidence(queryVariable, TypeOfEvidence.NotPresent);
                i++;                                      
            }

            return Normalization(Q);
        }
        private List<Node> CopyNodesList(List<Node> nodes)
        {
            List<Node> newList = new List<Node>();
            foreach(var node in nodes)
            {
                newList.Add(node.CloneNode());
            }
            return newList;
        }

        private double EnumerateAll(List<Node> vars, TypeOfEvidence type)
        {
            if (vars.Count == 0) 
                return 1.0;

            var y = vars.First();
            vars.Remove(y);

            List<Node> parents = new List<Node>();

            foreach (var node in _bayesNetwork.getNetworkGraph.GetNodes)
            {
                if (y.IsChidOf(node.NodeID))
                {
                    parents.Add(node);
                }
            }   

            var evidence = "";

            if (parents.Count == 0)
            {
                evidence = "p";
            }
            else
            {
                foreach (var parent in parents)
                {
                    evidence += parent.Evidence + " ";
                }

                evidence = evidence.Remove(evidence.Length - 1, 1);
            }

            if (y.Evidence != TypeOfEvidence.NotPresent)
            {
                return _bayesNetwork.getNetworkGraph.GetProbabilityOfNode(y.NodeID, y.Evidence, evidence) * EnumerateAll(vars, type);
            }
            else
            {
                double sum = 0.0;
                _bayesNetwork.getNetworkGraph.SetNodeEvidence(y.NodeID, TypeOfEvidence.No);
                sum += _bayesNetwork.getNetworkGraph.GetProbabilityOfNode(y.NodeID, TypeOfEvidence.No, evidence) * EnumerateAll(CopyNodesList(vars), TypeOfEvidence.No);
                _bayesNetwork.getNetworkGraph.SetNodeEvidence(y.NodeID, TypeOfEvidence.Yes);
                sum += _bayesNetwork.getNetworkGraph.GetProbabilityOfNode(y.NodeID, TypeOfEvidence.Yes, evidence) * EnumerateAll(CopyNodesList(vars), TypeOfEvidence.Yes);
                _bayesNetwork.getNetworkGraph.SetNodeEvidence(y.NodeID, TypeOfEvidence.NotPresent);
                return sum;
            }
        }

        public double[] Normalization(double[] Q)
        {
            double sum = Q.Sum();

            for (var i = 0; i < Q.Length; i++)
            {
                Q[i] /= sum;
            }

            return Q;
        }
    }
}

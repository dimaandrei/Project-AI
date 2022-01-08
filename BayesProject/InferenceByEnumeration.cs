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
        ///  Method that claculate all probabilities for a query node
        /// </summary>
        /// <param name="queryVariable">Query node</param>
        /// <returns>An array of probabilities</returns>
        public double[] EnumerationAsk(String queryVariable)
        {
            var nodes = _bayesNetwork.NetworkGraph.KahnSorting();

            var querryNode = _bayesNetwork.NetworkGraph.GetNode(queryVariable);

            if (querryNode == null) throw new Exception("Querry node - not found!");

            double[] q = new double[querryNode.GetEvidenceDomain().Count];

            foreach (var node in nodes)
            {
                var nodeEvidenceDomain = node.GetEvidenceDomain();
                if (node.NodeID == queryVariable && node.Evidence != Node.NOT_PRESENT)
                {
                    int j = 0;
                    foreach (var typeOfEvidence in nodeEvidenceDomain)
                    {
                        if (typeOfEvidence == Node.NOT_PRESENT)
                            continue;

                        if (node.Evidence == typeOfEvidence)
                        {
                            q[j] = 1.0;
                        }
                        else
                        {
                            q[j] = 0.0;
                        }
                        ++j;
                    }
                    return q;
                }
            }


            int i = 0;

            foreach (var typeOfEvidence in querryNode.GetEvidenceDomain())
            {
                if (typeOfEvidence == Node.NOT_PRESENT)
                    continue;

                _bayesNetwork.NetworkGraph.SetNodeEvidence(queryVariable, typeOfEvidence);
                var vars = CopyNodesList(nodes);

                _bayesNetwork.NetworkGraph.SetNodeEvidence(queryVariable, typeOfEvidence);

                q[i] = EnumerateAll(vars, querryNode.GetEvidenceDomain());

                _bayesNetwork.NetworkGraph.SetNodeEvidence(queryVariable, Node.NOT_PRESENT);
                i++;
            }

            return Normalization(q);
        }

        /// <summary>
        /// Recursive metod that calculate probability of a node in a given situation
        /// </summary>
        /// <param name="vars">List of nodes</param>
        /// <param name="domain">Evidences domain</param>
        /// <returns>Probability in a given situation</returns>
        private double EnumerateAll(List<Node> vars, List<String> domain)
        {
            if (vars.Count == 0)
                return 1.0;

            var y = vars.First();
            vars.Remove(y);

            List<Node> parents = new List<Node>();

            foreach (var node in _bayesNetwork.NetworkGraph.Nodes)
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

            if (y.Evidence != Node.NOT_PRESENT)
            {
                return _bayesNetwork.NetworkGraph.GetProbabilityOfNode(y.NodeID, y.Evidence, evidence) * EnumerateAll(vars, domain);
            }
            else
            {
                double sum = 0.0;
                foreach (var ev in domain)
                {
                    if (!y.GetEvidenceDomain().Contains(ev))
                        continue;
                    _bayesNetwork.NetworkGraph.SetNodeEvidence(y.NodeID, ev);
                    sum += _bayesNetwork.NetworkGraph.GetProbabilityOfNode(y.NodeID, ev, evidence) * EnumerateAll(CopyNodesList(vars), domain);
                };
                _bayesNetwork.NetworkGraph.SetNodeEvidence(y.NodeID, Node.NOT_PRESENT);
                return sum;
            }
        }

        /// <summary>
        /// Creates a deep copy to a list of nodes
        /// </summary>
        /// <param name="nodes">List of nodes</param>
        /// <returns>A copy of list</returns>
        private List<Node> CopyNodesList(List<Node> nodes)
        {
            List<Node> newList = new List<Node>();
            foreach (var node in nodes)
            {
                newList.Add(node.CloneNode());
            }
            return newList;
        }

        /// <summary>
        /// Normalize the probabilities
        /// </summary>
        /// <param name="q">Array of probailities</param>
        /// <returns>Normalized array of probabilities</returns>
        private double[] Normalization(double[] q)
        {
            double sum = q.Sum();

            for (var i = 0; i < q.Length; i++)
            {
                q[i] /= sum;
            }

            return q;
        }
    }
}

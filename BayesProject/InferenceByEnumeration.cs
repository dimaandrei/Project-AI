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
            var vars = _bayesNetwork.getNetworkGraph.KahnSorting();
            TypeOfEvidence[] evidences = (TypeOfEvidence[])Enum.GetValues(typeof(TypeOfEvidence));

            double[] Q = new double[evidences.Length - 1];
            int i = 0;

            foreach (var typeOfEvidence in evidences)
            {
                if (typeOfEvidence == TypeOfEvidence.NotPresent)
                    continue;

                Q[i] = EnumerateAll(vars, typeOfEvidence);
                i++;
            }

            return Normalization(Q);
        }

        private double EnumerateAll(List<Node> vars, TypeOfEvidence type)
        {
            if (vars.Count == 0) return 1.0;

            var y = vars.First();
            vars.Remove(y);

            if (y.Evidence != TypeOfEvidence.NotPresent)
            {
                var sum = 0.0;

                foreach (var parent in y.GetAdjacentVertices())
                {
                    //
                }

                return sum * EnumerateAll(vars, type);
            }
            else
            {
                // TO DO
                return 0.0;
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

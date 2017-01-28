using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MachineLearning.Core;

namespace MachineLearning.Clusterers
{
    public interface IClusterer
    {
        /// <summary>
        /// Classifies a given instance
        /// </summary>
        /// <param name="instance">Instance to be assigned a cluster</param>
        /// <returns>The number of the assigned cluster</returns>
        int ClusterInstance(Instance instance);

        /// <summary>
        /// Build the clusterer
        /// </summary>
        void Build();

        /// <summary>
        /// Predicts the cluster membership for a given instance.
        /// </summary>
        /// <param name="instance">Instance to be assigned a cluster</param>
        /// <returns>The number of the assigned cluster</returns>
        double[] DistributionForInstance(Instance instance);

        /// <summary>
        /// Returns the number of clusters
        /// </summary>
        int NumberOfClusters { get; }       

        /// <summary>
        /// Returns clusters
        /// </summary>
        Instances Centroids { get; }
    }
}

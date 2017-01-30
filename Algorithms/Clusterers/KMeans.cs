using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineLearning.Core;
using MachineLearning.Exceptions;

namespace MachineLearning.Clusterers
{
    public class KMeans : IClusterer
    {
        #region Private variables
        // Default cluster number
        private const int DEFAULT_NB_CLUSTERS = 4;

        // Maximum iteration number (if defined)
        private int? _maxIter;

        // Dataset
        private Instances _dataSet;
        
        // Number of cluster
        private int _nbClusters;

        // Dimension of centroid position
        private int _centroidDim;

        // Centroids position
        // Dim1 -> cluster, Dim2 -> dimension
        private double[,] _centroids;

        // Dim1 -> cluster, Dim2 -> dimension
        private double[,] _instances;

        // instance assignation
        private int[] _assignation;
        #endregion

        #region Properties

        /// <summary>
        /// Get the clusterized dataset
        /// </summary>
        public Instances DataSet
        {
            get
            {
                this.assignClusterToDataset();
                return this._dataSet;
            }
        }

        /// <summary>
        /// Returns the sets of centroids
        /// </summary>
        public Instances Centroids
        {
            get
            {
                var instances = new Instances();
                instances.Attributes = this._dataSet.Attributes;
                instances.ClassIndex = this._dataSet.ClassIndex;

                for (int i = 0; i < this._nbClusters; i++)
                {
                    Instance instance = new Instance();
                    int j = 0;
                    foreach (var attribute in instances.Attributes)
                    {                         
                        if (attribute.AttributeType == AttributeType.Numeric)
                        {
                            // set the centroids
                            instance.Attributes.Add(new InstanceAttribute
                            {
                                AttributeType = AttributeType.Numeric,
                                DoubleValue = this._centroids[i, j]
                            });
                            j++;
                        }
                        else
                        {
                            instance.Attributes.Add(attribute.Clone());
                        }
                    }
                    instance.Attributes[instances.ClassIndex].DoubleValue = i;
                    instances.DataSet.Add(instance);
                }

                return instances;
            }
        }

        public int NumberOfClusters
        {
            get
            {
                return _nbClusters;
            }
        }
        #endregion

        #region Constructors
        public KMeans(Instances dataSet, double[,] initCentroids = null, int? nbClusters = null, int? maxIter = null)
        {
            this._dataSet = dataSet;
            this._centroidDim = dataSet.Attributes.Where(a => a.AttributeType == AttributeType.Numeric).Count();

            if (initCentroids != null && nbClusters != null)
            {
                if (nbClusters != initCentroids.Length)
                {
                    throw new WrongClustersNumberException();
                }

                this._centroids = initCentroids;
            }              
            else if (initCentroids != null)
            {
                this._nbClusters = initCentroids.GetLength(0);
                this._centroids = initCentroids;
                if (initCentroids.GetLength(1) != this._centroidDim)
                {
                    throw new BadClusterDimensionException();
                }
            }
            else
            {
                this._nbClusters = nbClusters.HasValue ? nbClusters.Value : DEFAULT_NB_CLUSTERS;
                this._centroids = new double[this._nbClusters,_centroidDim];
                this.initCentroids();
            }

            this._maxIter = maxIter;

            //Private variable initilization
            this._instances = new double[this._dataSet.DataSet.Count, this._centroidDim];
            for (int i = 0; i < this._dataSet.DataSet.Count; i++)
            {
                int dim = 0;
                foreach (var attr in this._dataSet.DataSet[i].Attributes)
                {
                    if (attr.AttributeType == AttributeType.Numeric)
                    {
                        this._instances[i, dim] = attr.DoubleValue;
                        dim++;
                    }             
                }
            }
            this._assignation = new int[this._dataSet.DataSet.Count];
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Build clusterer
        /// </summary>
        public void Build()
        {
            int iteration = 0;
            while (true)
            {
                if (!this.Iteration())
                    break;

                iteration++;
                if (this._maxIter.HasValue)
                    if (iteration >= this._maxIter)
                        break;
            }
        }

        /// <summary>
        /// Performs an iteration. (   
        /// </summary>
        /// <returns></returns>
        public bool Iteration()
        {
            this.assignCluster();
            return (this.updateCentroids());
        }

        public int ClusterInstance(Instance instance)
        {
            throw new NotImplementedException();
        }

        public double[] DistributionForInstance(Instance instance)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private methods
        private void initCentroids()
        {
            const int MIN = 0;
            const int MAX = 1;

            double[,] bounds = new double[_centroidDim,2]; // bound[,0] -> min, bound[,1] -> max

            for (int i = 0; i < _centroidDim; i++)
            {
                bounds[i, MIN] = Double.MaxValue;
                bounds[i, MAX] = Double.MinValue;
            }

            // limits
            foreach (var instance in this._dataSet.DataSet)
            {
                int currentDim = 0;
                foreach(var attr in instance.Attributes)
                {
                    if (attr.AttributeType == AttributeType.Numeric)
                    {
                        if (attr.DoubleValue < bounds[currentDim, MIN])
                            bounds[currentDim, MIN] = attr.DoubleValue;
                        if (attr.DoubleValue > bounds[currentDim, MAX])
                            bounds[currentDim, MAX] = attr.DoubleValue;

                        currentDim++;
                    }
                }
            }

            Random random = new Random();
            // set cluster position
            for (int i = 0; i < this._nbClusters; i++)
            {
                for (int j = 0; j < this._centroidDim; j++)
                {
                    this._centroids[i,j] = random.NextDouble() * (bounds[j,MAX] - bounds[j,MIN]) + bounds[j,MIN];
                }
            }

        }

        /// <summary>
        /// Assign a cluster to each instance
        /// </summary>
        private void assignCluster()
        {
            // For each instance
            for(int instanceId = 0; instanceId < this._instances.GetLength(0); instanceId++)
            {
                int cluster = -1;
                double distanceMin = double.MaxValue;
                
                // Search the closest cluster
                for (int clusterId = 0; clusterId < this._nbClusters; clusterId++)
                {
                    double distance = 0;
                    for (int dim = 0; dim < _centroidDim; dim++)
                    {
                        distance += Math.Pow(_instances[instanceId, dim] - _centroids[clusterId, dim],2);
                        // Sqrt is useless in this case (distance is used only for comparison)
                    }
                    
                    //It's the closest
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        cluster = clusterId;
                    }
                }
                this._assignation[instanceId] = cluster;
            }
        }

        /// <summary>
        /// Updates the centroid
        /// </summary>
        /// <returns>True if the centroids were Updated</returns>
        private bool updateCentroids()
        {
            bool centroidUpdated = false;

            // for each centroids
            for (int clusterId = 0; clusterId < this._nbClusters; clusterId++)
            {   
                // for each dimension
                for (int dim = 0; dim < this._centroidDim; dim++)
                {
                    double sum = 0.0;
                    int clusterSize = 0;
                    
                    // for each instance of this cluster
                    for (int instanceId = 0; instanceId < this._instances.GetLength(0); instanceId++)
                    {
                        if (this._assignation[instanceId] == clusterId)
                        {
                            clusterSize++;
                            sum += _instances[instanceId, dim];
                        }
                    }

                    double position = sum / clusterSize;

                    if (this._centroids[clusterId, dim] != position)
                    {
                        centroidUpdated = true;
                        this._centroids[clusterId, dim] = position;
                    }
                }
            }

            return centroidUpdated;
        }

        private void assignClusterToDataset()
        {
            for (int instanceId = 0; instanceId < this._dataSet.DataSet.Count; instanceId++)
            {
                this._dataSet.DataSet[instanceId].Attributes[this._dataSet.ClassIndex].DoubleValue = this._assignation[instanceId];
            }
        }                
        #endregion
    }
}

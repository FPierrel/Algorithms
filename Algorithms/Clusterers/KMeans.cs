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

        // Initial dataset
        private Instances _dataSet;

        // Number of cluster
        private int _nbClusters;

        // Dimension of cluster position
        private int _clusterDim;

        // Dim1 -> cluster, Dim2 -> dimension
        private double[,] _clusters;

        // Dim1 -> cluster, Dim2 -> dimension
        private double[,] _instances;

        // instance assignation
        private int[] _assignation;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the sets of clusters
        /// </summary>
        public Instances Clusters
        {
            get
            {
                var instances = new Instances();
                instances.Attributes = this._dataSet.Attributes;

                for (int i = 0; i < this._nbClusters; i++)
                {
                    Instance instance = new Instance();
                    int j = 0;
                    foreach (var attribute in instances.Attributes)
                    {                         
                        if (attribute.AttributeType == AttributeType.Numeric)
                        {
                            // set the cluster position
                            instance.Attributes.Add(new InstanceAttribute
                            {
                                AttributeType = AttributeType.Numeric,
                                DoubleValue = this._clusters[i, j]
                            });
                            j++;
                        }
                        else
                        {
                            instance.Attributes.Add(attribute.Clone());
                        }
                    }
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
        public KMeans(Instances dataSet, double[,] initClusters = null, int? nbClusters = null, int? maxIter = null)
        {
            this._dataSet = dataSet;
            this._clusterDim = dataSet.Attributes.Where(a => a.AttributeType == AttributeType.Numeric).Count();

            if (initClusters != null && nbClusters != null)
            {
                if (nbClusters != initClusters.Length)
                {
                    throw new WrongClustersNumberException();
                }

                this._clusters = initClusters;
            }              
            else if (initClusters != null)
            {
                this._nbClusters = initClusters.Length;
                this._clusters = initClusters;
                if (initClusters.GetLength(0) != this._clusterDim)
                {
                    throw new BadClusterDimensionException();
                }
            }
            else
            {
                this._nbClusters = nbClusters.HasValue ? nbClusters.Value : DEFAULT_NB_CLUSTERS;
                this._clusters = new double[this._nbClusters,_clusterDim];
                this.initClusters();
            }

            this._maxIter = maxIter;

            //Private variable initilization
            this._instances = new double[this._nbClusters, this._clusterDim];
            for (int i = 0; i < this._dataSet.DataSet.Count; i++)
            {
                int dim = 0;
                foreach (var attr in this._dataSet.DataSet[i].Attributes)
                {
                    if (attr.AttributeType == AttributeType.Numeric)
                    {
                        this._instances[i, j] = attr.DoubleValue;
                        dim++;
                    }             
                }
            }
            this._assignation = new int[_nbClusters];
        }
        #endregion

        #region Public methods
        public void Build()
        {
            throw new NotImplementedException();
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

        #region Private method
        private void initClusters()
        {
            const int MIN = 0;
            const int MAX = 1;

            double[,] bounds = new double[_clusterDim,2]; // bound[,0] -> min, bound[,1] -> max

            for (int i = 0; i < _clusterDim; i++)
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
                for (int j = 0; j < this._clusterDim; j++)
                {
                    this._clusters[i,j] = random.NextDouble() * (bounds[j,MAX] - bounds[j,MIN]) + bounds[j,MIN];
                }
            }

        }

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
                    for (int dim = 0; dim < _clusterDim; dim++)
                    {
                        distance += Math.Pow(_instances[clusterId, dim] - _clusters[clusterId, dim],2);
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

        
        #endregion
    }
}

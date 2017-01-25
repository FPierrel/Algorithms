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
        // Default cluster number
        private const int DEFAULT_NB_CLUSTERS = 4;

        // Maximum iteration number (if defined)
        private int? _maxIter;

        // Dataset
        private Instances _dataSet;

        // Number of cluster
        private int _nbClusters;

        // Dimension of cluster position
        private int _clusterDim;

        // _clusters[cluster][dim]
        private double[][] _clusters;

        /// <summary>
        /// Returns the sets of clusters
        /// </summary>
        public Instances Clusters
        {
            get
            {
                var instances = new Instances();
                instances.Attributes = this._dataSet.Attributes;

                foreach (var cluster in _clusters)
                {
                    int i = 0;
                    Instance instance = new Instance();
                    foreach (var attribute in instances.Attributes)
                    {
                        if (attribute.AttributeType == AttributeType.Numeric)
                        {
                            // set the cluster position
                            instance.Attributes.Add(new InstanceAttribute
                            {
                                AttributeType = AttributeType.Numeric,
                                DoubleValue = cluster[i]
                            });
                            i++;
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

        public int ClusterInstance(Instance instance)
        {
            throw new NotImplementedException();
        }

        public double[] DistributionForInstance(Instance instance)
        {
            throw new NotImplementedException();
        }

        public KMeans(Instances dataSet, double[][] initClusters = null, int? nbClusters = null, int? maxIter = null)
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
                if (initClusters[0].Length != this._clusterDim)
                {
                    throw new BadClusterDimensionException();
                }
            }
            else
            {
                this._nbClusters = nbClusters.HasValue ? nbClusters.Value : DEFAULT_NB_CLUSTERS;
                this.initClusters();
            }

            this._maxIter = maxIter;
        }

        private void initClusters()
        {
            const int MIN = 0;
            const int MAX = 1;

            double[,] bounds = new double[_clusterDim,1]; // bound[,0] -> min, bound[,1] -> max

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
                    this._clusters[i][j] = random.NextDouble() * (bounds[j,MAX] - bounds[j,MIN]) + bounds[j,MIN];
                }
            }

        }

        public void Build()
        {
            throw new NotImplementedException();
        }
    }
}

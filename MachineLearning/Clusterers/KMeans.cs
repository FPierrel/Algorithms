using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineLearning.Core;

namespace MachineLearning.Clusterers
{
    public class KMeans : IClusterer
    {
        private Instances _dataSet;

        private int _nbClusters;

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
                    foreach(var attribute in instances.Attributes)
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

        public void BuildClusterer(Instances data)
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

        public KMeans(Instances dataSet, int nbClusters)
        {

        }
    }
}

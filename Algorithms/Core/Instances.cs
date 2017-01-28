using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Instances
    {
        /// <summary>
        /// The instances
        /// </summary>
        public List<Instance> DataSet { get; set; }

        /// <summary>
        /// The attribute information.
        /// </summary>
        public List<InstanceAttribute> Attributes { get; set; }

        /// <summary>
        /// The class attribute's index.
        /// </summary>
        public int ClassIndex { get; set; }

        /// <summary>
        /// DataSet name
        /// </summary>
        public string Name { get; set; }
        
        public Instances()
        {
            this.Attributes = new List<InstanceAttribute>();
            this.DataSet = new List<Instance>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning.Core
{
    /// <summary>
    /// An instance represents a data row
    /// </summary>
    public class Instance
    {
        public List<InstanceAttribute> Attributes { get; set; }
    }
}
 
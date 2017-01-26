using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning.Exceptions
{
    class BadClusterDimensionException : Exception
    {
        public BadClusterDimensionException() : base("The number of dimensions of the cluster does not match")
        {

        }
    }
}

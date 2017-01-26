using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning.Exceptions
{
    class WrongClustersNumberException : Exception
    {
        public WrongClustersNumberException() : base("The number of the cluster does not match")
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning.Core
{

    public enum AttributeType
    {
        /// <summary>
        /// This type represents a floating-point number
        /// </summary>
        Numeric,

        /// <summary>
        /// This type represents categorical data (like 'dog' or 'cat')
        /// </summary>
        Nominal,

        /// <summary>
        /// This type represents a dynamically expanding set of nominal values
        /// </summary>
        String,

        /// <summary>
        /// This type represents a date
        /// </summary>
        Date
    }
    
    /// <summary>
    /// Abstract attribute. An attribute represents a column of data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Attribute<T>
    {
        /// <summary>
        /// Attribute's value
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Attribute's name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Attribute's type
        /// </summary>
        public abstract AttributeType AttributeType { get; }        
    }

    public class NumericAttribute : Attribute<double>
    {
        public override AttributeType AttributeType
        {
            get
            {
                return AttributeType.Numeric;
            }
        }
    }

    public class NominalAttribute : Attribute<string>
    {
        public override AttributeType AttributeType
        {
            get
            {
                return AttributeType.Nominal;
            }
        }

        public List<string> Classes { get; set; }
    }

    public class StringAttribute : Attribute<string>
    {
        public override AttributeType AttributeType
        {
            get
            {
                return AttributeType.String;
            }
        }
    }

    public class DateAttribute : Attribute<DateTime>
    {
        public override AttributeType AttributeType
        {
            get
            {
                return AttributeType.Date;
            }
        }
    }
}

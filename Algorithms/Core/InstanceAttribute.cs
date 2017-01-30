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

    public class InstanceAttribute
    {
        public AttributeType AttributeType { get; set; }
        

        private string _internalString;
        private double _internalDouble;
        private DateTime _internalDate;

        public string StringValue
        {
            get
            {
                if (AttributeType == AttributeType.String || AttributeType == AttributeType.Nominal)
                {
                    return this._internalString;
                }
                else if (AttributeType == AttributeType.Numeric)
                {
                    return "" + this._internalDouble;
                }
                else if (AttributeType == AttributeType.Date)
                {
                    return this._internalDate.ToString();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (AttributeType == AttributeType.String || AttributeType == AttributeType.Nominal)
                {
                    this._internalString = value;
                }
                else if (AttributeType == AttributeType.Numeric)
                {
                    double val;
                    this._internalDouble = Double.TryParse(value, out val) ? val : 0.0;
                }
                else if (AttributeType == AttributeType.Date)
                {
                    this._internalDate = DateTime.Parse(value);
                }
            }
        }

        public double DoubleValue
        {
            get
            {
                if (AttributeType == AttributeType.String || AttributeType == AttributeType.Nominal)
                {
                    double value;
                    return Double.TryParse(this._internalString, out value) ? value : 0.0;
                }
                else if (AttributeType == AttributeType.Numeric)
                {
                    return this._internalDouble;
                }
                else if (AttributeType == AttributeType.Date)
                {
                    return this._internalDate.Ticks;
                }
                else
                {
                    return 0.0;
                }
            }
            set
            {
                if (AttributeType == AttributeType.String || AttributeType == AttributeType.Nominal)
                {
                    this._internalString = "" + value;
                }
                else if (AttributeType == AttributeType.Numeric)
                {
                    this._internalDouble = value;
                }
                else if (AttributeType == AttributeType.Date)
                {
                    this._internalDate = new DateTime((long)value);
                }
            }
        }
        public DateTime DateValue
        {
            get
            {
                if (AttributeType == AttributeType.String || AttributeType == AttributeType.Nominal)
                {
                    return DateTime.Parse(_internalString);
                }
                else if (AttributeType == AttributeType.Numeric)
                {
                    return new DateTime((long)_internalDouble);
                }
                else if (AttributeType == AttributeType.Date)
                {
                    return this._internalDate;
                }
                else
                {
                    return new DateTime();
                }
            }
            set
            {
                if (AttributeType == AttributeType.String || AttributeType == AttributeType.Nominal)
                {
                    this._internalString = value.ToString();
                }
                else if (AttributeType == AttributeType.Numeric)
                {
                    this._internalDouble = value.Ticks;
                }
                else if (AttributeType == AttributeType.Date)
                {
                    this._internalDate = value;
                }
            }
        }

        public InstanceAttribute Clone()
        {
            return (InstanceAttribute)this.MemberwiseClone();
        }
    }

}

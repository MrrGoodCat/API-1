using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelAPICore
{
    /// <summary>
    /// This is NICE representation for the metric. 
    /// It can be shown by the WinForms Property grid.
    /// </summary>
    [TypeConverter(typeof(MetricConverter))]
    public class NICEMetric : INICEMetric
    {
        public static readonly NICEMetric TotalArchivingBacklogMetric = new NICEMetric("Total Archiving backlog", "NICE Center",
                                                                        NICEMetricType.Gauge, "Calls");
        public static readonly NICEMetric TotalArchivingRateMetric = new NICEMetric("Total Archiving rate", "NICE Center",
                                                                        NICEMetricType.Gauge, "Calls per sec");
        public static readonly NICEMetric TotalCallsRateMetric = new NICEMetric("Total Calls rate", "NICE Center",
                                                                        NICEMetricType.Gauge, "Calls per sec");
        public NICEMetric(string name, string group, NICEMetricType type, string measurementUnit, MetricColorIDs colorID, bool isAggregationEnable)
        {
            Name = name;
            Group = group;
            MeasurementUnit = measurementUnit;
            Type = type;
            ColorID = colorID;
            IsAggregationEnable = isAggregationEnable;
        }

        public NICEMetric(string name, string group, NICEMetricType type, string measurementUnit, MetricColorIDs colorID)
            : this(name, group, type, measurementUnit, colorID, false) { }

        public NICEMetric(string name, string group, NICEMetricType type, string measurementUnit)
            : this(name, group, type, measurementUnit, MetricColorIDs.Empty, false) { }

        /// <summary>
        /// Creates new Metric for given Monitored System based on existing metric.
        /// </summary>
        public NICEMetric(NICEMetric metric, string monitoredSystemName)
        {
            Name = metric.Name + " " + monitoredSystemName;
            Group = metric.Group;
            MeasurementUnit = metric.MeasurementUnit;
            Type = metric.Type;
            ColorID = metric.ColorID;
            IsAggregationEnable = metric.IsAggregationEnable;
        }

        public NICEMetric(Metric metric)
        {
            Name = metric.MetricName;
            Group = metric.MetricGroup;
            MeasurementUnit = metric.MeasurementUnit;
            Type = CommonEnumsParseHelper.ParseMetricType(metric.MetricType);
            //TODO: AlexL set correct ColorID when we'll have posibility to get it from Metric class
            ColorID = MetricColorIDs.GreaterBetter;
            IsAggregationEnable = metric.IsAggregationEnabled;
        }

        public string Name { get; private set; }
        public string Group { get; private set; }
        public string MeasurementUnit { get; private set; }
        public NICEMetricType Type { get; private set; }
        //applicable only for percentage metrics. In other cases should be MetricColorIDs.Empty
        public MetricColorIDs ColorID { get; private set; }
        public bool IsAggregationEnable { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:[{1}]. Type: {2}({3}. Is aggregation {4})", Name, Group, Type, MeasurementUnit, IsAggregationEnable);
        }

        #region Equals and == override

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is NICEMetric))
            {
                return false;
            }
            NICEMetric castedObj = (NICEMetric)obj;
            return Equals(castedObj);
        }

        new public static bool Equals(object first, object second)
        {
            if (first == null)
            {
                return false;
            }
            return first.Equals(second);
        }

        /// <summary>
        /// Equals - measurement unit comparison is case sensitive.
        /// </summary>
        /// <param name="valueType1">
        /// The value type 1.
        /// </param>
        /// <param name="valueType2">
        /// The value type 2.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool EqualsCaseSensitiveUnit(NICEMetric valueType1, NICEMetric valueType2)
        {
            if (valueType2 == null)
            {
                return false;
            }

            if (ReferenceEquals(valueType1, valueType2))
            {
                return true;
            }

            if ((valueType1.Type != valueType2.Type) || (!valueType1.Name.Equals(valueType2.Name, StringComparison.OrdinalIgnoreCase))
                    || (!valueType1.Group.Equals(valueType2.Group, StringComparison.OrdinalIgnoreCase))
                    || (!valueType1.MeasurementUnit.Equals(valueType2.MeasurementUnit))
                    || (valueType1.IsAggregationEnable != valueType2.IsAggregationEnable))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Better equals to increase performance by saving boxing
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns>true if the given value type is equal to this one and false otherwise</returns>
        public bool Equals(NICEMetric valueType)
        {
            if (valueType == null) return false;
            if (ReferenceEquals(this, valueType)) return true;
            if ((Type != valueType.Type) || (!Name.Equals(valueType.Name, StringComparison.OrdinalIgnoreCase))
                    || (!Group.Equals(valueType.Group, StringComparison.OrdinalIgnoreCase))
                    || (!MeasurementUnit.Equals(valueType.MeasurementUnit, StringComparison.OrdinalIgnoreCase))
                    || (IsAggregationEnable != valueType.IsAggregationEnable)
                )
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(NICEMetric valueType1, NICEMetric valueType2)
        {
            if (ReferenceEquals(valueType1, null))
            {
                if (ReferenceEquals(valueType2, null))
                {
                    return true;
                }
                return false;
            }
            return valueType1.Equals(valueType2);
        }

        public static bool operator !=(NICEMetric valueType1, NICEMetric valueType2)
        {
            return !(valueType1 == valueType2);
        }

        #endregion Equals and == override

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Converter to display this metric in the Property grid
    /// </summary>
    internal class MetricConverter : TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context,
                                                                object value, Attribute[] filter)
        {
            return TypeDescriptor.GetProperties(value, filter);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}

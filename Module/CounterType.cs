using System.ComponentModel;


namespace Module
{
    public enum CounterType
    {
        [Description("Average")]
        Average,

        [Description("_Total")]
        Total,

        [Description("Simple")]
        Simple
    }
}

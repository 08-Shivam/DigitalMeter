using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMeter
{
    public class ListItem
    {
        public ListItem(string label, object value) //constructor for setting Labal and Value
        {
            Label = label;
            Value = value;
        }
        public string Label { get; set; } = "";
        public object Value { get; set; } = "";
    }
}

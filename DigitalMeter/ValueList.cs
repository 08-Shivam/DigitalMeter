using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitalMeter
{
    public class ValueList
    {
        public static ListItem[] StopBitsList = new ListItem[] {
            new ListItem("1", 1),
            new ListItem("1.5", 3),
            new ListItem("2", 2)
        };
        public static ListItem[] DataBits = new ListItem[]
        {
            new ListItem("5", 5),
            new ListItem("6", 6),
            new ListItem("7", 7),
            new ListItem("8", 8),
        };

        public static ListItem[] ParityList = new ListItem[]
        {
            new ListItem("None", 0),
            new ListItem("Odd", 1),
            new ListItem("Even", 2),
            new ListItem("Mark", 3),
            new ListItem("Space", 4),
        };

        public static ListItem[] HandShakingList = new ListItem[]
        {
            new ListItem("None",0),
            new ListItem("RTS",1),
            new ListItem("XOn/XOff",2),
            new ListItem("RTS + XOn/XOff",3),
        };

        public static ListItem[] BoudRateList = new ListItem[]
        {
            new ListItem("600",600),
            new ListItem("1200",1200),
            new ListItem("2400",2400),
            new ListItem("4800",4800),
            new ListItem("9600",9600),
            new ListItem("14400",14400),
            new ListItem("19200",19200),
            new ListItem("57600",57600),
            new ListItem("115200",115200),
            new ListItem("128000",128000),
            new ListItem("256000",256000),
        };

        //////
        public static Dictionary<string, Type> MoreForms = new Dictionary<string, Type>
            {
                { "Form1", typeof(Form1) },
                { "Form2", typeof(Form2) },
                { "Meter Settings", typeof(Form4) },
                { "kVAh, kWh", typeof(Form5) },
                {"Last Recharge Details",typeof(Form6) }
            };
    }
}

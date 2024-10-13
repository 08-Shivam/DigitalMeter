using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace DigitalMeter
{
    public partial class Form4 : Form
    {
        private static SerialPort _serialPort = new SerialPort();
        private byte[] ByteArray = new byte[8];
        //FormData formData = new FormData();
        public Form4()
        {
            InitializeComponent();
        }
        private void Form4_Load(object sender, EventArgs e)
        {
            lblOperation.Text=GlobalState.OperationMode.ToString();
            lblmeterID.Text = GlobalState.MeterID.ToString();
            lblPerUnitS1.Text = GlobalState.PerUnitS1.ToString();
            lblPerUnitS2.Text = GlobalState.PerUnitS2.ToString();
            lblPerUnitS3.Text = GlobalState.PerUnitS3.ToString();
            lblPerUnitS4.Text = GlobalState.PerUnitS4.ToString();
            lblAutoCut.Text = GlobalState.AutoCut.ToString();
            lblMaxDemand.Text=GlobalState.AutoCut.ToString();
            lblGracePeriod.Text=GlobalState.GracePeriod.ToString();
            lblSLS1.Text = GlobalState.SLS1.ToString();
            lblSLS2.Text = GlobalState.SLS2.ToString();
            lblSLS3.Text = GlobalState.SLS3.ToString();
            lblSLS4.Text = GlobalState.SLS4.ToString();
            lblRateS1.Text = GlobalState.RateS1.ToString();
            lblRateS2.Text = GlobalState.RateS2.ToString();
            lblRateS3.Text = GlobalState.RateS3.ToString();
            lblRateS4.Text = GlobalState.RateS4.ToString(); 
            lblFPD.Text=GlobalState.RateFPD.ToString();
            lblLoad.Text=GlobalState.Load.ToString();
            lblPhaseOnS1S2.Text="S1="+GlobalState.PhaseS1+" S2="+GlobalState.PhaseS2;
            lblPhaseOnS3S4.Text="S3="+GlobalState.PhaseS3+" S4="+GlobalState.PhaseS4;
        }
        public void lblOperation_TextChanged(object sender, EventArgs e)
        {
            
        }
        
        public ushort ModRTU_CRC(byte[] buf, int len)  //passing buf array of 1 byte= 8bits and len variable of int type
        {
            ushort num = ushort.MaxValue; //ushort is a data type of 2 bytes = 16 bits
            for (int index1 = 0; index1 < len; ++index1)
            {
                num ^= (ushort)buf[index1];
                for (int index2 = 8; index2 != 0; --index2)
                {
                    if (((uint)num & 1U) > 0U)
                        num = (ushort)((uint)(ushort)((uint)num >> 1) ^ 40961U);
                    else
                        num >>= 1;
                }
            }
            return num;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblmeterID_Click(object sender, EventArgs e)
        {
            
        }

        private void tableLayoutPanel10_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

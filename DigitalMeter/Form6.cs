using System;
using System.Drawing;
using System.Windows.Forms;

namespace DigitalMeter
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            try
            {
                lblmeterID.Text = GlobalState.MeterID.ToString();
                lblSerialNo.Text = GlobalState.SerialNumber.ToString();
                lblAmount.Text = GlobalState.Amount.ToString();
                lblSerial.Text = GlobalState.Serial.ToString();
                lblNumber.Text = GlobalState.Number.ToString();
                lblRechargCode.Text = GlobalState.RechargeCode.ToString();
                lblRechargeDate.Text = GlobalState.RechargeDate.ToString();
                lblRechargeValue.Text = GlobalState.RechargeValue.ToString();
                lblOpenTime1.Text = GlobalState.CoverOpenTime1.ToString();
                lblOpenTime2.Text = GlobalState.CoverOpenTime2.ToString();
                lblOpenDate1.Text = GlobalState.CoverOpenDate1.ToString();
                lblOpenDate2.Text = GlobalState.CoverOpenDate2.ToString();
                if (GlobalState.CurrentBalance >= 0)
                {
                    lblSignal7.BackColor = Color.GreenYellow;
                }
                else
                {
                    lblSignal7.BackColor = Color.Red;
                }
                lblCurrentBalance.Text = GlobalState.CurrentBalance.ToString();

                if (GlobalState.CoverOpenlight == 0)
                {
                    lblLight5.BackColor = Color.Green;
                }
                else
                {
                    lblLight5.BackColor = Color.Red;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

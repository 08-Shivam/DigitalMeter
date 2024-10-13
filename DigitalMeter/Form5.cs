using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitalMeter
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            lblmeterID.Text = GlobalState.MeterID.ToString();
            lblAvgCurrent.Text = GlobalState.AverageCurrent;
            lblLNVoltage.Text = Convert.ToString(GlobalState.AverageLLVoltage);
            lblAvgLLVoltage.Text=Convert.ToString(GlobalState.AverageLLVoltage);
            lblFrequency.Text = GlobalState.Frequency.ToString();
            lblPowerFactor.Text = GlobalState.PowerFactor.ToString();
            lblAppEnergyTotal.Text = GlobalState.AppEnergyTotal.ToString();
            lblActEnergyTotal.Text= GlobalState.ActEnergyTotal.ToString();
            lblAppEnergyEB.Text = GlobalState.AppEnergyEB.ToString();
            lblActEnergyEB.Text=GlobalState.ActEnergyEB.ToString();

            //default Red
            lblLight1.BackColor = Color.White;
            lblLight2.BackColor = Color.White;
            lblLight3.BackColor = Color.White;
            lblLight4.BackColor = Color.White;
            if (GlobalState.EBDBSW == 0)
            {
                lblLight1.BackColor = Color.Green;
                lblLight2.BackColor = Color.Red;
                lblLight3.BackColor = Color.Red;
                lblLight4.BackColor = Color.Red;
            }
            else if (GlobalState.EBDBSW == 1)
            {
                lblLight1.BackColor = Color.Red;
                lblLight2.BackColor = Color.Green;
                lblLight3.BackColor = Color.Red;
                lblLight4.BackColor = Color.Red;
            }
            else if (GlobalState.EBDBSW == 2)
            {
                lblLight1.BackColor = Color.Red;
                lblLight2.BackColor = Color.Red;
                lblLight3.BackColor = Color.Green;
                lblLight4.BackColor = Color.Red;
            }
            else if (GlobalState.EBDBSW == 3)
            {
                lblLight1.BackColor = Color.Red;
                lblLight2.BackColor = Color.Red;
                lblLight3.BackColor = Color.Red;
                lblLight4.BackColor = Color.Green;
            }
            lblAppEnergyDG.Text = GlobalState.AppEnergyDG.ToString();
            lblActEnergyDG.Text = GlobalState.AppEnergyDG.ToString();
            lblAppEnergySolar.Text = GlobalState.AppEnergySolar.ToString();
            lblActEnergySolar.Text = GlobalState.ActEnergySolar.ToString(); 
            lblAppEnergyWind.Text = GlobalState.AppEnergyWind.ToString(); 
            lblActEnergyWind.Text = GlobalState.ActEnergyWind.ToString();
        }
        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void meterID_Click(object sender, EventArgs e)
        {

        }
    }
}

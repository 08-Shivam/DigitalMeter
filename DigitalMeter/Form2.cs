using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitalMeter
{
    public partial class Form2 : Form
    {
        Form1 form1 = new Form1();
        public Form2()
        {
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
        private void lblSend1_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (!_serialPort.IsOpen)
            //        _serialPort.Open();

            //    byte Mid = Convert.ToByte(this.label1.Text);
            //    byte[] array1 = { Mid, 3, 0, 0, 0, 100 }; //changes  19 sept
            //    int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

            //    byte crcHB = Convert.ToByte(crc / 256),
            //        crcLB = Convert.ToByte(crc % 256);
            //    byte[] array = { Mid, 3, 0, 0, 0, 100, crcLB, crcHB };// changes 19 sept
            //    _serialPort.Write(array, 0, array.Length);
            //    Thread.Sleep(1000);
            //    int bytesToRead = _serialPort.BytesToRead;
            //    byte[] buffer = new byte[bytesToRead];
            //    _serialPort.Read(buffer, 0, bytesToRead);
            //    //Thread.Sleep(1000);
            //    this.ByteArray = buffer;
            //    string str1 = "";
            //    for (int index = 0; index <= this.ByteArray.Length - 1; ++index)
            //        str1 = str1 + "," + this.ByteArray[index].ToString();
            //    _serialPort.Close();
            //    //this.txtOutPut.Text = str1.ToString();
            //    if (this.ByteArray.Length < 9)
            //        return;

            //    int num1 = 1;
            //    int num2 = 0; //3
            //                  //after looping at index 6 and num3, it will enter inside loop
            //    for (int index = 3; index < this.ByteArray.Length - 1; ++index)
            //    {
            //        ++num2;
            //        if (num2 == 4)
            //        {
            //            byte num3 = this.ByteArray[index];
            //            byte num4 = this.ByteArray[index - 1];
            //            byte num5 = this.ByteArray[index - 2];
            //            byte num6 = this.ByteArray[index - 3];
            //            string str2 = BitConverter.ToInt32(new byte[4] { num6, num5, num4, num3 }, 0).ToString();
            //            //this.txtOutPutABCD.Text = this.txtOutPutABCD.Text + " V" + num1.ToString() + " = " + str2.ToString();
            //            string str3 = BitConverter.ToInt32(new byte[4] { num4, num3, num6, num5 }, 0).ToString();
            //            //this.txtOutputCDAB.Text = this.txtOutputCDAB.Text + " V" + num1.ToString() + " = " + str3.ToString();
            //            string str4 = BitConverter.ToInt32(new byte[4] { num3, num4, num5, num6 }, 0).ToString();
            //            //this.txtOutPutDCBA.Text = this.txtOutPutDCBA.Text + " V" + num1.ToString() + " = " + str4.ToString();
            //            string str5 = BitConverter.ToInt32(new byte[4] { num5, num6, num3, num4 }, 0).ToString();
            //            //this.txtOutPutBADC.Text = this.txtOutPutBADC.Text + " V" + num1.ToString() + " = " + str5.ToString();
            //            num2 = 0;
            //            ++num1;
            //        }
            //    }
            //    {
            //        //lblSLS1
            //        int lbl_sls1 = (Convert.ToInt32(this.ByteArray[35]) * 256) + Convert.ToInt32(this.ByteArray[36]);
            //        lblSLS1.Text = lbl_sls1.ToString();

            //        //lblSLS2
            //        int lbl_sls2 = (Convert.ToInt32(this.ByteArray[37]) * 256) + Convert.ToInt32(this.ByteArray[38]);
            //        lblSLS2.Text = lbl_sls2.ToString();

            //    }
            //}
            //catch (Exception ex)
            //{
            //    int num = (int)MessageBox.Show(ex.Message);
            //}
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
        private void lblsend5_Click(object sender, EventArgs e) // Send Write action button for SL:S1 
        {

        }
        private void lblsend6_Click(object sender, EventArgs e)  // Send action button for SL:S1
        {

        }
        public void lblmeterID_TextChanged (object sender, EventArgs e) // TextBox for meter ID readonl
        {
  
        }
        private void lblsendmeter4_Click(object sender, EventArgs e)
        {

        } // For Meter (ON/OFF) Write Action button
        private void lblsendmeter8_Click(object sender, EventArgs e) // For Deduction Write Action button
        {
        }
        private void lblDeduct_TextChanged(object sender, EventArgs e)
        {

        }
        private void lblsendwrite3_Click(object sender, EventArgs e) // For RR send Write Action button
        {
           // if (lblsendwrite3.Click.) { }
        }
        private void lblRE_TextChanged(object sender, EventArgs e) //Textbox RR (Meter(Read/Write))
        {
            string _REtext=lblRE.Text;
        }

        private void lblsendread2_Click(object sender, EventArgs e) 
        {

        }
    }
}

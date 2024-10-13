using DigitalMeter.Models;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace DigitalMeter
{
    public partial class Form1 : Form
    {
        private static SerialPort _serialPort = new SerialPort();
        private byte[] ByteArray = new byte[8];
        string strcode = "";

      
       public Form1()
        {
            InitializeComponent();
        }

        private void OpenForm2_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ddlBoudRate.DataSource = ValueList.BoudRateList; 
            ddlDataBits.DataSource = ValueList.DataBits;
            ddlParity.DataSource = ValueList.ParityList;
            ddlStopBits.DataSource = ValueList.StopBitsList;
            ddlHandShaking.DataSource = ValueList.HandShakingList;
            BindComPortList();
            string port = ddlComPort.SelectedValue.ToString() ?? "COM1"; //connect comport

            Form1._serialPort = new SerialPort(port);

            _serialPort.PortName = Form1.SetPortName(_serialPort.PortName);
            _serialPort.BaudRate = Form1.SetPortBoudRate(_serialPort.BaudRate);
            _serialPort.BaudRate = 4800;
            ddlBoudRate.SelectedValue = (int)_serialPort.BaudRate;

            _serialPort.Parity = Form1.SetPortParity(_serialPort.Parity);
            ddlParity.SelectedValue = (int)_serialPort.Parity;

            _serialPort.DataBits = Form1.SetPortDataBits(_serialPort.DataBits);
            ddlDataBits.SelectedValue = (int)_serialPort.DataBits;

            _serialPort.StopBits = Form1.SetPortStopBits(_serialPort.StopBits);
            ddlStopBits.SelectedValue = (int)_serialPort.StopBits;

            _serialPort.Handshake =Form1.SetPortHandshake(_serialPort.Handshake);
            ddlHandShaking.SelectedValue = (int)_serialPort.Handshake;

            _serialPort.ReadTimeout = 10500;
            _serialPort.WriteTimeout = 1500;

        }

        private void BindComPortList()
        {
            try
            {
                var x = SerialPort.GetPortNames();

                ddlComPort.DataSource = x;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, nameof(ex.InnerException));
            }
        }

        // send1 button Sept 19, 2024
        private void button1_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            GlobalState.MeterID = Convert.ToInt32(txtMeterID.Text);
            try
            {
                if (!_serialPort.IsOpen)
                    _serialPort.Open();

                byte Mid = Convert.ToByte(this.txtMeterID.Text);
                byte[] array1 = { Mid, 4, 0, 0,0, 100 }; //changes  19 sept
                int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

                byte crcHB = Convert.ToByte(crc / 256),
                    crcLB = Convert.ToByte(crc % 256);
                byte[] array = { Mid, 4, 0, 0, 0, 100,crcLB,crcHB };// changes 19 sept
                _serialPort.Write(array, 0, array.Length);
                Thread.Sleep(1000);
                int bytesToRead = _serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);
                //Thread.Sleep(1000);
                this.ByteArray = buffer;
                string str1 = "";
                for (int index = 0; index <= this.ByteArray.Length - 1; ++index)
                    str1 = str1 + "," + this.ByteArray[index].ToString();
                _serialPort.Close();
                //this.txtOutPut.Text = str1.ToString();
                if (this.ByteArray.Length < 9)
                    return;

                int num1 = 1;
                int num2 = 0; //3
                //after looping at index 6 and num3, it will enter inside loop
                for (int index = 3; index < this.ByteArray.Length - 1; ++index)
                {
                    ++num2;
                    if (num2 == 4)
                    {
                        byte num3 = this.ByteArray[index];
                        byte num4 = this.ByteArray[index - 1];
                        byte num5 = this.ByteArray[index - 2];
                        byte num6 = this.ByteArray[index - 3];
                        string str2 = BitConverter.ToInt32(new byte[4] { num6, num5, num4, num3 }, 0).ToString();
                        //this.txtOutPutABCD.Text = this.txtOutPutABCD.Text + " V" + num1.ToString() + " = " + str2.ToString();
                        string str3 = BitConverter.ToInt32(new byte[4] { num4, num3, num6, num5 }, 0).ToString();
                        //this.txtOutputCDAB.Text = this.txtOutputCDAB.Text + " V" + num1.ToString() + " = " + str3.ToString();
                        string str4 = BitConverter.ToInt32(new byte[4] { num3, num4, num5, num6 }, 0).ToString();
                        //this.txtOutPutDCBA.Text = this.txtOutPutDCBA.Text + " V" + num1.ToString() + " = " + str4.ToString();
                        string str5 = BitConverter.ToInt32(new byte[4] { num5, num6, num3, num4 }, 0).ToString();
                        //this.txtOutPutBADC.Text = this.txtOutPutBADC.Text + " V" + num1.ToString() + " = " + str5.ToString();
                        num2 = 0;
                        ++num1;
                    }
                }
                {
                    //showing to lblDeviceId textbox
                    lblDeviceId.Text = Convert.ToString(this.ByteArray[0]);

                    //Average current to lblAvgCurrent
                    string str2 = BitConverter.ToInt32(new byte[4] { this.ByteArray[3], this.ByteArray[4], this.ByteArray[5], this.ByteArray[6] }, 0).ToString();
                    GlobalState.AverageCurrent = (str2);
                    //form5.Show();

                    float strLLVoltage = (BitConverter.ToInt32(new byte[4] { this.ByteArray[26], this.ByteArray[25], this.ByteArray[24], this.ByteArray[23] }, 0))/100;
                    GlobalState.AverageLLVoltage = strLLVoltage;
                    //form5.Show();

                    float strLNVoltage= (BitConverter.ToInt32(new byte[4] { this.ByteArray[22], this.ByteArray[21], this.ByteArray[20], this.ByteArray[19] }, 0)) / 100;
                    GlobalState.AverageLNVoltage = strLNVoltage;
                    //form5.Show();

                    float frequency= (BitConverter.ToInt32(new byte[4] { this.ByteArray[54], this.ByteArray[53], this.ByteArray[52], this.ByteArray[51] }, 0))/100;
                    GlobalState.Frequency = frequency;
                    //form5.Show();

                    float powerFactor = (BitConverter.ToInt32(new byte[4] { this.ByteArray[70], this.ByteArray[69], this.ByteArray[68], this.ByteArray[67] },0))/100;
                    GlobalState.PowerFactor = powerFactor;
                    //form5.Show();

                    int lblAppEB = (BitConverter.ToInt32(new byte[4] { this.ByteArray[194], this.ByteArray[193], this.ByteArray[192], this.ByteArray[191] }, 0)) / 100; //1329792
                    GlobalState.AppEnergyEB = lblAppEB;
                    //form5.Show();

                    int lblActEB = (BitConverter.ToInt32(new byte[4] { this.ByteArray[198], this.ByteArray[197], this.ByteArray[196], this.ByteArray[195] }, 0))/100; //1225121
                    GlobalState.ActEnergyEB = lblActEB;
                    //form5.Show();

                    int lblAppTotal = (BitConverter.ToInt32(new byte[4] { this.ByteArray[174], this.ByteArray[173], this.ByteArray[172], this.ByteArray[171] }, 0))/100; //1329792
                    GlobalState.AppEnergyTotal = lblAppTotal;
                    //form5.Show();

                    int lblActTotal = (BitConverter.ToInt32(new byte[4] { this.ByteArray[178], this.ByteArray[177], this.ByteArray[176], this.ByteArray[175] }, 0))/100; //1225121
                    GlobalState.ActEnergyTotal = lblActTotal;
                    //form5.Show();

                    //Source for Lights (lblLight1, lblLight2, lblLight3, lblLight4)
                    int Source = BitConverter.ToInt32(new byte[4] { this.ByteArray[74], this.ByteArray[73], this.ByteArray[72], this.ByteArray[71] }, 0);
                    GlobalState.EBDBSW = Source;

                    form5.Show();
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message);
            }
        }

        //send2 button changes created at Sept 19, 2024
        private void button2_Click(object sender, EventArgs e)
        {
            GlobalState.MeterID = Convert.ToInt32(txtMeterID.Text);
            try
            {
                if (!_serialPort.IsOpen)
                    _serialPort.Open();

                byte Mid = Convert.ToByte(this.txtMeterID.Text);
                byte[] array1 = { Mid, 4, 1, 44, 0, 100 }; //changes at Sept 19, 2024
                int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

                byte crcHB = Convert.ToByte(crc / 256),
                    crcLB = Convert.ToByte(crc % 256);
                byte[] array = { Mid, 4, 1, 44, 0, 100, crcLB, crcHB };// changes at Sept 19, 2024
                _serialPort.Write(array, 0, array.Length);
                Thread.Sleep(1000);
                int bytesToRead = _serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);
                //Thread.Sleep(1000);
                this.ByteArray = buffer;
                string str1 = "";
                for (int index = 0; index <= this.ByteArray.Length - 1; ++index)
                    str1 = str1 + "," + this.ByteArray[index].ToString();
                _serialPort.Close();
                //this.txtOutPut.Text = str1.ToString();
                if (this.ByteArray.Length < 9)
                    return;

                int num1 = 1;
                int num2 = 0; //3
                for (int index = 3; index < this.ByteArray.Length - 1; ++index) //after looping at index 6 and num3, it will enter inside loop
                {
                    ++num2;
                    if (num2 == 4)
                    {
                        byte num3 = this.ByteArray[index];
                        byte num4 = this.ByteArray[index - 1];
                        byte num5 = this.ByteArray[index - 2];
                        byte num6 = this.ByteArray[index - 3];
                        string str2 = BitConverter.ToInt32(new byte[4] { num6, num5, num4, num3 }, 0).ToString();
                        //this.txtOutPutABCD.Text = this.txtOutPutABCD.Text + " V" + num1.ToString() + " = " + str2.ToString();
                        string str3 = BitConverter.ToInt32(new byte[4] { num4, num3, num6, num5 }, 0).ToString();
                        //this.txtOutputCDAB.Text = this.txtOutputCDAB.Text + " V" + num1.ToString() + " = " + str3.ToString();
                        string str4 = BitConverter.ToInt32(new byte[4] { num3, num4, num5, num6 }, 0).ToString();
                        //this.txtOutPutDCBA.Text = this.txtOutPutDCBA.Text + " V" + num1.ToString() + " = " + str4.ToString();
                        string str5 = BitConverter.ToInt32(new byte[4] { num5, num6, num3, num4 }, 0).ToString();
                        //this.txtOutPutBADC.Text = this.txtOutPutBADC.Text + " V" + num1.ToString() + " = " + str5.ToString();
                        num2 = 0;
                        ++num1;
                    }
                }

                {
                    //Gloal
                    //lastRechargeCode
                    int strCode1 = BitConverter.ToInt32(new byte[4] { this.ByteArray[42], this.ByteArray[41], this.ByteArray[40], this.ByteArray[39] }, 0);
                    int strCode2 = BitConverter.ToInt32(new byte[4] { this.ByteArray[46], this.ByteArray[45], this.ByteArray[44], this.ByteArray[43] }, 0);
                    string newCode = strCode1.ToString() + strCode2.ToString();
                    GlobalState.RechargeCode = (newCode);
                    strcode= newCode.ToString();
                    GlobalState.Amount=DecodeCode_Amount(strcode);
                    GlobalState.Number= DecodeCode_Number(strcode);
                    GlobalState.Serial = DecodeCode_Serial(strcode);
                    GlobalState.SerialNumber= DecodeCode( strcode);

                    //lastRechargeValue
                    string str2 = BitConverter.ToInt32(new byte[4] { this.ByteArray[34], this.ByteArray[33], this.ByteArray[32], this.ByteArray[31] }, 0).ToString();
                    GlobalState.RechargeValue = Convert.ToInt32(str2);
                    //lastRechargeValue.Text = str2;

                    //lastRechargeDate
                    int strDate = BitConverter.ToInt32(new byte[4] { this.ByteArray[38], this.ByteArray[37], this.ByteArray[36], this.ByteArray[35] }, 0);
                    GlobalState.RechargeDate= Convert.ToInt32(strDate);
                    //lastRechargeDate.Text = strDate.ToString();

                    //lblLight5
                    int lbllight5 = BitConverter.ToInt32(new byte[4] { this.ByteArray[182], this.ByteArray[181], this.ByteArray[170], this.ByteArray[169] }, 0);
                    GlobalState.CoverOpenlight = Convert.ToInt32(lbllight5);
                    //if(lbllight5 == 0)
                    //{
                    //    lblLight5.BackColor = Color.Green;
                    //}
                    //elses
                    //{
                    //    lblLight5.BackColor = Color.Red;
                    //}

                    //lblCoverOpenTime1
                    int lblopentime1= BitConverter.ToInt32(new byte[4] { this.ByteArray[190], this.ByteArray[189], this.ByteArray[188], this.ByteArray[187] }, 0);
                    GlobalState.CoverOpenTime1 = Convert.ToInt32(lblopentime1);
                    //lblOpenTime1.Text = lblopentime1.ToString();

                    //lblCoverOpenTime2
                    int lblopentime2 = BitConverter.ToInt32(new byte[4] { this.ByteArray[198], this.ByteArray[197], this.ByteArray[196], this.ByteArray[195] }, 0);
                    GlobalState.CoverOpenTime2 = Convert.ToInt32(lblopentime2);
                    //lblOpenTime2.Text = lblopentime2.ToString();

                    //lblOpenDate1
                    int lbldate1 = BitConverter.ToInt32(new byte[4] { this.ByteArray[186], this.ByteArray[185], this.ByteArray[184], this.ByteArray[183] }, 0);
                    GlobalState.CoverOpenDate1= Convert.ToInt32(lbldate1);
                    //lblOpenDate1.Text = lbldate1.ToString();

                    //lblOpenDate2
                    int lbldate2 = BitConverter.ToInt32(new byte[4] { this.ByteArray[194], this.ByteArray[193], this.ByteArray[192], this.ByteArray[191] }, 0);
                    GlobalState.CoverOpenDate2 = Convert.ToInt32(lbldate2);
                    //lblOpenDate2.Text = lbldate2.ToString();
                    Form6 form6=new Form6();
                    form6.Show();
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message);
            }
        }

        //send3 button changes at Sept19, 2024
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_serialPort.IsOpen)
                    _serialPort.Open();

                byte Mid = Convert.ToByte(this.txtMeterID.Text);
                byte[] array1 = { Mid, 4, 0, 100, 0, 100 }; //changes at Sept 19, 2024
                int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

                byte crcHB = Convert.ToByte(crc / 256),
                    crcLB = Convert.ToByte(crc % 256);
                byte[] array = { Mid, 4, 0, 100, 0, 100, crcLB, crcHB };// changes at Sept 19, 2024
                _serialPort.Write(array, 0, array.Length);
                Thread.Sleep(1000);
                int bytesToRead = _serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);
                this.ByteArray = buffer;
                string str1 = "";
                for (int index = 0; index <= this.ByteArray.Length - 1; ++index)
                    str1 = str1 + "," + this.ByteArray[index].ToString();
                _serialPort.Close();
                if (this.ByteArray.Length < 9)
                    return;

                int num1 = 1;
                int num2 = 0; //3
                for (int index = 3; index < this.ByteArray.Length - 1; ++index) //after looping at index 6 and num3, it will enter inside loop
                {
                    ++num2;
                    if (num2 == 4)
                    {
                        byte num3 = this.ByteArray[index];
                        byte num4 = this.ByteArray[index - 1];
                        byte num5 = this.ByteArray[index - 2];
                        byte num6 = this.ByteArray[index - 3];
                        string str2 = BitConverter.ToInt32(new byte[4] { num6, num5, num4, num3 }, 0).ToString();
                        //this.txtOutPutABCD.Text = this.txtOutPutABCD.Text + " V" + num1.ToString() + " = " + str2.ToString();
                        string str3 = BitConverter.ToInt32(new byte[4] { num4, num3, num6, num5 }, 0).ToString();
                        //this.txtOutputCDAB.Text = this.txtOutputCDAB.Text + " V" + num1.ToString() + " = " + str3.ToString();
                        string str4 = BitConverter.ToInt32(new byte[4] { num3, num4, num5, num6 }, 0).ToString();
                        //this.txtOutPutDCBA.Text = this.txtOutPutDCBA.Text + " V" + num1.ToString() + " = " + str4.ToString();
                        string str5 = BitConverter.ToInt32(new byte[4] { num5, num6, num3, num4 }, 0).ToString();
                        //this.txtOutPutBADC.Text = this.txtOutPutBADC.Text + " V" + num1.ToString() + " = " + str5.ToString();
                        num2 = 0;
                        ++num1;
                    }
                }
                {
                    //lblAppEnergyDG
                    int lblAppDG = (BitConverter.ToInt32(new byte[4] { this.ByteArray[14], this.ByteArray[13], this.ByteArray[12], this.ByteArray[11] }, 0))/100; //1225121
                    GlobalState.AppEnergyDG = lblAppDG;

                    //lblActEnergyDG
                    int lblActDG = (BitConverter.ToInt32(new byte[4] { this.ByteArray[18], this.ByteArray[17], this.ByteArray[16], this.ByteArray[15] }, 0))/100; //1225121
                    GlobalState.ActEnergyDG = lblActDG;

                    //lblAppEnergySolar
                    int lblAppSolar = (BitConverter.ToInt32(new byte[4] { this.ByteArray[34], this.ByteArray[33], this.ByteArray[32], this.ByteArray[31] }, 0))/100; //1225121
                    GlobalState.AppEnergySolar = lblAppSolar;

                    //lblActEnergySolar
                    int lblActSolar =( BitConverter.ToInt32(new byte[4] { this.ByteArray[38], this.ByteArray[37], this.ByteArray[36], this.ByteArray[35] }, 0))/100; //1225121
                    GlobalState.ActEnergySolar = lblActSolar;

                    //lblAppEnergyWind
                    int lblAppWind = (BitConverter.ToInt32(new byte[4] { this.ByteArray[54], this.ByteArray[53], this.ByteArray[52], this.ByteArray[51] }, 0))/100; //1225121
                    GlobalState.AppEnergyWind = lblAppWind;

                    //lblActEnergyWind
                    int lblActWind = (BitConverter.ToInt32(new byte[4] { this.ByteArray[58], this.ByteArray[57], this.ByteArray[56], this.ByteArray[55] }, 0)) / 100; //1225121
                    GlobalState.ActEnergySolar = lblActWind;

                    Form form5=new Form5();
                    form5.Show();
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message);
            }
        }

        //send4 button changes at Sept 19, 2024
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_serialPort.IsOpen)
                    _serialPort.Open();

                byte Mid = Convert.ToByte(this.txtMeterID.Text);
                byte[] array1 = { Mid, 4, 0, 200, 0, 100 }; //changes at Sept 19, 2024
                int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

                byte crcHB = Convert.ToByte(crc / 256),
                    crcLB = Convert.ToByte(crc % 256);
                byte[] array = { Mid, 4, 0, 200, 0, 100, crcLB, crcHB };// changes at Sept 19, 2024
                _serialPort.Write(array, 0, array.Length);
                Thread.Sleep(1000);
                int bytesToRead = _serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);
                //Thread.Sleep(1000);
                this.ByteArray = buffer;
                string str1 = "";
                for (int index = 0; index <= this.ByteArray.Length - 1; ++index)
                    str1 = str1 + "," + this.ByteArray[index].ToString();
                _serialPort.Close();
                //this.txtOutPut.Text = str1.ToString();
                if (this.ByteArray.Length < 9)
                    return;

                int num1 = 1;
                int num2 = 0; //3
                for (int index = 3; index < this.ByteArray.Length - 1; ++index) //after looping at index 6 and num3, it will enter inside loop
                {
                    ++num2;
                    if (num2 == 4)
                    {
                        byte num3 = this.ByteArray[index];
                        byte num4 = this.ByteArray[index - 1];
                        byte num5 = this.ByteArray[index - 2];
                        byte num6 = this.ByteArray[index - 3];
                        string str2 = BitConverter.ToInt32(new byte[4] { num6, num5, num4, num3 }, 0).ToString();
                        string str3 = BitConverter.ToInt32(new byte[4] { num4, num3, num6, num5 }, 0).ToString();
                        string str4 = BitConverter.ToInt32(new byte[4] { num3, num4, num5, num6 }, 0).ToString();
                        string str5 = BitConverter.ToInt32(new byte[4] { num5, num6, num3, num4 }, 0).ToString();
                        num2 = 0;
                        ++num1;
                    }
                }
                { 
                    //lblCurrentBalance
                    int lblcurrentbalance = BitConverter.ToInt32(new byte[4] { this.ByteArray[26], this.ByteArray[25], this.ByteArray[24], this.ByteArray[23] }, 0); //1225121
                    GlobalState.CurrentBalance = lblcurrentbalance;
                    Form6 form6 =new Form6();
                    form6.Show();
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message);
            }
        }

        //send5 button changes Sept 19, 2024
        private void button5_Click(object sender, EventArgs e) { 
        }

        //send6 button changes Sept 29, 2024
        private void button6_Click(object sender, EventArgs e)
        {
            FormData formData = new FormData();
            GlobalState.MeterID = Convert.ToInt32(txtMeterID.Text);
            try
            {
                if (!_serialPort.IsOpen)
                    _serialPort.Open();

                byte Mid = Convert.ToByte(this.txtMeterID.Text);
                byte[] array1 = { Mid, 3, 0, 0, 0, 100 }; //changes  19 sept
                int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

                byte crcHB = Convert.ToByte(crc / 256),
                    crcLB = Convert.ToByte(crc % 256);
                byte[] array = { Mid, 3, 0, 0, 0, 100, crcLB, crcHB };// changes 19 sept
                _serialPort.Write(array, 0, array.Length);
                _serialPort.DiscardInBuffer();// changes 20 Sept
                Thread.Sleep(1000);
                int bytesToRead = _serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);
                //Thread.Sleep(1000);
                this.ByteArray = buffer;
                string str1 = "";
                for (int index = 0; index <= this.ByteArray.Length - 1; ++index)
                    str1 = str1 + "," + this.ByteArray[index].ToString();
                _serialPort.Close();
                //this.txtOutPut.Text = str1.ToString();
                if (this.ByteArray.Length < 9)
                    return;

                int num1 = 1;
                int num2 = 0; //3
                //after looping at index 6 and num3, it will enter inside loop
                for (int index = 3; index < this.ByteArray.Length - 1; ++index)
                {
                    ++num2;
                    if (num2 == 4)
                    {
                        byte num3 = this.ByteArray[index];
                        byte num4 = this.ByteArray[index - 1];
                        byte num5 = this.ByteArray[index - 2];
                        byte num6 = this.ByteArray[index - 3];
                        string str2 = BitConverter.ToInt32(new byte[4] { num6, num5, num4, num3 }, 0).ToString();
                        //this.txtOutPutABCD.Text = this.txtOutPutABCD.Text + " V" + num1.ToString() + " = " + str2.ToString();
                        string str3 = BitConverter.ToInt32(new byte[4] { num4, num3, num6, num5 }, 0).ToString();
                        //this.txtOutputCDAB.Text = this.txtOutputCDAB.Text + " V" + num1.ToString() + " = " + str3.ToString();
                        string str4 = BitConverter.ToInt32(new byte[4] { num3, num4, num5, num6 }, 0).ToString();
                        //this.txtOutPutDCBA.Text = this.txtOutPutDCBA.Text + " V" + num1.ToString() + " = " + str4.ToString();
                        string str5 = BitConverter.ToInt32(new byte[4] { num5, num6, num3, num4 }, 0).ToString();
                        //this.txtOutPutBADC.Text = this.txtOutPutBADC.Text + " V" + num1.ToString() + " = " + str5.ToString();
                        num2 = 0;
                        ++num1;
                    }
                }
                {
                    //showing to lblDeviceId textbox
                    lblDeviceId.Text = Convert.ToString(this.ByteArray[0], 2).PadLeft(8, '0');  // Convert to 8-bit binary
                 
                    //lblOperation
                    int lbloperation = (Convert.ToInt32(this.ByteArray[7]) * 256) + Convert.ToInt32(this.ByteArray[8]);
                    GlobalState.OperationMode=lbloperation;

                    //lblPerUnitS1
                    int lblperunit_s1 = (Convert.ToInt32(this.ByteArray[11]) * 256) + Convert.ToInt32(this.ByteArray[12]);
                    //lblPerUnitS1.Text = lblperunit_s1.ToString();
                    GlobalState.PerUnitS1 = lblperunit_s1;

                    //lblPerUnitS2
                    int lblperunit_s2 = (Convert.ToInt32(this.ByteArray[13]) * 256) + Convert.ToInt32(this.ByteArray[14]);
                    GlobalState.PerUnitS2 = lblperunit_s2;

                    //lblPerUnitS3
                    int lblperunit_s3 = (Convert.ToInt32(this.ByteArray[15]) * 256) + Convert.ToInt32(this.ByteArray[16]);
                    GlobalState.PerUnitS3 = lblperunit_s3;

                    //lblPerUnitS4
                    int lblperunit_s4 = (Convert.ToInt32(this.ByteArray[17]) * 256) + Convert.ToInt32(this.ByteArray[18]);
                    GlobalState.PerUnitS4=lblperunit_s4;

                    //lblAutoCut
                    int lblautocut = (Convert.ToInt32(this.ByteArray[19]) * 256) + Convert.ToInt32(this.ByteArray[20]);
                    GlobalState.AutoCut = lblautocut;

                    //lblGracePeriod
                    int lblgraceperiod = (Convert.ToInt32(this.ByteArray[21]) * 256) + Convert.ToInt32(this.ByteArray[22]);
                    GlobalState.GracePeriod = lblgraceperiod;

                    //lblMaxDemandCut
                    int lblmaxdemand = (Convert.ToInt32(this.ByteArray[33]) * 256) + Convert.ToInt32(this.ByteArray[34]);
                    GlobalState.MaxDemandCut=lblmaxdemand;

                    //lblSLS1
                    int lbl_sls1 = (Convert.ToInt32(this.ByteArray[35]) * 256) + Convert.ToInt32(this.ByteArray[36]);
                    //lblSLS1.Text = lbl_sls1.ToString();
                    GlobalState.SLS1 = lbl_sls1;

                    //lblSLS2
                    int lbl_sls2 = (Convert.ToInt32(this.ByteArray[37]) * 256) + Convert.ToInt32(this.ByteArray[38]);
                    GlobalState.SLS2 = lbl_sls2;

                    //lblSLS3
                    int lbl_sls3 = (Convert.ToInt32(this.ByteArray[39]) * 256) + Convert.ToInt32(this.ByteArray[40]);
                    GlobalState.SLS3 = lbl_sls3;

                    //lblSLS4
                    int lbl_sls4 = (Convert.ToInt32(this.ByteArray[41]) * 256) + Convert.ToInt32(this.ByteArray[42]);
                    GlobalState.SLS4 = lbl_sls4;

                    //lblRateS1
                    int lblrate_s1 = (Convert.ToInt32(this.ByteArray[83]) * 256) + Convert.ToInt32(this.ByteArray[84]);
                    GlobalState.RateS1 = lblrate_s1;

                    //lblRateS2
                    int lblrate_s2 = (Convert.ToInt32(this.ByteArray[85]) * 256) + Convert.ToInt32(this.ByteArray[86]);
                    GlobalState.RateS2 = lblrate_s2;

                    //lblRateS3
                    int lblrate_s3 = (Convert.ToInt32(this.ByteArray[87]) * 256) + Convert.ToInt32(this.ByteArray[88]);
                    GlobalState.RateS3 = lblrate_s3;

                    //lblRateS4
                    int lblrate_s4 = (Convert.ToInt32(this.ByteArray[89]) * 256) + Convert.ToInt32(this.ByteArray[90]);
                    GlobalState.RateS4 = lblrate_s4;

                    //lblRateFPD
                    int lblrate_fpd = (Convert.ToInt32(this.ByteArray[91]) * 256) + Convert.ToInt32(this.ByteArray[92]);
                    GlobalState.RateFPD = lblrate_fpd;
                    //lblLoad

                    //lblPhaseOnS1,S2
                    int lblphase_s1 = (Convert.ToInt32(this.ByteArray[25]) * 256) + Convert.ToInt32(this.ByteArray[26]); //s1
                    int lblphase_s2 = (Convert.ToInt32(this.ByteArray[27]) * 256) + Convert.ToInt32(this.ByteArray[28]); //s2
                    GlobalState.PhaseS1 = lblphase_s1; 
                    GlobalState.PhaseS2= lblphase_s2;
                    
                    //lblPhaseOnS3,S4
                    int lblphase_s3 = (Convert.ToInt32(this.ByteArray[29]) * 256) + Convert.ToInt32(this.ByteArray[30]); //s3
                    int lblphase_s4 = (Convert.ToInt32(this.ByteArray[31]) * 256) + Convert.ToInt32(this.ByteArray[32]); //s4
                    GlobalState.PhaseS3 = lblphase_s3;
                    GlobalState.PhaseS4= lblphase_s4;

                    //lblDate
                    int hour = (Convert.ToInt32(this.ByteArray[63]) * 256) + Convert.ToInt32(this.ByteArray[64]);
                    int minute = (Convert.ToInt32(this.ByteArray[65]) * 256) + Convert.ToInt32(this.ByteArray[66]);
                    int second = (Convert.ToInt32(this.ByteArray[67]) * 256) + Convert.ToInt32(this.ByteArray[68]);
                    int date = (Convert.ToInt32(this.ByteArray[69]) * 256) + Convert.ToInt32(this.ByteArray[70]);
                    int month = (Convert.ToInt32(this.ByteArray[71]) * 256) + Convert.ToInt32(this.ByteArray[72]);
                    int year = (Convert.ToInt32(this.ByteArray[73]) * 256) + Convert.ToInt32(this.ByteArray[74]);
                    lblDate.Text = hour.ToString() + ":" + minute.ToString() + ":" + second.ToString() + "  " + date.ToString() + "/" + month.ToString() + "/" + year.ToString();

                    //lblFree_S1
                    int free_s1 = (Convert.ToInt32(this.ByteArray[175]) * 256) + Convert.ToInt32(this.ByteArray[176]);
                    lblFree_S1.Text = free_s1.ToString();

                    //lblFree_S2
                    int free_s2 = (Convert.ToInt32(this.ByteArray[177]) * 256) + Convert.ToInt32(this.ByteArray[178]);
                    lblFree_S2.Text = free_s2.ToString();

                    //lblFree_S3
                    int free_s3 = (Convert.ToInt32(this.ByteArray[179]) * 256) + Convert.ToInt32(this.ByteArray[180]);
                    lblFree_S3.Text = free_s3.ToString();

                    //lblFree_S4
                    int free_s4 = (Convert.ToInt32(this.ByteArray[181]) * 256) + Convert.ToInt32(this.ByteArray[182]);
                    lblFree_S4.Text = free_s4.ToString();

                    Form4 form4 = new Form4();
                    form4.Show();
                }
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message);
            }
        }
        public static string  SetPortName(string defaultPortName)
        {
            string str = "";
            Console.WriteLine("Avaiable Ports");
            foreach (string portName in SerialPort.GetPortNames())
                str = portName;  //"COM1" by default exist
            if (str == "")
                str = defaultPortName;
            return str;
        }

        public static int SetPortBoudRate(int defaultPortBoudRate)
        {
            string s=defaultPortBoudRate.ToString();
            if (s == "")
                s = defaultPortBoudRate.ToString();
            return int.Parse(s);
        }

        public static Parity SetPortParity(Parity defaultPortParity)
        {
            string str = "";
            Console.WriteLine("Available Parity options:");
            foreach (string name in Enum.GetNames(typeof(Parity)))
                str = name;
            if (str == "")
                defaultPortParity.ToString();
            return (Parity)Enum.Parse(typeof(Parity), defaultPortParity.ToString());
        }

        public static int SetPortDataBits(int defaultPortDataBits)
        {
            string s = "";
            if (s == "")
                s = defaultPortDataBits.ToString();
            return int.Parse(s);
        }

        public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string str = "";
            Console.WriteLine("Available Stop Bits options:");
            foreach (object name in Enum.GetNames(typeof(StopBits)))
                Console.WriteLine("   {0}", name);
            Console.Write("Stop Bits({0}):", (object)defaultPortStopBits.ToString());
            if (Console.ReadLine() == "")
                str = defaultPortStopBits.ToString();
            return (StopBits)Enum.Parse(typeof(StopBits), defaultPortStopBits.ToString());
        }

        public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string str = "";
            Console.WriteLine("Available Handshake options:");
            foreach (string name in Enum.GetNames(typeof(Handshake)))
                str = name; //set value by default 
            if (str == "") // if str equals to empty
                defaultPortHandshake.ToString();
            return (Handshake)Enum.Parse(typeof(Handshake), "None");
        }
       
        //CRC Funtion
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

        //Recharge function
        public string DecodeCode(string strCode)
        {
            //strCode = "3803040659542272";
            string str1 = (string)null;
            string str2 = "KRUTIASDAQ";
            string str3 = "";
            for (int index = 1; index <= 3; ++index)
            {
                int num = 0;
                for (int Start1 = 1; Start1 <= 13; ++Start1)
                {
                    string str4 = Microsoft.VisualBasic.Strings.Mid(strCode, Start1, 1);
                    int Start2 = (Start1 - 1 + index * 2) % Microsoft.VisualBasic.Strings.Len(str2) + 1;
                    string String = Microsoft.VisualBasic.Strings.Mid(str2, Start2, 1);
                    num += Convert.ToInt32(str4) ^ Microsoft.VisualBasic.Strings.Asc(String) & 7;
                }
                str3 += Convert.ToString(num % 10);
            }
            if (Microsoft.VisualBasic.Strings.Mid(str3, 1, 3) == Microsoft.VisualBasic.Strings.Mid(strCode, 14, 3))
            {
                double num1 = Convert.ToDouble(Microsoft.VisualBasic.Strings.Mid(strCode, 1, 13));
                long num2 = Convert.ToInt64(num1 / 16777216.0 - 0.5);
                double num3 = Convert.ToDouble(num2);
                long num4 = Convert.ToInt64(num1 - num3 * 16777216.0);
                strCode = "";
                string Expression1 = "";
                while (num2 > 0L)
                {
                    int num5 = (int)(num2 % 8L);
                    num2 = (num2 - (long)num5) / 8L;
                    Expression1 += Convert.ToString(num5);
                }
                strCode += Microsoft.VisualBasic.Strings.StrReverse(Expression1);
                string Expression2 = "";
                for (int index = 1; index <= 8; ++index)
                {
                    int num6 = (int)(num4 % 8L);
                    num4 = (num4 - (long)num6) / 8L;
                    Expression2 += Convert.ToString(num6);
                }
                strCode = Microsoft.VisualBasic.Strings.Right("00000000000000" + strCode + Microsoft.VisualBasic.Strings.StrReverse(Expression2), 14);
                for (int index = 4; index >= 1; index += -1)
                {
                    string str5 = strCode;
                    strCode = "";
                    for (int Start3 = 1; Start3 <= 14; ++Start3)
                    {
                        string str6 = Microsoft.VisualBasic.Strings.Mid(str5, Start3, 1);
                        int Start4 = (Start3 - 1 + index * 2) % Microsoft.VisualBasic.Strings.Len(str2) + 1;
                        string String = Microsoft.VisualBasic.Strings.Mid(str2, Start4, 1);
                        strCode += Convert.ToString(Convert.ToInt32(str6) ^ Microsoft.VisualBasic.Strings.Asc(String) & 7);
                    }
                }
                str1 = "";
                double num7 = 0.0;
                for (int Start = 1; Start <= 14; ++Start)
                {
                    int int32 = Convert.ToInt32(Microsoft.VisualBasic.Strings.Mid(strCode, Start, 1));
                    num7 = num7 * 8.0 + (double)int32;
                }
                strCode = Microsoft.VisualBasic.Strings.Right("0000000000000" + Convert.ToString(num7), 12);


            }
            else
            {
                int num8 = (int)MessageBox.Show("Checksum Mismatch");
            }
            return strCode;
        }

        //Generate Code 
        public string GenerateCode(string rnd, string serialnumber, string amount)
        {


            string str1 = (string)null;
            string str2 = "KRUTIASDAQ";
            double num1 = Convert.ToDouble(Strings.Right("000" + rnd, 3) + Strings.Right("00000" + serialnumber, 5) + Strings.Right("0000" + amount, 4));
            long num2 = Convert.ToInt64(num1 / 16777216.0 - 0.5);
            double num3 = Convert.ToDouble(num2);
            long num4 = Convert.ToInt64(num1 - num3 * 16777216.0);
            string str3 = "";
            string Expression1 = "";
            while (num2 > 0L)
            {
                int num5 = (int)(num2 % 8L);
                num2 = (num2 - (long)num5) / 8L;
                Expression1 += Convert.ToString(num5);
            }
            string str4 = str3 + Strings.StrReverse(Expression1);
            string Expression2 = "";
            for (int index = 1; index <= 8; ++index)
            {
                int num6 = (int)(num4 % 8L);
                num4 = (num4 - (long)num6) / 8L;
                Expression2 += Convert.ToString(num6);
            }
            string str5 = Strings.Right("00000000000000" + str4 + Strings.StrReverse(Expression2), 14);
            for (int index = 1; index <= 4; ++index)
            {
                string str6 = str5;
                str5 = "";
                for (int Start1 = 1; Start1 <= 14; ++Start1)
                {
                    string str7 = Strings.Mid(str6, Start1, 1);
                    int Start2 = (Start1 - 1 + index * 2) % Strings.Len(str2) + 1;
                    string String = Strings.Mid(str2, Start2, 1);
                    str5 += Convert.ToString(Convert.ToInt32(str7) ^ Strings.Asc(String) & 7);
                }
            }
            str1 = "";
            double num7 = 0.0;
            for (int Start = 1; Start <= 14; ++Start)
            {
                int int32 = Convert.ToInt32(Strings.Mid(str5, Start, 1));
                num7 = num7 * 8.0 + (double)int32;
            }
            string code = Strings.Right("0000000000000" + Convert.ToString(num7), 13);
            string str8 = code;
            for (int index = 1; index <= 3; ++index)
            {
                int num8 = 0;
                for (int Start3 = 1; Start3 <= 13; ++Start3)
                {
                    string str9 = Strings.Mid(str8, Start3, 1);
                    int Start4 = (Start3 - 1 + index * 2) % Strings.Len(str2) + 1;
                    string String = Strings.Mid(str2, Start4, 1);
                    num8 += Convert.ToInt32(str9) ^ Strings.Asc(String) & 7;
                }
                code += Convert.ToString(num8 % 10);
            }
            return code;
        }

        //Decode_Amount
        public string DecodeCode_Amount(string strCode)
        {
           // strCode = "3803040659542272";
            string str1 = (string)null;
            string str2 = "KRUTIASDAQ";
            string str3 = "";
            string str4 = "";
            for (int index = 1; index <= 3; ++index)
            {
                int num = 0;
                for (int Start1 = 1; Start1 <= 13; ++Start1)
                {
                    string str5 = Strings.Mid(strCode, Start1, 1);
                    int Start2 = (Start1 - 1 + index * 2) % Strings.Len(str2) + 1;
                    string String = Strings.Mid(str2, Start2, 1);
                    num += Convert.ToInt32(str5) ^ Strings.Asc(String) & 7;
                }
                str4 += Convert.ToString(num % 10);
            }
            if (Strings.Mid(str4, 1, 3) == Strings.Mid(strCode, 14, 3))
            {
                double num1 = Convert.ToDouble(Strings.Mid(strCode, 1, 13));
                long num2 = Convert.ToInt64(num1 / 16777216.0 - 0.5);
                double num3 = Convert.ToDouble(num2);
                long num4 = Convert.ToInt64(num1 - num3 * 16777216.0);
                strCode = "";
                string Expression1 = "";
                while (num2 > 0L)
                {
                    int num5 = (int)(num2 % 8L);
                    num2 = (num2 - (long)num5) / 8L;
                    Expression1 += Convert.ToString(num5);
                }
                strCode += Strings.StrReverse(Expression1);
                string Expression2 = "";
                for (int index = 1; index <= 8; ++index)
                {
                    int num6 = (int)(num4 % 8L);
                    num4 = (num4 - (long)num6) / 8L;
                    Expression2 += Convert.ToString(num6);
                }
                strCode = Strings.Right("00000000000000" + strCode + Strings.StrReverse(Expression2), 14);
                for (int index = 4; index >= 1; index += -1)
                {
                    string str6 = strCode;
                    strCode = "";
                    for (int Start3 = 1; Start3 <= 14; ++Start3)
                    {
                        string str7 = Strings.Mid(str6, Start3, 1);
                        int Start4 = (Start3 - 1 + index * 2) % Strings.Len(str2) + 1;
                        string String = Strings.Mid(str2, Start4, 1);
                        strCode += Convert.ToString(Convert.ToInt32(str7) ^ Strings.Asc(String) & 7);
                    }
                }
                str1 = "";
                double num7 = 0.0;
                for (int Start = 1; Start <= 14; ++Start)
                {
                    int int32 = Convert.ToInt32(Strings.Mid(strCode, Start, 1));
                    num7 = num7 * 8.0 + (double)int32;
                }
                strCode = Strings.Right("0000000000000" + Convert.ToString(num7), 12);
                str3 = Strings.Mid(strCode, 9, 4);
            }
            else
            {
                int num8 = (int)MessageBox.Show("Checksum Mismatch");
            }
            return str3;
        }

        //Decode_Number
        public string DecodeCode_Number(string strCode)
        {
            //strCode = "3803040659542272";
            string str1 = (string)null;
            string str2 = "KRUTIASDAQ";
            string str3 = "";
            string str4 = "";
            for (int index = 1; index <= 3; ++index)
            {
                int num = 0;
                for (int Start1 = 1; Start1 <= 13; ++Start1)
                {
                    string str5 = Strings.Mid(strCode, Start1, 1);
                    int Start2 = (Start1 - 1 + index * 2) % Strings.Len(str2) + 1;
                    string String = Strings.Mid(str2, Start2, 1);
                    num += Convert.ToInt32(str5) ^ Strings.Asc(String) & 7;
                }
                str4 += Convert.ToString(num % 10);
            }
            if (Strings.Mid(str4, 1, 3) == Strings.Mid(strCode, 14, 3))
            {
                double num1 = Convert.ToDouble(Strings.Mid(strCode, 1, 13));
                long num2 = Convert.ToInt64(num1 / 16777216.0 - 0.5);
                double num3 = Convert.ToDouble(num2);
                long num4 = Convert.ToInt64(num1 - num3 * 16777216.0);
                strCode = "";
                string Expression1 = "";
                while (num2 > 0L)
                {
                    int num5 = (int)(num2 % 8L);
                    num2 = (num2 - (long)num5) / 8L;
                    Expression1 += Convert.ToString(num5);
                }
                strCode += Strings.StrReverse(Expression1);
                string Expression2 = "";
                for (int index = 1; index <= 8; ++index)
                {
                    int num6 = (int)(num4 % 8L);
                    num4 = (num4 - (long)num6) / 8L;
                    Expression2 += Convert.ToString(num6);
                }
                strCode = Strings.Right("00000000000000" + strCode + Strings.StrReverse(Expression2), 14);
                for (int index = 4; index >= 1; index += -1)
                {
                    string str6 = strCode;
                    strCode = "";
                    for (int Start3 = 1; Start3 <= 14; ++Start3)
                    {
                        string str7 = Strings.Mid(str6, Start3, 1);
                        int Start4 = (Start3 - 1 + index * 2) % Strings.Len(str2) + 1;
                        string String = Strings.Mid(str2, Start4, 1);
                        strCode += Convert.ToString(Convert.ToInt32(str7) ^ Strings.Asc(String) & 7);
                    }
                }
                str1 = "";
                double num7 = 0.0;
                for (int Start = 1; Start <= 14; ++Start)
                {
                    int int32 = Convert.ToInt32(Strings.Mid(strCode, Start, 1));
                    num7 = num7 * 8.0 + (double)int32;
                }
                strCode = Strings.Right("0000000000000" + Convert.ToString(num7), 12);
                str3 = Strings.Mid(strCode, 1, 3);
            }
            else
            {
                int num8 = (int)MessageBox.Show("Checksum Mismatch");
            }
            return str3;
        }
        
        //Decode_Serial
        public string DecodeCode_Serial(string strCode)
        {
            //strCode = "3803040659542272";
            string str1 = (string)null;
            string str2 = "KRUTIASDAQ";
            string str3 = "";
            string str4 = "";
            for (int index = 1; index <= 3; ++index)
            {
                int num = 0;
                for (int Start1 = 1; Start1 <= 13; ++Start1)
                {
                    string str5 = Strings.Mid(strCode, Start1, 1);
                    int Start2 = (Start1 - 1 + index * 2) % Strings.Len(str2) + 1;
                    string String = Strings.Mid(str2, Start2, 1);
                    num += Convert.ToInt32(str5) ^ Strings.Asc(String) & 7;
                }
                str4 += Convert.ToString(num % 10);
            }
            if (Strings.Mid(str4, 1, 3) == Strings.Mid(strCode, 14, 3))
            {
                double num1 = Convert.ToDouble(Strings.Mid(strCode, 1, 13));
                long num2 = Convert.ToInt64(num1 / 16777216.0 - 0.5);
                double num3 = Convert.ToDouble(num2);
                long num4 = Convert.ToInt64(num1 - num3 * 16777216.0);
                strCode = "";
                string Expression1 = "";
                while (num2 > 0L)
                {
                    int num5 = (int)(num2 % 8L);
                    num2 = (num2 - (long)num5) / 8L;
                    Expression1 += Convert.ToString(num5);
                }
                strCode += Strings.StrReverse(Expression1);
                string Expression2 = "";
                for (int index = 1; index <= 8; ++index)
                {
                    int num6 = (int)(num4 % 8L);
                    num4 = (num4 - (long)num6) / 8L;
                    Expression2 += Convert.ToString(num6);
                }
                strCode = Strings.Right("00000000000000" + strCode + Strings.StrReverse(Expression2), 14);
                for (int index = 4; index >= 1; index += -1)
                {
                    string str6 = strCode;
                    strCode = "";
                    for (int Start3 = 1; Start3 <= 14; ++Start3)
                    {
                        string str7 = Strings.Mid(str6, Start3, 1);
                        int Start4 = (Start3 - 1 + index * 2) % Strings.Len(str2) + 1;
                        string String = Strings.Mid(str2, Start4, 1);
                        strCode += Convert.ToString(Convert.ToInt32(str7) ^ Strings.Asc(String) & 7);
                    }
                }
                str1 = "";
                double num7 = 0.0;
                for (int Start = 1; Start <= 14; ++Start)
                {
                    int int32 = Convert.ToInt32(Strings.Mid(strCode, Start, 1));
                    num7 = num7 * 8.0 + (double)int32;
                }
                strCode = Strings.Right("0000000000000" + Convert.ToString(num7), 12);
                str3 = Strings.Mid(strCode, 4, 5);
            }
            else
            {
                int num8 = (int)MessageBox.Show("Checksum Mismatch");
            }
            return str3;
        }

        //Comport
        private void ddlComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var port = ddlComPort.SelectedValue as string;

                _serialPort = new SerialPort(port);
                _serialPort.PortName = (_serialPort.PortName);
                _serialPort.BaudRate = (_serialPort.BaudRate);
                _serialPort.Parity = (_serialPort.Parity);
                _serialPort.DataBits = (_serialPort.DataBits);
                _serialPort.StopBits = (_serialPort.StopBits);
                _serialPort.Handshake = (_serialPort.Handshake);
                _serialPort.ReadTimeout = 10500;
                _serialPort.WriteTimeout = 1500;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, nameof(ex.InnerException));
            }
        }

        private void ddlDataBits_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ddlBoudRate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Parity
        private void ddlParity_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel10_Paint(object sender, PaintEventArgs e)
        {

        }
        private void label70_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void label33_Click(object sender, EventArgs e)
        {

        }
        private void label67_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void ComPort_Click(object sender, EventArgs e)
        {

        }

        private void lblAvgCurrent_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblAvgLLVoltage_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblLNVoltage_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblFrequency_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblEnergyTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblActEnergyTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblAppEnergyEB_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblActEnergyEB_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox21_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox23_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {

        }

        //Operation, textbox1
        private void button7_Click(object sender, EventArgs e)
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
            int setValue = Convert.ToInt32(textBox1.Text);
            byte setValueHB = Convert.ToByte(setValue / 256),
                    setValueLB = Convert.ToByte(setValue % 256);

            byte Mid = Convert.ToByte(this.txtMeterID.Text);
            byte[] array1 = { Mid, 6, 0, 2, setValueHB, setValueLB }; //changes  20 sept
            int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

            byte crcHB = Convert.ToByte(crc / 256),
                crcLB = Convert.ToByte(crc % 256);
            byte[] array = { Mid, 6, 0, 2, setValueHB, setValueLB, crcLB, crcHB };// changes 20 sept
            _serialPort.Write(array, 0, array.Length);
        }

        //Mains Rate, textbox2
        private void button8_Click(object sender, EventArgs e)
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
            int setValue = Convert.ToInt32(textBox2.Text);
            byte setValueHB = Convert.ToByte(setValue / 256),
                    setValueLB = Convert.ToByte(setValue % 256);

            byte Mid = Convert.ToByte(this.txtMeterID.Text);
            byte[] array1 = { Mid, 6, 0, 40, setValueHB, setValueLB }; //changes  20 sept
            int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

            byte crcHB = Convert.ToByte(crc / 256),
                crcLB = Convert.ToByte(crc % 256);
            byte[] array = { Mid, 6, 0, 40, setValueHB, setValueLB, crcLB, crcHB };// changes 20 sept
            _serialPort.Write(array, 0, array.Length);
        }
        
        //DG Rate, textbox3
        private void button9_Click(object sender, EventArgs e)
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
            _serialPort.DiscardInBuffer();
            int setValue = Convert.ToInt32(textBox3.Text);
            byte setValueHB = Convert.ToByte(setValue / 256),
                    setValueLB = Convert.ToByte(setValue % 256);

            byte Mid = Convert.ToByte(this.txtMeterID.Text);
            byte[] array1 = { Mid, 6, 0, 41, setValueHB, setValueLB }; //changes  20 sept
            int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

            byte crcHB = Convert.ToByte(crc / 256),
                crcLB = Convert.ToByte(crc % 256);
            byte[] array = { Mid, 6, 0, 41, setValueHB, setValueLB, crcLB, crcHB };// changes 20 sept
            _serialPort.Write(array, 0, array.Length);
        }

        //Deduction
        private void button10_Click(object sender, EventArgs e)
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
            _serialPort.DiscardInBuffer();
            int setValue = Convert.ToInt32(textBox4.Text); 
            byte setValueHB = Convert.ToByte(setValue / 256),
                    setValueLB = Convert.ToByte(setValue % 256);

            byte Mid = Convert.ToByte(this.txtMeterID.Text);
            byte[] array1 = { Mid, 6, 0, 45, setValueHB, setValueLB }; //changes  20 sept
            int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

            byte crcHB = Convert.ToByte(crc / 256),
                crcLB = Convert.ToByte(crc % 256);
            byte[] array = { Mid, 6, 0, 45, setValueHB, setValueLB, crcLB, crcHB };// changes 20 sept
            _serialPort.Write(array, 0, array.Length);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
            _serialPort.DiscardInBuffer();
            int setValue = Convert.ToInt32(textBox5.Text);
            byte setValueHB = Convert.ToByte(setValue / 256),
                    setValueLB = Convert.ToByte(setValue % 256);

            byte Mid = Convert.ToByte(this.txtMeterID.Text);
            byte[] array1 = { Mid, 6, 0, 26, setValueHB, setValueLB }; //changes  20 sept
            int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

            byte crcHB = Convert.ToByte(crc / 256),
                crcLB = Convert.ToByte(crc % 256);
            byte[] array = { Mid, 6, 0, 26, setValueHB, setValueLB, crcLB, crcHB };// changes 20 sept
            _serialPort.Write(array, 0, array.Length);
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
            // _serialPort.DiscardInBuffer();
            int setValue = Convert.ToInt32(textBox7.Text);
            byte setValueHB = Convert.ToByte(setValue / 256),
                    setValueLB = Convert.ToByte(setValue % 256);

            byte Mid = Convert.ToByte(this.txtMeterID.Text);
            byte[] array1 = { Mid, 6, 0, 9, setValueHB, setValueLB }; //changes  20 sept
            int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

            byte crcHB = Convert.ToByte(crc / 256),
                crcLB = Convert.ToByte(crc % 256);
            byte[] array = { Mid, 6, 0, 9, setValueHB, setValueLB, crcLB, crcHB };// changes 20 sept
            _serialPort.Write(array, 0, array.Length);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            string amount = textBox18.Text;
            string serialno = textBox23.Text;
            serialno = "7" + serialno;
            int rnd1 = Convert.ToInt32(textBox22.Text);
            rnd1++;
            string rnd = rnd1.ToString();
            string rcode = GenerateCode(rnd, serialno, amount);
            int rcode1 = 0;
            int rcode2 = 0;
            int rcode3 = 0;
            int rcode4 = 0;
            rcode1 = Convert.ToInt32(rcode.Substring(0, 4));
            rcode2 = Convert.ToInt32(rcode.Substring(4, 4));
            rcode3 = Convert.ToInt32(rcode.Substring(8, 4));
            rcode4 = Convert.ToInt32(rcode.Substring(12, 4));

            byte rcode1HB = Convert.ToByte(rcode1 / 256), rcode1LB = Convert.ToByte(rcode1 % 256);
            byte rcode2HB = Convert.ToByte(rcode2 / 256), rcode2LB = Convert.ToByte(rcode2 % 256);
            byte rcode3HB = Convert.ToByte(rcode3 / 256), rcode3LB = Convert.ToByte(rcode3 % 256);
            byte rcode4HB = Convert.ToByte(rcode4 / 256), rcode4LB = Convert.ToByte(rcode4 % 256);

            if (!_serialPort.IsOpen)
                _serialPort.Open();
            Thread.Sleep(500);
            _serialPort.DiscardInBuffer();  

            byte Mid = Convert.ToByte(this.txtMeterID.Text);
            byte[] array1 = { Mid, 6, 0, 36, rcode1HB, rcode1LB }; //changes  20 sept (36)
            int crc = Convert.ToInt32(ModRTU_CRC(array1, 6));

            byte crcHB = Convert.ToByte(crc / 256),
                crcLB = Convert.ToByte(crc % 256);
            byte[] array = { Mid, 6, 0, 36, rcode1HB, rcode1LB, crcLB, crcHB };
            _serialPort.Write(array, 0, array.Length);
            Thread.Sleep(500);
            _serialPort.DiscardInBuffer();
            //////////////////////////
           
            byte[] array2 = { Mid, 6, 0, 37, rcode2HB, rcode2LB }; //changes  20 sept (37)
            int crc2 = Convert.ToInt32(ModRTU_CRC(array2, 6));

            byte crcHB2 = Convert.ToByte(crc2 / 256),
                crcLB2 = Convert.ToByte(crc2 % 256);
            byte[] array3 = { Mid, 6, 0, 37, rcode2HB, rcode2LB, crcLB2, crcHB2 };
            _serialPort.Write(array3, 0, array3.Length);
            Thread.Sleep(500);
            _serialPort.DiscardInBuffer();
            ////////////////////////////
            
            byte[] array4 = { Mid, 6, 0, 38, rcode3HB, rcode3LB }; //changes  20 sept  (38)
            int crc3 = Convert.ToInt32(ModRTU_CRC(array4, 6));

            byte crcHB3 = Convert.ToByte(crc3 / 256),
                crcLB3 = Convert.ToByte(crc3 % 256);
            byte[] array5 = { Mid, 6, 0, 38, rcode3HB, rcode3LB, crcLB3, crcHB3 };
            _serialPort.Write(array5, 0, array5.Length);
            Thread.Sleep(500);
            _serialPort.DiscardInBuffer();
            ////////////////////////////
            
            byte[] array6 = { Mid, 6, 0, 39, rcode4HB, rcode4LB }; //changes  20 sept (39)
            int crc4 = Convert.ToInt32(ModRTU_CRC(array6, 6));

            byte crcHB4 = Convert.ToByte(crc4 / 256),
                crcLB4 = Convert.ToByte(crc4 % 256);
            byte[] array7 = { Mid, 6, 0, 39, rcode4HB, rcode4LB, crcLB4, crcHB4 };
            _serialPort.Write(array7, 0, array7.Length);
            _serialPort.DiscardInBuffer();
        }//Recharge action button

        private void lblDeviceId_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox18_TextChanged(object sender, EventArgs e) //Recharge textbox
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e) // Deduction textbox
        {

        }

        private void textBox21_TextChanged_1(object sender, EventArgs e)
        {
            
        }
        private void lblClearAll_Click(object sender, EventArgs e)  // Erasing all data
        {
            // Show a confirmation dialog
            DialogResult response = MessageBox.Show("All data will be erased. Do you want to continue?",
                                                    "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (response == DialogResult.OK)
            {
                lblDeviceId.Text = "";
                lblCurrentBalance.Text = "";
                lblLight1.BackColor= Color.Black;
                lblLight2.BackColor= Color.Black;
                lblLight3.BackColor= Color.Black;
                lblLight4.BackColor= Color.Black;
                lblLight5.BackColor= Color.Black;
                lblLight6.BackColor= Color.Black;
                lblOpenDate1.Text = "";
                lblOpenDate2.Text = "";
                lblOpenTime1.Text = "";
                lblOpenTime2.Text = "";
                lblAppEnergyTotal.Text = "";
                lblActEnergyTotal.Text = "";
                lblAppEnergyEB.Text = "";
                lblActEnergyEB.Text = "";
                lblAppEnergyDG.Text = "";
                lblActEnergyDG.Text = "";
                lblAppEnergyWind.Text = "";
                lblActEnergyWind.Text = "";
                lblAppEnergySolar.Text = "";
                lblActEnergySolar.Text = "";
                lblAppPower.Text = "";
                lblActPower.Text = "";
                lbl_M3.Text = "";
                lbl_M4.Text = "";
                lbl_M5.Text = "";
                lbl_M6.Text = "";
                lbl_M7.Text = "";
                lbl_M8.Text = "";
                lbl_M9.Text = "";
                lbl_M10.Text = "";
                lbl_M11.Text = ""; 
                lblDataLogger.Text = "";
                lblComPort.Text = "";
                lblAvgCurrent.Text = "";
                lblAvgLLVoltage.Text = "";
                lblLNVoltage.Text = "";
                lblFrequency.Text = "";
                lblPowerFactor.Text = "";
                lblCurrentDayDeduction.Text = "";
                lblCD_EB.Text = "";
                lblCD_DG.Text = "";
                lblCD_Solar.Text = "";
                lblCD_Wind.Text = "";
                lblCD_Fixed.Text = "";
                lbl224.Text = "";
                lblPD_EB.Text = "";
                lblPD_DG.Text = "";
                lblPD_Solar.Text = "";
                lblPD_Wind.Text = "";
                lblPD_Fixed.Text = "";
                lastRechargeCode.Text="";
                lastRechargeDate.Text = "";
                lastRechargeValue.Text = "";
                lblCurrentMonth.Text = "";
                lblCM_EB.Text = "";
                lblCM_DG.Text = "";
                lblCM_Solar.Text = "";
                lblCM_Wind.Text = "";
                CM_Fixed.Text = "";
                PM_Total.Text = "";
                PM_EB.Text = "";
                PM_DG.Text = "";
                PM_Solar.Text = "";
                PM_Wind.Text = "";
                lbl282.Text = "";
                PPM_Total.Text = "";
                PPM_EB.Text = "";
                PPM_DG.Text = "";
                PPM_Solar.Text = "";
                PPM_Wind.Text = "";
                PPM_Fixed.Text = "";
                lblSerialNo.Text ="";
                lblDate.Text = "";
                lblFree_S1.Text = "";
                lblFree_S2.Text = "";
                lblFree_S3.Text = "";
                lblFree_S4.Text = "";
                lblOperation.Text ="";
                lblPerUnitS1.Text = "";
                lblPerUnitS2.Text = "";
                lblPerUnitS3.Text = "";
                lblPerUnitS4.Text = "";
                lblAutoCut.Text = "";
                lblGracePeriod.Text = "";
                lblMaxDemand.Text = "";
                lblSLS1.Text = "";
                lblSLS2.Text = "";
                lblSLS3.Text = "";
                lblSLS4.Text = "";
                lblRateS1.Text ="";
                lblRateS2.Text = "";
                lblRateS3.Text = "";
                lblRateS4.Text = "";
                lblFPD.Text = "";
                lblLoad.Text = "";
                lblPhaseOnS1S2.Text = "";
                lblPhaseOnS3S4.Text = "";
                MessageBox.Show("All data successfully erased.");
            }
        }

        private void ddlBoudRate_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void label59_Click(object sender, EventArgs e)
        {

        }

        private void lblPerUnitS3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label63_Click(object sender, EventArgs e)
        {

        }

        private void lblPerUnitS4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox22_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}

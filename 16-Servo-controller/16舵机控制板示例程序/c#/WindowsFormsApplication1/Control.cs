using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Windows.Forms.DataVisualization.Charting;
using MotorControl;
using System.Management;//添加引用
using System.Threading;
using UsbLibrary;
using Microsoft.VisualBasic;
using System.Globalization;

namespace MotorControl
{
    public enum Baud
    {
        BAUD_NONE,
        BAUD_2400,
        BAUD_4800,
        BAUD_9600,
        BAUD_19200,
        BAUD_38400,
        BAUD_57600,
        BAUD_115200,
        BAUD_230400,
        BAUD_460800,
        BAUD_921600,
        BAUD_1382400
    };
    public enum USBCmd
    {
        NONE,
        BTKEY,
        UART1,
        UART2,
        UART3,
        UART4,
        UART5
    };
    public enum DATATYPE
    {
        CHIP_DATA,
        MODULE_DATA,
        CHIP_CMD,
        MODULE_CMD
    };
    public enum CMD 
    { Read = 0, SMSpeed = 1, SM = 2, DCSpeed = 3, StepSpeed = 4, Step = 5, Delay = 6, Baund = 7, ID = 8, Action = 9, Record = 10, Stop = 11, IICAddr = 12, CycleCnt = 13, ReadMem = 14 };
    public struct MSG
    {
        public CMD cmd;
        public byte ch;
        public int data;
        public MSG(CMD bytCmd, byte bytCh, int iData)
        {
            cmd = (CMD)bytCmd;
            ch = bytCh;
            data = iData;
        }
    }
        
    public partial class Form1 : Form
    {
        MSG stcMSG;
        public Form1()
        {
            InitializeComponent();
        }
        public byte OnlineBoard = 0;
        public byte byteID = 0;
        private bool bListening = false;
        private bool bClosing = false;
        private DateTime TimeStart = DateTime.Now;
        private Int32 Baund = 9600;
        private string GetLocalText(string ChString, string EnString)
        {
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.Name == "zh-CN")
                return ChString;
            else
                return EnString;
        
        }
        public void RunActionIndex(int Index)
        {
          //  if (button2.Text == "停止") button2_Click(null, null);
            if (Index == 16) { Send(0x0b, 0, 1);}
            else if (Index == 17) { Send(0x0b, 0, 0);}
            else
            {
                Send(0x09, 0, Index);
            }
        }
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("深圳维特智能科技有限公司\r\nWITMOTION HK CO.,LIMITED \r\nhttp://RobotControl.taobao.com\r\nVersion：V6.4", "About");
        }


        private void SetBaudrate(int iBaund)
        {
            try
            {
                spSerialPort.BaudRate = iBaund;                

                byte[] byteChipCMD = new byte[4];
                switch (iBaund)
                {
                    case 2400: byteChipCMD[1] = (byte)Baud.BAUD_2400; break;
                    case 4800: byteChipCMD[1] = (byte)Baud.BAUD_4800; break;
                    case 9600: byteChipCMD[1] = (byte)Baud.BAUD_9600; break;
                    case 19200: byteChipCMD[1] = (byte)Baud.BAUD_19200; break;
                    case 38400: byteChipCMD[1] = (byte)Baud.BAUD_38400; break;
                    case 57600: byteChipCMD[1] = (byte)Baud.BAUD_57600; break;
                    case 115200: byteChipCMD[1] = (byte)Baud.BAUD_115200; break;
                    case 230400: byteChipCMD[1] = (byte)Baud.BAUD_230400; break;
                    case 460800: byteChipCMD[1] = (byte)Baud.BAUD_460800; break;
                    case 921600: byteChipCMD[1] = (byte)Baud.BAUD_921600; break;
                    case 1382400: byteChipCMD[1] = (byte)Baud.BAUD_1382400; break;
                    default: byteChipCMD[1] = (byte)Baud.BAUD_NONE; break;
                }
                byteChipCMD[2] = 0;

                byteChipCMD[0] = (byte)USBCmd.UART1;
                SendUSBMsg((byte)DATATYPE.CHIP_CMD, byteChipCMD, 3);
                Thread.Sleep(100);

                byteChipCMD[0] = (byte)USBCmd.UART2;
                SendUSBMsg((byte)DATATYPE.CHIP_CMD, byteChipCMD, 3);
                Thread.Sleep(100);

                byteChipCMD[0] = (byte)USBCmd.UART3;
                SendUSBMsg((byte)DATATYPE.CHIP_CMD, byteChipCMD, 3);
            }
            catch (Exception err) { }
        }

        private bool PortStop = true;
       
        private void SetPort()
        {
            while (true)
            {
                Thread.Sleep(500);
                if (PortStop) continue;
                if (spSerialPort.IsOpen == false)
                {
                    OnlineBoard &= 0xfe;
                    try
                    {
                        DateTime TimeStart = DateTime.Now;
                        spSerialPort.Open();
                        OnlineBoard |= 0x01;
                        DateTime TimeEnd = DateTime.Now;
                    }
                    catch (Exception err) { }

                }
                else
                    OnlineBoard |= 0x01;
            }
           
        }
        private void timer2_Tick(object sender, EventArgs e)
        {//在线检查
            if (OnlineBoard == 0)
            {
                textBoxBoard.Text = GetLocalText("离线", "Offline");
                textBoxBoard.BackColor = Color.Yellow;
                return;
            }
            if (bCheckID)
            {
                bCheckID = false;
                usOnboardID = 0xffff;
                Send(0x00, 0x12, 0);
            }
            else
            {
                bCheckID = true;
                if (usOnboardID == 0xffff) OfflineCnt++;
                else OfflineCnt = 0;

                if (usOnboardID == 0xffff)
                {
                    textBoxBoard.Text = GetLocalText("离线", "Offline");
                }
                else
                {
                    textBoxBoard.Text = GetLocalText("在线", "Online");
                }
                if (textBoxBoard.Text == GetLocalText("离线", "Offline")) textBoxBoard.BackColor = Color.Yellow;
                else if ((usOnboardID & (0x01 << byteID)) == 0) { textBoxBoard.BackColor = Color.Lime; }
            }
        }
        private void PortSelect(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            string strPortName = sender.ToString();

            try
            {
                if (spSerialPort.PortName != strPortName)
                {
                    if (spSerialPort.IsOpen) spSerialPort.Close();
                    spSerialPort.PortName = strPortName;
                }
                PortStop = false;
                menu.Checked = true;
            }
            catch (Exception ex)
            {
            }
        }
        private void PortClose(object sender, EventArgs e)
        {
            PortStop = true;
            if (spSerialPort.IsOpen)
            {
                bClosing = true;
                while (bListening) Application.DoEvents();
                spSerialPort.Dispose();
                spSerialPort.Close();
            }
        }

        private void RefreshComPort(object sender, EventArgs e)
        {
            toolStripComSet.DropDownItems.Clear();
            foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                toolStripComSet.DropDownItems.Add(portName, null, PortSelect);
                if ((spSerialPort.IsOpen) & (spSerialPort.PortName == portName))
                {
                    ToolStripMenuItem menu = (ToolStripMenuItem)toolStripComSet.DropDownItems[toolStripComSet.DropDownItems.Count - 1];
                    menu.Checked = true;
                }
            }
            toolStripComSet.DropDownItems.Add(new ToolStripSeparator());
            toolStripComSet.DropDownItems.Add("Close", null, PortClose);
        }
        public void Send(byte cmd, byte ch, int data)
        {
            Byte [] byteSend = new byte[5];
            Int16 data16 = (Int16)data;
            byteSend[0] = 0xff;
            byteSend[1] = cmd;
            byteSend[2] = ch;
            byteSend[3] = (Byte)(data16 & 0xff);
            byteSend[4] = (Byte)(data16 >> 8);
            try
            {

                SendUSBMsg((byte)DATATYPE.MODULE_CMD, byteSend, (byte)byteSend.Length);

                if (spSerialPort.IsOpen) spSerialPort.Write(byteSend, 0, 5);
                if (cmd == 0x00) return;
                if (OnlineBoard==0)
                {
                    Status1.Text = GetLocalText("端口未打开！待发送数据：","Port not open, datas to send are:") + byteSend[0].ToString("x") + " "
                                    + byteSend[1].ToString("x") + " " + byteSend[2].ToString("x") + " " + byteSend[3].ToString("x") + " "
                                    + byteSend[4].ToString("x") + " ";
                }
                else
                {
                    Status1.Text = GetLocalText("已发送：","Send:") + byteSend[0].ToString("x") + " "
                                    + byteSend[1].ToString("x") + " " + byteSend[2].ToString("x") + " " + byteSend[3].ToString("x") + " "
                                    + byteSend[4].ToString("x") + " ";
                }
            }
            catch (Exception)
            {}
        }
        public void SendMessage(byte cmd, byte ch, int data)
        {
            stcMSG.cmd = (CMD)(cmd | (byteID<<4));
            stcMSG.ch = ch;
            stcMSG.data = data;
            Send((byte)stcMSG.cmd, stcMSG.ch, stcMSG.data);            
        }

        public ushort usOnboardID = 0xffff;
        
        private bool bCheckID = true;
        private byte OfflineCnt = 0;
        
        string GetOnboardIDStr(UInt16 usID)
        {
            string str="";
            for (int i = 0; i < 15; i++)
                if ((usID & (0x01 << i)) == 0) str += i.ToString() + " ";
            return str;
        }
        delegate void UpdateData(byte[] byteData);//声明一个委托
        byte[] RxBuffer = new byte[1000];
        private void Log(string str)
        {
            Console.WriteLine(DateTime.Now.ToString("mm:ss.fff ") + str);
        }
        delegate void DecodeDataHandler(byte[] byteTemp, ushort usLength);

        void ByteCopy(byte[] byteFrom, byte[] byteTo, ushort usFromIndex, ushort usToIndex, ushort usLength)
        {
            for (int i = 0; i < usLength; i++) byteTo[usToIndex + i] = byteFrom[usFromIndex + i];
        }

        ushort usTotalLength = 5;
        UInt16 usRxCnt = 0; 
        byte[] byteHead = new byte[2] { 0x55, 0xAA };
        bool CheckHead(byte[] byteData, byte[] byteHeadTemp, int byteHeadLength)
        {
            for (byte i = 0; i < byteHeadLength; i++)
            {
                if (byteData[i] != byteHeadTemp[i]) return false;
            }
            return true;
        }
        public void DecodeData(byte[] byteTemp, ushort usLength)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new DecodeDataHandler(DecodeData), new object[] { byteTemp, usLength });
                }
                catch (Exception ex) { }
                return;
            }
            ByteCopy(byteTemp,RxBuffer,  0, usRxCnt, usLength);
            usRxCnt += usLength;
            while (usRxCnt >= usTotalLength)
            {
                if (CheckHead(RxBuffer, new byte[2] { 0xFF,0xf0 }, 2) == false)
                {
                    ByteCopy(RxBuffer, RxBuffer, 1, 0, usRxCnt);
                    usRxCnt--;
                    continue;
                }
                if (RxBuffer[1] == 0xf0)
                {
                    Int16 sID = BitConverter.ToInt16(RxBuffer, 3);
                    if ((sID==0)|((sID&0xff)==0xff)) usOnboardID=0;
                 //   usOnboardID &= (ushort)(~(0x01 <<  BitConverter.ToInt16(byteTemp, 3)));
                }

                ByteCopy(RxBuffer, RxBuffer, (ushort)usTotalLength, 0, (ushort)(usRxCnt - usTotalLength));
                usRxCnt -= usTotalLength;
            }

        }

        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte [] byteTemp = new byte[1000];
            try
            {
                UInt16 usLength = (UInt16)spSerialPort.Read(byteTemp, 0, 500);
                DecodeData(byteTemp, usLength);                
            }
            catch (Exception err) { }

        }

        private Thread PortDetectThread;
        public SteeringControl SteeringMotor;
                
        private void Form1_Load(object sender, EventArgs e)
        {
            SteeringMotor = new SteeringControl(0);
            SteeringMotor.SendMessage = new SteeringControl.SendMSG(SendMessage); 
            SteeringMotor.Show();
            splitContainer1.Panel1.Controls.Add(SteeringMotor);
            SteeringMotor.Location = new Point(splitContainer1.Panel1.Size.Width / 6, splitContainer1.Panel1.Size.Height / 6);
                               
            PortDetectThread = new Thread(new ThreadStart(SetPort)); //也可简写为new Thread(ThreadMethod);                
            PortDetectThread.Start(); //启动线程

            for (int i = 0; i < 32; i++)
                cbMotorNo.Items.Add(i.ToString());
            cbMotorNo.SelectedIndex = 0;
            spSerialPort.PortName = "COM3";
            spSerialPort.BaudRate = 9600;           
            
            PortStop = false;
        }
        void btUse_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {                
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            PortClose(null, null);
            PortDetectThread.Abort();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.M:
                    break;
                case Keys.Return:
                    break;
            }
        }           

        public List<MSG>[] listMSG = new List<MSG>[16];

       
        private int TimeRemain = 0;
        private DateTime T0 = DateTime.Now;
        int iGroupRunCnt = 0;       

        private void 网上商城ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            System.Diagnostics.Process.Start(GetLocalText("http://RobotControl.taobao.com", "https://www.aliexpress.com/store/2029054?spm=2114.12010108.0.0.Vn4gIp"));
        }

        private void 技术论坛ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            System.Diagnostics.Process.Start("http://elecmaster.net");
        }


        int SteeringMotorNum = 32;
        
      
        protected override void OnHandleCreated(EventArgs e)
        {
            try
            {
                base.OnHandleCreated(e);
                usb.RegisterHandle(Handle);
            }
            catch (Exception err) { }
        }
        protected override void WndProc(ref Message m)
        {
            try
            {
                usb.ParseMessages(ref m);
                base.WndProc(ref m);	// pass message on to base form
            }
            catch (Exception err) { }
        }


        private void usb_OnDataRecieved(object sender, UsbLibrary.DataRecievedEventArgs args)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new DataRecievedEventHandler(usb_OnDataRecieved), new object[] { sender, args });
                }
                catch (Exception ex) { }
            }
            else
            {
                byte byteLength = args.data[1];
                switch (args.data[2])
                {
                    case (byte)DATATYPE.CHIP_DATA:
                        break;
                    case (byte)DATATYPE.MODULE_DATA:
                        for (int i = 0; i < byteLength; i++)
                            args.data[i] = args.data[i + 3]; 
                        DecodeData(args.data, byteLength);
                        break;
                }
            }
        }
        private sbyte SendUSBMsg(byte ucType, byte[] byteSend, byte ucLength)
        {
            try
            {
                if (this.usb.SpecifiedDevice != null)
                {
                    byte[] byteUSB = new Byte[0x43];
                    byteUSB[1] = ucLength;
                    byteUSB[2] = ucType;
                    byteSend.CopyTo(byteUSB, 3);
                    this.usb.SpecifiedDevice.SendData(byteUSB);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return 0;
        }
        private void usb_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {
            Status1.Text = "My Device Connected!";
            OnlineBoard |= 0x02;
            SetBaudrate(spSerialPort.BaudRate);

        }

        private void usb_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {
            Status1.Text = "My Device DisConnected!";
            OnlineBoard &= 0xfd;
        }

        private void usb_OnDeviceArrived(object sender, EventArgs e)
        {
            Status1.Text = "Find USB Device!";
        }

        private void usb_OnDeviceRemoved(object sender, EventArgs e)
        {
            Status1.Text = "USB Device Removed!";
        }

        void onDeviceFind(byte byteDeviceNo)
        {
        }

        
        private void button_Click(object sender, EventArgs e)
        {
            int ButtonIndex = 0;
            ButtonIndex = int.Parse((string)((Button)sender).Tag);
            RunActionIndex(ButtonIndex);
            if (ButtonIndex==16) { btnResume.ForeColor = Color.Red; btnStop.ForeColor = Color.DimGray; }
            if (ButtonIndex==17) { btnStop.ForeColor = Color.Red; btnResume.ForeColor = Color.DimGray; }
        }

        private void cbMotorNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SteeringMotor.SetChannel((byte)cbMotorNo.SelectedIndex);
        }
        
      

    }
}

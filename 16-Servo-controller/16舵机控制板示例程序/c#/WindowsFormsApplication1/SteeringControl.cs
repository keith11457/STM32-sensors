using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MotorControl
{
    public partial class SteeringControl : UserControl
    {
        private enum EnumMousePointPosition
        {
            MouseSizeNone = 0, //'无
            MouseSizeRight = 1, //'拉伸右边框
            MouseSizeLeft = 2, //'拉伸左边框
            MouseSizeBottom = 3, //'拉伸下边框
            MouseSizeTop = 4, //'拉伸上边框
            MouseSizeTopLeft = 5, //'拉伸左上角
            MouseSizeTopRight = 6, //'拉伸右上角
            MouseSizeBottomLeft = 7, //'拉伸左下角
            MouseSizeBottomRight = 8, //'拉伸右下角
            MouseDrag = 9 // '鼠标拖动
        }
        const int Band = 5;
        const int MinWidth = 10;
        const int MinHeight = 10;
        private EnumMousePointPosition m_MousePointPosition;
        private Point p, p1;

        private void MyMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            p.X = e.X;
            p.Y = e.Y;

            p1.X = e.X;
            p1.Y = e.Y;
        }

        private void MyMouseLeave(object sender, System.EventArgs e)
        {
            m_MousePointPosition = EnumMousePointPosition.MouseSizeNone;
            this.Cursor = Cursors.Arrow;
        }

        private EnumMousePointPosition MousePointPosition(Size size, System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.X >= 0) | (e.X <= size.Width) | (e.Y >= 0) | (e.Y <= size.Height))
            {                
                return EnumMousePointPosition.MouseDrag;
            }
            else
            { return EnumMousePointPosition.MouseSizeNone; }
        }
        private void MyMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Control lCtrl = (sender as Control).Parent;

            if (e.Button == MouseButtons.Left)
            {
                switch (m_MousePointPosition)
                {
                    case EnumMousePointPosition.MouseDrag:
                        lCtrl.Left = lCtrl.Left + e.X - p.X;
                        lCtrl.Top = lCtrl.Top + e.Y - p.Y;
                        break;
                    case EnumMousePointPosition.MouseSizeBottom:
                        lCtrl.Height = lCtrl.Height + e.Y - p1.Y;
                        p1.X = e.X;
                        p1.Y = e.Y; //'记录光标拖动的当前点
                        break;
                    case EnumMousePointPosition.MouseSizeBottomRight:
                        lCtrl.Width = lCtrl.Width + e.X - p1.X;
                        lCtrl.Height = lCtrl.Height + e.Y - p1.Y;
                        p1.X = e.X;
                        p1.Y = e.Y; //'记录光标拖动的当前点
                        break;
                    case EnumMousePointPosition.MouseSizeRight:
                        lCtrl.Width = lCtrl.Width + e.X - p1.X;
                        //      lCtrl.Height = lCtrl.Height + e.Y - p1.Y;
                        p1.X = e.X;
                        p1.Y = e.Y; //'记录光标拖动的当前点
                        break;
                    case EnumMousePointPosition.MouseSizeTop:
                        lCtrl.Top = lCtrl.Top + (e.Y - p.Y);
                        lCtrl.Height = lCtrl.Height - (e.Y - p.Y);
                        break;
                    case EnumMousePointPosition.MouseSizeLeft:
                        lCtrl.Left = lCtrl.Left + e.X - p.X;
                        lCtrl.Width = lCtrl.Width - (e.X - p.X);
                        break;
                    case EnumMousePointPosition.MouseSizeBottomLeft:
                        lCtrl.Left = lCtrl.Left + e.X - p.X;
                        lCtrl.Width = lCtrl.Width - (e.X - p.X);
                        lCtrl.Height = lCtrl.Height + e.Y - p1.Y;
                        p1.X = e.X;
                        p1.Y = e.Y; //'记录光标拖动的当前点
                        break;
                    case EnumMousePointPosition.MouseSizeTopRight:
                        lCtrl.Top = lCtrl.Top + (e.Y - p.Y);
                        lCtrl.Width = lCtrl.Width + (e.X - p1.X);
                        lCtrl.Height = lCtrl.Height - (e.Y - p.Y);
                        p1.X = e.X;
                        p1.Y = e.Y; //'记录光标拖动的当前点
                        break;
                    case EnumMousePointPosition.MouseSizeTopLeft:
                        lCtrl.Left = lCtrl.Left + e.X - p.X;
                        lCtrl.Top = lCtrl.Top + (e.Y - p.Y);
                        lCtrl.Width = lCtrl.Width - (e.X - p.X);
                        lCtrl.Height = lCtrl.Height - (e.Y - p.Y);
                        break;
                    default:
                        break;
                }
                if (lCtrl.Width < MinWidth) lCtrl.Width = MinWidth;
                if (lCtrl.Height < MinHeight) lCtrl.Height = MinHeight;

            }
            else
            {
                m_MousePointPosition = MousePointPosition(lCtrl.Size, e); //'判断光标的位置状态
                switch (m_MousePointPosition) //'改变光标
                {
                    case EnumMousePointPosition.MouseSizeNone:
                        this.Cursor = Cursors.Arrow;       //'箭头
                        break;
                    case EnumMousePointPosition.MouseDrag:
                        this.Cursor = Cursors.SizeAll;     //'四方向
                        break;
                    case EnumMousePointPosition.MouseSizeBottom:
                        this.Cursor = Cursors.SizeNS;      //'南北
                        break;
                    case EnumMousePointPosition.MouseSizeTop:
                        this.Cursor = Cursors.SizeNS;      //'南北
                        break;
                    case EnumMousePointPosition.MouseSizeLeft:
                        this.Cursor = Cursors.SizeWE;      //'东西
                        break;
                    case EnumMousePointPosition.MouseSizeRight:
                        this.Cursor = Cursors.SizeWE;      //'东西
                        break;
                    case EnumMousePointPosition.MouseSizeBottomLeft:
                        this.Cursor = Cursors.SizeNESW;    //'东北到南西
                        break;
                    case EnumMousePointPosition.MouseSizeBottomRight:
                        this.Cursor = Cursors.SizeNWSE;    //'东南到西北
                        break;
                    case EnumMousePointPosition.MouseSizeTopLeft:
                        this.Cursor = Cursors.SizeNWSE;    //'东南到西北
                        break;
                    case EnumMousePointPosition.MouseSizeTopRight:
                        this.Cursor = Cursors.SizeNESW;    //'东北到南西
                        break;
                    default:
                        break;
                }
            }
        }

        private byte Channel = 0;
        public void SetChannel(byte byteCh)
        {
            Channel = byteCh;
            groupBox1.Text = "M" + byteCh.ToString();
        }
        public SteeringControl(byte byteChannel)
        {
            InitializeComponent();
            SetChannel(byteChannel);
            for (int i = 1; i < 21; i++)
            {
                comboBox1.Items.Add((18 * i).ToString() + "°/s");
            }
            comboBox1.SelectedIndex = 19;
            SetAngle(90);
            iSpeed = comboBox1.SelectedIndex;
            this.groupBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(MyMouseDown);
            this.groupBox1.MouseLeave += new System.EventHandler(MyMouseLeave);
            this.groupBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(MyMouseMove);

        }
        public delegate void SendMSG(byte cmd, byte ch, int data);
        public SendMSG SendMessage;
        public bool bUse = true;
        public double Angle = 0;
        public int iAngleUs = 0;
        public int iSpeed = 0;
        public int iMaxPWM = 2500;
        public int iMinPWM = 500;
        public void SetAngle(double AngleInput)
        {
            Angle = AngleInput;
            iAngleUs = (int)(iMinPWM + Angle / 180.0 * (iMaxPWM - iMinPWM));
            trackBar1.Value = (int)Angle;
            textBox1.Text = Angle.ToString("F0");
        }
        public void SetSpeed(int SpeedInput)
        {
            if ((SpeedInput >= 0) && (SpeedInput < 20))
            {
                iSpeed = SpeedInput;
                comboBox1.SelectedIndex = iSpeed;
            }

        }
        public short GetPWMFromAngle(double AngleInput)
        {
            short sPWM= (short)(iMinPWM + AngleInput / 180.0 * (iMaxPWM - iMinPWM));
            return sPWM;
         }
        public void SetPWMRange(int iMin, int iMax)
        {
            iMaxPWM = iMax;
            iMinPWM = iMin;
        }
        private void Send()
        {
            short sData = 0;
            sData = (short)iAngleUs;
            SendMessage(2, Channel, sData);
        }
        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            SetAngle((double)trackBar1.Value);
            Send();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            try
            {
                SetAngle(double.Parse(textBox1.Text));
                Send();
            }
            catch (Exception err) { ; }
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SetSpeed(comboBox1.SelectedIndex);
                SendMessage(1, Channel, (iSpeed + 1)*2);
            }
            catch (Exception err) { }
        }
        public void SetUse(bool bUseFlag)
        {            
            bUse = bUseFlag;
            trackBar1.Enabled = bUseFlag;
            textBox1.Enabled = bUseFlag;
            comboBox1.Enabled = bUseFlag;
            checkBox1.Checked = bUseFlag;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SetUse(true);
            }
            else
            {
                SetUse(false);
            }
                
        }
    }
}

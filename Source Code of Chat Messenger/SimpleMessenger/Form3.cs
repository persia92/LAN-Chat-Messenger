using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Resources;
using System.Reflection;

namespace SimpleMessenger
{

    /// <summary>
    /// **************************** This Form(3) is CHAT WINDOW************************
    /// *************Every Client in the list has a own chat box***************
    /// ********Normally this chat box is hidden. When User start a chat with someone, than it appers***
    /// </summary>
    


    public partial class Form3 : Form
    {
        private ClientInfo client;
        private int lineNumber;
        private bool ShiftEnter = true;

        SoundPlayer buzzSound = new SoundPlayer(Properties.Resources.BUZZER);
        SoundPlayer MessageSound = new SoundPlayer(@"C:\WINDOWS\Media\chimes.wav");


        public Form3(ClientInfo client)
        {
            this.client = client;
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form3_FormClosing);
            SendMsgBox.KeyDown += new KeyEventHandler(SendMsgBox_KeyDown);
            SendMsgBox.KeyPress += new KeyPressEventHandler(SendMsgBox_KeyPress);

        }



        /// <summary>
        /// Form Loading. Set the Form's name with Buddy's(with whome user is chatting) name. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form3_Load(object sender, EventArgs e)
        {
            this.Text = client.Name;
        }



        /// <summary>
        /// Form Closing...Here Form is not actually closed, if User click Form's close option. it simply Hide. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }



        /// <summary>
        /// Checkbox true/false. Change, how message will send. By pressing Enter or clicking button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SendMsgBox.Focus();
            if (ShiftEnter == true)
                ShiftEnter = false;
            else ShiftEnter = true;
        }



        /// <summary>
        /// if User Send message by Pressing Enter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SendMsgBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (checkBox1.Checked == false) return;
            ShiftEnter = false;
            if (Control.ModifierKeys == Keys.Shift  && e.KeyCode==Keys.Enter)
            {
                ShiftEnter = true;
            }
        }



        /// <summary>
        /// If User Send Message By clicking SEND Button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SendMsgBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (checkBox1.Checked == false) return;
            if (e.KeyChar == 13 && ShiftEnter==false)
            {
                e.Handled = true;
                btnSend.PerformClick();
            }
            //throw new NotImplementedException();
        }




        /// <summary>
        /// When SEND Button is clicked for messageg sending, this block will execute.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (SendMsgBox.Text != "")
            {
                ClientMsg m = new ClientMsg();
                m.Type = (int)ClientMsgType.Msg;
                m.Info = client;
                m.Msg = SendMsgBox.Text;
                string[] temp = SendMsgBox.Lines;
                int y;
                lineNumber = temp.Length;
                for (int i = 0; i < temp.Length; i++)
                {
                    y = temp[i].Length;
                    lineNumber += ((y /20));
                }
                m.LineNumb = lineNumber;
                m.From = Program.app.myInfo.ClientID;
                Program.app.client.L.Send(Program.app.client.serverIP, 12345, m.Serialize());
                UserControl1 myUsercon = new UserControl1(Program.app.myInfo, SendMsgBox.Text, lineNumber);
                flowLayoutPanel1.Controls.Add(myUsercon);
                flowLayoutPanel1.VerticalScroll.Value = flowLayoutPanel1.VerticalScroll.Maximum;
                SendMsgBox.Text = "";
            }
        }
        



        delegate void GET_MSG(ClientInfo info, string m,int line);
        /// <summary>
        /// Here User Get messgae from Buddy and show it.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="m"></param>
        /// <param name="line"></param>
        public void GetMsg(ClientInfo info, string m,int line)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new GET_MSG(GetMsg), new object[3] { info, m, line });
            }
            else
            {
                //this.Show();
                if (this.Visible == true)
                    WindowState = FormWindowState.Normal;
                if(Program.app.client.messgSound==true)
                this.MessageSound.Play();
                UserControl1 myUsercon = new UserControl1(info,m,line);
                flowLayoutPanel1.Controls.Add(myUsercon);
                flowLayoutPanel1.VerticalScroll.Value = flowLayoutPanel1.VerticalScroll.Maximum;
            }
        }


        
        /// <summary>
        /// Chat Window will Vibrate/Shake when user get a BuZZ!!!.
        /// </summary>
        void ShakeMe()
        {
            int X = this.Left;
            int Y = this.Top;

            int position = 0;

            Random RandomClass = new Random();

            for (int i = 0; i <= 25;i++ )
            {
                position = RandomClass.Next(X + 1, X + 15);

                this.Left = position;

                position = RandomClass.Next(Y + 1, Y + 15);
                this.Top = position;

                this.Left = X;
                this.Top = Y;
            }

        }



        delegate void GET_BUZZ(int senderID);
        /// <summary>
        /// Here User get Buzz from Buddy.
        /// </summary>
        /// <param name="senderID"></param>
        public void GetBuzz(int senderID)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new GET_BUZZ(GetBuzz), new object[1] { senderID });
            }
            else
            {
                this.Show();
                this.Activate();
                ShakeMe();
                this.TopMost = true;
                this.SendMsgBox.Select();
                lineNumber = 0;
                string x = "BuZZ!!!!!!!!";
                lineNumber++;
                UserControl1 myUsercon = new UserControl1(client,x, lineNumber);
                flowLayoutPanel1.Controls.Add(myUsercon);
                flowLayoutPanel1.VerticalScroll.Value = flowLayoutPanel1.VerticalScroll.Maximum;
                this.buzzSound.Play();
                lineNumber = 0;
            }
        }



        /// <summary>
        /// When User Give Buzz to abuddy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBuZZ_Click(object sender, EventArgs e)
        {
            ClientMsg m = new ClientMsg();
            m.Type = (int)ClientMsgType.Buzz;
            m.Info = client;
            m.From = Program.app.myInfo.ClientID;
            Program.app.client.L.Send(Program.app.client.serverIP, 12345, m.Serialize());
        }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;


namespace SimpleMessenger
{ 
    /// <summary>
    ///   ****************************This Form(1) is the 1st Window***********************************
    ///           ***********************User log in From here*******************************
    /// </summary>
    

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing); 
        }


        /// <summary>
        /// From Loading...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // temporary work, 
            txtIP.Text = Program.OwnIP;
        }




        /// <summary>
        /// Form Closing...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            this.Close();
        }
        



        /// <summary>
        /// This will Block execute if Client start a server.
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                Program.app.hasOwnServer = true;
                Program.app.myInfo.Name = textBox1.Text;
                Program.app.myInfo.IP = Program.OwnIP;
                Program.app.ServerIP = txtIP.Text;
                //Creating SERVER
                Program.app.server = new MessengerServer(Program.app.myInfo.IP);
                //Server is also a Client. So creating a client.
                Program.app.client = new MessengerClient();
                Program.app.client.ConnectionStatus += new SERVER_CONNECTION_DELIGATE(client_ConnectionStatus);
                Program.app.client.Start("127.0.0.1", textBox1.Text);
            }
            else
                MessageBox.Show("You have to enter your name first.");
        }



        /// <summary>
        /// This will execiute if client only give input the server's ip and join
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && txtIP.Text != "")
            {
                Program.app.myInfo.Name = textBox1.Text;
                Program.app.ServerIP = txtIP.Text;
                Program.app.myInfo.IP = Program.OwnIP;
                // Creating Client...
                Program.app.client = new MessengerClient();
                Program.app.client.ConnectionStatus += new SERVER_CONNECTION_DELIGATE(client_ConnectionStatus);
                Program.app.client.Start(txtIP.Text, Program.app.myInfo.Name);
            }
            else 
                MessageBox.Show("You have to enter your name and server IP and then Join!!!");
        }




        delegate void CONNECTION_STATUS(string ip, bool success);
       /// <summary>
        /// Confirmoing If Client is connected with Server
       /// </summary>
       /// <param name="serverIP"></param>
       /// <param name="success"></param>    
        void client_ConnectionStatus(string serverIP, bool success)
        {    
            if (txtIP.InvokeRequired)
            {
                txtIP.BeginInvoke(new CONNECTION_STATUS(client_ConnectionStatus), new object[2] { serverIP, success });
                return;
            }
            else if (success)
            {
                //It is confirmed that Client is Connected with Server, So Now Showing their own Client Window.
                Form2 f2 = new Form2(this);
                f2.Show();
                // Form1 hideing...
                this.Hide();
            }
            else
            {
                // Client is not Connected with Server, So showing this msg...
                MessageBox.Show("Ooops!!! Connection failed with  " + serverIP);

                //Closing All thread that I started For connecting.
                if (Program.app.hasOwnServer)
                {
                    Program.app.server.Dispose();
                    Program.app.server = null;
                }
                Program.app.client.Dispose();
                Program.app.client = null;
            }
        }
        //Leave
        private void leaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Manual
        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("Project__Chat_Messenger__Report.pdf"))
                Process.Start("Project__Chat_Messenger__Report.pdf");
            else
                MessageBox.Show("File is not found");
        }
        //about
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }
                
    }
}

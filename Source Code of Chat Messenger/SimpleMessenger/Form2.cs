using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SimpleMessenger
{

    /// <summary>
    /// **************************** THIS FORM(2) is CLIENT WINDOW**************************************
    ///             ********This will appear when Connention will be established with SERVER**********
    /// </summary>
     


    public partial class Form2 : Form
    {
        Form1 f1;
        private int originalHight;
        private int StatusNumber = 0; 
        private int[] indexArray = new int[1000];



        public Form2(Form1 f)
        {
            InitializeComponent();
            f1 = f;
            listClients.DisplayMember = "NAME";

            //Giving New Client already existing client list
            foreach (ClientInfo info in Program.app.client.ClientDic.Values)
            {
                if (listClients.Items.Contains(info) == false)
                {
                    listClients.Items.Add(info);
                    Program.app.client.numberoOfMessage[info.ClientID]= 0;
                    Program.app.client.numberoOfMessageString[info.ClientID] ="";
                    Form3 newForm = new Form3(info);
                    Program.app.FormDic.Add(info.ClientID, newForm);
                    newForm.Hide();
                }

            }

            listClients.SelectedIndexChanged += new EventHandler(listClients_SelectedIndexChanged);
            this.FormClosing += new FormClosingEventHandler(Form2_FormClosing);
            Program.app.client.NewList += new SERVER_List(GetList);
            Program.app.client.ClientLeaved += new SERVER_LEAVE(RemoveFromList);
            Program.app.client.NewMsg += new SERVER_MSG(client_NewMsg);
            Program.app.client.NewStatus += new SERVER_STATUS(Client_NewMStatus);
            Program.app.client.NewBuzz += new SERVER_BUZZ(Client_NewBuzz);
            Program.app.client.DisconnectByServer += new SERVER_DISCONNECTED_BY_SERVER(client_DisconnectByServer);
            Program.app.client.L.ServerGone += new ServerNotExist(L_ServerGone);
        }




        /// <summary>
        /// Form2 loading..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form2_Load(object sender, EventArgs e)
        {
            btnHide.Visible = false; // this button lies in expanded area, so it is hiding.
            lblBuddyList.Visible = true; // this button lies in expanded area, so it is hiding.
            richTxtNewsFeed.Visible = false; // this textbox lies in expanded area, so it is hiding.
            originalHight = this.Width;
            this.Text = Program.app.myInfo.Name; // Setting Form's name with CLient Name.
        }





        delegate void ServerIsGone();      
        /// <summary>
        /// If server is not found, client window will be disappeared and show user a message.
        /// </summary>
        void L_ServerGone()
        {
            if (this.InvokeRequired)
                this.Invoke(new ServerIsGone(L_ServerGone), new object[] { });
            else
            {
                Program.app.client.Dispose();
                Program.app.client=null;
                this.Hide();
                f1.Show();
                MessageBox.Show("Server is not Found");
                this.Dispose();
                this.Close();
            }

        }



        delegate void disBySer();
        /// <summary>
        /// if client is disconnected by server.client window will be disappeared and show user a message.
        /// </summary>
        void client_DisconnectByServer()
        {
            if (this.InvokeRequired)
                this.Invoke(new disBySer(client_DisconnectByServer), new object[] { });
            else 
            {
                Program.app.client.Dispose();
                Program.app.client = null;
                f1.Show();
                this.Hide();
                MessageBox.Show("You are Disconnected by the server");
                this.Dispose();
                this.Close();
            }
        }




        delegate void ServerGetMsg(ClientInfo info, string msg, int remoteID,int line);
        /// <summary>
        /// Getting msg From other Clients through Server
        /// </summary>
        /// <param name="info"></param>
        /// <param name="msg"></param>
        /// <param name="remoteID"></param>
        /// <param name="line"></param>
        void client_NewMsg(ClientInfo info, string msg, int remoteID, int line)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ServerGetMsg(client_NewMsg), new object[4] { info, msg, remoteID,line });
            }
            else
            {
                if (Program.app.client.ClientDic.ContainsKey(remoteID))
                {
                    Program.app.FormDic[remoteID].GetMsg(Program.app.client.ClientDic[remoteID], msg,line);
                    if (Program.app.FormDic[remoteID].Visible == false)
                    {
                        ClientInfo tempinfo = new ClientInfo();
                        tempinfo = Program.app.client.ClientDic[remoteID];
                        int x = listClients.Items.IndexOf(tempinfo);
                        Program.app.client.numberoOfMessage[tempinfo.ClientID]++;
                        Program.app.client.numberoOfMessageString[tempinfo.ClientID] = "(" + Program.app.client.numberoOfMessage[tempinfo.ClientID].ToString() + ")";
                        listClients.Items.RemoveAt(x);
                        listClients.Items.Add(tempinfo);
                        listClients.BackColor = Color.Pink;
                    }
                }
            }
            
         //throw new NotImplementedException();
        }





        /// <summary>
        ///  When a name will select from the client List, a chat box for that client will be appeared.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listClients.SelectedIndex!=-1)
            {
                ClientInfo c = (ClientInfo)listClients.SelectedItem;
                if (c == null) return;
                ShowChatWindow(c);
                if (Program.app.client.numberoOfMessageString[c.ClientID] != "")
                {
                    listClients.ClearSelected();
                    listClients.BackColor = Color.MediumOrchid;
                    int x = listClients.Items.IndexOf(c);
                    Program.app.client.numberoOfMessage[c.ClientID] = 0;
                    Program.app.client.numberoOfMessageString[c.ClientID] = "";
                    listClients.Items.RemoveAt(x);
                    listClients.Items.Add(c);
                }
            }       
        }

     


       
        /// <summary>
        /// chat window show.
        /// </summary>
        /// <param name="c"></param>
        private void ShowChatWindow(ClientInfo c)
        {
            if(Program.app.FormDic[c.ClientID].Visible==false)
            Program.app.FormDic[c.ClientID].Visible=true;
            Program.app.FormDic[c.ClientID].Activate();
        }


        
        /// <summary>
        /// Form closing event...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.app.hasOwnServer)
            {
                Program.app.server.Dispose();
                Program.app.server = null;
            }
            Program.app.client.Dispose();
            Program.app.client = null;
            f1.Dispose();
            f1.Close();
            this.Dispose();
            this.Close();
        }
       




    
        delegate void ServerList(List<ClientInfo> l);
        /// <summary>
        /// When a new client is signed in. Server sent their info to other clients. here already existing clients 
        /// are getting newly updated client list from server
        /// </summary>
        /// <param name="l"></param>
        void GetList(List<ClientInfo> l)
        {
            if (listClients.InvokeRequired)
            {
                listClients.BeginInvoke(new ServerList(GetList), new object[1] { l });
                return;
            }
            else
            {
                foreach(ClientInfo info in l)
                {
                    if (listClients.Items.Contains(info)==false)
                    {
                        listClients.Items.Add(info);
                        Program.app.client.numberoOfMessage[info.ClientID] = 0;
                        Program.app.client.numberoOfMessageString[info.ClientID] = "";
                        Form3 newForm = new Form3(info);
                        Program.app.FormDic.Add(info.ClientID, newForm);
                        newForm.Hide();
                    }
                   
                }
            }
        }





   
        delegate void ServerLeaved(ClientInfo info);
        /// <summary>
        /// if someone leaved from messenger, other clients will remove him/her from their list. 
        /// </summary>
        /// <param name="info"></param>
        void RemoveFromList(ClientInfo info)
        {
            if (listClients.InvokeRequired)
            {
                listClients.Invoke(new ServerLeaved(RemoveFromList), new object[1] { info });
            }
            else
            {
                listClients.Items.Remove(info);
                listClients.Refresh();
                Program.app.FormDic.Remove(info.ClientID);
            }
        }




       
        /// <summary>
        /// Form2(Client window) is expendable. By clicking this button window will expand.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewsFeed_Click(object sender, EventArgs e)
        {
            if (this.Width == originalHight)
            {
                this.Width = originalHight + 288;
                btnHide.Visible = true;
                lblBuddyList.Visible = true;
                richTxtNewsFeed.Visible = true;
                btnNewsFeed.Font = new Font(btnNewsFeed.Font, FontStyle.Regular);
                btnNewsFeed.BackColor = Color.White;
                btnNewsFeed.Text = "News Feed";
                StatusNumber = 0;
            }
        }



        /// <summary>
        /// As Form2(Client window) is expendable. By clicking this button window will hide the expanded area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHide_Click(object sender, EventArgs e)
        {
            richTxtNewsFeed.Visible = false;
            btnHide.Visible = false;
            lblBuddyList.Visible = false;
            this.Width = originalHight;
        }




       
        /// <summary>
        /// setting font for writing Status/public message in Testbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtStatus_TextChanged(object sender, EventArgs e)
        {
            txtStatus.Font = new Font(txtStatus.Font, FontStyle.Regular);
            txtStatus.ForeColor = Color.Black;
        }




       
        /// <summary>
        /// This button send Public Message/Status to all other clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendStatus_Click(object sender, EventArgs e)
        {
            ClientMsg msg = new ClientMsg();
            msg.Type = (int)ClientMsgType.Status;
            msg.Status = txtStatus.Text;
            msg.Info.ClientID = Program.app.myInfo.ClientID;
            msg.Info.Name = Program.app.myInfo.Name;
            Program.app.client.L.Send(Program.app.ServerIP,12345,msg.Serialize());
            txtStatus.Font = new Font(txtStatus.Font, FontStyle.Italic);
            txtStatus.ForeColor = Color.LightGray;
            txtStatus.Text = "";
        }




        delegate void ServerStatus(ClientInfo info, string sts, ClientMsgType type);
        /// <summary>
        /// When Other clients Update their status/Public Message, NewsFeed or Buddy news textbox will be added that news
        /// </summary>
        /// <param name="info"></param>
        /// <param name="sts"></param>
        void Client_NewMStatus(ClientInfo info, string sts, ClientMsgType type)
        {
            if (richTxtNewsFeed.InvokeRequired)
            {
                richTxtNewsFeed.BeginInvoke(new ServerStatus(Client_NewMStatus), new object[3] { info, sts, type });
            }
            else 
            {

                Font font1 = new Font( richTxtNewsFeed.Font, FontStyle.Bold);

                richTxtNewsFeed.SelectionFont = font1;
                richTxtNewsFeed.SelectionColor = Color.Red;
                if(info.Name!=Program.app.myInfo.Name)
                richTxtNewsFeed.SelectedText = Environment.NewLine + info.Name+":";
                else richTxtNewsFeed.SelectedText = Environment.NewLine + "You" + ":";
                
                if (type == ClientMsgType.Status)
                {
                    Font font2 = new Font(richTxtNewsFeed.Font, FontStyle.Italic);
                    richTxtNewsFeed.SelectionFont = font2;
                    richTxtNewsFeed.SelectionColor = Color.DimGray;
                    richTxtNewsFeed.SelectedText = " Updated status at " + DateTime.Now.ToString("HH:mm:ss");
                    Font font3 = new Font(richTxtNewsFeed.Font, FontStyle.Regular);
                    richTxtNewsFeed.SelectionFont = font3;
                    richTxtNewsFeed.SelectionColor = Color.Black;
                    richTxtNewsFeed.SelectedText = Environment.NewLine + sts + Environment.NewLine;
                }
                else if (type == ClientMsgType.Join)
                {
                    Font font2 = new Font(richTxtNewsFeed.Font, FontStyle.Italic);
                    richTxtNewsFeed.SelectionFont = font2;
                    richTxtNewsFeed.SelectionColor = Color.DimGray;
                    richTxtNewsFeed.SelectedText = " Joined at " + DateTime.Now.ToString("HH:mm:ss");
                    Font font3 = new Font(richTxtNewsFeed.Font, FontStyle.Regular);
                    richTxtNewsFeed.SelectionFont = font3;
                    richTxtNewsFeed.SelectionColor = Color.Black;
                    richTxtNewsFeed.SelectedText = Environment.NewLine + Environment.NewLine;
                }
                else if (type == ClientMsgType.Disconnect)
                {
                    Font font2 = new Font(richTxtNewsFeed.Font, FontStyle.Italic);
                    richTxtNewsFeed.SelectionFont = font2;
                    richTxtNewsFeed.SelectionColor = Color.DimGray;
                    richTxtNewsFeed.SelectedText = " Leaved at " + DateTime.Now.ToString("HH:mm:ss");
                    Font font3 = new Font(richTxtNewsFeed.Font, FontStyle.Regular);
                    richTxtNewsFeed.SelectionFont = font3;
                    richTxtNewsFeed.SelectionColor = Color.Black;
                    richTxtNewsFeed.SelectedText = Environment.NewLine + Environment.NewLine;
                }
                if (info.ClientID != Program.app.myInfo.ClientID && this.Width==originalHight)
                {
                    Font font2 = new Font(richTxtNewsFeed.Font, FontStyle.Italic);
                    richTxtNewsFeed.SelectionFont = font2;
                    richTxtNewsFeed.SelectionColor = Color.DimGray;
                    StatusNumber++;
                    btnNewsFeed.Font = new Font(btnNewsFeed.Font, FontStyle.Bold);
                    btnNewsFeed.BackColor = Color.Yellow;
                    btnNewsFeed.Text = "NewsFeed(" + StatusNumber + ")";
                }
            }
        }





       
        delegate void ServerBuzz(int senderID);
        /// <summary>
        /// When a client get Buzz from other, than this news will make change in Form3(Chat box).
        /// </summary>
        /// <param name="senderID"></param>
        void Client_NewBuzz(int senderID)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ServerBuzz(Client_NewBuzz), new object[1] { senderID });
            }
            else
            {
                if (Program.app.client.ClientDic.ContainsKey(senderID))
                {
                    Program.app.FormDic[senderID].GetBuzz(senderID);
                    if (Program.app.client.numberoOfMessageString[senderID] != "")
                    {
                        listClients.BackColor = Color.MediumOrchid;
                        int x = listClients.Items.IndexOf(Program.app.client.ClientDic[senderID]);
                        Program.app.client.numberoOfMessage[senderID] = 0;
                        Program.app.client.numberoOfMessageString[senderID] = "";
                        listClients.Items.RemoveAt(x);
                        listClients.Items.Add(Program.app.client.ClientDic[senderID]);
                    }

                }
            }

        }




        
        /// <summary>
        /// This is the menu at the top of Client Window. If client  click Leave option, there will send a leave
        /// message to server and close all window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leaveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ClientMsg m = new ClientMsg();
            m.Type = (int)ClientMsgType.Disconnect;
            m.Info.ClientID = Program.app.myInfo.ClientID;
            m.Info.Name = Program.app.myInfo.Name;
            m.From = Program.app.myInfo.ClientID;
            Program.app.client.L.Send(Program.app.ServerIP, 12345, m.Serialize());
            if (Program.app.hasOwnServer)
            {
                Program.app.server.Dispose();
                Program.app.server = null;
            }
            Program.app.client.Dispose();
            Program.app.client = null;
            f1.Show();
            this.Dispose();
            this.Close();
        }




       /// <summary>
        /// This is the menu at the top of Client Window. Client can ON the Message tone,
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void oNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.app.client.messgSound = true;
        }




        /// <summary>
        /// This is the menu at the top of Client Window. Client can OFF the Message tone,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void oFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.app.client.messgSound = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists("Project__Chat_Messenger__Report.pdf"))
                Process.Start("Project__Chat_Messenger__Report.pdf");
            else
                MessageBox.Show("File is not found");
        }
    }
}

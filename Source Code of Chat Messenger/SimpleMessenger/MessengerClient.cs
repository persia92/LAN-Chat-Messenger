using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Drawing;
using System.IO;

namespace SimpleMessenger
{
    public delegate void SERVER_CONNECTION_DELIGATE(string serverIP, bool success);
    public delegate void SERVER_JOIN(ClientInfo info);
    public delegate void SERVER_LEAVE(ClientInfo info);
    public delegate void SERVER_MSG(ClientInfo info, string msg, int remoteID, int line);
    public delegate void SERVER_STATUS(ClientInfo info, string sts, ClientMsgType type);
    public delegate void SERVER_List(List<ClientInfo> l);
    public delegate void SERVER_BUZZ(int senderID);
    public delegate void SERVER_DISCONNECTED_BY_SERVER();

    /// <summary>
    ///  ************************** This Class is  For CLIENT****************************
    ///  ****Here Strat client's SocketListening thread, Message Analysis happens,and hold other neccesary info for client*********.
    /// </summary>


    public class MessengerClient
    {
        
        /// <summary>
        /// Inroducing custom Events
        /// </summary>
        /// 
        public event SERVER_CONNECTION_DELIGATE ConnectionStatus;
        public event SERVER_LEAVE ClientLeaved;
        public event SERVER_MSG NewMsg;
        public event SERVER_STATUS NewStatus;
        public event SERVER_List NewList;
        public event SERVER_BUZZ NewBuzz;
        public event SERVER_DISCONNECTED_BY_SERVER DisconnectByServer;
        



        private Dictionary<int, ClientInfo> clientDic;
        public int[] numberoOfMessage = new int[1000];
        public string[] numberoOfMessageString = new string[1000];
        public bool messgSound = true;
        private ClientInfo me;
        private SocketListener l;
        public string serverIP;
        public string ownIP;
        Timer timer = new Timer(3000);
        Timer timerForAlive = new Timer(3000);




        /// <summary>
        /// Sharing Dictionary instance with other class
        /// </summary>
        public Dictionary<int, ClientInfo> ClientDic
        {
            get { return clientDic; }
        }

      
        /// <summary>
        /// Shareing SocketListener instance with other class.
        /// </summary>
        public SocketListener L
        {
            get { return l; }
        }


        /// <summary>
        /// Defalt Constructor.
        /// </summary>
        public MessengerClient()
        {

        }

        
        /// <summary>
        /// Start listening in a port, send joining message to server,
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="name"></param>
        public void Start(string serverIP,string name)
        {
            this.serverIP = serverIP;
            me = new ClientInfo();
            me.IP = Program.OwnIP;
            me.Name = name;
            
            clientDic = new Dictionary<int, ClientInfo>();
            l = new SocketListener(0, gotClientMsg);
            Program.app.myInfo.ListenPort = l.Port;
            me.ListenPort = l.Port;
            // Sending joining message to server 
            ClientMsg msg = new ClientMsg();
            msg.Info = me;
            msg.Type = (int)ClientMsgType.Join;
            l.Send(serverIP,12345,msg.Serialize());
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
            timerForAlive.Elapsed += new ElapsedEventHandler(timerForAlive_Elapsed);
            timerForAlive.Start();
        }




        /// <summary>
        /// It will send a alive message within 3 seconds.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerForAlive_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerForAlive.Stop();
            //throw new NotImplementedException();
            ClientMsg m = new ClientMsg();
            m.Type = (int)ClientMsgType.Alive;
            m.Info = Program.app.myInfo;
            l.Send(Program.app.ServerIP,12345,m.Serialize());
            timerForAlive.Start();
        }



        /// <summary>
        /// if client become unable to connect with server within 3 seconds.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ConnectionStatus(serverIP, false);
            timer.Stop();
        }



        /// <summary>
        /// When Client get any message. Here Message type will be checked. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataAvailable"></param>
        private void gotClientMsg(byte[] data, int dataAvailable)
        {

            ClientMsg msg = ClientMsg.DeSerialize(data, 0, dataAvailable);

            //Checking Which Type of Message Client got.
            switch ((ClientMsgType)msg.Type)
            {

                case ClientMsgType.ClientList:

                    timer.Stop();
                    Program.app.myInfo.ClientID = msg.Info.ClientID;        
                    foreach (ClientInfo info in msg.CurrentClients)
                    {
                        if (clientDic.ContainsKey(info.ClientID) == false)
                            clientDic.Add(info.ClientID, info);
                    }
                    if (ConnectionStatus!=null)
                    ConnectionStatus(serverIP, true);
                    break;



                case ClientMsgType.clientListForALL:

                    msg.CurrentClients.Add(msg.Info);
                    foreach (ClientInfo info in msg.CurrentClients)
                    {
                        if (clientDic.ContainsKey(info.ClientID) == false)
                            clientDic.Add(info.ClientID, info);
                    }
                    if(NewList!=null)
                    NewList(msg.CurrentClients);
                    if (NewStatus != null)
                        NewStatus(msg.Info, msg.Status, ClientMsgType.Join);
                    break;
               


                case ClientMsgType.Msg:

                    if(NewMsg!=null) 
                        NewMsg(msg.Info, msg.Msg, msg.From,msg.LineNumb);
                    break;



                case ClientMsgType.Disconnect:
                    if (NewStatus != null)
                        NewStatus(msg.Info, msg.Status, ClientMsgType.Disconnect);
                    if (clientDic.ContainsKey(msg.Info.ClientID) == true)
                        clientDic.Remove(msg.Info.ClientID);
                    if (ClientLeaved!=null)
                    ClientLeaved(msg.Info);
                    break;

                case ClientMsgType.Status:

                    if(NewStatus!=null)
                        NewStatus(msg.Info,msg.Status, ClientMsgType.Status);
                    break;

                case ClientMsgType.Buzz:

                    if (NewBuzz != null)
                        NewBuzz(msg.From);
                    break;

                case ClientMsgType.disconnectedByServer:
                    if (DisconnectByServer!=null)
                    DisconnectByServer();
                    break;
            }
        }



        /// <summary>
        /// This will stop all client thread and dispose.
        /// </summary>
        public void Dispose()
        {
            l.RunServer = false;

            foreach (Form3 f in Program.app.FormDic.Values)
            {
                f.Close();
                f.Dispose();

            }
            Program.app.FormDic.Clear();
            Program.app.client.clientDic.Clear();

        }


    }
}



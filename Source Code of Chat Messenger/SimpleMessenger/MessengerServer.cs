using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SimpleMessenger
{
    /// <summary>
    ///   ****************************** This Class is  For SERVER****************************
    ///     ****Here Strat Server's SocketListening thread, Message Analysis happens, and hold other neccesary info for Server*********
    /// </summary>
    /// 

    public class MessengerServer
    {
        int lastClientID = 0;
        Dictionary<int, ClientInfo> myList;
        Dictionary<int, ClientInfo> AliveList;
        SocketListener listener;
        string ownIP;
        Timer serverTimer = new Timer(6000);


        /// <summary>
        /// Defalt Constructor.
        /// </summary>
        /// <param name="ownIP"></param>
        public MessengerServer(string ownIP)
        {
            this.ownIP = ownIP;
            myList = new Dictionary<int, ClientInfo>();
            AliveList=new Dictionary<int ,ClientInfo>();
            listener = new SocketListener(12345,msgFromClient);
            serverTimer.Elapsed += new ElapsedEventHandler(serverTimer_Elapsed);
            serverTimer.Start();
        }




        /// <summary>
        /// Server Checks Within 6 seconds weather every Clients send alive message or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void serverTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            serverTimer.Stop();
            DateTime now = DateTime.Now;
            TimeSpan diff;
            string res;

            lock (Program.lockObject)
            {

                foreach (ClientInfo info in AliveList.Values)
                {
                    diff = now - info.lastAlivemsg;
                    res = diff.Seconds.ToString();
                    int x = int.Parse(res);
                    if (x >= 6)
                    {
                        lock (Program.lockObject2)
                        {
                            myList.Remove(info.ClientID);
                            ClientMsg M = new ClientMsg();
                            M.Type = (int)ClientMsgType.Disconnect;
                            M.Info = info;
                            byte[] data3 = M.Serialize();
                            foreach (ClientInfo c in myList.Values)
                            {
                                listener.Send(c.IP, c.ListenPort, data3);
                            }
                            ClientMsg m = new ClientMsg();
                            m.Type = (int)ClientMsgType.disconnectedByServer;
                            listener.Send(info.IP, info.ListenPort, m.Serialize());
                        }
                    }
                    serverTimer.Start();
                }
                //throw new NotImplementedException();
            }
        }




        /// <summary>
        /// Checking Which Type of message Server got for further Processing  
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataAvailable"></param>
        private void msgFromClient(byte[] data,int dataAvailable)
        {
            ClientMsg msg = ClientMsg.DeSerialize(data,0,dataAvailable);

            switch ((ClientMsgType)msg.Type)
            { 

                case ClientMsgType.Join:
 
                    ClientMsg m = new ClientMsg(); // this m will sent to new comer.
                    m.Type = (int) ClientMsgType.ClientList;
                    m.Info.ClientID = ++lastClientID;

                    msg.Info.ClientID = m.Info.ClientID; // this msg will sent to other clients for notify about new comer.
                    msg.Type =(int)ClientMsgType.clientListForALL;
                    byte[] data2 = msg.Serialize();
                    lock (Program.lockObject2)
                    {
                        foreach (ClientInfo c in myList.Values)
                        {
                            m.CurrentClients.Add(c);
                            // Send all clients the join msg
                            listener.Send(c.IP, c.ListenPort, data2);
                        }

                        // Sending new client server's client list
                        listener.Send(msg.Info.IP, msg.Info.ListenPort, m.Serialize());
                        //// Add this client to server's own client list
                        myList.Add(msg.Info.ClientID, msg.Info);
                    }
                    break;



                case ClientMsgType.Msg:

                    listener.Send(msg.Info.IP, msg.Info.ListenPort,data);
                    break;

  
                case ClientMsgType.Disconnect:
                    lock (Program.lockObject2)
                    {
                        myList.Remove(msg.Info.ClientID);
                        ClientMsg M = new ClientMsg();
                        M.Type = (int)ClientMsgType.Disconnect;
                        M.Info = msg.Info;
                        byte[] data3 = M.Serialize();
                        foreach (ClientInfo c in myList.Values)
                        {
                            listener.Send(c.IP, c.ListenPort, data3);
                        }
                    }
                    if (Program.app.myInfo.ClientID == msg.Info.ClientID)
                    {
                        this.Dispose();
                    }
                    
                    break;


                case ClientMsgType.Status:

                    foreach (ClientInfo c in myList.Values)
                    {
                        listener.Send(c.IP, c.ListenPort, msg.Serialize());
                    }
                    break;



                case ClientMsgType.Buzz:

                    listener.Send(msg.Info.IP, msg.Info.ListenPort,data);
                    break;

                case ClientMsgType.Alive:

                    msg.Info.lastAlivemsg = DateTime.Now;

                    lock(Program.lockObject)
                    {
                       // AliveList[msg.Info.ClientID] = new DateTime();                  
                        AliveList[msg.Info.ClientID] = msg.Info;
                    }
                    break;

              }

        }



        /// <summary>
        /// Stopping All threads and dispose.
        /// </summary>
        public void Dispose()
        {
            listener.RunServer = false;
            Program.app.server.myList.Clear();
        }
    }
}

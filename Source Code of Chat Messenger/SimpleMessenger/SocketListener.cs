using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Timers;

namespace SimpleMessenger
{
    
    public delegate void SocketListenerMsg(byte[] data,int dataAvailable);
    public delegate void ServerNotExist();

    /// <summary>
    ///  ***************************** This class is Socket Listening ********************************
    ///  *********it Continuously listen, weather any type of Message is arrived or not***********
    ///  *******This class also sends any message to others with the help of IP nad port.****
    /// </summary>

    public class SocketListener
    {

        //Introducing a custom event
        public event ServerNotExist ServerGone;


        public int Port; 
        SocketListenerMsg _handler;
        Thread listenerThread = null;
        bool serverRunning = true;
        TcpListener l;

        
        public SocketListener(int port, SocketListenerMsg handler)
        {
            _handler = handler;
            Port = port;


            l = new TcpListener(IPAddress.Any, Port);
            l.Start();
            Port = ((IPEndPoint)l.LocalEndpoint).Port;

            listenerThread = new Thread(new ThreadStart(serverThread));
            listenerThread.Start();
        }


        /// <summary>
        /// Creat a New thread for Continuously listening in a port. 
        /// </summary>
        void serverThread()
        {
            int dataAvailable = 0;
            string msg = "";
            string remoteIP="";
            while (serverRunning)
            {
                
                    if (l.Pending())
                    {
                        TcpClient c = l.AcceptTcpClient();

                        // read data if available
                        if (c.Available > 0)
                        {
                            NetworkStream ns = c.GetStream();
                            if (ns.CanRead)
                            {
                                byte[] data = new byte[1024 * 8];
                                dataAvailable = ns.Read(data, 0, data.Length);
                                msg = Encoding.ASCII.GetString(data, 0, dataAvailable);
                                remoteIP = ((IPEndPoint)c.Client.RemoteEndPoint).ToString();
                                // call delegate
                                _handler(data, dataAvailable);
                                
                            }
                        }
                        // close socket
                        c.Close();

                    }
                    else Thread.Sleep(10);
            }
            
            l.Stop();
            
        }



        /// <summary>
        /// Passing information about weather Socket listening or not, to other class.
        /// </summary>
        public bool RunServer
        {
            get
            {
                return serverRunning;
            }
            set
            {
                if (serverRunning != value)
                {
                    serverRunning = value;
                }
            }
        }
        


        /// <summary>
        /// Sending message in specific ip and port.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="data"></param>
        /// <param name="broadCast"></param>
        public void Send(string ip, int port, byte[] data,bool broadCast=false)
        {
            Thread s = new Thread(new ParameterizedThreadStart(sendingThread));
            s.Start(new object[4] { ip, port, data, broadCast });      
        }



        /// <summary>
        /// Start a thread for send meaage, AS it can take time.
        /// </summary>
        /// <param name="param"></param>
        void sendingThread(object param)
        {
            object[] p=(object[]) param;
            string ip = (string)p[0];
            int port = (int)p[1];
            byte[] data = (byte[])p[2];
            //bool broadCast = (bool)p[3];
            try
            {
               
                TcpClient c = new TcpClient(ip,port);
                c.GetStream().Write(data, 0, data.Length);
                c.Close();
           }
            catch
            {
                if (RunServer == false)
                    return;
                if (Program.app.hasOwnServer == false)
                {
                    if (ServerGone != null)
                        ServerGone();
                    Thread.Sleep(300);
                }
            }
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SimpleMessenger
{
    /// <summary>
    /// *******************************Some Common Resource is in this class********************
    /// </summary>
    public class ApplicationData
    {
        public Dictionary<int, Form3> FormDic;

        public string ServerIP;

        public Image Userimage;

        // Storing my info
        public ClientInfo myInfo=new ClientInfo();

        public bool hasOwnServer = false;

        public MessengerClient client;

        public MessengerServer server;

        public SocketListener l;


    }
}

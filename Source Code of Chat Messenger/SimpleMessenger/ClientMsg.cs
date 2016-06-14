using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Imaging;


namespace SimpleMessenger
{
    /// <summary>
    /// **************************************This Class define Message Protocol*****************************
    /// </summary>

    [Serializable]
    public enum ClientMsgType
    { 
        Msg,
        Join,
        Connect,
        Disconnect,
        Alive,
        ClientList,
        clientListForALL,
        Buzz,
        disconnectedByServer,
        Client_Image,
        Status
    }

    [Serializable]
    public class ClientMsg
    {
        public int Type;
        public int Port;
        public string Msg;
        public string Status;
        public string To;
        public int LineNumb;
        public List<ClientInfo> CurrentClients;
        public string SourceIP;
        public ClientInfo Info;
        public int From;
        public byte[] ProfilePic; 

     

        /// <summary>
        /// Defalt constructor.
        /// </summary>
        public ClientMsg()
        {
            CurrentClients = new List<ClientInfo>();
            ProfilePic=new byte[1];
            Info = new ClientInfo();
            Msg = "";
            SourceIP = "";
            To = "";
        }



        /// <summary>
        /// Serializing Object.
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {
            XmlSerializer x = new XmlSerializer(this.GetType());
            MemoryStream ms=new MemoryStream();
            x.Serialize(ms, this);
            return ms.GetBuffer();
        }


        /// <summary>
        /// DeSerialize Object
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public static ClientMsg DeSerialize(string rawString)
        {
            XmlSerializer x = new XmlSerializer(typeof(ClientMsg));
            MemoryStream ms = new MemoryStream();
            byte[] data=Encoding.ASCII.GetBytes(rawString);
            ms.Write(data, 0, data.Length);
            return (ClientMsg) x.Deserialize(ms);
        }


        /// <summary>
        /// Overload DeSerialize.
        /// </summary>
        /// <param name="asciiBytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static ClientMsg DeSerialize(byte[] asciiBytes,int offset,int length)
        {
            XmlSerializer x = new XmlSerializer(typeof(ClientMsg));
            //string data = Encoding.ASCII.GetString(asciiBytes, offset, length);
            //byte[] asciiData=
            MemoryStream ms = new MemoryStream(asciiBytes,false);
            //ms.Write(asciiBytes, offset, length);
            return (ClientMsg)x.Deserialize(ms);
        }


    }
}

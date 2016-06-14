using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMessenger
{
    /// <summary>
    ///  ************************ This class is for store Client Information**************************
    /// </summary>

    [Serializable]
    public class ClientInfo
    {
        public int ClientID;
        public string Name;
        public string IP;
        public int ListenPort;
        public DateTime lastAlivemsg;


        
        public override bool Equals(object obj)
        {
            if (obj is ClientInfo)
                return (this.ClientID == ((ClientInfo)obj).ClientID);
            return false;
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


       public override string ToString()
        {
            return (Name + Program.app.client.numberoOfMessageString[ClientID]);
        }
    }
}

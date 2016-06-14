using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace SimpleMessenger
{
    /// <summary>
    /// **********************The main entry point for the application**********************
    /// </summary>
    static class Program
    {
        // To handle Some Race Condition these Lockobject has introduced.
        public static object lockObject;
        public static object lockObject2;

        public static ApplicationData app;
        
        public static string OwnIP;
       
       
        [STAThread]
        static void Main()
        {
            lockObject = new object();
            lockObject2 = new object();
            
            
            // For Getting Own IP. 
            IPHostEntry host; 
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    OwnIP = ip.ToString();
                }
            }





            app = new ApplicationData();

            app.FormDic = new Dictionary<int, Form3>();

            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Form1());

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            foreach (Form3 f in app.FormDic.Values)
            {
                f.Close();

                f.Dispose();

            }
            app.FormDic.Clear();
        }
    }
}

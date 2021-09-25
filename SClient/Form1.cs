using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
             String host = Dns.GetHostName();          
            IPHostEntry ipHost = Dns.GetHostEntry("49.170.231.107");            
            IPAddress ipAddr = ipHost.AddressList[0];            
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);           
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate();  });


            //while (true)
            //{               
            //    try
            //    {
            //        SessionManager.Instance.SendForEach();
            //    }
            //    catch (Exception exception)
            //    {
            //        Console.WriteLine(e.ToString());
            //    }
            //    Thread.Sleep(250);
            //}

        }
    }
}

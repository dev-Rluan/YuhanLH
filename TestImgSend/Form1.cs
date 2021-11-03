using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestImgSend
{
    public partial class Form1 : Form
    {

        public static Form1 form1;

        Socket listen_socket;
        public Form1()
        {
            InitializeComponent();
            form1 = this;          
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7776);


            listen_socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listen_socket.Bind(endPoint);
            listen_socket.Listen(30);

            listen_socket.Accept();
            




        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PClient
{
    public partial class PClientForm : Form
    {
        public static PClientForm pclientform;
        public static SessionManager sessionManager;
        public static PacketManager packetManager;

        public PClientForm()
        {
            InitializeComponent();
            pclientform = this;
            sessionManager = new SessionManager();
            packetManager = new PacketManager();
        }

        private void PClientForm_Load(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse("49.247.149.125");
            IPEndPoint endPoint = new IPEndPoint(ip, 7777);
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return sessionManager.Generate(); });

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            sessionManager.LoginSend();
        }       

       
    }
}

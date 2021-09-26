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
        public PClientForm()
        {
            InitializeComponent();
            pclientform = this;
        }

        private void PClientForm_Load(object sender, EventArgs e)
        {
            IPHostEntry ipHost = Dns.GetHostEntry("49.170.231.107");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7773);
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); });

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            SessionManager.Instance.LoginSend();
        }
    }
}

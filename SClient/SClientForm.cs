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
    public partial class SClientForm : Form
    {
        public static SClientForm sclientform;

        public SClientForm()
        {
            InitializeComponent();
            sclientform = this;
        }

        private void SClientForm_Load(object sender, EventArgs e)
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

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] img = ScreenCopy.Copy();
            SessionManager.Instance.ImgSend(img);
        }
    }
}

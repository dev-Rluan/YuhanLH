
namespace SClient
{
    partial class SClientForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbLogin = new System.Windows.Forms.Label();
            this.lbChat = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.ptBox = new System.Windows.Forms.PictureBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lbConn = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ptBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lbLogin
            // 
            this.lbLogin.AutoSize = true;
            this.lbLogin.Location = new System.Drawing.Point(32, 22);
            this.lbLogin.Name = "lbLogin";
            this.lbLogin.Size = new System.Drawing.Size(78, 15);
            this.lbLogin.TabIndex = 12;
            this.lbLogin.Text = "로그인 : 실패";
            // 
            // lbChat
            // 
            this.lbChat.AutoSize = true;
            this.lbChat.Location = new System.Drawing.Point(32, 91);
            this.lbChat.Name = "lbChat";
            this.lbChat.Size = new System.Drawing.Size(31, 15);
            this.lbChat.TabIndex = 11;
            this.lbChat.Text = "채팅";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(32, 112);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(171, 297);
            this.textBox1.TabIndex = 10;
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(128, 43);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 9;
            this.btnLogout.Text = "로그아웃";
            this.btnLogout.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(32, 43);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 8;
            this.btnLogin.Text = "로그인";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // ptBox
            // 
            this.ptBox.Location = new System.Drawing.Point(291, 43);
            this.ptBox.Name = "ptBox";
            this.ptBox.Size = new System.Drawing.Size(211, 126);
            this.ptBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ptBox.TabIndex = 13;
            this.ptBox.TabStop = false;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(553, 43);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 14;
            this.btnSend.Text = "전송";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lbConn
            // 
            this.lbConn.AutoSize = true;
            this.lbConn.Location = new System.Drawing.Point(665, 393);
            this.lbConn.Name = "lbConn";
            this.lbConn.Size = new System.Drawing.Size(91, 15);
            this.lbConn.TabIndex = 15;
            this.lbConn.Text = "서버연결 : Faild";
            // 
            // SClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbConn);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.ptBox);
            this.Controls.Add(this.lbLogin);
            this.Controls.Add(this.lbChat);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnLogin);
            this.Name = "SClientForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.SClientForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ptBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbChat;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnSend;
        public System.Windows.Forms.Label lbConn;
        public System.Windows.Forms.Label lbLogin;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.PictureBox ptBox;
    }
}


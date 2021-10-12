
namespace PClient
{
    partial class PClientForm
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
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.txtBox = new System.Windows.Forms.TextBox();
            this.ptBox = new System.Windows.Forms.PictureBox();
            this.btnImgRequest = new System.Windows.Forms.Button();
            this.lbServerConn = new System.Windows.Forms.Label();
            this.lbChat = new System.Windows.Forms.Label();
            this.lbLogin = new System.Windows.Forms.Label();
            this.lbImg = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ptBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(46, 32);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 23);
            this.btnLogin.TabIndex = 0;
            this.btnLogin.Text = "로그인";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(142, 32);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 1;
            this.btnLogout.Text = "로그아웃";
            this.btnLogout.UseVisualStyleBackColor = true;
            // 
            // txtBox
            // 
            this.txtBox.Location = new System.Drawing.Point(46, 101);
            this.txtBox.Multiline = true;
            this.txtBox.Name = "txtBox";
            this.txtBox.Size = new System.Drawing.Size(171, 297);
            this.txtBox.TabIndex = 2;
            // 
            // ptBox
            // 
            this.ptBox.Location = new System.Drawing.Point(291, 43);
            this.ptBox.Name = "ptBox";
            this.ptBox.Size = new System.Drawing.Size(211, 126);
            this.ptBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ptBox.TabIndex = 3;
            this.ptBox.TabStop = false;
            // 
            // btnImgRequest
            // 
            this.btnImgRequest.Location = new System.Drawing.Point(543, 32);
            this.btnImgRequest.Name = "btnImgRequest";
            this.btnImgRequest.Size = new System.Drawing.Size(123, 23);
            this.btnImgRequest.TabIndex = 4;
            this.btnImgRequest.Text = "이미지 요청";
            this.btnImgRequest.UseVisualStyleBackColor = true;
            // 
            // lbServerConn
            // 
            this.lbServerConn.AutoSize = true;
            this.lbServerConn.Location = new System.Drawing.Point(543, 383);
            this.lbServerConn.Name = "lbServerConn";
            this.lbServerConn.Size = new System.Drawing.Size(0, 15);
            this.lbServerConn.TabIndex = 5;
            // 
            // lbChat
            // 
            this.lbChat.AutoSize = true;
            this.lbChat.Location = new System.Drawing.Point(46, 80);
            this.lbChat.Name = "lbChat";
            this.lbChat.Size = new System.Drawing.Size(0, 15);
            this.lbChat.TabIndex = 6;
            // 
            // lbLogin
            // 
            this.lbLogin.AutoSize = true;
            this.lbLogin.Location = new System.Drawing.Point(46, 11);
            this.lbLogin.Name = "lbLogin";
            this.lbLogin.Size = new System.Drawing.Size(0, 15);
            this.lbLogin.TabIndex = 7;
            // 
            // lbImg
            // 
            this.lbImg.AutoSize = true;
            this.lbImg.Location = new System.Drawing.Point(543, 62);
            this.lbImg.Name = "lbImg";
            this.lbImg.Size = new System.Drawing.Size(0, 15);
            this.lbImg.TabIndex = 8;
            // 
            // PClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 450);
            this.Controls.Add(this.lbImg);
            this.Controls.Add(this.lbLogin);
            this.Controls.Add(this.lbChat);
            this.Controls.Add(this.lbServerConn);
            this.Controls.Add(this.btnImgRequest);
            this.Controls.Add(this.ptBox);
            this.Controls.Add(this.txtBox);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnLogin);
            this.Name = "PClientForm";
            this.Load += new System.EventHandler(this.PClientForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ptBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnImgRequest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lbChat;
        private System.Windows.Forms.Button lbImageRequest;
        private System.Windows.Forms.Button btnImgRe;
        public System.Windows.Forms.PictureBox ptBox;
        public System.Windows.Forms.Label lbLogin;
        public System.Windows.Forms.Label lbServerConn;
        public System.Windows.Forms.TextBox txtBox;
        public System.Windows.Forms.Label lbImg;
    }
}


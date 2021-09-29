
namespace TestImgRecv
{
    partial class Form1
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
            this.ptBox = new System.Windows.Forms.PictureBox();
            this.btnRecv = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ptBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ptBox
            // 
            this.ptBox.Location = new System.Drawing.Point(116, 76);
            this.ptBox.Name = "ptBox";
            this.ptBox.Size = new System.Drawing.Size(189, 91);
            this.ptBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ptBox.TabIndex = 0;
            this.ptBox.TabStop = false;
            // 
            // btnRecv
            // 
            this.btnRecv.Location = new System.Drawing.Point(432, 112);
            this.btnRecv.Name = "btnRecv";
            this.btnRecv.Size = new System.Drawing.Size(75, 23);
            this.btnRecv.TabIndex = 1;
            this.btnRecv.Text = "button1";
            this.btnRecv.UseVisualStyleBackColor = true;
            this.btnRecv.Click += new System.EventHandler(this.btnRecv_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnRecv);
            this.Controls.Add(this.ptBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ptBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox ptBox;
        public System.Windows.Forms.Button btnRecv;
    }
}


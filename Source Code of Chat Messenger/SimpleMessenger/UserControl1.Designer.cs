namespace SimpleMessenger
{
    partial class UserControl1
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControl1));
            this.lblName = new System.Windows.Forms.Label();
            this.txtRichMsg = new System.Windows.Forms.RichTextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.PicBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(46, 4);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(54, 19);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "label1";
            // 
            // txtRichMsg
            // 
            this.txtRichMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.txtRichMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRichMsg.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRichMsg.Location = new System.Drawing.Point(1, 47);
            this.txtRichMsg.Name = "txtRichMsg";
            this.txtRichMsg.ReadOnly = true;
            this.txtRichMsg.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtRichMsg.Size = new System.Drawing.Size(247, 52);
            this.txtRichMsg.TabIndex = 2;
            this.txtRichMsg.Text = "";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.DimGray;
            this.lblDate.Location = new System.Drawing.Point(47, 26);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(43, 16);
            this.lblDate.TabIndex = 3;
            this.lblDate.Text = "label1";
            // 
            // PicBox
            // 
            this.PicBox.Image = ((System.Drawing.Image)(resources.GetObject("PicBox.Image")));
            this.PicBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("PicBox.InitialImage")));
            this.PicBox.Location = new System.Drawing.Point(5, 3);
            this.PicBox.Name = "PicBox";
            this.PicBox.Size = new System.Drawing.Size(36, 38);
            this.PicBox.TabIndex = 4;
            this.PicBox.TabStop = false;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Controls.Add(this.PicBox);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.txtRichMsg);
            this.Controls.Add(this.lblName);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(251, 102);
            this.Load += new System.EventHandler(this.UserControl1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.RichTextBox txtRichMsg;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.PictureBox PicBox;
    }
}

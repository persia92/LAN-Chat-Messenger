using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleMessenger
{
    /// <summary>
    ///  *********************************** This is Custom User Control******************************
    ///  ******** It shows the message to user in Different Colors(User message in Blue and Buddy's message in Yello)*************
    /// </summary>
     

    public partial class UserControl1 : UserControl
    {

        private ClientInfo info;
        private string msg;
        private int getline;
        private RichTextBox temp = new RichTextBox();


        public UserControl1(ClientInfo info, string msg, int line)
        {
            this.info = info;
            this.msg = msg;
            getline = line;
            InitializeComponent();
        }
       
        /// <summary>
        /// UserControl Loading.... Setting colors and name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl1_Load(object sender, EventArgs e)
        {
            txtRichMsg.ForeColor = Color.Black;
            lblName.ForeColor = Color.Black;
            lblDate.ForeColor = Color.Black;
            if(info.ClientID==Program.app.myInfo.ClientID)
            {
                lblName.Text ="You";
                txtRichMsg.BackColor=Color.CornflowerBlue;
                this.BackColor = Color.CornflowerBlue;
                
            }
            else
            {
                lblName.Text =info.Name;
            }
            changeHeight();
        }



        /// <summary>
        /// The RichTextbox (message showing field) will change its height Dynamically with the height of messgae.
        /// </summary>
        private void changeHeight()
        {
            txtRichMsg.Visible = false;
            if (msg != "BuZZ!!!!!!!!")
                txtRichMsg.Text = msg;
            else
            {
                Font font1 = new Font(txtRichMsg.Font, FontStyle.Bold);
                txtRichMsg.SelectionFont = font1;
                txtRichMsg.SelectionColor = Color.Red;
                txtRichMsg.SelectedText = msg;
            }
            int H = getline * txtRichMsg.Font.Height + txtRichMsg.Margin.Vertical;
            txtRichMsg.Height = H;
            txtRichMsg.Visible = true;
            string myDate;
            myDate = DateTime.Now.ToString("yyyy.MM.dd") +" at " + DateTime.Now.ToString("HH:mm:ss") + " says,";
            lblDate.Text = myDate;
        }
    }
}

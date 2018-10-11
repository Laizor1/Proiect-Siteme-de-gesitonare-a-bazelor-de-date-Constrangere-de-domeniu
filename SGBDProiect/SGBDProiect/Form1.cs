using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGBDProiect
{
    public partial class Form1 : Form
    {
        public static Form1 LoginForm;

        public Form1()
        {
            InitializeComponent();
            LoginForm = this;
        }

        private void btnConectare_Click(object sender, EventArgs e)
        {
            bool isOk = SGBD.Connect(txtIP.Text, UInt32.Parse(txtPort.Text), txtUser.Text, txtPass.Text, txtTabela.Text);

            if (isOk)
            {
                Form2 mgrWindow = new Form2();
                mgrWindow.Visible = true;
                this.Visible = false;
            } else
            {
                MessageBox.Show("Conectare esuata.");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BomberStudio
{
    public partial class FrmDisplayText : Form
    {
        string text;
        public FrmDisplayText(string text)
        {
            InitializeComponent();
            this.text = text;
        }

        private void FrmDisplayText_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.icon;
            textBox1.Text = text;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(text);
            MessageBox.Show("复制成功");
        }
    }
}

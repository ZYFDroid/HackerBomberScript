using ScriptInterpreter;
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
    public partial class FrmTextEscape : Form
    {
        public FrmTextEscape()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            txtTag.Text = TextUtil.escapeText(txtSrc.Text);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                txtSrc.Text = TextUtil.unescapeText(txtTag.Text);
            }
            catch (Exception ex) {
                MessageBox.Show("需要反转义的文本中有语法错误");
            }
        }

        private void FrmTextEscape_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.icon;
        }
    }
}

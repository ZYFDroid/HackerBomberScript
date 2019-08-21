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
    public partial class FrmSplash : Form
    {
        public FrmSplash()
        {
            InitializeComponent();
        }

        private void FrmDisplayText_Load(object sender, EventArgs e)
        {
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnthonySeymourGOL
{
    public partial class ModalDialog : Form
    {
        public ModalDialog()
        {
            InitializeComponent();
        }

        public int GetNumber()
        {
            return (int)numericUpDown1.Value;
        }

        public void SetNumber(int number)
        {
            numericUpDown1.Minimum = number;
            numericUpDown1.Value = number;
        }
    }
}

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
    public partial class OptionsModal : Form
    {
        public OptionsModal()
        {
            InitializeComponent();
        }

        // Timer interval
        public int Interval
        {
            get { return (int)intervalNumericUpDown.Value; }
            set { intervalNumericUpDown.Value = value; }
        }

        // Cell Width in Universe
        public float CellWidth
        {
            get { return (float)widthNumericUpDown.Value; }
            set { widthNumericUpDown.Value = (decimal)value; }
        }

        // Cell Height in Universe
        public float CellHeight
        {
            get { return (float)heightNumericUpDown.Value; }
            set { heightNumericUpDown.Value = (decimal)value; }
        }
    }
}

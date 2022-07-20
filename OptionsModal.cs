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

            // Set maximum values of numericUpDowns
            // to int max to accept more inputs
            intervalNumericUpDown.Maximum = int.MaxValue;
            widthNumericUpDown.Maximum = int.MaxValue;
            heightNumericUpDown.Maximum = int.MaxValue;
        }

        // Timer interval
        public int Interval
        {
            get { return (int)intervalNumericUpDown.Value; }
            set { intervalNumericUpDown.Value = value; }
        }

        // Cell Width in Universe
        public int WidthCells
        {
            get { return (int)widthNumericUpDown.Value; }
            set { widthNumericUpDown.Value = value; }
        }

        // Cell Height in Universe
        public int HeightCells
        {
            get { return (int)heightNumericUpDown.Value; }
            set { heightNumericUpDown.Value = value; }
        }
    }
}

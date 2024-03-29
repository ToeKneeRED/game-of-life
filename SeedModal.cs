﻿using System;
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
    public partial class SeedModal : Form
    {
        public SeedModal()
        {
            InitializeComponent();

            numericUpDown1.Maximum = int.MaxValue;
        }

        public int Seed
        {
            get { return (int)numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            numericUpDown1.Value = rand.Next(0, int.MaxValue);
        }
    }
}

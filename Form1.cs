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
    public partial class Form1 : Form
    {
        bool[,] universe = new bool[25, 25];
        bool[,] scratchPad = new bool[25, 25];

        // Drawing colors
        Color gridColor = Color.Gray;
        Color cellColor = Color.LightGray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Run To Check
        bool bRunToGeneration = false;
        int? runToGeneration = null;

        // Randomize seed
        int seed = 0;

        // Timer interval
        int interval = 35; // ms

        // Number of alive cells
        int alive = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = interval; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer paused

            // Toolstrip initializations
            toolStripStatusLabelSeed.Text = "Seed: " + seed.ToString();
            toolStripStatusLabelInterval.Text = "Interval: " + interval.ToString();
            toolStripStatusLabelAlive.Text = "Alive: " + alive.ToString();
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            if ((bRunToGeneration == true) && (generations >= runToGeneration))
            {
                timer.Enabled = false;
                bRunToGeneration = false;
                return;
            }

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (scratchPad[x, y] == true)
                        scratchPad[x, y] = !scratchPad[x, y];
                }
            }

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count = 0;

                    // Check what method is to be used for next generation
                    if (toroidalToolStripMenuItem.Checked == true)
                        count = CountNeighborsToroidal(x, y);
                    else if (finiteToolStripMenuItem.Checked == true)
                        count = CountNeighborsFinite(x, y);

                    // Rules of life
                    if ((count < 2 || count > 3) && (universe[x, y] == true) ||
                        (count == 3) && (universe[x, y] == false))
                        scratchPad[x, y] = !universe[x, y];
                    else if ((count == 2 || count == 3) && (universe[x, y] == true))
                        scratchPad[x, y] = universe[x, y];

                }
            }

            // Copy scratchPad to universe
            SwapArrays(ref scratchPad, ref universe);
            
            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString();

            graphicsPanel1.Invalidate();
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0); // universe x length
            int yLen = universe.GetLength(1); // universe y length

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    // indexes to get accurate neighbor count
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    // Boundary checks
                    if ((xOffset == 0) && (yOffset == 0))
                        continue;
                    if (xCheck < 0)
                        xCheck = xLen - 1;
                    if (yCheck < 0)
                        yCheck = yLen - 1;
                    if (xCheck >= xLen)
                        xCheck = 0;
                    if (yCheck >= yLen)
                        yCheck = 0;

                    // if cell is on, add to the neighbor count
                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0); // universe x length
            int yLen = universe.GetLength(1); // universe y length

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    // indexes to get accurate neighbor count
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    // Boundary checks
                    if ((xOffset == 0 && yOffset == 0) || (xCheck < 0) || 
                        (yCheck < 0) || (xCheck >= xLen) || (yCheck >= yLen))
                        continue;

                    // if cell is on, add to the neighbor count
                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        // Helper method for swapping 2D Arrays
        private void SwapArrays(ref bool[,] arr1, ref bool[,] arr2)
        {
            bool[,] temp = arr1;
            arr1 = arr2;
            arr2 = temp;
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 0.75f);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // StringFormat for neighbor colors and text
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            Brush neighborCountColor = new SolidBrush(Color.Red);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    Font font = new Font("Consolas", cellHeight / 2);

                    int neighbors = 0;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen while grid option is checked
                    if(gridToolStripMenuItem.Checked == true)
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    if (neighborCountToolStripMenuItem.Checked == true)
                    {
                        // Check what method is used to get proper neighbor count
                        if (toroidalToolStripMenuItem.Checked == true)
                            neighbors = CountNeighborsToroidal(x, y);
                        else if (finiteToolStripMenuItem.Checked == true)
                            neighbors = CountNeighborsFinite(x, y);

                        // ensure 0s dont show in universe
                        if (neighbors != 0)
                        {
                            // Reset color back to red
                            neighborCountColor = new SolidBrush(Color.Red);

                            // Only change brush color to green once a cell will live
                            if ((neighbors == 2 || neighbors == 3) && (universe[x, y] == true) ||
                                (neighbors == 3) && (universe[x, y] == false))
                                neighborCountColor = new SolidBrush(Color.Green);

                            // Draw the neighbor count into the currently iterated cell
                            e.Graphics.DrawString(neighbors.ToString(), font, neighborCountColor, cellRect, stringFormat);
                        }
                    }
                }
            }
            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            neighborCountColor.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = (int)(e.X / cellWidth);
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = (int)(e.Y / cellHeight);

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            } 
        }

        // Exit Menu Item
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Play Button
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;

            if (timer.Enabled == true)
            {
                toolStripButton1.Enabled = false; // Disable Play button
                startToolStripMenuItem.Enabled = false; // Disable Start menu item
                toolStripButton2.Enabled = true; // Enable Pause button
                stopToolStripMenuItem.Enabled = true; // Enable Stop menu item
            }

            graphicsPanel1.Invalidate();
        }

        // Pause Button
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;

            if(timer.Enabled == false)
            {
                toolStripButton1.Enabled = true; // Enable Play button
                startToolStripMenuItem.Enabled = true; // Enable Start menu item 
                toolStripButton2.Enabled = false; // Disable Pause button
                stopToolStripMenuItem.Enabled = false; // Disable Stop menu item
            }
        }

        // Next Button
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            NextGeneration();

            if (timer.Enabled == false)
            {
                toolStripButton1.Enabled = true; // Enable Play button
                startToolStripMenuItem.Enabled = true; // Enable Start menu item
                toolStripButton2.Enabled = false; // Disable Pause button
                stopToolStripMenuItem.Enabled = false; // Disable Stop menu item
            }
        }

        // New Button
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                        universe[x, y] = !universe[x, y];
                }
            }

            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripButton1.Enabled = true; // Enable Play button
            startToolStripMenuItem.Enabled = true; // Enable Start menu item
            toolStripButton2.Enabled = false; // Disable Pause button
            stopToolStripMenuItem.Enabled = false; // Disable Stop menu item
            graphicsPanel1.Invalidate();
        }

        // Keybinds
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                // Play
                case Keys.F5:
                    toolStripButton1_Click(sender, e);
                    break;
                // Pause
                case Keys.F6:
                    toolStripButton2_Click(sender, e);
                    break;
                // Next
                case Keys.F7:
                    toolStripButton3_Click(sender, e);
                    NextGeneration();
                    break;
                default:
                    break;
            }
        }

        // Toroidal Menu Item
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if other method is selected
            // If so, clear the universe
            if (finiteToolStripMenuItem.Checked == true)
                newToolStripButton_Click(sender, e);

            // Set menu items to proper checked states
            finiteToolStripMenuItem.Checked = false;
            toroidalToolStripMenuItem.Checked = true;
            finiteToolStripMenuItem1.Checked = false;
            toroidalToolStripMenuItem1.Checked = true;
        }

        // Finite Menu Item
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if other method is selected
            // If so, clear the universe
            if (toroidalToolStripMenuItem.Checked == true)
                newToolStripButton_Click(sender, e);

            // Set menu items to proper checked states
            toroidalToolStripMenuItem.Checked = false;
            finiteToolStripMenuItem.Checked = true;
            toroidalToolStripMenuItem1.Checked = false;
            finiteToolStripMenuItem1.Checked = true;
        }

        // Grid Menu Item
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Invert grid option when clicked
            gridToolStripMenuItem.Checked = !gridToolStripMenuItem.Checked;
            gridToolStripMenuItem1.Checked = !gridToolStripMenuItem1.Checked;

            graphicsPanel1.Invalidate();
        }

        // HUD Menu Item
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Invert HUD option when clicked in toolstrip and context menu
            hUDToolStripMenuItem.Checked = !hUDToolStripMenuItem.Checked;
            hUDToolStripMenuItem1.Checked = !hUDToolStripMenuItem1.Checked;

            graphicsPanel1.Invalidate();
        }

        // NeighborCount Menu Item
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Invert neighbor count option when clicked in toolstrip and context menu
            neighborCountToolStripMenuItem.Checked = !neighborCountToolStripMenuItem.Checked;
            neighborCountToolStripMenuItem1.Checked = !neighborCountToolStripMenuItem1.Checked;

            graphicsPanel1.Invalidate();
        }

        // Run To Menu Item
        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModalDialog toModal = new ModalDialog();

            // Set minimum number on numbericupdown in Run To ModalDialog
            // to current generation int
            toModal.SetNumber(generations);

            if (DialogResult.OK == toModal.ShowDialog())
            {
                runToGeneration = toModal.GetNumber();

                // NextGeneration() checks if bRunToGeneration is true
                // to disable timer on certain generation
                bRunToGeneration = true;
                timer.Enabled = true;
            }
            toModal.Dispose();
        }

        // Randomize helper method
        private void Randomize(int seed)
        {
            // Get random number from seed
            Random rand = new Random(seed);

            // Iterate through universe
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // if random number generated is even
                    // cell is on otherwise cell is off
                    if (rand.Next() % 2 == 0)
                        universe[x, y] = true;
                    else
                        universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }

        // Randomize From Time
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Seed equal to the ticks representing current date
            // and time then update seed text, clear universe,
            // and randomize based on seed
            seed = (int)DateTime.Now.Ticks;
            toolStripStatusLabelSeed.Text = "Seed: " + seed.ToString();
            newToolStripButton_Click(sender, e);
            Randomize(seed);
        }

        // Randomize From Seed
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedModal modal = new SeedModal();

            // Set numericUpDown value to the current seed
            modal.Seed = seed;
            
            if(modal.ShowDialog() == DialogResult.OK)
            {
                // Set seed to the value entered on the modal
                // and randomize based on that seed
                seed = modal.Seed;
                newToolStripButton_Click(sender, e);
                toolStripStatusLabelSeed.Text = "Seed: " + seed.ToString();
                Randomize(seed);
            }
        }

        // Randomize From Current Seed
        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clear universe then set seed text
            // and randomize from current seed
            newToolStripButton_Click(sender, e);
            toolStripStatusLabelSeed.Text = "Seed: " + seed.ToString();
            Randomize(seed);
        }
    }
}

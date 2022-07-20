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
        // Default amount of cells in universe
        // Only used to initialize the universe on startup
        static int defaultUniverseWidth = Properties.Settings.Default.WidthCells;
        static int defaultUniverseHeight = Properties.Settings.Default.HeightCells;

        // The actual universe
        bool[,] universe = new bool[defaultUniverseWidth, defaultUniverseHeight];
        bool[,] scratchPad = new bool[defaultUniverseWidth, defaultUniverseHeight];

        // Drawing colors
        Color gridColor = Color.Gray;
        Color gridx10Color = Color.Black;
        Color cellColor = Color.LightGray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Run To Check
        bool bRunToGeneration = false;
        int? runToGeneration = null;

        // Randomize seed
        int seed = 1337;

        // Timer interval
        int interval = Properties.Settings.Default.GenerationInterval; // ms

        // Number of alive cells
        int alive = 0;

        public Form1()
        {
            InitializeComponent();

            // Check settings for user-saved Color options
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            gridx10Color = Properties.Settings.Default.Gridx10Color;

            // Check settings for user-saved View options
            hUDToolStripMenuItem.Checked = Properties.Settings.Default.HUDVisible;
            neighborCountToolStripMenuItem.Checked = Properties.Settings.Default.NeighborCountVisible;
            gridToolStripMenuItem.Checked = Properties.Settings.Default.GridVisible;

            // Check settings for user-saved boundary options
            toroidalToolStripMenuItem.Checked = Properties.Settings.Default.ToroidalState;
            toroidalToolStripMenuItem1.Checked = Properties.Settings.Default.ToroidalState;
            finiteToolStripMenuItem.Checked = !Properties.Settings.Default.ToroidalState;
            finiteToolStripMenuItem1.Checked = !Properties.Settings.Default.ToroidalState;

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
            // Set to 0 for accurate count from generation
            // after randomizing
            alive = 0;

            // Run To Generation check
            if ((bRunToGeneration == true) && (generations >= runToGeneration))
            {
                timer.Enabled = false;
                bRunToGeneration = false;
                return;
            }

            // Clear scratchPad before applying rules to the generation
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (scratchPad[x, y] == true)
                        scratchPad[x, y] = !scratchPad[x, y];

                    if(universe[x, y] == true)
                        alive++;
                }
            }

            // Universe loop
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
                    if ((count < 2 || count > 3) && (universe[x, y] == true))
                    {
                        // Update scratchPad
                        scratchPad[x, y] = !universe[x, y];

                        // Update alive count
                        alive--;
                    } 
                    else if((count == 3) && (universe[x, y] == false))
                    {
                        // Update scratchPad
                        scratchPad[x, y] = !universe[x, y];

                        // Update alive count
                        alive++;
                    }
                    else if ((count == 2 || count == 3) && (universe[x, y] == true))
                    {
                        // Update scratchPad
                        scratchPad[x, y] = universe[x, y];
                    }
                }
            }

            // Copy scratchPad to universe
            SwapArrays(ref scratchPad, ref universe);

            // Increment generation count
            generations++;

            // Update status strip
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString();
            toolStripStatusLabelAlive.Text = "Alive: " + alive.ToString();

            graphicsPanel1.Invalidate();
        }

        // Toroidal Neighbor Count
        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0); // universe x length
            int yLen = universe.GetLength(1); // universe y length

            // Universe loop
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
                    if (universe[xCheck, yCheck] == true) 
                        count++;
                }
            }
            return count;
        }

        // Finite Neighbor Count
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
                    if (universe[xCheck, yCheck] == true) 
                        count++;
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

            // Pen for drawing thicker grid lines every 10 cells
            Pen gridx10Pen = new Pen(gridx10Color, 2.25f);

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
                    if (gridToolStripMenuItem.Checked == true)
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

            // Seperate universe loop to draw gridx10 lines over universe's grid rectangles
            // Not a great way but is a way
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    RectangleF cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // If grid is shown, draw thicker line every 10 cells 
                    if (gridToolStripMenuItem.Checked == true)
                    {
                        if (x % 10 == 0)
                            e.Graphics.DrawLine(gridx10Pen, cellRect.X, cellRect.Y, cellRect.X, cellRect.Y + cellRect.Height);
                        if (y % 10 == 0)
                            e.Graphics.DrawLine(gridx10Pen, cellRect.X, cellRect.Y, cellRect.X + cellRect.Width, cellRect.Y);
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

                // Update alive cell count
                if (universe[x, y] == true)
                    alive++;
                else
                    alive--;

                // Update alive status strip
                toolStripStatusLabelAlive.Text = "Alive: " + alive.ToString();

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
        private void playToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;

            // Sanity check
            if (timer.Enabled == true)
            {
                playToolStripButton.Enabled = false; // Disable Play button
                startToolStripMenuItem.Enabled = false; // Disable Start menu item
                pauseToolStripButton.Enabled = true; // Enable Pause button
                stopToolStripMenuItem.Enabled = true; // Enable Stop menu item
            }

            graphicsPanel1.Invalidate();
        }

        // Pause Button
        private void pauseToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;

            // Sanity check
            if(timer.Enabled == false)
            {
                playToolStripButton.Enabled = true; // Enable Play button
                startToolStripMenuItem.Enabled = true; // Enable Start menu item 
                pauseToolStripButton.Enabled = false; // Disable Pause button
                stopToolStripMenuItem.Enabled = false; // Disable Stop menu item
            }
        }

        // Next Button
        private void nextToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            NextGeneration();

            // Sanity check
            if (timer.Enabled == false)
            {
                playToolStripButton.Enabled = true; // Enable Play button
                startToolStripMenuItem.Enabled = true; // Enable Start menu item
                pauseToolStripButton.Enabled = false; // Disable Pause button
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
                    // "Clear" the universe
                    // Turn off any cells that are on in universe
                    if (universe[x, y] == true)
                        universe[x, y] = !universe[x, y];
                }
            }

            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations: " + generations.ToString();

            // Enable/Disable proper buttons and menu items
            playToolStripButton.Enabled = true; // Enable Play button
            startToolStripMenuItem.Enabled = true; // Enable Start menu item
            pauseToolStripButton.Enabled = false; // Disable Pause button
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
                    playToolStripButton_Click(sender, e);
                    break;
                // Pause
                case Keys.F6:
                    pauseToolStripButton_Click(sender, e);
                    break;
                // Next
                case Keys.F7:
                    nextToolStripButton_Click(sender, e);
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

        // Options Menu Item
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsModal dlg = new OptionsModal();

            // Variables to check the modal's input against
            int widthCells = Properties.Settings.Default.WidthCells;
            int heightCells = Properties.Settings.Default.HeightCells;

            // Set values of numericUpDowns to current respective values
            dlg.Interval = timer.Interval;
            dlg.WidthCells = widthCells;
            dlg.HeightCells = heightCells;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Check if interval has been changed
                // Don't clear universe if only interval changes
                if (Properties.Settings.Default.GenerationInterval != dlg.Interval)
                {
                    timer.Interval = dlg.Interval;
                    toolStripStatusLabelInterval.Text = "Interval: " + timer.Interval.ToString();

                    // Update Settings Default with new interval
                    Properties.Settings.Default.GenerationInterval = timer.Interval;
                }

                if (dlg.WidthCells != widthCells || dlg.HeightCells != heightCells)
                {
                    newToolStripButton_Click(sender, e); // Call this to clear universe to not repeat code

                    // Remake the universe with the new size
                    universe = new bool[dlg.WidthCells, dlg.HeightCells];
                    scratchPad = new bool[dlg.WidthCells, dlg.HeightCells];

                    // Update Settings Default with new universe width and height
                    Properties.Settings.Default.WidthCells = dlg.WidthCells;
                    Properties.Settings.Default.HeightCells = dlg.HeightCells;
                }
            }
            dlg.Dispose();
            graphicsPanel1.Invalidate();
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
                    // If number is divisble by 3 cell is on
                    if (rand.Next() % 3 == 0)
                        universe[x, y] = true;
                }
            }
            graphicsPanel1.Invalidate();
        }

        // Randomize From Time Menu Item
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Seed equal to the ticks representing current date
            // and time then update seed text, clear universe,
            // and randomize based on seed
            seed = Math.Abs((int)DateTime.Now.Ticks); // use absolute value to allow for proper setting the seed in SeedModal
            toolStripStatusLabelSeed.Text = "Seed: " + seed.ToString();
            newToolStripButton_Click(sender, e);
            Randomize(seed);

            // Update alive count
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                        alive++;
                }
            }
            // Update status strip
            toolStripStatusLabelAlive.Text = "Alive: " + alive.ToString();
            graphicsPanel1.Invalidate();
        }

        // Randomize From Seed Menu Item
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedModal modal = new SeedModal();

            // Set numericUpDown value to the current seed
            modal.Seed = seed;

            if (modal.ShowDialog() == DialogResult.OK)
            {
                // Set seed to the value entered on the modal
                // Randomize based on that seed
                seed = modal.Seed;
                newToolStripButton_Click(sender, e);
                toolStripStatusLabelSeed.Text = "Seed: " + seed.ToString();
                Randomize(seed);

                // Update alive count
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == true)
                            alive++;
                    }
                }
            }
            modal.Dispose();

            // Update status strip
            toolStripStatusLabelAlive.Text = "Alive: " + alive.ToString();
            graphicsPanel1.Invalidate();
        }

        // Randomize From Current Seed
        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clear universe then set seed text
            // Randomize from current seed
            newToolStripButton_Click(sender, e);
            Randomize(seed);

            // Update alive count
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                        alive++;
                }
            }

            // Update status strip
            toolStripStatusLabelAlive.Text = "Alive: " + alive.ToString();
            toolStripStatusLabelSeed.Text = "Seed: " + seed.ToString();
            graphicsPanel1.Invalidate();
        }

        // Settings Back Color Menu Item
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = graphicsPanel1.BackColor;

            if(dlg.ShowDialog() == DialogResult.OK)
            {
                // Set the graphics panel's back color to the selected color
                graphicsPanel1.BackColor = dlg.Color;
            }
            dlg.Dispose();
        }

        // Settings Cell Color Menu Item
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = cellColor;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Set cellColor to the selected color
                cellColor = dlg.Color;
            }
            dlg.Dispose();
            graphicsPanel1.Invalidate();
        }
        
        // Settings Grid Color Menu Item
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = gridColor;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Set gridColor to the selected color
                gridColor = dlg.Color;
            }
            dlg.Dispose();
            graphicsPanel1.Invalidate();
        }

        // Settings Gridx10 Color Menu Item
        private void gridX10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = gridx10Color;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Set gridx10Color to the selected color
                gridx10Color = dlg.Color;
            }
            dlg.Dispose();
            graphicsPanel1.Invalidate();
        }

        // Settings Reset Menu Item
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reset colors back to default values
            graphicsPanel1.BackColor = Color.White;
            cellColor = Color.LightGray;
            gridColor = Color.Gray;
            gridx10Color = Color.Black;

            // Reset Settings Default colors back to default values
            Properties.Settings.Default.BackColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.Gridx10Color = gridx10Color;

            // Reset Settings Default interval, and amount of cells to default values
            Properties.Settings.Default.GenerationInterval = 35; // ms
            Properties.Settings.Default.WidthCells = 30;
            Properties.Settings.Default.HeightCells = 30;

            Properties.Settings.Default.Save();

            graphicsPanel1.Invalidate();
        }

        // Reload Menu Item
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reload colors back to Settings Default (last saved settings)
            graphicsPanel1.BackColor = Properties.Settings.Default.BackColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            gridx10Color = Properties.Settings.Default.Gridx10Color;

            // Rload universe variables to Settings Default (last saved settings)
            timer.Interval = Properties.Settings.Default.GenerationInterval;
            universe = new bool[Properties.Settings.Default.WidthCells, Properties.Settings.Default.HeightCells];

            Properties.Settings.Default.Save();

            graphicsPanel1.Invalidate();
        }

        // Save Menu Item
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // FormClosing in case need to cancel the close
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save current color options to Settings Default
            Properties.Settings.Default.BackColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.Gridx10Color = gridx10Color;

            // Save current visibility options to Settings Default
            Properties.Settings.Default.HUDVisible = hUDToolStripMenuItem.Checked;
            Properties.Settings.Default.NeighborCountVisible = neighborCountToolStripMenuItem.Checked;
            Properties.Settings.Default.GridVisible = gridToolStripMenuItem.Checked;

            // Save current generation & universe variables to Settings Default
            Properties.Settings.Default.GenerationInterval = timer.Interval;
            Properties.Settings.Default.WidthCells = universe.GetLength(0); // x dimension
            Properties.Settings.Default.HeightCells = universe.GetLength(1); // y dimension

            // Save current boundary check option to Settings Default
            Properties.Settings.Default.ToroidalState = toroidalToolStripMenuItem.Checked;

            Properties.Settings.Default.Save();
        }

    }
}

using System;
using System.Diagnostics.Metrics;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Penetration_C_
{
    public partial class Form1 : Form
    {

        private bool isDragging = false;
        private Point lastCursorPosition;

        private bool isResizing = false;
        private Point resizeStart;
        private Size originalSize;

        PictureBox pictureBox1;

        Chart line_attacks, histogram_attacks;
        Bitmap b_attacks;

        Chart absolute_line, absolute_histogram;
        Bitmap b_absolute;

        Chart relative_line, relative_histogram;
        Bitmap b_relative;

        Chart ratio_line, ratio_histogram;
        Bitmap b_ratio;


        public Form1()
        {
            InitializeComponent();

            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 1;

            numericUpDown2.Minimum = 0;
            numericUpDown2.Maximum = Int32.MaxValue;

            numericUpDown3.Minimum = 0;
            numericUpDown3.Maximum = Int32.MaxValue;

        }

        int prev = 0;
        int curr = 1;
        System.Timers.Timer myTimer;
        bool status1, status2, status3, status4 = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1 = new PictureBox();
            pictureBox1.Location = new Point(0, 100);
            this.WindowState = FormWindowState.Maximized;
            pictureBox1.Height = this.Size.Height;
            pictureBox1.Width = this.Size.Width;
            //pictureBox1.BackColor = Color.Red;
            this.Controls.Add(pictureBox1);


            panel1.Width = pictureBox1.Width;
            panel1.Height = pictureBox1.Height / 5;
            panel1.Location = new Point(pictureBox1.Location.X, pictureBox1.Location.Y);
            //panel1.BackColor = Color.Red;

            panel2.Width = pictureBox1.Width;
            panel2.Height = pictureBox1.Height / 5;
            panel2.Location = new Point(pictureBox1.Location.X, (pictureBox1.Height / 4)+50);
            //panel2.BackColor = Color.Blue;

            panel3.Width = pictureBox1.Width;
            panel3.Height = pictureBox1.Height / 5;
            panel3.Location = new Point(pictureBox1.Location.X, pictureBox1.Height / 2);
            //panel3.BackColor = Color.Green;

            panel4.Width = pictureBox1.Width;
            panel4.Height = pictureBox1.Height / 5;
            panel4.Location = new Point(pictureBox1.Location.X, (pictureBox1.Height * 3) / 4 - 50);
            //panel4.BackColor = Color.Violet;

            this.DoubleBuffered = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            float attack_success = (float)numericUpDown1.Value;
            int number_attacks = (int)numericUpDown2.Value;
            int number_servers = (int)numericUpDown3.Value;

            if (attack_success > 1 | attack_success < 0 | number_attacks <= 0 | number_servers <= 0)
            {
                MessageBox.Show("Selected invalid values_lines, exiting...");
                Environment.Exit(-1);
            }

            myTimer = new System.Timers.Timer();
            myTimer.Interval = 500;
            myTimer.Elapsed += myElapsed;
            myTimer.AutoReset = true;
            myTimer.Start();

            int[] breached = createAttackGraphs_Panel(attack_success, number_attacks, number_servers);
            createAbsoluteFrequencyGraph(breached);
            createRelativeFrequencyGraph(breached, number_servers);
            createRatioFrequencyGraph(breached, number_servers);

            myTimer.Stop();

        }



        private void myElapsed(object sender, ElapsedEventArgs e)
        {
            curr++;
        }

        private void myRefresh()
        {
            if (status1)
            {
                this.line_attacks.DrawToBitmap(this.b_attacks, new Rectangle(0, 0, panel1.Width / 2, panel1.Height));
                this.histogram_attacks.DrawToBitmap(this.b_attacks, new Rectangle(panel1.Width / 2, 0, panel1.Width, panel1.Height));
                this.panel1.BackgroundImage = this.b_attacks;
                this.panel1.BackgroundImageLayout = ImageLayout.Stretch;
                this.panel1.Refresh();
            }
            if (status2)
            {
                this.absolute_line.DrawToBitmap(this.b_absolute, new Rectangle(0, 0, panel2.Width / 2, panel2.Height));
                this.absolute_histogram.DrawToBitmap(this.b_absolute, new Rectangle(panel2.Width / 2, 0, panel2.Width, panel2.Height));
                this.panel2.BackgroundImage = this.b_absolute;
                this.panel2.BackgroundImageLayout = ImageLayout.Stretch;
                this.panel2.Refresh();
            }
            if (status3)
            {
                this.relative_line.DrawToBitmap(this.b_relative, new Rectangle(0, 0, panel3.Width / 2, panel3.Height));
                this.relative_histogram.DrawToBitmap(this.b_relative, new Rectangle(panel3.Width / 2, 0, panel3.Width, panel3.Height));
                this.panel3.BackgroundImage = this.b_relative;
                this.panel3.BackgroundImageLayout = ImageLayout.Stretch;
                this.panel3.Refresh();
            }
            if (status4)
            {
                this.ratio_line.DrawToBitmap(this.b_ratio, new Rectangle(0, 0, panel4.Width / 2, panel4.Height));
                ratio_histogram.DrawToBitmap(this.b_ratio, new Rectangle(panel4.Width / 2, 0, panel4.Width, panel4.Height));
                this.panel4.BackgroundImage = this.b_ratio;
                this.panel4.BackgroundImageLayout = ImageLayout.Stretch;
                this.panel4.Refresh();
            }
            prev = curr;
        }



        private int[] createAttackGraphs_Panel(float attack_success, int number_attacks, int number_servers)
        {
            status1 = true;

            // Create the Chart for the security score 
            line_attacks = new Chart();
            line_attacks.Width = panel1.Width / 2;
            line_attacks.Height = panel1.Height;
            line_attacks.Titles.Add("SECURITY SCORE CHART");

            ChartArea chartArea = new ChartArea("SecurityChart");
            chartArea.AxisX.Title = "Security Score";
            chartArea.AxisY.Title = "Attacks";
            chartArea.AxisX.Minimum = 0;
            line_attacks.ChartAreas.Add(chartArea);


            // Create the Chart for the histogram
            histogram_attacks = new Chart();
            histogram_attacks.Width = (panel1.Width / 2);
            histogram_attacks.Height = panel1.Height;
            histogram_attacks.Titles.Add("SECURITY SCORE HISTOGRAM");

            ChartArea histogramArea = new ChartArea("HistogramChart");
            histogramArea.AxisX.Title = "Security Score";
            histogramArea.AxisY.Title = "N";
            histogramArea.AxisX.MajorGrid.LineWidth = 0;
            histogramArea.AxisY.MajorGrid.LineWidth = 0;
            histogram_attacks.ChartAreas.Add(histogramArea);


            // Create bitmap that will contain both charts
            b_attacks = new Bitmap(panel1.Width, panel1.Height);


            // Variables for the security score chart
            Series[] lines_array = new Series[number_servers];
            int[] values_lines = new int[number_servers];
            Random rnd = new Random();
            int[] breached = new int[number_attacks+1];


            // Initialization of security score chart
            for (int i = 0; i < lines_array.Length; i++)
            {
                Series line_series = new Series();
                lines_array[i] = line_series;
                line_series.Color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                line_series.ChartType = SeriesChartType.Line;
                line_attacks.Series.Add(line_series);
                line_series.Points.AddXY(0, 0);
                values_lines[i] = 0;
            }

            
            // Simulation to draw chart
            for (int i = 1; i < number_attacks + 1; i++)
            {
                for (int j = 0; j < number_servers; j++)
                {
                    Series series = lines_array[j];

                    if (rnd.NextDouble() < attack_success)
                    {
                        series.Points.AddXY(i, values_lines[j] - 1);
                        values_lines[j] -= 1;
                        breached[i] += 1;
                    }
                    else
                    {
                        series.Points.AddXY(i, values_lines[j] + 1);
                        values_lines[j] += 1;
                    }

                    line_attacks.Series.Append(series);
                    line_attacks.Update();
                }
                if (prev != curr) myRefresh();
            }
            myRefresh();
            


            Array.Sort(values_lines);
            int current = Int32.MinValue;
            for (int i = 0; i < values_lines.Length; i++)
            {
                if (values_lines[i] == current) continue;
                current = values_lines[i];
                Series histogram_series = new Series();
                histogram_series.ChartType = SeriesChartType.Column;
                histogram_series.Color = Color.Red;
                histogram_series["PointWidth"] = "1";
                histogram_series.SmartLabelStyle.IsMarkerOverlappingAllowed = false;
                histogram_series.Points.Add(new DataPoint(current, values_lines.Count(s => s == current)));
                histogram_attacks.Series.Add(histogram_series);
                histogram_attacks.Update();
                if (prev != curr) myRefresh();
            }
            myRefresh();

            status1 = false;

            return breached;

        }


        private void createAbsoluteFrequencyGraph(int[] breached)
        {
            status2 = true;

            // Create the Chart for the security score 
            absolute_line = new Chart();
            absolute_line.Width = panel2.Width / 2;
            absolute_line.Height = panel2.Height;
            absolute_line.Titles.Add("ABSOLUTE FREQUENCY CHART");

            ChartArea chartArea = new ChartArea("SecurityChart");
            chartArea.AxisX.Title = "Attacks";
            chartArea.AxisY.Title = "Cumulative Frequency";
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisX.Minimum = 0;
            absolute_line.ChartAreas.Add(chartArea);


            // Create the Chart for the histogram
            absolute_histogram = new Chart();
            absolute_histogram.Width = (panel2.Width / 2);
            absolute_histogram.Height = panel2.Height;
            absolute_histogram.Titles.Add("ABSOLUTE FREQUENCY HISTOGRAM");

            ChartArea histogramArea = new ChartArea("HistogramChart");
            histogramArea.AxisX.Title = "N";
            histogramArea.AxisY.Title = "Cumulative Frequency";
            histogramArea.AxisX.MajorGrid.LineWidth = 0;
            histogramArea.AxisY.MajorGrid.LineWidth = 0;
            absolute_histogram.ChartAreas.Add(histogramArea);


            // Create bitmap that will contain both charts
            b_absolute= new Bitmap(panel2.Width, panel2.Height);


            // Variables for the security score chart
            Series line = new Series();
            line.Color = Color.Blue;
            line.ChartType = SeriesChartType.Line;
            line.Points.AddXY(0, 0);
            absolute_line.Series.Add(line);


            int conta = 0;
            // Simulation to draw chart
            for (int i = 1; i < breached.Length; i++)
            {
                conta += breached[i];
                line.Points.AddXY(i, conta);
                absolute_line.Series.Append(line);
                absolute_line.Update();
                if (prev != curr) myRefresh();
            }
            myRefresh();


            conta = 0;
            int current = Int32.MinValue;
            Array.Sort(breached);
            for (int i = 1; i < breached.Length; i++)
            {
                if (breached[i] == current) continue;
                current = breached[i];
                conta = breached.Count(s => s == current);
                if (current == 0) conta--;
                Series histogram_series = new Series();
                histogram_series.ChartType = SeriesChartType.Column;
                histogram_series["PointWidth"] = "1";
                histogram_series.Color = Color.Blue;
                histogram_series.SmartLabelStyle.IsMarkerOverlappingAllowed = false;
                histogram_series.Points.Add(new DataPoint(current, conta));
                absolute_histogram.Series.Add(histogram_series);
                absolute_histogram.Update();
                if (prev != curr) myRefresh();
            }
            myRefresh();
            
            
            status2 = false;

        }


        private void createRelativeFrequencyGraph(int[] breached, int number_servers)
        {
            status3 = true;

            // Create the Chart for the security score 
            relative_line = new Chart();
            relative_line.Width = panel3.Width / 2;
            relative_line.Height = panel3.Height;
            relative_line.Titles.Add("RELATIVE FREQUENCY CHART");

            ChartArea chartArea = new ChartArea("SecurityChart");
            chartArea.AxisX.Title = "Attacks";
            chartArea.AxisY.Title = "Relative Frequency";
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisX.Minimum = 0;
            relative_line.ChartAreas.Add(chartArea);


            // Create the Chart for the histogram
            relative_histogram = new Chart();
            relative_histogram.Width = (panel3.Width / 2);
            relative_histogram.Height = panel3.Height;
            relative_histogram.Titles.Add("RELATIVE FREQUENCY HISTOGRAM");

            ChartArea histogramArea = new ChartArea("HistogramChart");
            histogramArea.AxisX.Title = "N";
            histogramArea.AxisY.Title = "Relative Frequency";
            histogramArea.AxisX.MajorGrid.LineWidth = 0;
            histogramArea.AxisY.MajorGrid.LineWidth = 0;
            relative_histogram.ChartAreas.Add(histogramArea);


            // Create bitmap that will contain both charts
            b_relative = new Bitmap(panel3.Width, panel3.Height);


            // Variables for the security score chart
            Series line = new Series();
            line.Color = Color.Green;
            line.ChartType = SeriesChartType.Line;
            line.Points.AddXY(0, 0);
            relative_line.Series.Add(line);

            int conta = 0;
            for (int i = 1; i < breached.Length; i++)
            {
                conta += breached[i];
                line.Points.AddXY(i, (double)conta/(double)((i)*number_servers));
                line["PointWidth"] = "5";
                relative_line.Series.Append(line);
                relative_line.Update();
                if (prev != curr) myRefresh();
            }
            myRefresh();

            
            conta = 0;
            for (int i = 1; i < breached.Length; i++)
            {
                conta += breached[i];
                Series histogram_series = new Series();
                histogram_series.ChartType = SeriesChartType.Column;
                histogram_series["PointWidth"] = "1";
                histogram_series.Color = Color.Green;
                histogram_series.Points.Add(new DataPoint(i, (double)conta / (double)((i) * number_servers)));
                relative_histogram.Series.Add(histogram_series);
                relative_histogram.Update();
                if (prev != curr) myRefresh();
            }
            myRefresh();
            

            status3 = false;
        }


        private void createRatioFrequencyGraph(int[] breached, int number_servers)
        {
            status4 = true;

            // Create the Chart for the security score 
            ratio_line = new Chart();
            ratio_line.Width = panel4.Width / 2;
            ratio_line.Height = panel4.Height;
            ratio_line.Titles.Add("RATIO FREQUENCY CHART");

            ChartArea chartArea = new ChartArea("SecurityChart");
            chartArea.AxisX.Title = "Attacks";
            chartArea.AxisY.Title = "Ratio Frequency";
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisX.Minimum = 0;
            ratio_line.ChartAreas.Add(chartArea);


            // Create the Chart for the histogram
            ratio_histogram = new Chart();
            ratio_histogram.Width = (panel4.Width / 2);
            ratio_histogram.Height = panel4.Height;
            ratio_histogram.Titles.Add("RATIO FREQUENCY HISTOGRAM");

            ChartArea histogramArea = new ChartArea("HistogramChart");
            histogramArea.AxisX.Title = "N";
            histogramArea.AxisY.Title = "Ratio Frequency";
            histogramArea.AxisX.MajorGrid.LineWidth = 0;
            histogramArea.AxisY.MajorGrid.LineWidth = 0;
            ratio_histogram.ChartAreas.Add(histogramArea);


            // Create bitmap that will contain both charts
            b_ratio = new Bitmap(panel4.Width, panel4.Height);


            // Variables for the security score chart
            Series line = new Series();
            line.Color = Color.Violet;
            line.ChartType = SeriesChartType.Line;
            line.Points.AddXY(0, 0);
            ratio_line.Series.Add(line);


            int conta = 0;
            for (int i = 1; i < breached.Length; i++)
            {
                conta += breached[i];
                line.Points.AddXY(i, (double)conta / Math.Sqrt((double)((i) * number_servers)));
                ratio_line.Series.Append(line);
                ratio_line.Update();

                if (prev != curr) myRefresh();
            }
            myRefresh();
            
            conta = 0;
            for (int i = 1; i < breached.Length; i++)
            {
                conta += breached[i];
                Series histogram_series = new Series();
                histogram_series.ChartType = SeriesChartType.Column;
                histogram_series["PointWidth"] = "1";
                histogram_series.Color = Color.Violet;
                histogram_series.Points.Add(new DataPoint(i, (double)conta / Math.Sqrt((double)((i) * number_servers))));
                ratio_histogram.Series.Add(histogram_series);
                ratio_histogram.Update();
                
                if (prev != curr) myRefresh();
            }
            myRefresh();
            
            status4 = false;

        }



        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            this.panel1.BringToFront();
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursorPosition = e.Location;
            }

            if (e.Button == MouseButtons.Right)
            {
                isResizing = true;
                resizeStart = e.Location;
                originalSize = this.panel1.Size;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - lastCursorPosition.X;
                int deltaY = e.Y - lastCursorPosition.Y;
                panel1.Left += deltaX;
                panel1.Top += deltaY;
            }

            if (isResizing)
            {
                int deltaX = e.X - resizeStart.X;
                int deltaY = e.Y - resizeStart.Y;

                // Calcola la nuova dimensione del pannello in base allo spostamento del mouse.
                int newWidth = originalSize.Width + deltaX;
                int newHeight = originalSize.Height + deltaY;

                if (newWidth > 0 && newHeight > 0)
                {
                    this.panel1.Size = new Size(newWidth, newHeight);
                }
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            isResizing = false;

        }


        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            this.panel2.BringToFront();
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursorPosition = e.Location;
            }

            if (e.Button == MouseButtons.Right)
            {
                isResizing = true;
                resizeStart = e.Location;
                originalSize = this.panel2.Size;
            }
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - lastCursorPosition.X;
                int deltaY = e.Y - lastCursorPosition.Y;
                panel2.Left += deltaX;
                panel2.Top += deltaY;
            }

            if (isResizing)
            {
                int deltaX = e.X - resizeStart.X;
                int deltaY = e.Y - resizeStart.Y;

                // Calcola la nuova dimensione del pannello in base allo spostamento del mouse.
                int newWidth = originalSize.Width + deltaX;
                int newHeight = originalSize.Height + deltaY;

                if (newWidth > 0 && newHeight > 0)
                {
                    this.panel2.Size = new Size(newWidth, newHeight);
                }
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            isResizing = false;

        }


        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            this.panel3.BringToFront();
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursorPosition = e.Location;
            }

            if (e.Button == MouseButtons.Right)
            {
                isResizing = true;
                resizeStart = e.Location;
                originalSize = this.panel3.Size;
            }
        }

        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - lastCursorPosition.X;
                int deltaY = e.Y - lastCursorPosition.Y;
                panel3.Left += deltaX;
                panel3.Top += deltaY;
            }

            if (isResizing)
            {
                int deltaX = e.X - resizeStart.X;
                int deltaY = e.Y - resizeStart.Y;

                // Calcola la nuova dimensione del pannello in base allo spostamento del mouse.
                int newWidth = originalSize.Width + deltaX;
                int newHeight = originalSize.Height + deltaY;

                if (newWidth > 0 && newHeight > 0)
                {
                    this.panel3.Size = new Size(newWidth, newHeight);
                }
            }
        }

        private void panel3_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            isResizing = false;

        }


        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            this.panel4.BringToFront();
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursorPosition = e.Location;
            }

            if (e.Button == MouseButtons.Right)
            {
                isResizing = true;
                resizeStart = e.Location;
                originalSize = this.panel4.Size;
            }
        }

        private void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - lastCursorPosition.X;
                int deltaY = e.Y - lastCursorPosition.Y;
                panel4.Left += deltaX;
                panel4.Top += deltaY;
            }

            if (isResizing)
            {
                int deltaX = e.X - resizeStart.X;
                int deltaY = e.Y - resizeStart.Y;

                // Calcola la nuova dimensione del pannello in base allo spostamento del mouse.
                int newWidth = originalSize.Width + deltaX;
                int newHeight = originalSize.Height + deltaY;

                if (newWidth > 0 && newHeight > 0)
                {
                    this.panel4.Size = new Size(newWidth, newHeight);
                }
            }
        }

        private void panel4_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            isResizing = false;

        }
    }
}
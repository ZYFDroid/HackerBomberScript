using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChartsTest
{
    public partial class MonitorChart : UserControl
    {

        private float value = 0;

        public MonitorChart()
        {
            InitializeComponent();
        }

        public float Value { get => value; set => this.value = value; }
        public float Max { get => max; set => max = value; }
        public float Min { get => min; set => min = value; }
        public bool LockMaxMin { get => lockMaxMin; set => lockMaxMin = value; }
        public bool StockMode { get => _StockMode; set => _StockMode = value; }

        bool _StockMode = false;

        Graphics graphics = null;
        Graphics buffer = null;
        Bitmap bufferimg = null;

        float smoothedvalue = 0;

        float velotry = 0;

        float velotry2 = 0;

        float deltavelotry = 0;

        float smoothfactor = 0.16f;

    

        List<float> values = new List<float>();

        float horLineInterval = 32;
        float horLineOffset = 0;

        Pen line = new Pen(Brushes.Lime, 3);
        Brush brush = Brushes.Lime;

        Pen linedn = new Pen(Brushes.Lime, 3);
        Brush brushdn = Brushes.Lime;

        Pen lineup = new Pen(Brushes.Red, 3);
        Brush brushup = Brushes.Red;

        Pen bgline = Pens.Gray;
        Pen fgline = new Pen(Brushes.White, 3);

        private void MonitorChart_Load(object sender, EventArgs e)
        {
            values.Add(0);
            values.Add(0);
            values.Add(0);
            graphics = Graphics.FromHwnd(Handle);
            bufferimg = new Bitmap(Width, Height);
            buffer = Graphics.FromImage(bufferimg);
            buffer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        float max = 1;
        float min = 0;

        bool lockMaxMin = false;

        float rightPadding = 64;

        Font f = SystemFonts.DefaultFont;

        private void Timer1_Tick(object sender, EventArgs e)
        {
            float delta = Max - Min;
            if (!lockMaxMin)
            {
                Max = values.Max();
                Min = values.Min();

                if (Max - Min <= 0.00001)
                {
                    Max += 1;
                    Min -= 1;
                }
               
            }
            if (null != buffer)
            {
                buffer.Clear(Color.Black);

                for (float i = 0; i <= 12; i++)
                {
                    float liney = (float)(Height - 1) / 12f * i;
                    buffer.DrawLine(bgline, 0, liney, Width, liney);
                    if ((i - 1) % 10 == 0) {
                        buffer.DrawLine(fgline, 0, liney, Width, liney);
                    }
                }

                float hlines = ((Width) / horLineInterval);

                for (float i = 0; i <= hlines + 3; i++)
                {
                    float linex = (float)(Width - 1) / hlines * i - (float)horLineOffset;
                    buffer.DrawLine(Pens.Gray, linex, 0, linex, Height);

                }
                horLineOffset += 1f;
                if (horLineOffset > horLineInterval) { horLineOffset -= horLineInterval; }
                float dx = 0, dy = 0;
                float lx = 0, ly = 0;

                for (int i = 0; i < values.Count; i++)
                {
                    dx = Width-rightPadding - values.Count + i;
                    float tdy= Height - Height * ((values[i] - Min)) / delta ;
                    dy = tdy/12f*10f+ Height /12f;
                    if (lx == 0 && ly == 0) { lx = dx; ly = dy; }
                    if (_StockMode)
                    {
                        if (ly < dy)
                        {
                            line = linedn;
                            brush = brushdn;
                        }
                        else
                        {
                            line = lineup;
                            brush = brushup;
                        }
                    }
                    buffer.DrawLine(line, dx, dy, lx, ly);
                    lx = dx; ly = dy;
                }

                buffer.DrawString(value.ToString(), f, brush, lx+5, ly-f.Size/2f-1);
                buffer.FillEllipse(brush, lx - 4, ly - 4, 8, 8);

                buffer.DrawString((Max).ToString(), f, brush,Width-rightPadding, Height / 12f * 1f-f.Size-5);
                buffer.DrawString((Min).ToString(), f, brush, Width - rightPadding, Height / 12f * 11f+5);

                velotry = (value - smoothedvalue)*smoothfactor;

                deltavelotry += (velotry - deltavelotry) * smoothfactor;

                velotry2 = deltavelotry;
                smoothedvalue += velotry2;


                if (Math.Abs(smoothedvalue - value) < 0.000001f)
                {
                    smoothedvalue = value;
                }

                values.Add(smoothedvalue);
                while (values.Count > (Width - rightPadding))
                {
                    values.RemoveAt(0);
                }
            }
            buffer.Flush();
            graphics.DrawImage(bufferimg, 0, 0);

            //if (DesignMode) { timer1.Enabled = false; }
        }

        private void MonitorChart_Resize(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (null != buffer) { buffer.Dispose(); }
            if (null != bufferimg) { bufferimg.Dispose(); }
            if (null != graphics) { graphics.Dispose(); }
            buffer = null;
            bufferimg = new Bitmap(Width, Height);
            buffer = Graphics.FromImage(bufferimg);
            buffer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics = Graphics.FromHwnd(Handle);
            timer1.Enabled = true;
        }

        private void MonitorChart_SizeChanged(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (null != buffer) { buffer.Dispose(); }
            if (null != bufferimg) { bufferimg.Dispose(); }
            if (null != graphics) { graphics.Dispose(); }
            buffer = null;
            bufferimg = new Bitmap(Width, Height);
            buffer = Graphics.FromImage(bufferimg);
            buffer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics = Graphics.FromHwnd(Handle);
            timer1.Enabled = true;
        }

        private void MonitorChart_Paint(object sender, PaintEventArgs e)
        {
            timer1.Enabled = true;
        }
    }
}

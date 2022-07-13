using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASDumpAnalyzer
{
    public partial class StackProgressBar : UserControl
    {
        private int m_frameCount = 15;
        private int m_frameGap = 3;
        private Brush m_frameBrush = Brushes.Blue;
        private Brush m_activeFrameBrush = Brushes.Red;
        private int m_activeFrame = 2;

        public StackProgressBar()
        {
        }

        public double Progress 
        {
            get 
            {
                if (this.FrameCount > 0)
                    return ((double)this.ActiveFrame) / this.FrameCount;
                else 
                    return 0;
            }
            set 
            {
                if (value > 1 || value < 0)
                    throw new ArgumentException("The specified Progress values should be between 0 and 1");

                this.ActiveFrame = (int)Math.Floor(value * this.FrameCount);
            }
        }

        public Brush FrameBrush
        {
            get 
            {
                return this.m_frameBrush;
            }
            set 
            {
                if (this.m_frameBrush != value) 
                {
                    this.m_frameBrush = value;
                    this.Invalidate();
                }
            }
        }

        public Brush ActiveFrameBrush
        {
            get 
            {
                return this.m_activeFrameBrush;
            }
            set 
            {
                if (this.m_activeFrameBrush != value) 
                {
                    this.m_activeFrameBrush = value;
                    this.Invalidate();
                }
            }
        }

        public int FrameCount
        {
            get 
            {
                return this.m_frameCount;
            }
            set
            {
                if (value != this.m_frameCount)
                {
                    this.m_frameCount = value;
                    this.Invalidate();
                }
            }
        }

        public int FrameGap 
        {
            get 
            {
                return this.m_frameGap;
            }
            set 
            {
                if (this.m_frameGap != value) 
                {
                    this.m_frameGap = value;
                    this.Invalidate();
                }
            }
        }


        public int ActiveFrame 
        {
            get 
            {
                return this.m_activeFrame;
            }
            set 
            {
                if (this.m_activeFrame != value) 
                {
                    int oldActiveFrame = this.m_activeFrame;
                    this.m_activeFrame = value;

                    this.SuspendLayout();
                    this.Invalidate(this.GetFrameRectangle(oldActiveFrame));
                    this.Invalidate(this.GetFrameRectangle(m_activeFrame));
                    this.ResumeLayout();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        private Rectangle GetFrameRectangle(int frameNumber) 
        {
            if (0 == FrameCount)
                return Rectangle.Empty;

            double height = this.Height + this.FrameGap;
            double top = frameNumber * height / FrameCount;
            double bottom = (frameNumber + 1) * height / FrameCount;

            bottom -= FrameGap;
            double frameHeight = bottom - top;
            
            return new Rectangle(0, (int)Math.Round(top), this.Size.Width, (int)Math.Floor(frameHeight));

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics g = e.Graphics;

            g.Clear(this.BackColor);



            for (int i = 0; i < this.FrameCount; i++) 
            {
                Brush b = this.FrameBrush;
                if (i == this.m_activeFrame)
                    b = this.ActiveFrameBrush;

                g.FillRectangle(b, this.GetFrameRectangle(i));
            }
        }
    }
}

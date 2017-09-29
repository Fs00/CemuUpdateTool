using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace CemuUpdateTool
{
    // Custom progress bar class originally made by [] and adapted for the program.
    // Needed to display "Current file" text without any bugs.

    public enum ProgressBarDisplayText
    {
        Percentage,
        CustomText
    }

    class CustomProgressBar : ProgressBar
    {
        public ProgressBarDisplayText DisplayStyle { get; set; }    // Property to set to decide whether to print a % or Text
        public String CustomText { get; set; }                      // Property to hold the custom text

        public CustomProgressBar()
        {
            // Modify the ControlStyles flags
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        [DllImportAttribute("uxtheme.dll")]
        private static extern int SetWindowTheme(IntPtr hWnd, string appname, string idlist);

        protected override void OnHandleCreated(EventArgs e)
        {
            SetWindowTheme(Handle, "", "");
            base.OnHandleCreated(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            ProgressBarRenderer.DrawHorizontalBar(g, rect);
            if (Value > 0)
            {
                // As we doing this ourselves we need to draw the chunks on the progress bar
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);
                ProgressBarRenderer.DrawHorizontalChunks(g, clip);
            }

            // Set the Display text (either a % amount or our custom text)
            string text = DisplayStyle == ProgressBarDisplayText.Percentage ? Value.ToString() + '%' : CustomText;

            // Calculate the location of the text (the middle of progress bar)
            SizeF len = g.MeasureString(text, SystemFonts.DefaultFont);
            // Point location = new Point(Convert.ToInt32((rect.Width / 2) - (len.Width / 2)), Convert.ToInt32((rect.Height / 2) - (len.Height / 2)));   // centre the text into the highlighted area only
            // Point location = new Point(Convert.ToInt32((Width / 2) - len.Width / 2), Convert.ToInt32((Height / 2) - len.Height / 2));                 // centre the text regardless of the highlighted area
            Point location = new Point(6, Convert.ToInt32((Height / 2) - len.Height / 2));

            // Draw the custom text
            g.DrawString(text, SystemFonts.DefaultFont, Brushes.Black, location);
        }

        public void SetCustomText(string text)
        {
            CustomText = text;
            Refresh();
        }
    }
}

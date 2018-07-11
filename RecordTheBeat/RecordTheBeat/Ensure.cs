using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordTheBeat
{
    public static class Ensure // *Ensures* that the text is readable
    {
        public static void DrawString(Bitmap bmp, Graphics grph, string text, Font font, Point xy, StringFormat format)
        {
            SizeF sf = grph.MeasureString(text, font, xy, format);

            //https://stackoverflow.com/questions/1068373/how-to-calculate-the-average-rgb-color-values-of-a-bitmap
            Rectangle rect = new Rectangle((int)(xy.X - (sf.Width / 2)), (int)(xy.Y - (sf.Height)), (int)sf.Width, (int)sf.Height);
            
            if(rect.X < 0)
            {
                rect.X = 0;
            } else if(rect.Y < 0)
            {
                rect.Y = 0;
            } else if(rect.Bottom > bmp.Height)
            {
                rect.Height = bmp.Height - rect.Y;
            } else if (rect.Right > bmp.Width)
            {
                rect.Width = bmp.Width - rect.X;
            }

            BitmapData srcData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = srcData.Stride;

            IntPtr Scan0 = srcData.Scan0;

            long[] totals = new long[] { 0, 0, 0 };

            int width = srcData.Width;
            int height = srcData.Height;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int color = 0; color < 3; color++)
                        {
                            int idx = (y * stride) + x * 4 + color;

                            totals[color] += p[idx];
                        }
                    }
                }
            }

            bmp.UnlockBits(srcData);

            int avgB = (int)(totals[0] / (width * height));
            int avgG = (int)(totals[1] / (width * height));
            int avgR = (int)(totals[2] / (width * height));

            //https://stackoverflow.com/questions/1855884/determine-font-color-based-on-background-color
            double a = 1 - (0.299 * avgR + 0.587 * avgG + 0.114 * avgB) / 255;

            SolidBrush brush;
            if(a < 0.5)
            {
                brush = new SolidBrush(Color.FromArgb(0, 0, 0));
            }
            else
            {
                brush = new SolidBrush(Color.FromArgb(255, 255, 255));
            }

            grph.DrawString(text, font, brush, xy, format);
        }
    }
}

using System;
using Android.Graphics;

namespace OdborkyApp.Tools
{
    public static class BitmapEditor
    {
        private static Bitmap DrawProgressLinear(Bitmap original, int steps, int finished)
        {
            int[] buffer = new int[original.Width * original.Height];
            original.GetPixels(buffer, 0, original.Width, 0, 0, original.Width, original.Height);

            var segmentHeight = original.Height / steps;
            var segmentWidth = original.Width;

            for (int segmentId = 0; segmentId < steps; segmentId++)
            {
                for (int y = 0; y < segmentHeight; y++)
                {
                    for (int x = 0; x < segmentWidth; x++)
                    {
                        Color pixel =
                            new Color(buffer[x + y * segmentWidth + segmentId * segmentHeight * segmentWidth]);

                        if (segmentId < steps - finished)
                        {
                            int greyscale = (int) (pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                            var newColor = new Color(greyscale, greyscale, greyscale, pixel.A);
                            buffer[x + y * segmentWidth + segmentId * segmentHeight * segmentWidth] =
                                newColor.ToArgb();
                        }
                        else
                        {
                            buffer[x + y * segmentWidth + segmentId * segmentHeight * segmentWidth] =
                                pixel.ToArgb();
                        }
                    }
                }
            }

            var result = Bitmap.CreateBitmap(original.Width, original.Height, Bitmap.Config.Argb8888);

            result.SetPixels(buffer, 0, original.Width, 0, 0, original.Width, original.Height);

            return result;
        }

        private static Bitmap DrawProgressCircular(Bitmap original, int steps, int finished)
        {
            int[] buffer = new int[original.Width * original.Height];
            original.GetPixels(buffer, 0, original.Width, 0, 0, original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixel = new Color(buffer[x + y * original.Width]);

                    var p = CoordConvertor.ToPolar(
                        x - original.Width / 2, original.Height / 2 - y);
                    if ((p.Angle > 0 ? p.Angle : Math.PI * 2 + p.Angle) > Math.PI * 2 / steps * finished)
                    {
                        int greyscale = (int) (pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                        var newColor = new Color(greyscale, greyscale, greyscale, pixel.A);
                        buffer[x + y * original.Width] = newColor.ToArgb();
                    }
                    else
                    {
                        buffer[x + y * original.Width] = pixel.ToArgb();
                    }
                }
            }

            var result = Bitmap.CreateBitmap(original.Width, original.Height, Bitmap.Config.Argb8888);

            result.SetPixels(buffer, 0, original.Width, 0, 0, original.Width, original.Height);

            return result;
        }

        private static class CoordConvertor
        {
            public static CarthesianCoordinations ToCarthesian(PolarCoordinations p)
            {
                return new CarthesianCoordinations
                {
                    X = (int) Math.Round(p.Radius * Math.Cos(p.Angle)),
                    Y = (int) Math.Round(p.Radius * Math.Sin(p.Angle))
                };
            }

            public static CarthesianCoordinations ToCarthesian(int angle, int radius)
            {
                return ToCarthesian(new PolarCoordinations
                {
                    Angle = angle,
                    Radius = radius,
                });
            }

            public static PolarCoordinations ToPolar(CarthesianCoordinations c)
            {
                return new PolarCoordinations
                {
                    Radius = Math.Sqrt(Math.Pow(c.X, 2) + Math.Pow(c.Y, 2)),
                    Angle = Math.Atan2(c.Y, c.X)
                };
            }

            public static PolarCoordinations ToPolar(int x, int y)
            {
                return ToPolar(new CarthesianCoordinations
                {
                    X = x,
                    Y = y
                });
            }
        }

        private class CarthesianCoordinations
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        private class PolarCoordinations
        {
            public double Angle { get; set; }
            public double Radius { get; set; }
        }
    }
}
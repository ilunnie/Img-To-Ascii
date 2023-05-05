using System.IO;
using static System.Console;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

string input = "rick.gif";
AsciiGenerate.Proportion(input, 300);

WriteLine("Iniciando...");
while(true)
    AsciiGenerate.ToAscii(input, "output.txt");


public static class AsciiGenerate
{
    public static string Gscale { get; set; } = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\| ()1 {} []?-_+~i!lI;:,\\\"^`\". ";
    public static int Width { get; set; } = 300;
    public static int Height { get; set; } = 100;
    public static int FPS { get; set; } = 24;
    // public static Process[] Notepad { get; set; } = new Process[2];

    public static void Proportion(string reference, int width)
    {
        Image originalImage = Image.FromFile(reference);
        AsciiGenerate.Width = width;
        AsciiGenerate.Height = (int)(width * (float)originalImage.Height / originalImage.Width);
    }

    public static void ToAscii(string input, string output)
    {
        Image gifImg = Image.FromFile(input);
        FrameDimension dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
        int frameCount = gifImg.GetFrameCount(dimension);
        Bitmap[] frames = new Bitmap[frameCount];

        Bitmap resizedFrame = new Bitmap(Width, Height);
        using (Graphics g = Graphics.FromImage(resizedFrame))
        {
            for (int i = 0; i < frameCount; i++)
            {
                gifImg.SelectActiveFrame(dimension, i);
                frames[i] = new Bitmap(gifImg);
                g.Clear(Color.Black);
                g.DrawImage(frames[i], 0, 0, Width, Height);
                frames[i] = new Bitmap(resizedFrame);
            }
        }

        ToAscii(frames, output);
    }

    public static void ToAscii(Bitmap[] input, string output)
    {
        for (int i = 0; i < input.Length; i++)
        {
            // Notepad[1] = Process.Start("notepad.exe", output);
            ToAscii(input[i], output);
            Thread.Sleep(1000/FPS);
            // try{Notepad[0].Kill();} catch {}
            // Notepad[0] = Notepad[1];
        }
    }

    public static void ToAscii(Bitmap input, string output)
    {
        using (StreamWriter writer = new StreamWriter(output))
        {
            BitmapData data = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int stride = data.Stride;
            IntPtr Scan0 = data.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - input.Width * 3;
                for (int y = 0; y < input.Height; ++y)
                {
                    for (int x = 0; x < input.Width; ++x)
                    {
                        int luminosity = (int)((p[2] * 0.3) + (p[1] * 0.59) + (p[0] * 0.11));
                        int index = (int)(luminosity / 255.0 * (Gscale.Length - 1));
                        writer.Write(Gscale[index]);
                        p += 3;
                    }
                    writer.WriteLine();
                    p += nOffset;
                }
            }
            input.UnlockBits(data);
        }
    }
}

//Process.Start("notepad++.exe", output);
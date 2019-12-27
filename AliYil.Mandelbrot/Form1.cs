using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AliYil.Mandelbrot
{
    public partial class MandelbrotViewerForm : Form
    {
        private Mandelbrot _mandelbrot;
        private Bitmap _bitmap;
        private const int w = 900;
        private const int h = 900;

        private double _zoom = 5;
        private double camPosX = 0;
        private double camPosY = 0;

        private int colorOffset = 0;

        public MandelbrotViewerForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _bitmap = new Bitmap(w, h);
            _mandelbrot = new Mandelbrot();
            pictureBox1.Image = _bitmap;

            UpdateMandelbrot();
        }

        private void UpdateMandelbrot()
        {
            var rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            var data = _bitmap.LockBits(rect, ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            var depth = Image.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel
            var buffer = new byte[data.Width * data.Height * depth];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            double max = 0;
            double avg = 0;
            var halfW = w / 2;
            var halfH = h / 2;
            Parallel.For(0, w * h, i =>
            {
                var x = i % w;
                var y = i / h;

                var mX = (double)(x - halfW) / w * _zoom + camPosX;
                var mY = (double)(y - halfH) / h * -_zoom + camPosY;
                var calc = _mandelbrot.Calculate(mX, mY);

                var bitmapOffset = (y * w + x) * depth;

                if (calc == 1)
                {
                    buffer[bitmapOffset] = 0;
                    buffer[bitmapOffset + 1] = 0;
                    buffer[bitmapOffset + 2] = 0;
                    //buffer[bitmapOffset] = 255;
                    //buffer[bitmapOffset + 1] = 255;
                    //buffer[bitmapOffset + 2] = 255;
                }
                else
                {
                    var hslColor = new HslColor((calc * 250 + colorOffset) % 360, 200, 20 + Math.Sin(calc) * 340);
                    Color col = hslColor;
                    buffer[bitmapOffset] = col.R;
                    buffer[bitmapOffset + 1] = col.G;
                    buffer[bitmapOffset + 2] = col.B;
                    //buffer[bitmapOffset] = 255;
                    //buffer[bitmapOffset + 1] = 255;
                    //buffer[bitmapOffset + 2] = 255;
                }

                buffer[bitmapOffset + 3] = 255;
            });
            Console.WriteLine($@"Max: {max} - Avg: {avg}");

            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            _bitmap.UnlockBits(data);

            pictureBox1.Image = _bitmap;
            pictureBox1.Update();

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var speed = 0.1;
            switch (e.KeyCode)
            {
                case Keys.Q:
                    _mandelbrot.TotalIterations -= 10;
                    break;
                case Keys.W:
                    _mandelbrot.TotalIterations += 10;
                    break;

                case Keys.A:
                    _mandelbrot.Limit -= 10;
                    break;
                case Keys.S:
                    _mandelbrot.Limit += 10;
                    break;

                case Keys.Z:
                    colorOffset -= 5;
                    break;
                case Keys.X:
                    colorOffset += 5;
                    break;

                case Keys.Subtract:
                    _zoom *= 1.1;
                    break;
                case Keys.Add:
                    _zoom *= 0.9;
                    break;

                case Keys.Left:
                    camPosX -= speed * _zoom;
                    break;
                case Keys.Right:
                    camPosX += speed * _zoom;
                    break;
                case Keys.Up:
                    camPosY += speed * _zoom;
                    break;
                case Keys.Down:
                    camPosY -= speed * _zoom;
                    break;

                case Keys.Space:
                    _zoom = 5;
                    camPosX = 0;
                    camPosY = 0;
                    break;
            }

            UpdateMandelbrot();
        }
    }
}

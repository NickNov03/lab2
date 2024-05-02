using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transforms
{
    [Version(1, 0)]
    public class IncBrightness : IPlugin
    {
        public string Name => "Increase brightness";

        public string Author => "nick";

        public string Description => Name;

        public Bitmap Transform(Bitmap image)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = image.Width;
            int height = image.Height;
            float brightness = 1.5F;

            float[][] colorMatrixElements = {
                                            new float[] {brightness, 0, 0, 0, 0},
                                            new float[] {0, brightness, 0, 0, 0},
                                            new float[] {0, 0, brightness, 0, 0},
                                            new float[] {0, 0, 0, 1, 0},
                                            new float[] {0, 0, 0, 0, 1}
                                        };

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
                colorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);
            Graphics graphics = Graphics.FromImage(image);
            graphics.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, width,
                                height,
                                GraphicsUnit.Pixel,
                                imageAttributes);

            return image;
        }
    }
}

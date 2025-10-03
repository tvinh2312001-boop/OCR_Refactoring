using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DensoMTecGaugeReader.Core.Common
{
    public class ImageUtils
    {
        public static Bitmap MatToBitmap(Mat mat)
        {
            byte[] imageData = mat.ImEncode(".png");

            using MemoryStream ms = new MemoryStream(imageData);
            return new Bitmap(ms);
        }

        public static Mat ResizeImage(Mat image, double scalingFactor)
        {
            if (scalingFactor == 1)
            {
                return image;
            }
            // Get the dimensions of the original image
            int height = image.Rows;
            int width = image.Cols;

            // Calculate the new dimensions
            int newWidth = (int)(width * scalingFactor);
            int newHeight = (int)(height * scalingFactor);

            // Resize the image
            Mat resizedImage = new Mat();
            Cv2.Resize(image, resizedImage, new OpenCvSharp.Size(newWidth, newHeight), 0, 0, InterpolationFlags.Linear);

            return resizedImage;
        }

        public static Bitmap ResizeBitmap(Bitmap originalImage, int newWidth, int newHeight)
        {
            Bitmap resizedImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }
            return resizedImage;
        }

        public static Bitmap AddPaddingToBitmap(Bitmap originalImage, int padding)
        {
            int paddedWidth = originalImage.Width + 2 * padding;
            int paddedHeight = originalImage.Height + 2 * padding;
            Bitmap paddedImage = new Bitmap(paddedWidth, paddedHeight);

            using (Graphics g = Graphics.FromImage(paddedImage))
            {
                g.Clear(Color.Black);

                // Draw the resized image in the center, leaving padding on all sides
                g.DrawImage(originalImage, padding, padding);
            }
            return paddedImage;
        }
    }
}

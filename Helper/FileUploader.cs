using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class FileUploader
    {
        public enum ImageComperssion
        {
            Maximum = 50,
            Product = 60,
            Normal = 75,
            Fast = 80,
            Minimum = 90,
            None = 100,
        }

        public enum ImageWidth
        {
            Small = 350,
            Medium = 550,
            Large = 850,
            Standard = 1024,
            FullHD = 1920,
        }
        public enum ImageHeight
        {
            Small = 350,
            Medium = 550,
            Large = 850,
            Standard = 768,
            FullHD = 1080,
        }
        //10 mb
        const int _maxImageLength = 10;


        public static (bool succsseded, string result) UploadImage(IFormFile file, string path, int maxLength = _maxImageLength, int width = (int)ImageWidth.Medium, int height = (int)ImageHeight.Medium, int compression = (int)ImageComperssion.Normal)
        {

            if (!IsImageMimeTypeValid(file) || !IsImageExtentionValid(file))
            {
                return (false, "فرمت عکس صحیح نیست.");
            }

            if (!IsImageSizeValid(file, maxLength))
            {
                return (false, $"سایز عکس باید کمتر از {maxLength} باشد");
            }

            try
            {
                var image = Image.Load(file.OpenReadStream());
                var resizeOptions = new ResizeOptions()
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop
                };
                image.Mutate(x => x.Resize(resizeOptions));
                //var encoder = new JpegEncoder()
                //{
                //    Quality = compression
                //};
                var encoder = new PngEncoder()
                {
                    CompressionLevel=PngCompressionLevel.BestCompression,
                   
                };
                var fileName = GetRandomFileName(file);
                var savePath = Path.GetFullPath(Path.Combine(path, fileName));
                //image.Saveasp(savePath, encoder);
                image.SaveAsPng(savePath,encoder);




                return (true, fileName);
            }
            catch(Exception e)
            {
                return (false,e.Message);
            }

        }

        private static bool IsImageSizeValid(IFormFile image, int validLength = _maxImageLength)
        {
            if (image.Length > (validLength * 1024 * 1024))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsImageMimeTypeValid(IFormFile image)
        {
            string mimeType = image.ContentType.ToLower();
            if (mimeType != "image/jpg" &&
                 mimeType != "image/jpeg" &&
                 mimeType != "image/pjpeg" &&
                 mimeType != "image/gif" &&
                 mimeType != "image/x-png" &&
                 mimeType != "image/png")
            {
                return false;
            }
            return true;
        }

        private static string GetRandomFileName(IFormFile file)
        {
            return Guid.NewGuid() + Path.GetExtension(file.FileName).ToLower();
        }

        private static bool IsImageExtentionValid(IFormFile image)
        {
            string extention = Path.GetExtension(image.FileName).ToLower();

            if (extention != ".jpg"
                && extention != ".png"
                && extention != ".gif"
                && extention != ".jpeg")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsFileExtentionValid(IFormFile file)
        {
            string[] validExt = { ".jpg", ".gif", ".png", ".rar", ".pdf", ".zip", ".mp4", ".flv", ".avi", ".wmv", ".mp3", ".wav", ".aac", ".3gp", ".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx" };

            string extention = Path.GetExtension(file.FileName).ToLower();

            if (Array.IndexOf(validExt, extention) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

}

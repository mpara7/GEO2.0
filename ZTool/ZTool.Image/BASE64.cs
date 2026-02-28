using System.Drawing;
using System.Drawing.Imaging;

namespace ZTool.ImageTools
{
    public class BASE64
    {

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
        public static string ConvertPngBase64ToJpgBase64(string pngBase64, long quality = 80)
        {
            try
            {
                byte[] pngBytes = Convert.FromBase64String(pngBase64);

                using (MemoryStream pngStream = new MemoryStream(pngBytes))
                using (Image originalImage = Image.FromStream(pngStream))
                {
                    // 创建一个新的位图，使用白色背景  
                    using (Bitmap newBitmap = new Bitmap(originalImage.Width, originalImage.Height))
                    {
                        // 创建Graphics对象来绘制图像  
                        using (Graphics graphics = Graphics.FromImage(newBitmap))
                        {
                            // 设置白色背景  
                            graphics.Clear(Color.White);

                            // 在白色背景上绘制原始图像  
                            graphics.DrawImage(originalImage, 0, 0, originalImage.Width, originalImage.Height);

                            // 创建内存流来保存JPEG  
                            using (MemoryStream jpgStream = new MemoryStream())
                            {
                                // 设置JPEG编码器参数  
                                EncoderParameters encoderParameters = new EncoderParameters(1);
                                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                                // 获取JPEG编码器  
                                ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg);

                                // 保存为JPEG  
                                newBitmap.Save(jpgStream, jpegEncoder, encoderParameters);

                                // 转换为base64  
                                byte[] jpgBytes = jpgStream.ToArray();
                                return Convert.ToBase64String(jpgBytes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("转换过程中发生错误: " + ex.Message);
            }
        }
        public static class ImageValidator
        {
            public static class Examples
            {
                public static void DemoImageValidation()
                {
                    // Base64字符串示例  
                    string imageBase64 = "your_base64_string_here";

                    // 获取完整的图片信息  
                    ImageInfo imageInfo = ImageValidator.GetImageInfo(imageBase64);
                    Console.WriteLine($"Image Info: {imageInfo}");

                    // 检查是否为有效图片  
                    bool isValidImage = ImageValidator.IsImage(imageBase64);
                    Console.WriteLine($"Is valid image: {isValidImage}");

                    // 检查特定格式  
                    bool isPng = ImageValidator.IsImageType(imageBase64, ImageType.PNG);
                    Console.WriteLine($"Is PNG: {isPng}");

                    // 获取MIME类型  
                    string mimeType = ImageValidator.GetMimeType(imageBase64);
                    Console.WriteLine($"MIME type: {mimeType}");

                    // 获取文件扩展名  
                    string extension = ImageValidator.GetFileExtension(imageBase64);
                    Console.WriteLine($"File extension: {extension}");
                }
            }
            /// <summary>  
            /// 图片格式的魔数（文件签名）定义  
            /// </summary>  
            private static readonly Dictionary<ImageType, List<byte[]>> ImageSignatures = new()
    {
        {
            ImageType.JPEG, new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
            }
        },
        {
            ImageType.PNG, new List<byte[]>
            {
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
            }
        },
        {
            ImageType.GIF, new List<byte[]>
            {
                new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, // GIF87a  
                new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }  // GIF89a  
            }
        },
        {
            ImageType.BMP, new List<byte[]>
            {
                new byte[] { 0x42, 0x4D } // BM  
            }
        },
        {
            ImageType.WEBP, new List<byte[]>
            {
                new byte[] { 0x52, 0x49, 0x46, 0x46 } // RIFF.... WEBP  
            }
        },
        {
            ImageType.TIFF, new List<byte[]>
            {
                new byte[] { 0x49, 0x49, 0x2A, 0x00 }, // Little-endian  
                new byte[] { 0x4D, 0x4D, 0x00, 0x2A }  // Big-endian  
            }
        },
        {
            ImageType.ICO, new List<byte[]>
            {
                new byte[] { 0x00, 0x00, 0x01, 0x00 }
            }
        },
        {
            ImageType.HEIC, new List<byte[]>
            {
                new byte[] { 0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70, 0x68, 0x65, 0x69, 0x63 }
            }
        }
    };

            /// <summary>  
            /// MIME类型映射  
            /// </summary>  
            private static readonly Dictionary<ImageType, string> MimeTypes = new()
    {
        { ImageType.JPEG, "image/jpeg" },
        { ImageType.PNG, "image/png" },
        { ImageType.GIF, "image/gif" },
        { ImageType.BMP, "image/bmp" },
        { ImageType.WEBP, "image/webp" },
        { ImageType.TIFF, "image/tiff" },
        { ImageType.ICO, "image/x-icon" },
        { ImageType.HEIC, "image/heic" },
        { ImageType.Unknown, "application/octet-stream" }
    };

            /// <summary>  
            /// 文件扩展名映射  
            /// </summary>  
            private static readonly Dictionary<ImageType, string> FileExtensions = new()
    {
        { ImageType.JPEG, ".jpg" },
        { ImageType.PNG, ".png" },
        { ImageType.GIF, ".gif" },
        { ImageType.BMP, ".bmp" },
        { ImageType.WEBP, ".webp" },
        { ImageType.TIFF, ".tiff" },
        { ImageType.ICO, ".ico" },
        { ImageType.HEIC, ".heic" },
        { ImageType.Unknown, ".bin" }
    };

            /// <summary>  
            /// 检查字节数组是否以指定的签名开始  
            /// </summary>  
            private static bool StartsWith(byte[] source, byte[] pattern)
            {
                if (source.Length < pattern.Length)
                    return false;

                for (int i = 0; i < pattern.Length; i++)
                {
                    if (source[i] != pattern[i])
                        return false;
                }
                return true;
            }

            /// <summary>  
            /// 检查Base64字符串的图片类型  
            /// </summary>  
            /// <param name="base64String">Base64字符串</param>  
            /// <returns>图片类型枚举</returns>  
            public static ImageType GetBase64ImageType(string base64String)
            {
                if (string.IsNullOrWhiteSpace(base64String))
                    return ImageType.Unknown;

                // 如果包含data:image前缀，去除它  
                if (base64String.Contains(","))
                {
                    base64String = base64String.Split(',')[1];
                }

                try
                {
                    byte[] bytes = Convert.FromBase64String(base64String);

                    // 检查每种图片格式的签名  
                    foreach (var signature in ImageSignatures)
                    {
                        foreach (var pattern in signature.Value)
                        {
                            if (StartsWith(bytes, pattern))
                            {
                                // WEBP需要额外检查  
                                if (signature.Key == ImageType.WEBP)
                                {
                                    // 检查WEBP标识符（在RIFF之后）  
                                    if (bytes.Length >= 12 &&
                                        bytes[8] == 0x57 && // W  
                                        bytes[9] == 0x45 && // E  
                                        bytes[10] == 0x42 && // B  
                                        bytes[11] == 0x50)  // P  
                                    {
                                        return ImageType.WEBP;
                                    }
                                    continue;
                                }
                                return signature.Key;
                            }
                        }
                    }

                    return ImageType.Unknown;
                }
                catch
                {
                    return ImageType.Unknown;
                }
            }

            /// <summary>  
            /// 检查Base64字符串是否为特定类型的图片  
            /// </summary>  
            public static bool IsImageType(string base64String, ImageType imageType)
            {
                return GetBase64ImageType(base64String) == imageType;
            }

            /// <summary>  
            /// 检查Base64字符串是否为支持的图片格式  
            /// </summary>  
            public static bool IsImage(string base64String)
            {
                return GetBase64ImageType(base64String) != ImageType.Unknown;
            }

            /// <summary>  
            /// 获取Base64字符串的MIME类型  
            /// </summary>  
            public static string GetMimeType(string base64String)
            {
                var imageType = GetBase64ImageType(base64String);
                return MimeTypes.TryGetValue(imageType, out string mimeType) ? mimeType : MimeTypes[ImageType.Unknown];
            }

            /// <summary>  
            /// 获取图片类型对应的文件扩展名  
            /// </summary>  
            public static string GetFileExtension(string base64String)
            {
                var imageType = GetBase64ImageType(base64String);
                return FileExtensions.TryGetValue(imageType, out string extension) ? extension : FileExtensions[ImageType.Unknown];
            }

            /// <summary>  
            /// 获取图片的详细信息  
            /// </summary>  
            public static ImageInfo GetImageInfo(string base64String)
            {
                var imageType = GetBase64ImageType(base64String);
                return new ImageInfo
                {
                    Type = imageType,
                    MimeType = GetMimeType(base64String),
                    FileExtension = GetFileExtension(base64String),
                    IsValid = imageType != ImageType.Unknown
                };
            }
        }

        /// <summary>  
        /// 图片类型枚举  
        /// </summary>  
        public enum ImageType
        {
            Unknown,
            JPEG,
            PNG,
            GIF,
            BMP,
            WEBP,
            TIFF,
            ICO,
            HEIC
        }

        /// <summary>  
        /// 图片信息类  
        /// </summary>  
        public class ImageInfo
        {
            public ImageType Type { get; set; }
            public string MimeType { get; set; }
            public string FileExtension { get; set; }
            public bool IsValid { get; set; }

            public override string ToString()
            {
                return $"Type: {Type}, MIME: {MimeType}, Extension: {FileExtension}, Valid: {IsValid}";
            }
        }

        // 使用示例类  
        
    }
}

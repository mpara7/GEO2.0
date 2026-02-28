using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ZTool.ImageTools
{
    public class ImageTool
    {
        public static Image FromBase64(string base64String)
        {
            // 假设这是你的 base64 字符串  

            // 移除可能存在的 base64 头部信息（如果有的话）  
            // 例如: "data:image/jpeg;base64," 这样的前缀  
            if (base64String.Contains(","))
            {
                base64String = base64String.Split(',')[1];
            }

            // 将 base64 转换为字节数组  
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // 使用 ImageSharp 加载图片  
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                Image image = Image.Load(stream);
                return image;
            }
                
        }
        // 方法1：直接从 Image 对象转换  
        public static string ToBase64(Image image, IImageEncoder encoder = null)
        {
            using (var ms = new MemoryStream())
            {
                // 如果没有指定编码器，默认使用JPEG  
                encoder ??= new JpegEncoder();

                // 将图片保存到内存流  
                image.Save(ms, encoder);

                // 转换为base64字符串  
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }
        public static Image PngToJPG(Image image)
        {
            // Create a new image with white background  
            var destinationImage = new Image<Rgba32>(image.Width, image.Height);
            // Fill with white background  
            destinationImage.Mutate(x => x.BackgroundColor(Color.White));
            // Draw the original image on top  
            destinationImage.Mutate(x => x.DrawImage(image, new Point(0, 0), 1f));
            return destinationImage;
        }
    }
}

/**********************************************
 * 类作用：   图片处理类
 * 建立人：   abaal
 * 建立时间： 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace Svnhost.Common
{
    public enum ThumbnailMode { HW, W, H, Cut };
    /// <summary>
    /// 图片实用类
    /// </summary>
    public sealed class ImageUtil
    {
        private ImageUtil() { }

        public static string[] postion = new string[] { "WM_TOP_LEFT", "WM_TOP_RIGHT", "WM_TOP_MIDDLE", "WM_BOTTOM_RIGHT", "WM_BOTTOM_LEFT", "WM_BOTTOM_MIDDLE" };

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">原图片路径</param>
        /// <param name="thumbnailPath">缩略图路径</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="mode">生成模式</param>
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, ThumbnailMode mode)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case ThumbnailMode.HW://指定高宽缩放（可能变形）                
                    break;
                case ThumbnailMode.W://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailMode.H://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailMode.Cut://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        /// <summary>
        ///  加水印图片
        /// </summary>
        /// <param name="picture">imge 对象</param>
        /// <param name="WaterMarkPicPath">水印图片的地址</param>
        /// <param name="_watermarkPosition">水印位置</param>
        /// <param name="_width">被加水印图片的宽</param>
        /// <param name="_height">被加水印图片的高</param>
        public static void addWatermarkImage(Graphics picture, string WaterMarkPicPath, string _watermarkPosition, int _width, int _height)
        {
            Image watermark = new Bitmap(WaterMarkPicPath);

            int xpos = 0;
            int ypos = 0;

            int w = watermark.Width;
            int h = watermark.Height;

            switch (_watermarkPosition)
            {
                case "WM_TOP_LEFT":
                    xpos = 0 + 10;
                    ypos = 0 + 10;
                    break;
                case "WM_TOP_RIGHT":
                    xpos = _width - w - 10;
                    ypos = 0 + 10;
                    break;
                case "WM_BOTTOM_RIGHT":
                    xpos = _width - w - 10;
                    ypos = _height - h - 10;
                    break;
                case "WM_BOTTOM_LEFT":
                    xpos = 0 + 10;
                    ypos = _height - h - 10;
                    break;
                case "WM_BOTTOM_MIDDLE":
                    xpos = _width / 2 - w / 2;
                    ypos = _height - h - 10;
                    break;
                case "WM_TOP_MIDDLE":
                    xpos = _width / 2 - w / 2;
                    ypos = 0 + 10;
                    break;
            }

            picture.DrawImage(watermark, xpos, ypos, w, h);


            watermark.Dispose();
        }

        /// <summary>
        /// 打水印
        /// </summary>
        /// <param name="img"></param>
        /// <param name="saveFile">保存文件路径</param>
        /// <param name="logoFile">LOGO文件地址</param>
        public static void MakeLogo(Image img, string saveFile, string logoFile)
        {
            using (System.Drawing.Image logo = System.Drawing.Image.FromFile(logoFile))
            {
                if (img.Width < logo.Width || img.Height < logo.Height)
                    img.Save(saveFile);
                else
                {
                    Graphics g = System.Drawing.Graphics.FromImage(img);
                    Random r = new Random();
                    g.DrawImage(logo, img.Width - logo.Width - r.Next(img.Width - logo.Width), img.Height - logo.Height - r.Next(img.Height - logo.Height), logo.Width, logo.Height);

                    g.Save();
                    img.Save(saveFile);
                    g.Dispose();
                }
                logo.Dispose();
            }
        }

        #region 图片压缩(降低质量)Compress
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitmap">传入的Bitmap对象</param>
        /// <param name="destStream">压缩后的Stream对象</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Bitmap srcBitmap, Stream destStream, long level)
        {
            ImageCodecInfo myImageCodecInfo;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID

            // for the Quality parameter category.
            myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one

            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPEG file with 给定的 quality level
            myEncoderParameter = new EncoderParameter(myEncoder, level);
            myEncoderParameters.Param[0] = myEncoderParameter;
            srcBitmap.Save(destStream, myImageCodecInfo, myEncoderParameters);
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitMap">传入的Bitmap对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Bitmap srcBitMap, string destFile, long level)
        {
            Stream s = new FileStream(destFile, FileMode.Create);
            Compress(srcBitMap, s, level);
            s.Close();
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcFile">传入的Stream对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Stream srcStream, string destFile, long level)
        {
            Bitmap bm = new Bitmap(srcStream);
            Compress(bm, destFile, level);
            bm.Dispose();
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcFile">传入的Image对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Image srcImg, string destFile, long level)
        {
            Bitmap bm = new Bitmap(srcImg);
            Compress(bm, destFile, level);
            bm.Dispose();
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcFile">待压缩的BMP文件名</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(string srcFile, string destFile, long level)
        {
            // Create a Bitmap object based on a BMP file.
            Bitmap bm = new Bitmap(srcFile);
            Compress(bm, destFile, level);
            bm.Dispose();
        }

        #endregion 图片压缩(降低质量)
    }
}

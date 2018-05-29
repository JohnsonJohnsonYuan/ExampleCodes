/**********************************************
 * �����ã�   ͼƬ������
 * �����ˣ�   abaal
 * ����ʱ�䣺 2008-09-03 
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
    /// ͼƬʵ����
    /// </summary>
    public sealed class ImageUtil
    {
        private ImageUtil() { }

        public static string[] postion = new string[] { "WM_TOP_LEFT", "WM_TOP_RIGHT", "WM_TOP_MIDDLE", "WM_BOTTOM_RIGHT", "WM_BOTTOM_LEFT", "WM_BOTTOM_MIDDLE" };

        /// <summary>
        /// ��������ͼ
        /// </summary>
        /// <param name="originalImagePath">ԭͼƬ·��</param>
        /// <param name="thumbnailPath">����ͼ·��</param>
        /// <param name="width">���</param>
        /// <param name="height">�߶�</param>
        /// <param name="mode">����ģʽ</param>
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
                case ThumbnailMode.HW://ָ���߿����ţ����ܱ��Σ�                
                    break;
                case ThumbnailMode.W://ָ�����߰�����                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailMode.H://ָ���ߣ�������
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailMode.Cut://ָ���߿�ü��������Σ�                
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

            //�½�һ��bmpͼƬ
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //�½�һ������
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //���ø�������ֵ��
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //���ø�����,���ٶȳ���ƽ���̶�
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //��ջ�������͸������ɫ���
            g.Clear(System.Drawing.Color.Transparent);

            //��ָ��λ�ò��Ұ�ָ����С����ԭͼƬ��ָ������
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                new System.Drawing.Rectangle(x, y, ow, oh),
                System.Drawing.GraphicsUnit.Pixel);

            try
            {
                //��jpg��ʽ��������ͼ
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
        ///  ��ˮӡͼƬ
        /// </summary>
        /// <param name="picture">imge ����</param>
        /// <param name="WaterMarkPicPath">ˮӡͼƬ�ĵ�ַ</param>
        /// <param name="_watermarkPosition">ˮӡλ��</param>
        /// <param name="_width">����ˮӡͼƬ�Ŀ�</param>
        /// <param name="_height">����ˮӡͼƬ�ĸ�</param>
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
        /// ��ˮӡ
        /// </summary>
        /// <param name="img"></param>
        /// <param name="saveFile">�����ļ�·��</param>
        /// <param name="logoFile">LOGO�ļ���ַ</param>
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

        #region ͼƬѹ��(��������)Compress
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
        /// ͼƬѹ��(���������Լ�С�ļ��Ĵ�С)
        /// </summary>
        /// <param name="srcBitmap">�����Bitmap����</param>
        /// <param name="destStream">ѹ�����Stream����</param>
        /// <param name="level">ѹ���ȼ���0��100��0 ���������100 ���</param>
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

            // Save the bitmap as a JPEG file with ������ quality level
            myEncoderParameter = new EncoderParameter(myEncoder, level);
            myEncoderParameters.Param[0] = myEncoderParameter;
            srcBitmap.Save(destStream, myImageCodecInfo, myEncoderParameters);
        }
        /// <summary>
        /// ͼƬѹ��(���������Լ�С�ļ��Ĵ�С)
        /// </summary>
        /// <param name="srcBitMap">�����Bitmap����</param>
        /// <param name="destFile">ѹ�����ͼƬ����·��</param>
        /// <param name="level">ѹ���ȼ���0��100��0 ���������100 ���</param>
        public static void Compress(Bitmap srcBitMap, string destFile, long level)
        {
            Stream s = new FileStream(destFile, FileMode.Create);
            Compress(srcBitMap, s, level);
            s.Close();
        }
        /// <summary>
        /// ͼƬѹ��(���������Լ�С�ļ��Ĵ�С)
        /// </summary>
        /// <param name="srcFile">�����Stream����</param>
        /// <param name="destFile">ѹ�����ͼƬ����·��</param>
        /// <param name="level">ѹ���ȼ���0��100��0 ���������100 ���</param>
        public static void Compress(Stream srcStream, string destFile, long level)
        {
            Bitmap bm = new Bitmap(srcStream);
            Compress(bm, destFile, level);
            bm.Dispose();
        }
        /// <summary>
        /// ͼƬѹ��(���������Լ�С�ļ��Ĵ�С)
        /// </summary>
        /// <param name="srcFile">�����Image����</param>
        /// <param name="destFile">ѹ�����ͼƬ����·��</param>
        /// <param name="level">ѹ���ȼ���0��100��0 ���������100 ���</param>
        public static void Compress(Image srcImg, string destFile, long level)
        {
            Bitmap bm = new Bitmap(srcImg);
            Compress(bm, destFile, level);
            bm.Dispose();
        }
        /// <summary>
        /// ͼƬѹ��(���������Լ�С�ļ��Ĵ�С)
        /// </summary>
        /// <param name="srcFile">��ѹ����BMP�ļ���</param>
        /// <param name="destFile">ѹ�����ͼƬ����·��</param>
        /// <param name="level">ѹ���ȼ���0��100��0 ���������100 ���</param>
        public static void Compress(string srcFile, string destFile, long level)
        {
            // Create a Bitmap object based on a BMP file.
            Bitmap bm = new Bitmap(srcFile);
            Compress(bm, destFile, level);
            bm.Dispose();
        }

        #endregion ͼƬѹ��(��������)
    }
}

/**********************************************
 * �����ã�   �ļ�ʵ����
 * �����ˣ�   abaal
 * ����ʱ�䣺 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

namespace Svnhost.Common
{
    /// <summary>
    /// �ļ�ʵ����
    /// </summary>
    public sealed class FileUtil
    {
        private FileUtil()
        {
        }

        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        /// <param name="FileOrPath">�ļ���Ŀ¼</param>
        public static void CreateDirectory(string FileOrPath)
        {
            if (FileOrPath != null)
            {
                string path;
                if (FileOrPath.Contains("."))
                    path = Path.GetDirectoryName(FileOrPath);
                else
                    path = FileOrPath;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        /// <summary>
        /// ��ȡ�ļ�
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="encoding"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public static string ReadFile(string filename, Encoding encoding, bool isCache)
        {
            string body;
            if (isCache)
            {
                body = (string)HttpContext.Current.Cache[filename];
                if (body == null)
                {
                    body = ReadFile(filename, encoding, false);
                    HttpContext.Current.Cache.Add(filename, body, new System.Web.Caching.CacheDependency(filename), DateTime.MaxValue, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, null);
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader(filename, encoding))
                {
                    body = sr.ReadToEnd();
                }
            }

            return body;
        }

        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="sourceFileName">Դ�ļ���</param>
        /// <param name="destFileName">Ŀ���ļ���</param>
        /// <param name="overwrite">��Ŀ���ļ�����ʱ�Ƿ񸲸�</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!System.IO.File.Exists(sourceFileName))
            {
                throw new FileNotFoundException(sourceFileName + "�ļ������ڣ�");
            }
            if (!overwrite && System.IO.File.Exists(destFileName))
            {
                return false;
            }
            try
            {
                System.IO.File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// �����ļ�,��Ŀ���ļ�����ʱ����
        /// </summary>
        /// <param name="sourceFileName">Դ�ļ���</param>
        /// <param name="destFileName">Ŀ���ļ���</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        public static bool BackupFile(string sourceFileName, string destFileName)
        {
            return BackupFile(sourceFileName, destFileName, true);
        }


        /// <summary>
        /// �ָ��ļ�
        /// </summary>
        /// <param name="backupFileName">�����ļ���</param>
        /// <param name="targetFileName">Ҫ�ָ����ļ���</param>
        /// <param name="backupTargetFileName">Ҫ�ָ��ļ��ٴα��ݵ�����,���Ϊnull,���ٱ��ݻָ��ļ�</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName, string backupTargetFileName)
        {
            try
            {
                if (!System.IO.File.Exists(backupFileName))
                {
                    throw new FileNotFoundException(backupFileName + "�ļ������ڣ�");
                }
                if (backupTargetFileName != null)
                {
                    if (!System.IO.File.Exists(targetFileName))
                    {
                        throw new FileNotFoundException(targetFileName + "�ļ������ڣ��޷����ݴ��ļ���");
                    }
                    else
                    {
                        System.IO.File.Copy(targetFileName, backupTargetFileName, true);
                    }
                }
                System.IO.File.Delete(targetFileName);
                System.IO.File.Copy(backupFileName, targetFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public static bool RestoreFile(string backupFileName, string targetFileName)
        {
            return RestoreFile(backupFileName, targetFileName, null);
        }
    }
}

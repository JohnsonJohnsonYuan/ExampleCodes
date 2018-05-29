using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// ���õ�Ŀ¼����������
    /// </summary>
    public class DirectoryUtil
    {
        #region Ŀ¼��д��ռ����

        /// <summary>
        ///���Ŀ¼�Ƿ��д��������ԣ�����True������False
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsWriteable(string path)
        {
            if (!Directory.Exists(path))
            {
                // if the directory is not exist
                try
                {
                    // if you can create a new directory, it's mean you have write right
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    return false;
                }
            }


            try
            {
                string testFileName = ".test." + Guid.NewGuid().ToString().Substring(0, 5);
                string testFilePath = Path.Combine(path, testFileName);
                File.WriteAllLines(testFilePath, new string[] { "test" });
                File.Delete(testFilePath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// �������Ƿ����㹻�Ŀ��ÿռ�
        /// </summary>
        /// <param name="path"></param>
        /// <param name="requiredSpace"></param>
        /// <returns></returns>
        public static bool IsDiskSpaceEnough(string path, ulong requiredSpace)
        {
            string root = Path.GetPathRoot(path);
            ulong freeSpace = GetFreeSpace(root);

            return requiredSpace <= freeSpace;
        }

        /// <summary>
        /// ��ȡ�����̷��Ŀ��ÿռ��С
        /// </summary>
        /// <param name="driveName">Direve name</param>
        /// <returns>free space (byte)</returns>
        public static ulong GetFreeSpace(string driveName)
        {
            ulong freefreeBytesAvailable = 0;
            try
            {
                DriveInfo drive = new DriveInfo(driveName);
                freefreeBytesAvailable = (ulong)drive.AvailableFreeSpace;
            }
            catch
            {

            }

            return freefreeBytesAvailable;
        }

        public static ulong ConvertByteCountToKByteCount(ulong byteCount)
        {
            return byteCount / 1024;
        }

        public static float ConvertKByteCountToMByteCount(ulong kByteCount)
        {
            return kByteCount / 1024;
        }

        public static float ConvertMByteCountToGByteCount(float kByteCount)
        {
            return kByteCount / 1024;
        } 

        #endregion

        #region Ŀ¼����

        #region ��ȡָ��Ŀ¼�е��ļ��б�
        /// <summary>
        /// ��ȡָ��Ŀ¼�������ļ��б�
        /// </summary>
        /// <param name="directoryPath">ָ��Ŀ¼�ľ���·��</param>
        public static string[] GetFileNames(string directoryPath)
        {
            //���Ŀ¼�����ڣ����׳��쳣
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }

            //��ȡ�ļ��б�
            return Directory.GetFiles(directoryPath);
        }

        /// <summary>
        /// ��ȡָ��Ŀ¼����Ŀ¼�������ļ��б�
        /// </summary>
        /// <param name="directoryPath">ָ��Ŀ¼�ľ���·��</param>
        /// <param name="searchPattern">ģʽ�ַ�����"*"����0��N���ַ���"?"����1���ַ���
        /// ������"Log*.xml"��ʾ����������Log��ͷ��Xml�ļ���</param>
        /// <param name="isSearchChild">�Ƿ�������Ŀ¼</param>
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            //���Ŀ¼�����ڣ����׳��쳣
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }

            try
            {
                if (isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ��ȡָ��Ŀ¼�е���Ŀ¼�б�
        /// <summary>
        /// ��ȡָ��Ŀ¼��������Ŀ¼�б�,��Ҫ����Ƕ�׵���Ŀ¼�б�,��ʹ�����ط���.
        /// </summary>
        /// <param name="directoryPath">ָ��Ŀ¼�ľ���·��</param>
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ��ȡָ��Ŀ¼����Ŀ¼��������Ŀ¼�б�
        /// </summary>
        /// <param name="directoryPath">ָ��Ŀ¼�ľ���·��</param>
        /// <param name="searchPattern">ģʽ�ַ�����"*"����0��N���ַ���"?"����1���ַ���
        /// ������"Log*.xml"��ʾ����������Log��ͷ��Xml�ļ���</param>
        /// <param name="isSearchChild">�Ƿ�������Ŀ¼</param>
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ��������Ŀ¼

        /// <summary>
        /// �������� �ļ���    ��ʽ��yyyy\mm\dd
        /// </summary>
        /// <remarks>
        /// ����ʱ��Ŀ¼   ���� ���磺 c:\directory\2009\03\01
        /// </remarks>
        /// <param name="rootPath">����·��   [�ڴ�Ŀ¼�½�����Ŀ¼]</param>
        /// <returns>��������·��  </returns>
        public static string CreateDirectoryByDate(string rootPath)
        {
            return CreateDirectoryByDate(rootPath, "yyyy-MM-dd");
        }

        /// <summary>
        /// ��Ӧ��ʽ��������Ŀ¼
        /// </summary>
        /// <remarks>
        /// formatString:
        ///              yyyy-MM-dd        :2009\03\01
        ///              yyyy-MM-dd-HH     :2009\03\01\01
        /// </remarks>
        /// <param name="rootPath">����·��   [�ڴ�Ŀ¼�½�����Ŀ¼]</param>
        /// <param name="formatString">��ʽ</param>
        /// <returns>��������·�� </returns>
        public static string CreateDirectoryByDate(string rootPath, string formatString)
        {
            if (!IsExistDirectory(rootPath))
                throw new DirectoryNotFoundException("the rootPath is not found");

            //СʱĿ¼
            bool hour = false;

            switch (formatString)
            {
                case "yyyy-MM-dd":
                    hour = false;
                    break;
                case "yyyy-MM-dd-HH":
                    hour = true;
                    break;
                default:
                    hour = false;
                    break;
            }

            string tempPath;

            tempPath = rootPath + "\\" + DateTime.Now.Year.ToString();

            CreateDirectory(tempPath);

            tempPath = tempPath + "\\" + DateTime.Now.Month.ToString("00");

            CreateDirectory(tempPath);

            tempPath = tempPath + "\\" + DateTime.Now.Day.ToString("00");

            CreateDirectory(tempPath);

            if (hour)
            {
                tempPath = tempPath + "\\" + DateTime.Now.Hour.ToString("00");

                CreateDirectory(tempPath);
            }

            return tempPath;
        }

        #endregion

        /// <summary>
        /// ȷ���ļ��б�����
        /// </summary>
        /// <param name="filePath">�ļ���ȫ������·����</param>
        public static void AssertDirExist(string filePath)
        {
            DirectoryInfo dir = new DirectoryInfo(filePath);
            if (!dir.Exists)
            {
                dir.Create();
            }
        }

        /// <summary>
        /// ���ָ��Ŀ¼�Ƿ����
        /// </summary>
        /// <param name="directoryPath">Ŀ¼�ľ���·��</param>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        /// <summary>
        /// ���ָ��Ŀ¼�Ƿ�Ϊ��
        /// </summary>
        /// <param name="directoryPath">ָ��Ŀ¼�ľ���·��</param>
        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                //�ж��Ƿ�����ļ�
                string[] fileNames = GetFileNames(directoryPath);
                if (fileNames.Length > 0)
                {
                    return false;
                }

                //�ж��Ƿ�����ļ���
                string[] directoryNames = GetDirectories(directoryPath);
                if (directoryNames.Length > 0)
                {
                    return false;
                }

                return true;
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ���ָ��Ŀ¼���Ƿ����ָ�����ļ�,��Ҫ������Ŀ¼��ʹ�����ط���.
        /// </summary>
        /// <param name="directoryPath">ָ��Ŀ¼�ľ���·��</param>
        /// <param name="searchPattern">ģʽ�ַ�����"*"����0��N���ַ���"?"����1���ַ���
        /// ������"Log*.xml"��ʾ����������Log��ͷ��Xml�ļ���</param>
        public static bool ContainFile(string directoryPath, string searchPattern)
        {
            try
            {
                //��ȡָ�����ļ��б�
                string[] fileNames = GetFileNames(directoryPath, searchPattern, false);

                //�ж�ָ���ļ��Ƿ����
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ���ָ��Ŀ¼���Ƿ����ָ�����ļ�
        /// </summary>
        /// <param name="directoryPath">ָ��Ŀ¼�ľ���·��</param>
        /// <param name="searchPattern">ģʽ�ַ�����"*"����0��N���ַ���"?"����1���ַ���
        /// ������"Log*.xml"��ʾ����������Log��ͷ��Xml�ļ���</param>
        /// <param name="isSearchChild">�Ƿ�������Ŀ¼</param>
        public static bool ContainFile(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                //��ȡָ�����ļ��б�
                string[] fileNames = GetFileNames(directoryPath, searchPattern, true);

                //�ж�ָ���ļ��Ƿ����
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// ����һ��Ŀ¼
        /// </summary>
        /// <param name="directoryPath">Ŀ¼�ľ���·��</param>
        public static void CreateDirectory(string directoryPath)
        {
            //���Ŀ¼�������򴴽���Ŀ¼
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// ���ָ��Ŀ¼�������ļ�����Ŀ¼,����Ŀ¼��Ȼ����.
        /// </summary>
        /// <param name="directoryPath">ָ��Ŀ¼�ľ���·��</param>
        public static void ClearDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                //ɾ��Ŀ¼�����е��ļ�
                string[] fileNames = GetFileNames(directoryPath);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    FileUtil.DeleteFile(fileNames[i]);
                }

                //ɾ��Ŀ¼�����е���Ŀ¼
                string[] directoryNames = GetDirectories(directoryPath);
                for (int i = 0; i < directoryNames.Length; i++)
                {
                    DeleteDirectory(directoryNames[i]);
                }
            }
        }

        /// <summary>
        /// ɾ��ָ��Ŀ¼����������Ŀ¼
        /// </summary>
        /// <param name="directoryPath">ָ��Ŀ¼�ľ���·��</param>
        public static void DeleteDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        /// <summary>
        /// ȡϵͳĿ¼
        /// </summary>
        /// <returns></returns>
        public static string GetSystemDirectory()
        {
            return System.Environment.SystemDirectory;
        }

        /// <summary>
        /// ȡϵͳ���ر�Ŀ¼
        /// </summary>
        /// <param name="folderType"></param>
        /// <returns></returns>
        public static string GetSpeicalFolder(Environment.SpecialFolder folderType)
        {
            return System.Environment.GetFolderPath(folderType);
        }

        /// <summary>
        /// ���ص�ǰϵͳ����ʱĿ¼
        /// </summary>
        /// <returns></returns>
        public static string GetTempPath()
        {
            return Path.GetTempPath();
        }

        /// <summary>
        /// ȡ��ǰĿ¼
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// �赱ǰĿ¼
        /// </summary>
        /// <param name="path"></param>
        public static void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }

        /// <summary>
        /// ȡ·���в�������ڵ��ַ�
        /// </summary>
        /// <returns></returns>
        public static char[] GetInvalidPathChars()
        {
            return Path.GetInvalidPathChars();
        }

        /// <summary>
        /// ȡϵͳ���е��߼�������
        /// </summary>
        /// <returns></returns>
        public static DriveInfo[] GetAllDrives()
        {
            return DriveInfo.GetDrives();
        }

        #endregion

    }
}

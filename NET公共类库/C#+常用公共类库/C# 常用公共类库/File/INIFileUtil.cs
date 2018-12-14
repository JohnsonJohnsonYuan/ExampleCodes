using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// INI�ļ�����������
    /// </summary>
    public class INIFileUtil
    {
        public string path;

        /// <summary>
        /// ����INI�ļ�·���������
        /// </summary>
        /// <param name="INIPath">INI�ļ�·��</param>
        public INIFileUtil(string INIPath)
		{
			path = INIPath;
		}

		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section,string key,string def, StringBuilder retVal,int size,string filePath);

	
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);


		/// <summary>
		/// дINI�ļ�
		/// </summary>
		/// <param name="Section">����ڵ�</param>
		/// <param name="Key">�ؼ���</param>
		/// <param name="Value">ֵ</param>
		public void IniWriteValue(string Section,string Key,string Value)
		{
			WritePrivateProfileString(Section,Key,Value,this.path);
		}

		/// <summary>
		/// ��ȡINI�ļ�
		/// </summary>
		/// <param name="Section">����ڵ�</param>
		/// <param name="Key">�ؼ���</param>
		/// <returns></returns>
		public string IniReadValue(string Section,string Key)
		{
			StringBuilder temp = new StringBuilder(255);
			int i = GetPrivateProfileString(Section,Key,"",temp, 255, this.path);
			return temp.ToString();
		}

		public byte[] IniReadValues(string section, string key)
		{
			byte[] temp = new byte[255];
			int i = GetPrivateProfileString(section, key, "", temp, 255, this.path);
			return temp;

		}

		/// <summary>
		/// ɾ��ini�ļ������ж���
		/// </summary>
		public void ClearAllSection()
		{
			IniWriteValue(null,null,null);
		}

		/// <summary>
		/// ɾ��ini�ļ���ָ�������µ����м�
		/// </summary>
		/// <param name="Section"></param>
		public void ClearSection(string Section)
		{
			IniWriteValue(Section,null,null);
		}
    }
}

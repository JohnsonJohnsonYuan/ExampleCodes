using System;
using System.Windows.Forms;

namespace WHC.OrderWater.Commons
{
	/// <summary>
	/// MessageUtil ��ժҪ˵����
	/// </summary>
	public class MessageUtil
	{
		/// <summary>
		/// ��ʾһ�����ʾ��Ϣ
		/// </summary>
		/// <param name="message">��ʾ��Ϣ</param>
		public static DialogResult ShowTips(string message)
		{
			return MessageBox.Show(message, "��ʾ��Ϣ",MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <summary>
		/// ��ʾ������Ϣ
		/// </summary>
		/// <param name="message">������Ϣ</param>
		public static DialogResult ShowWarning(string message)
		{
			return MessageBox.Show(message, "������Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// ��ʾ������Ϣ
		/// </summary>
		/// <param name="message">������Ϣ</param>
		public static DialogResult ShowError(string message)
		{
			return MessageBox.Show(message, "������Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// ��ʾѯ���û���Ϣ������ʾ�����־
		/// </summary>
		/// <param name="message">������Ϣ</param>
		public static DialogResult ShowYesNoAndError(string message)
		{
			return MessageBox.Show(message, "������Ϣ", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
		}

		/// <summary>
		/// ��ʾѯ���û���Ϣ������ʾ��ʾ��־
		/// </summary>
		/// <param name="message">������Ϣ</param>
		public static DialogResult ShowYesNoAndTips(string message)
		{
			return MessageBox.Show(message, "��ʾ��Ϣ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
		}

        /// <summary>
        /// ��ʾѯ���û���Ϣ������ʾ�����־
        /// </summary>
        /// <param name="message">������Ϣ</param>
        public static DialogResult ShowYesNoAndWarning(string message)
        {
            return MessageBox.Show(message, "������Ϣ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// ��ʾѯ���û���Ϣ������ʾ��ʾ��־
        /// </summary>
        /// <param name="message">������Ϣ</param>
        public static DialogResult ShowYesNoCancelAndTips(string message)
        {
            return MessageBox.Show(message, "��ʾ��Ϣ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
        }

        /// <summary>
        /// ��ʾһ��YesNoѡ��Ի���
        /// </summary>
        /// <param name="prompt">�Ի����ѡ��������ʾ��Ϣ</param>
        /// <returns>���ѡ��Yes�򷵻�true�����򷵻�false</returns>
        public static bool ConfirmYesNo(string prompt)
        {
            return MessageBox.Show(prompt, "ȷ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// ��ʾһ��YesNoCancelѡ��Ի���
        /// </summary>
        /// <param name="prompt">�Ի����ѡ��������ʾ��Ϣ</param>
        /// <returns>����ѡ�����ĵ�DialogResultֵ</returns>
        public static DialogResult ConfirmYesNoCancel(string prompt)
        {
            return MessageBox.Show(prompt, "ȷ��", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
	}
}

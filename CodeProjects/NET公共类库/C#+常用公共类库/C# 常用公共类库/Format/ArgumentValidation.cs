using System;
using System.Collections.Generic;
using System.Text;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// Common validation routines for argument validation.(����Enterprise Library2005��Common��Ŀ)
    /// </summary>
    public sealed class ArgumentValidation
    {
        #region Constant String Members

        private const string ExceptionEmptyString = "���� '{0}'��ֵ����Ϊ���ַ�����";
        private const string ExceptionInvalidNullNameArgument = "����'{0}'�����Ʋ���Ϊ�����û���ַ�����";
        private const string ExceptionByteArrayValueMustBeGreaterThanZeroBytes = "��ֵ�������0�ֽ�.";
        private const string ExceptionExpectedType = "��Ч�����ͣ��ڴ������ͱ���Ϊ'{0}'��";
        private const string ExceptionEnumerationNotDefined = "{0}����{1}��һ����Чֵ";

        #endregion

        private ArgumentValidation()
        {
        }

        /// <summary>
        /// <para>������<paramref name="variable"/>�Ƿ�Ϊ���ַ�����</para>
        /// </summary>
        /// <param name="variable">
        /// <para>The value to check.</para>
        /// </param>
        /// <param name="variableName">
        /// <para>The name of the variable being checked.</para>
        /// </param>
        /// <remarks>
        /// <para>Before checking the <paramref name="variable"/>, a call is made to <see cref="ArgumentValidation.CheckForNullReference"/>.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <pararef name="variable"/> can not be <see langword="null"/> (Nothing in Visual Basic).
        /// <para>- or -</para>
        /// <pararef name="variableName"/> can not be <see langword="null"/> (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <pararef name="variable"/> can not be a zero length <see cref="string"/>.
        /// </exception>
        public static void CheckForEmptyString(string variable, string variableName)
        {
            CheckForNullReference(variable, variableName);
            CheckForNullReference(variableName, "variableName");
            if (variable.Length == 0)
            {
                string message = string.Format(ExceptionEmptyString, variableName);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// <para>������<paramref name="variable"/>�Ƿ�Ϊ������(Null)��</para>
        /// </summary>
        /// <param name="variable">
        /// <para>The value to check.</para>
        /// </param>
        /// <param name="variableName">
        /// <para>The name of the variable being checked.</para>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <pararef name="variable"/> can not <see langword="null"/> (Nothing in Visual Basic).
        /// <para>- or -</para>
        /// <pararef name="variableName"/> can not <see langword="null"/> (Nothing in Visual Basic).
        /// </exception>
        public static void CheckForNullReference(object variable, string variableName)
        {
            if (variableName == null)
            {
                throw new ArgumentNullException("variableName");
            }

            if (null == variable)
            {
                throw new ArgumentNullException(variableName);
            }
        }

        /// <summary>
        /// ��֤����Ĳ���messageName�ǿ��ַ�����Ҳ�ǿ�����
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="messageName">Parameter value</param>
        public static void CheckForInvalidNullNameReference(string name, string messageName)
        {
            if ((null == name) || (name.Length == 0))
            {
                string message = string.Format(ExceptionInvalidNullNameArgument, messageName);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// <para>��֤����<paramref name="bytes"/>���㳤�ȣ����Ϊ�㳤�ȣ����׳��쳣<see cref="ArgumentException"/>��</para>
        /// </summary>
        /// <param name="bytes">
        /// The <see cref="byte"/> array to check.
        /// </param>
        /// <param name="variableName">
        /// <para>The name of the variable being checked.</para>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <pararef name="variable"/> can not <see langword="null"/> (Nothing in Visual Basic).
        /// <para>- or -</para>
        /// <pararef name="variableName"/> can not <see langword="null"/> (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="bytes"/> can not be zero length.</para>
        /// </exception>
        public static void CheckForZeroBytes(byte[] bytes, string variableName)
        {
            CheckForNullReference(bytes, "bytes");
            CheckForNullReference(variableName, "variableName");
            if (bytes.Length == 0)
            {
                string message = string.Format(ExceptionByteArrayValueMustBeGreaterThanZeroBytes, variableName);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// <para>������<paramref name="variable"/>�Ƿ����ָ�������͡�</para>
        /// </summary>
        /// <param name="variable">
        /// <para>The value to check.</para>
        /// </param>
        /// <param name="type">
        /// <para>The <see cref="Type"/> expected type of <paramref name="variable"/>.</para>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <pararef name="variable"/> can not <see langword="null"/> (Nothing in Visual Basic).
        /// <para>- or -</para>
        /// <pararef name="typeName"/> can not <see langword="null"/> (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="variable"/> is not the expected <see cref="Type"/>.
        /// </exception>
        public static void CheckExpectedType(object variable, Type type)
        {
            CheckForNullReference(variable, "variable");
            CheckForNullReference(type, "type");
            if (!type.IsAssignableFrom(variable.GetType()))
            {
                string message = string.Format(ExceptionExpectedType, type.FullName);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// <para>Check <paramref name="variable"/> to determine if it is a valid defined enumeration for <paramref name="enumType"/>.</para>
        /// </summary>
        /// <param name="variable">
        /// <para>The value to check.</para>
        /// </param>
        /// <param name="enumType">
        /// <para>The <see cref="Type"/> expected type of <paramref name="variable"/>.</para>
        /// </param>
        /// <param name="variableName">
        /// <para>The name of the variable being checked.</para>
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <pararef name="variable"/> can not <see langword="null"/> (Nothing in Visual Basic).
        /// <para>- or -</para>
        /// <pararef name="enumType"/> can not <see langword="null"/> (Nothing in Visual Basic).
        /// <para>- or -</para>
        /// <pararef name="variableName"/> can not <see langword="null"/> (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="variable"/> is not the expected <see cref="Type"/>.
        /// <para>- or -</para>
        /// <par><paramref name="enumType"/> is not an <see cref="Enum"/>. </par>
        /// </exception>
        public static void CheckEnumeration(Type enumType, object variable, string variableName)
        {
            CheckForNullReference(variable, "variable");
            CheckForNullReference(enumType, "enumType");
            CheckForNullReference(variableName, "variableName");

            if (!Enum.IsDefined(enumType, variable))
            {
                string message = string.Format(ExceptionEnumerationNotDefined,
                    variable.ToString(), enumType.FullName, variableName);
                throw new ArgumentException(message);
            }
        }
    }
}

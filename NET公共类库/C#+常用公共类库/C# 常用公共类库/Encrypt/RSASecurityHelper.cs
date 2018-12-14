using System;
using System.Security.Cryptography;
using System.Text;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// �ǶԳƼ�����֤������
    /// </summary>
    public class RSASecurityHelper
    {
        /// <summary>
        /// ��ע����Ϣ���ݲ��÷ǶԳƼ��ܵķ�ʽ����
        /// </summary>
        /// <param name="originalString">δ���ܵ��ı����������</param>
        /// <param name="encrytedString">���ܺ���ı�����ע�����к�</param>
        /// <returns>�����֤�ɹ�����True������ΪFalse</returns>
        public static bool Validate(string originalString, string encrytedString)
        {
            bool bPassed = false;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(UIConstants.PublicKey); //��Կ
                    RSAPKCS1SignatureDeformatter formatter = new RSAPKCS1SignatureDeformatter(rsa);
                    formatter.SetHashAlgorithm("SHA1");

                    byte[] key = Convert.FromBase64String(encrytedString); //��֤
                    SHA1Managed sha = new SHA1Managed();
                    byte[] name = sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(originalString));
                    if (formatter.VerifySignature(name, key))
                    {
                        bPassed = true;
                    }
                }
                catch
                {
                }
            }
            return bPassed;
        }
    }
}
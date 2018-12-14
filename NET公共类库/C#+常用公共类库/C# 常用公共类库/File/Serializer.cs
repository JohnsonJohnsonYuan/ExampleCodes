using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Xml.Serialization;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// ���кŲ���������
    /// </summary>
    /// 
    public class Serializer
    {
        private Serializer()
        {
        }

        #region ���ָ�ʽ�����л�����
        /// <summary>
        /// ���л����󵽶������ֽ�����
        /// </summary>
        /// <param name="obj">�����л��Ķ���</param>
        /// <returns></returns>
        public static byte[] SerializeToBinary(object obj)
        {
            byte[] b = new byte[2500];
            MemoryStream ms = new MemoryStream();

            try
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(ms, obj);
                ms.Seek(0, 0);
                if (ms.Length > b.Length) b = new byte[ms.Length];
                b = ms.ToArray();
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                ms.Close();
            }
            return b;
        }

        /// <summary>
        /// ���л�����ָ�����ļ���
        /// </summary>
        /// <param name="obj">�����л��Ķ���</param>
        /// <param name="path">�ļ�·��</param>
        /// <param name="mode">�ļ��򿪷�ʽ</param>
        public static void SerializeToBinary(object obj, string path, FileMode mode)
        {
            FileStream fs = new FileStream(path, mode);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, obj);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("���л�����ʧ��: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// ���кŶ����ļ��У�����һ�����ļ�
        /// </summary>
        /// <param name="obj">�����л��Ķ���</param>
        /// <param name="path">�ļ�·��</param>
        public static void SerializeToBinary(object obj, string path)
        {
            SerializeToBinary(obj, path, FileMode.Create);
        }

        /// <summary>
        /// ���л�����Soap�ַ�����
        /// </summary>
        /// <param name="obj">�����л��Ķ���</param>
        /// <returns></returns>
        public static string SerializeToSoap(object obj)
        {
            string s = "";
            MemoryStream ms = new MemoryStream();

            try
            {
                SoapFormatter sformatter = new SoapFormatter();
                sformatter.Serialize(ms, obj);
                ms.Seek(0, 0);
                s = Encoding.ASCII.GetString(ms.ToArray());
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                ms.Close();
            }

            return s;
        }

        /// <summary>
        /// ���л�����Soap�ַ����У������浽�ļ�
        /// </summary>
        /// <param name="obj">�����л��Ķ���</param>
        /// <param name="path">�ļ�·��</param>
        /// <param name="mode">�ļ��򿪷�ʽ</param>
        public static void SerializeToSoap(object obj, string path, FileMode mode)
        {
            FileStream fs = new FileStream(path, mode);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            SoapFormatter formatter = new SoapFormatter();
            try
            {
                formatter.Serialize(fs, obj);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// ���л�����Soap�ַ����У������浽�ļ�
        /// </summary>
        /// <param name="obj">�����л��Ķ���</param>
        /// <param name="path">�ļ�·��</param>
        public static void SerializeToSoap(object obj, string path)
        {
            SerializeToSoap(obj, path, FileMode.Create);
        }

        /// <summary>
        /// ���л�����XML�ַ�����
        /// </summary>
        /// <param name="obj">�����л��Ķ���</param>
        /// <returns></returns>
        public static string SerializeToXml(object obj)
        {
            string s = "";
            MemoryStream ms = new MemoryStream();

            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(ms, obj);
                ms.Seek(0, 0);
                s = Encoding.ASCII.GetString(ms.ToArray());
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                ms.Close();
            }
            return s;
        }

        /// <summary>
        /// ���л�����XML�ַ���,�����浽�ļ���
        /// </summary>
        /// <param name="obj">�����л��Ķ���</param>
        /// <param name="path">�ļ�·��</param>
        /// <param name="mode">�ļ��򿪷�ʽ</param>
        public static void SerializeToXmlFile(object obj, string path, FileMode mode)
        {
            FileStream fs = new FileStream(path, mode);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            try
            {
                serializer.Serialize(fs, obj);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// ���л�����XML�ַ���,�����浽�ļ���
        /// </summary>
        /// <param name="obj">�����л��Ķ���</param>
        /// <param name="path">�ļ�·��</param>
        public static void SerializeToXmlFile(object obj, string path)
        {
            SerializeToXmlFile(obj, path, FileMode.Create);
        } 
        #endregion


        /// <summary>
        /// ��ָ�����ļ��з����л�������Ķ���
        /// </summary>
        /// <param name="type">���������</param>
        /// <param name="path">�ļ�·��</param>
        /// <returns></returns>
        public static object DeserializeFromXmlFile(Type type, string path)
        {
            object o = new object();
            FileStream fs = new FileStream(path, FileMode.Open);

            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                o = serializer.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
            return o;
        }

        /// <summary>
        /// ��ָ����XML�ַ����з����л�������Ķ���
        /// </summary>
        /// <param name="type">���������</param>
        /// <param name="s">XML�ַ���</param>
        /// <returns></returns>
        public static object DeserializeFromXml(Type type, string s)
        {
            object o = new object();

            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                o = serializer.Deserialize(new StringReader(s));
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
            }
            return o;
        }

        /// <summary>
        /// ��ָ����SoapЭ���ַ����з����л�������Ķ���
        /// </summary>
        /// <param name="type">���������</param>
        /// <param name="s">SoapЭ���ַ���</param>
        /// <returns></returns>
        public static object DeserializeFromSoap(Type type, string s)
        {
            object o = new object();
            MemoryStream ms = new MemoryStream(new UTF8Encoding().GetBytes(s));

            try
            {
                SoapFormatter serializer = new SoapFormatter();
                o = serializer.Deserialize(ms);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
            }
            return o;
        }

        /// <summary>
        /// ��ָ���Ķ������ֽ������з����л�������Ķ���
        /// </summary>
        /// <param name="type">���������</param>
        /// <param name="bytes">�������ֽ�����</param>
        /// <returns></returns>
        public static object DeserializeFromBinary(Type type, byte[] bytes)
        {
            object o = new object();
            MemoryStream ms = new MemoryStream(bytes);

            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                o = serializer.Deserialize(ms);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
            }
            return o;
        }

        /// <summary>
        /// ��ָ�����ļ��ܣ��Զ������ֽ������з����л�������Ķ���
        /// </summary>
        /// <param name="type">���������</param>
        /// <param name="bytes">�������ֽ�����洢���ļ�</param>
        /// <returns></returns>
        public static object DeserializeFromBinary(Type type, string path)
        {
            object o = new object();
            FileStream fs = new FileStream(path, FileMode.Open);

            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                o = serializer.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }
            return o;
        }

        /// <summary>
        /// ��ȡ������ֽ������С
        /// </summary>
        /// <param name="o">����</param>
        /// <returns></returns>
        public static long GetByteSize(object o)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bFormatter.Serialize(stream, o);
            return stream.Length;
        }

        /// <summary>
        /// ��¡һ������
        /// </summary>
        /// <param name="o">����</param>
        /// <returns></returns>
        public static object Clone(object o)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            object cloned = null;

            try
            {
                bFormatter.Serialize(stream, o);
                stream.Seek(0, SeekOrigin.Begin);
                cloned = bFormatter.Deserialize(stream);
            }
            catch //(Exception e)
            {
            }
            finally
            {
                stream.Close();
            }

            return cloned;
        }
    }
}
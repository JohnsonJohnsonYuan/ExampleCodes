using System;
using System.Data;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// ת��IDataReader�ֶζ���ĸ�ʽ������
    /// ����ת����Ĭ��ֵ���ɿ����͵��ֶ�����
    /// </summary>
    public sealed class SmartDataReader
    {
        private DateTime defaultDate;
        private IDataReader reader;

        /// <summary>
        /// ���캯��������IDataReader����
        /// </summary>
        /// <param name="reader"></param>
        public SmartDataReader(IDataReader reader)
        {
            this.defaultDate = Convert.ToDateTime("01/01/1970 00:00:00");
            this.reader = reader;
        }

        /// <summary>
        /// ������ȡ��һ������
        /// </summary>
        public bool Read()
        {
            return this.reader.Read();
        }
        
        /// <summary>
        /// ת��ΪInt��������
        /// </summary>
        public int GetInt32(string column)
        {
            return GetInt32(column, 0);
        }

        /// <summary>
        /// ת��ΪInt��������
        /// </summary>
        public int GetInt32(string column, int defaultIfNull)
        {
            int data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (int)defaultIfNull : int.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪInt��������
        /// </summary>
        public int? GetInt32Nullable(string column)
        {
            int? data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (int?)null : int.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪInt16��������
        /// </summary>
        public short GetInt16(string column)
        {
            return GetInt16(column, 0);
        }

        /// <summary>
        /// ת��ΪInt16��������
        /// </summary>
        public short GetInt16(string column, short defaultIfNull)
        {
            short data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : short.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪInt16��������
        /// </summary>
        public short? GetInt16Nullable(string column)
        {
            short? data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (short?)null : short.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪFloat��������
        /// </summary>
        public float GetFloat(string column)
        {
            return GetFloat(column, 0);
        }

        /// <summary>
        /// ת��ΪFloat��������
        /// </summary>
        public float GetFloat(string column, float defaultIfNull)
        {
            float data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : float.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪFloat��������
        /// </summary>
        public float? GetFloatNullable(string column)
        {
            float? data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (float?)null : float.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪDouble��������
        /// </summary>
        public double GetDouble(string column)
        {
            return GetDouble(column, 0);
        }

        /// <summary>
        /// ת��ΪDouble��������
        /// </summary>
        public double GetDouble(string column, double defaultIfNull)
        {
            double data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : double.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪDouble��������(�ɿ����ͣ�
        /// </summary>
        public double? GetDoubleNullable(string column)
        {
            double? data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (double?)null : double.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪDecimal��������
        /// </summary>
        public decimal GetDecimal(string column)
        {
            return GetDecimal(column, 0);
        }

        /// <summary>
        /// ת��ΪDecimal��������
        /// </summary>
        public decimal GetDecimal(string column, decimal defaultIfNull)
        {
            decimal data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : decimal.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪDecimal��������(�ɿ����ͣ�
        /// </summary>
        public decimal? GetDecimalNullable(string column)
        {
            decimal? data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (decimal?)null : decimal.Parse(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪSingle��������
        /// </summary>
        public Single GetSingle(string column)
        {
            return GetSingle(column, 0);
        }

        /// <summary>
        /// ת��ΪSingle��������
        /// </summary>
        public Single GetSingle(string column, Single defaultIfNull)
		{
            Single data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : Single.Parse(reader[column].ToString());
			return data;
		}

        /// <summary>
        /// ת��ΪSingle��������(�ɿ����ͣ�
        /// </summary>
        public Single? GetSingleNullable(string column)
		{
            Single? data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (Single?)null : Single.Parse(reader[column].ToString());
			return data;
		}

        /// <summary>
        /// ת��Ϊ������������
        /// </summary>
        public bool GetBoolean(string column)
        {
            return GetBoolean(column, false);
        }

        /// <summary>
        /// ת��Ϊ������������
        /// </summary>
        public bool GetBoolean(string column, bool defaultIfNull)
        {
            string str = reader[column].ToString();
            try
            {
                int i = Convert.ToInt32(str);
                return i > 0;
            }
            catch { }

            bool data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : bool.Parse(str);
            return data;
        }

        /// <summary>
        /// ת��Ϊ������������(�ɿ����ͣ�
        /// </summary>
        public bool? GetBooleanNullable(string column)
        {
            string str = reader[column].ToString();
            try
            {
                int i = Convert.ToInt32(str);
                return i > 0;
            }
            catch { }

            bool? data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (bool?)null : bool.Parse(str);
            return data;
        }

        /// <summary>
        /// ת��Ϊ�ַ�����������
        /// </summary>
        public String GetString(string column)
        {
            return GetString(column, "");
        }

        /// <summary>
        /// ת��Ϊ�ַ�����������
        /// </summary>
        public string GetString(string column, string defaultIfNull)
        {
            string data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : reader[column].ToString();
            return data;
        }

        /// <summary>
        /// ת��ΪByte�ֽ�������������
        /// </summary>
        public byte[] GetBytes(string column)
        {
            return GetBytes(column, null);
        }

        /// <summary>
        /// ת��ΪByte�ֽ�������������
        /// </summary>
        public byte[] GetBytes(string column, string defaultIfNull)
        {
            string data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : reader[column].ToString();
            return System.Text.Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// ת��ΪGuid��������
        /// </summary>
        public Guid GetGuid(string column)
        {
            return GetGuid(column, null);
        }

        /// <summary>
        /// ת��ΪGuid��������
        /// </summary>
        public Guid GetGuid(string column, string defaultIfNull)
        {
            string data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : reader[column].ToString();
            Guid guid = Guid.Empty;
            if (data != null)
            {
                guid = new Guid(data);             
            }
            return guid;
        }

        /// <summary>
        /// ת��ΪGuid��������(�ɿ����ͣ�
        /// </summary> 
        public Guid? GetGuidNullable(string column)
        {
            string data = (reader.IsDBNull(reader.GetOrdinal(column))) ? null : reader[column].ToString();
            Guid? guid = null;
            if (data != null)
            {
                guid = new Guid(data);
            }
            return guid;
        }

        /// <summary>
        /// ת��ΪDateTime��������
        /// </summary>
        public DateTime GetDateTime(string column)
        {
            return GetDateTime(column, defaultDate);
        }

        /// <summary>
        /// ת��ΪDateTime��������
        /// </summary>
        public DateTime GetDateTime(string column, DateTime defaultIfNull)
        {
            DateTime data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultIfNull : Convert.ToDateTime(reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��Ϊ�ɿ�DateTime��������
        /// </summary>
        public DateTime? GetDateTimeNullable(string column)
        {
            DateTime? data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (DateTime?)null : Convert.ToDateTime(reader[column].ToString());
            return data;
        }
    }

}

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// This class provides some utility methods to convert the xml and object.
    /// </summary>
    public sealed class XmlConvertor
    {
        private XmlConvertor()
        {
        }

        /// <summary>
        /// Converts the xml string to the specified object.
        /// </summary>
        /// <param name="xml">The xml string.</param>
        /// <param name="type">The object type.</param>
        /// <returns>The object deserialized from the xml string.</returns>
        public static object XmlToObject(string xml, Type type)
        {
            if (null == xml)
            {
                throw new ArgumentNullException("xml");
            }
            if (null == type)
            {
                throw new ArgumentNullException("type");
            }

            object obj = null;
            XmlSerializer serializer = new XmlSerializer(type);
            StringReader strReader = new StringReader(xml);
            XmlReader reader = new XmlTextReader(strReader);

            try
            {
                obj = serializer.Deserialize(reader);
            }
            catch (InvalidOperationException ie)
            {
                throw new InvalidOperationException("Can not convert xml to object", ie);
            }
            finally
            {
                reader.Close();
            }
            return obj;
        }

        /// <summary>
        /// Converts the object to xml string.
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="toBeIndented"><c>true</c> if wants the xml string was indented, otherwise <c>false</c>.</param>
        /// <returns>The xml string.</returns>
        public static string ObjectToXml(object obj, bool toBeIndented)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            UTF8Encoding encoding = new UTF8Encoding(false);
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, encoding);
            writer.Formatting = (toBeIndented ? Formatting.Indented : Formatting.None);

            try
            {
                serializer.Serialize(writer, obj);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Can not convert object to xml.");
            }
            finally
            {
                writer.Close();
            }

            string xml = encoding.GetString(stream.ToArray());
            return xml;
        }
    }
}
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechInfrastructure
{
    [XmlRoot("Dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}

/*
http://www.codeproject.com/Questions/454134/Serialize-Dictionary-in-csharp
SerializableDictionary<string, List<string>> b = new SerializableDictionary<string, List<string>>();
List<string> stringList = new List<string>();
stringList.Add("1");
b.Add("One", stringList);
                       
XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, List<string>>));
TextWriter textWriter = new StreamWriter(@"dictionarySerialized.xml");
serializer.Serialize(textWriter, b);
textWriter.Close();
*/

/*
private void Deserialize()
{
try
{
    var f_fileStream = File.OpenRead(@"dictionarySerialized.xml");
    var f_binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    myDictionary = (Dictionary<string,>)f_binaryFormatter.Deserialize(f_fileStream);
    f_fileStream.Close();
}
catch (Exception ex)
{
    ;
}
}
private void Serialize()
{
try
{
    var f_fileStream = new FileStream(@"dictionarySerialized.xml", FileMode.Create, FileAccess.Write);
    var f_binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    f_binaryFormatter.Serialize(f_fileStream, myDictionary);
    f_fileStream.Close();
}
catch (Exception ex)
{
    ;
}
}*/

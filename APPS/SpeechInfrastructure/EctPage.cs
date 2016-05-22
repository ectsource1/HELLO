using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpeechInfrastructure
{
    public class EctPage
    {
        public string Txt { get; set; }
        public string ImgFile  { get; set; }
        public string PlayFile { get; set; }
        public string SubTitle { get; set; }

        public string append2String(string s1, string s2, string delimiter)
        {
            return (s1 + delimiter + s2);
        }
    }

    public class EctModule
    {
        public EctModule()
        {
            Pages = new ObservableCollection<EctPage>();
            Dialogs = new List<KeyValuePair<string, string>>();
        }
        // module type
        public string Type { get; set; }
        // title
        public string Title { get; set; }
        // vocabuary
        public string Vocab { get; set; }

        public ObservableCollection<EctPage> Pages { get; set; }
        public List<KeyValuePair<string, string>> Dialogs { get; set; }
    }

    public class EctXML
    {
        public static void writeClassXML(EctModule module, string xmlFile)
        {
            XmlSerializer writer = new XmlSerializer(typeof(EctModule));
            FileStream file = File.Create(xmlFile);

            writer.Serialize(file, module);
            file.Close();
        }

        public static EctModule readClassXML(string xmlFile)
        {
            EctModule obj = null;
            if (!File.Exists(xmlFile)) return obj;

            StreamReader file = new StreamReader(xmlFile);
            XmlSerializer reader = new XmlSerializer(typeof(EctModule));
            
            obj = (EctModule)reader.Deserialize(file);
            file.Close();

            return obj;
        }
    }
}

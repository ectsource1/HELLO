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
        }
        // module type
        public string Type { get; set; }
        // title
        public string Title { get; set; }
        // vocabuary
        public string Vocab { get; set; }
        // Dialogs
        public string Dialogs { get; set; }

        public ObservableCollection<EctPage> Pages { get; set; }

        public EctModule clone()
        {
            EctModule ret = new EctModule();
            ret.Title = this.Title;
            ret.Type = this.Type;
            ret.Dialogs = this.Dialogs;
            ret.Vocab = this.Vocab;
            int cnt = Pages.Count;
            for (int i = 0; i < cnt; i++)
            {
                ret.Pages.Add(this.Pages[i]);
            }
            return ret;
        } 
        public static void copy(EctModule m1, EctModule m2)
        { 
            m2.Title = m1.Title;
            m2.Type = m1.Type;
            m2.Dialogs = m1.Dialogs;
            m2.Vocab = m1.Vocab;

            // clear m2 first
            m2.Pages.Clear();

            int cnt = m1.Pages.Count;
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    m2.Pages.Add(m1.Pages[i]);
                }
            }
        }
          

       
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

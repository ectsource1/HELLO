using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpeechInfrastructure
{
    [Serializable]
    public class Offline
    {
        public static string OFF_BIN = "off.bin";

        public bool NormalLogin { get; set; }

        public DateTime LoginDate { get; set; }

        public static void write(Offline offline, string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, offline);
            stream.Close();
        }

        public static Offline read(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            Offline obj = (Offline)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }

    }
}

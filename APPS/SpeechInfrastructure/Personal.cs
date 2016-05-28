using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpeechInfrastructure
{
    [Serializable]
    public class Personal : ICloneable
    {
        public static string PERSON_BIN = "PERSON.bin";
        public static DateTime start = new DateTime(2000, 1, 1);

        public string StudentId { get; set; }

        public string StudentName { get; set; }

        public string Sex { get; set; }

        public string Hobby { get; set; }

        public string Nickname { get; set; }

        public string Birthdate { get; set; }

        public string Dadname { get; set; }

        public string Momname { get; set; }

        public string Fgrandpa { get; set; }

        public string Fgrandma { get; set; }

        public string Mgrandpa { get; set; }

        public string Mgrandma { get; set; }

        public string Sister { get; set; }

        public string Brother { get; set; }

        public string Cousin { get; set; }

        public string Uncle { get; set; }

        public string Aunt { get; set; }

        public string Friend { get; set; }

        public bool NormalLogin { get; set; }

        public DateTime LoginDate { get; set; }

        public bool validate(int inputDays, int allow)
        {
            bool ret = false;
            int last = (LoginDate.Date - start.Date).Days;
            int curr = (DateTime.Now - start.Date).Days;
            int diff = curr - last;
            int num = last + 129;
            if (diff < allow && num == inputDays) ret = true;

            return ret;
        }

        public bool isFilled()
        {
            bool notFilled = string.IsNullOrEmpty(Sex) ||
                             string.IsNullOrEmpty(Hobby) ||
                             string.IsNullOrEmpty(Nickname) ||
                             string.IsNullOrEmpty(Birthdate) ||
                             string.IsNullOrEmpty(Dadname) ||
                             string.IsNullOrEmpty(Momname) ||
                             string.IsNullOrEmpty(Fgrandpa) ||
                             string.IsNullOrEmpty(Fgrandma) ||
                             string.IsNullOrEmpty(Mgrandpa) ||
                             string.IsNullOrEmpty(Mgrandma) ||
                             string.IsNullOrEmpty(Sister) ||
                             string.IsNullOrEmpty(Brother) ||
                             string.IsNullOrEmpty(Cousin) ||
                             string.IsNullOrEmpty(Uncle) ||
                             string.IsNullOrEmpty(Aunt) ||
                             string.IsNullOrEmpty(Friend);
            return !notFilled;
        }

        public static void write(Personal person, string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, person);
            stream.Close();
        }

        public static Personal read(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName,
                                      FileMode.Open,
                                      FileAccess.Read,
                                      FileShare.Read);
            Personal obj = (Personal)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }

        public object Clone()
        {
            Personal doc = (Personal)this.MemberwiseClone();
            return doc;
        }
    }
}

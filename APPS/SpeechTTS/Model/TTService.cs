//-------------------------------------------------
// Copyright (c) ECT Education Group
// Author: Charlie Jiang
// Date  : 03/02/16
//-------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using SpeechInfrastructure;
using System.Text.RegularExpressions;

namespace SpeechTTS.Model
{
    [Export(typeof(ITTService))]
    public class TTService : ITTService
    {
        public static string A = "A::";
        public static string B = "B::";
        public static string SUB = "IDIOM::";

        public static string STORY = ".NOTES";
        public static string CARTOON = ".CARTOON";
        public static string CLASS = ".CLASS";
        public static string IDIOM = ".IDIOM";
        public static string SOUND = ".SOUND";
        public static string GRAMMAR = ".GRAMMAR";

        public static string TITLE_KEY = "TITLE::";
        public static string VOCAB_KEY = "VOCAB::";
        public static string DIALOG_KEY = "DIALOG::";
        public static string PAGE_KEY = "PAGE::";
        public static string SEP_CHAR = "::";

        public static string FUN_FILE     = "ectclass.notes";
        public static string CARTOON_FILE = "ectclass.cartoon";
        public static string CLASS_FILE   = "ectclass.class";
        public static string IDIOM_FILE   = "ectclass.idiom";
        public static string SOUND_FILE   = "ectclass.sound";
        public static string GRAMMAR_FILE = "ectclass.grammar";

        public int numNotes   = 0;
        public int numCarts   = 0;
        public int numClasses = 0;
        public int numIdioms = 0;
        public int numSounds = 0;
        public int numGrammars = 0;

        private readonly List<TextDocument> funDocuments;
        private readonly List<TextDocument> cardsDocuments;
        private readonly List<TextDocument> activitiesDocuments;
        private readonly List<TextDocument> idiomsDocuments;
        private readonly List<TextDocument> soundDocuments;
        private readonly List<TextDocument> grammarDocuments;

        private string _appDataRoot  = "";
        private string _userDataRoot = "";
        private string _userAudioRoot = "";

        public TTService()
        {
            bool newFile = false;
            string userRoot = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _userDataRoot = userRoot + @"\ECTData\";
            if (!Directory.Exists(_userDataRoot))
            {
                Directory.CreateDirectory(_userDataRoot);
            }

            _userAudioRoot = _userDataRoot + @"\ECTAudio\";
            if (!Directory.Exists(_userAudioRoot))
            {
                Directory.CreateDirectory(_userAudioRoot);
            }


            // setup application default files
            _appDataRoot = AppDomain.CurrentDomain.BaseDirectory + "DataFiles\\";
            string personPath = _appDataRoot + Personal.PERSON_BIN;
            string studentId = "";
            string studentName = "";
            if (File.Exists(personPath))
            {
                Personal person = Personal.read(personPath);
                studentId = person.StudentId;
                studentName = person.StudentName;
            }

            // Cartoons documents
            this.cardsDocuments = new List<TextDocument>();
            string _path = _appDataRoot + "Cartoons\\";
            TextDocument doc = new TextDocument();
            doc.FileName = _path + "LM.cartoon";
            doc.Subject = "The Lion and The Mouse";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.cardsDocuments.Add(doc);
            numCarts = 1;

            // Class documents
            this.activitiesDocuments = new List<TextDocument>();
            _path = _appDataRoot + "Classes\\";
            doc = new TextDocument();
            doc.FileName = _path + "classsample.class";
            doc.Subject = "The Body Parts";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.activitiesDocuments.Add(doc);
            numClasses = 1;

            // Idiom documents
            this.idiomsDocuments = new List<TextDocument>();
            _path = _appDataRoot + "Idioms\\";
            doc = new TextDocument();
            doc.FileName = _path + "idiomsample.idiom";
            doc.Subject = "15 Commom Idioms";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.idiomsDocuments.Add(doc);
            numIdioms = 1;

            // pronunciation documents
            this.soundDocuments = new List<TextDocument>();
            _path = _appDataRoot + "Sounds\\";
            doc = new TextDocument();
            doc.FileName = _path + "aeiou.sound";
            doc.Subject = "Vowel Sounds";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.soundDocuments.Add(doc);
            numSounds = 1;

            // grammar documents
            this.grammarDocuments = new List<TextDocument>();
            _path = _appDataRoot + "Grammars\\";
            doc = new TextDocument();
            doc.FileName = _path + "singular.grammar";
            doc.Subject = "Singular and Plurals";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.grammarDocuments.Add(doc);

            doc = new TextDocument();
            doc.FileName = _path + "countNouns.grammar";
            doc.Subject = "Count and Non-Count Nouns";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.grammarDocuments.Add(doc);
            numGrammars = 2;

            // Notes documents
            this.funDocuments = new List<TextDocument>(); 
            _path = _appDataRoot + "Notes\\";
            numNotes = 8;
            // 1 ok
            doc = new TextDocument();
            doc.FileName = _path + "myVocab.notes";
            doc.Subject = "My Vocaburary 词汇";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            // 3 ok
            doc = new TextDocument();
            doc.FileName = _path + "irregularVerb.notes";
            doc.Subject = "Irregular Verb 不规则动词";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            // ok
            doc = new TextDocument();
            doc.FileName = _path + "plurals.notes";
            doc.Subject = "Plural Nouns 不规则名词复数";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            doc.FileName = _path + "synonyms.notes";
            doc.Subject = "Synonyms 近义词";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            doc.FileName = _path + "antonyms.notes";
            doc.Subject = "Antonyms 反义词";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            doc.FileName = _path + "soundHard.notes";
            doc.Subject = "Words that are hard to pronounce correctly 难发音的单词";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            doc.FileName = _path + "soundClose.notes";
            doc.Subject = "Words that have close sounds 发音接近的单词";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            // ok
            doc = new TextDocument();
            doc.FileName = _path + "famous.notes";
            doc.Subject = "Famous Epigram 警句名言";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);


            string filePath = _userDataRoot + FUN_FILE;
            if (!File.Exists(filePath))
            {
                newFile = true;
                File.Create(filePath);
            }

            if (!newFile)
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] col = line.Split(new char[] { ',' });
                    if ( col.Length > 1 && File.Exists(col[0]) )
                    {
                        doc = new TextDocument();
                        doc.FileName = col[0];
                        doc.Subject = col[1];
                        doc.StudentId = studentId;
                        doc.From = studentName;
                        this.funDocuments.Add(doc);
                    }
                    
                } // foreach
            }// new file

            // cartoon documents
            newFile = false;
            filePath = _userDataRoot + CARTOON_FILE;
            if (!File.Exists(filePath))
            {
                newFile = true;
                File.Create(filePath);
            }

            if (!newFile)
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] col = line.Split(new char[] { ',' });
                    if (col.Length > 1 && File.Exists(col[0]))
                    {
                        doc = new TextDocument();
                        doc.FileName = col[0];
                        doc.Subject = col[1];
                        doc.StudentId = studentId;
                        doc.From = studentName;
                        this.cardsDocuments.Add(doc);
                    }

                } // foreach
            }// new file

            // activites documents
            newFile = false;
            filePath = _userDataRoot + CLASS_FILE;
            if (!File.Exists(filePath))
            {
                newFile = true;
                File.Create(filePath);
            }

            if (!newFile)
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] col = line.Split(new char[] { ',' });
                    if (col.Length > 1 && File.Exists(col[0]))
                    {
                        doc = new TextDocument();
                        doc.FileName = col[0];
                        doc.Subject = col[1];
                        doc.StudentId = studentId;
                        doc.From = studentName;
                        this.activitiesDocuments.Add(doc);
                    }

                } // foreach
            }// new file

            // idioms documents
            newFile = false;
            filePath = _userDataRoot + IDIOM_FILE;
            if (!File.Exists(filePath))
            {
                newFile = true;
                File.Create(filePath);
            }

            if (!newFile)
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] col = line.Split(new char[] { ',' });
                    if (col.Length > 1 && File.Exists(col[0]))
                    {
                        doc = new TextDocument();
                        doc.FileName = col[0];
                        doc.Subject = col[1];
                        doc.StudentId = studentId;
                        doc.From = studentName;
                        this.idiomsDocuments.Add(doc);
                    }

                } // foreach
            }// new file

            // pronunciation documents
            newFile = false;
            filePath = _userDataRoot + SOUND_FILE;
            if (!File.Exists(filePath))
            {
                newFile = true;
                File.Create(filePath);
            }

            if (!newFile)
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] col = line.Split(new char[] { ',' });
                    if (col.Length > 1 && File.Exists(col[0]))
                    {
                        doc = new TextDocument();
                        doc.FileName = col[0];
                        doc.Subject = col[1];
                        doc.StudentId = studentId;
                        doc.From = studentName;
                        this.soundDocuments.Add(doc);
                    }

                } // foreach
            }// new file

            // grammar documents
            newFile = false;
            filePath = _userDataRoot + GRAMMAR_FILE;
            if (!File.Exists(filePath))
            {
                newFile = true;
                File.Create(filePath);
            }

            if (!newFile)
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] col = line.Split(new char[] { ',' });
                    if (col.Length > 1 && File.Exists(col[0]))
                    {
                        doc = new TextDocument();
                        doc.FileName = col[0];
                        doc.Subject = col[1];
                        doc.StudentId = studentId;
                        doc.From = studentName;
                        this.grammarDocuments.Add(doc);
                    }

                } // foreach
            }// new file
        }

        public int getNumNotes()
        {
            return numNotes;
        }
        public int getNumCarts()
        {
            return numCarts;
        }

        public int getNumClasses()
        {
            return numClasses;
        }
        public int getNumIdioms()
        {
            return numIdioms;
        }
        public int getNumSounds()
        {
            return numSounds;
        }
        public int getNumGrammars()
        {
            return numGrammars;
        }

        public string getAudioPath()
        {
             return this._userAudioRoot;
        }

        public void SaveFunDocuments()
        {
            string filePath = _userDataRoot + FUN_FILE;
            int i = 0;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                foreach (TextDocument doc in funDocuments)
                {
                    i++;
                    if ( i > numNotes )
                       file.WriteLine(doc.append2String(doc.FileName, doc.Subject, ","));  
                }
            }
        }


        public IAsyncResult BeginGetFunDocuments(AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<IEnumerable<TextDocument>>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    asyncResult.SetComplete(new ReadOnlyCollection<TextDocument>(this.funDocuments), false);
                });

            return asyncResult;
        }

        public IEnumerable<TextDocument> EndGetFunDocuments(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<IEnumerable<TextDocument>>.End(asyncResult);

            return localAsyncResult.Result;
        }

        public IAsyncResult BeginSendFunDocument(TextDocument text, AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<object>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    Thread.Sleep(500);
                    asyncResult.SetComplete(null, false);
                });

            return asyncResult;
        }

        public void EndSendFunDocument(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<object>.End(asyncResult);
        }

        public TextDocument GetFunDocument(Guid id)
        {
            TextDocument doc = this.funDocuments.FirstOrDefault(e => e.Id == id);

            if (string.IsNullOrEmpty(doc.Text) ) {
               try
               {
                   using (StreamReader sr = new StreamReader(doc.FileName))
                   {
                       
                        if (doc.FileName.ToUpper().Contains(STORY))
                        {
                            string line;
                            while ( (line = sr.ReadLine()) != null )
                            {
                                if (!line.Contains(TITLE_KEY))
                                    doc.Text += line + "\n";
                            }
                        }
                   }
               }
               catch (Exception e)
               {
                   Console.WriteLine("The file could not be read:");
                   Console.WriteLine(e.Message);
                }
            }

            return doc;
        }

        
        public void AddFunDocument(TextDocument doc)
        {
            this.funDocuments.Add(doc);
            SaveFunDocuments();
        }

        public int FunIdx(TextDocument doc)
        {
            int idx = funDocuments.IndexOf(doc);
            return idx;
        }

        public int CardIdx(TextDocument doc)
        {
            int idx = cardsDocuments.IndexOf(doc);
            return idx;
        }

        public int ClassIdx(TextDocument doc)
        {
            int idx = activitiesDocuments.IndexOf(doc);
            return idx;
        }

        public int IdiomIdx(TextDocument doc)
        {
            int idx = idiomsDocuments.IndexOf(doc);
            return idx;
        }

        public int SoundIdx(TextDocument doc)
        {
            int idx = soundDocuments.IndexOf(doc);
            return idx;
        }

        public int GrammarIdx(TextDocument doc)
        {
            int idx = grammarDocuments.IndexOf(doc);
            return idx;
        }

        public void RemoveFunDocument(TextDocument doc)
        {
            funDocuments.Remove(doc);
            SaveFunDocuments();
        }

        // cartoon documents
        public void SaveCardsDocuments()
        {
            string filePath = _userDataRoot + CARTOON_FILE;
            int i = 0;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                foreach (TextDocument doc in cardsDocuments)
                {
                    i++;
                    if (i > numCarts)
                       file.WriteLine(doc.append2String(doc.FileName, doc.Subject, ","));
                }
            }
        }
 
        public IAsyncResult BeginGetCardsDocuments(AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<IEnumerable<TextDocument>>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    asyncResult.SetComplete(new ReadOnlyCollection<TextDocument>(this.cardsDocuments), false);
                });

            return asyncResult;
        }

        public IEnumerable<TextDocument> EndGetCardsDocuments(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<IEnumerable<TextDocument>>.End(asyncResult);

            return localAsyncResult.Result;
        }

        public IAsyncResult BeginSendCardsDocument(TextDocument text, AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<object>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    Thread.Sleep(500);
                    asyncResult.SetComplete(null, false);
                });

            return asyncResult;
        }

        public void EndSendCardsDocument(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<object>.End(asyncResult);
        }

        public TextDocument GetCardsDocument(Guid id)
        {
            TextDocument doc = this.cardsDocuments.FirstOrDefault(e => e.Id == id);

            if (string.IsNullOrEmpty(doc.Text))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(doc.FileName))
                    {
                        if (doc.FileName.ToUpper().Contains(CARTOON))
                        {
                            int idx = -1;
                            bool paged = false;
                            string line;
                            string multiLines = null;
                            while ((line = sr.ReadLine()) != null)
                            {
                                //if (line.Contains(TTService.TITLE_KEY)) continue;

                                if (line.ToUpper().Contains(PAGE_KEY))
                                {
                                    if (multiLines != null) doc.addTxtList(multiLines);
                                    string[] col = Regex.Split(line, TTService.SEP_CHAR);
                                    doc.addImage(col[1]);
                                    if (col.Length > 2)
                                        doc.addMyAudio(col[2]);
                                    multiLines = null;
                                    idx = 0;
                                    paged = true;
                                } else {
                                    idx += 1;
                                }

                                if ( paged && idx > 0)
                                {
                                    multiLines = multiLines + line + "\n";
                                }       
                                
                            }

                            doc.addTxtList(multiLines);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            string txt = "";
            if (doc.TxtList.Count > 0)
               txt =  doc.TxtList[0];

            doc.Idx = 0;
            doc.Text = txt;

            return doc;
        }

        public void AddCardsDocument(TextDocument doc)
        {
            this.cardsDocuments.Add(doc);
            SaveCardsDocuments();
        }

        public void RemoveCardsDocument(TextDocument doc)
        {
            cardsDocuments.Remove(doc);
            SaveCardsDocuments();
        }

        // activities documents
        public void SaveActivitiesDocuments()
        {
            string filePath = _userDataRoot + CLASS_FILE;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                int i = 0;
                foreach (TextDocument doc in activitiesDocuments)
                {
                    i++;
                    if ( i > numClasses)
                    file.WriteLine(doc.append2String(doc.FileName, doc.Subject, ","));
                }
            }
        }

        public IAsyncResult BeginGetActivitiesDocuments(AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<IEnumerable<TextDocument>>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    asyncResult.SetComplete(new ReadOnlyCollection<TextDocument>(this.activitiesDocuments), false);
                });

            return asyncResult;
        }

        public IEnumerable<TextDocument> EndGetActivitiesDocuments(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<IEnumerable<TextDocument>>.End(asyncResult);

            return localAsyncResult.Result;
        }

        public IAsyncResult BeginSendActivitiesDocument(TextDocument text, AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<object>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    Thread.Sleep(500);
                    asyncResult.SetComplete(null, false);
                });

            return asyncResult;
        }

        public void EndSendActivitiesDocument(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<object>.End(asyncResult);
        }

        public TextDocument GetActivitiesDocument(Guid id)
        {
            TextDocument doc = this.activitiesDocuments.FirstOrDefault(e => e.Id == id);

            if (string.IsNullOrEmpty(doc.Text))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(doc.FileName))
                    {
                        if (doc.FileName.ToUpper().Contains(CLASS))
                        {
                            bool isFirst = false;
                            int txtType = 10;
                            string line;
                            string pageLines = null;
                            string vocabLines = null;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.ToUpper().Contains(VOCAB_KEY))
                                {
                                    isFirst = true;
                                    txtType = 0;
                                }

                                if (line.ToUpper().Contains(DIALOG_KEY))
                                {
                                    isFirst = true;
                                    txtType = 1;
                                }

                                if (line.ToUpper().Contains(PAGE_KEY))
                                {
                                    isFirst = true;
                                    txtType = 2;
                                    if (pageLines != null) doc.addTxtList(pageLines);
                                    string[] col = Regex.Split(line, TTService.SEP_CHAR);
                                    doc.addImage(col[1]);
                                    pageLines = null;
                                }

                                if (txtType == 0)
                                {
                                    if (isFirst) isFirst = false;
                                    else vocabLines = vocabLines + line + "\n";
                                }

                                if (txtType == 1)
                                {
                                    if (isFirst) isFirst = false;
                                    else
                                    {
                                        if (line.Contains(A) || line.Contains(B) )
                                        {
                                            string[] col = Regex.Split(line, TTService.SEP_CHAR);
                                            doc.addGender(col[0]);
                                            doc.addSentence(col[1]);
                                            doc.MergeSentences(col[0], col[1]);
                                        }
                                    }
                                }

                                if (txtType == 2)
                                {
                                    if (isFirst) isFirst = false;
                                    else pageLines = pageLines + line + "\n";
                                }
                                
                            }

                            doc.addTxtList(pageLines);
                            doc.Vocalburay = vocabLines;

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            string txt = "";
            if (doc.TxtList.Count > 0)
                txt = doc.TxtList[0];

            doc.Idx = 0;
            doc.Text = txt;

            return doc;
        }

        public void AddActivitiesDocument(TextDocument doc)
        {
            this.activitiesDocuments.Add(doc);
            SaveActivitiesDocuments();
        }
        
        public void RemoveActivitiesDocument(TextDocument doc)
        {
            activitiesDocuments.Remove(doc);
            SaveActivitiesDocuments();
        }

        // idioms documents
        public void SaveIdiomsDocuments()
        {
            string filePath = _userDataRoot + IDIOM_FILE;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                int i = 0;
                foreach (TextDocument doc in idiomsDocuments)
                {
                    i++;
                    if ( i > numIdioms )
                    file.WriteLine(doc.append2String(doc.FileName, doc.Subject, ","));
                }
            }
        }

        public IAsyncResult BeginGetIdiomsDocuments(AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<IEnumerable<TextDocument>>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    asyncResult.SetComplete(new ReadOnlyCollection<TextDocument>(this.idiomsDocuments), false);
                });

            return asyncResult;
        }

        public IEnumerable<TextDocument> EndGetIdiomsDocuments(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<IEnumerable<TextDocument>>.End(asyncResult);

            return localAsyncResult.Result;
        }

        public IAsyncResult BeginSendIdiomsDocument(TextDocument text, AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<object>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    Thread.Sleep(500);
                    asyncResult.SetComplete(null, false);
                });

            return asyncResult;
        }

        public void EndSendIdiomsDocument(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<object>.End(asyncResult);
        }

        public TextDocument GetIdiomsDocument(Guid id)
        {
            TextDocument doc = this.idiomsDocuments.FirstOrDefault(e => e.Id == id);

            if (string.IsNullOrEmpty(doc.Text))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(doc.FileName))
                    {
                        if (doc.FileName.ToUpper().Contains(IDIOM))
                        {
                            bool isFirst = false;
                            int txtType = 0;
                            string line;
                            string pageLines = null;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.ToUpper().Contains(DIALOG_KEY))
                                {
                                    isFirst = true;
                                    txtType = 1;
                                }

                                if (line.ToUpper().Contains(PAGE_KEY))
                                {
                                    isFirst = true;
                                    txtType = 2;
                                    if (pageLines != null) doc.addTxtList(pageLines);
                                    string[] col = Regex.Split(line, TTService.SEP_CHAR);
                                    doc.addImage(col[1]);
                                    doc.addSubject(col[2]);
                                    pageLines = null;
                                }

                                if (txtType == 1)
                                {
                                    if (isFirst) isFirst = false;
                                    else
                                    {
                                        if (line.ToUpper().Contains(A) || line.ToUpper().Contains(B) 
                                            || line.ToUpper().Contains(SUB))
                                        {
                                            string[] col = Regex.Split(line, TTService.SEP_CHAR);
                                            if (line.ToUpper().Contains(SUB) && doc.GenderList.Count > 0)
                                            {
                                                doc.InsertEmptyLine();
                                            }
                                            doc.addGender(col[0]);
                                            doc.addSentence(col[1]);
                                            
                                            doc.MergeSentences(col[0], col[1]);
                                        }
                                    }
                                }

                                if (txtType == 2)
                                {
                                    if (isFirst) isFirst = false;
                                    else pageLines = pageLines + line + "\n";
                                }

                            }

                            doc.addTxtList(pageLines);

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            string txt = "";
            if (doc.TxtList.Count > 0)
                txt = doc.TxtList[0];

            doc.Idx = 0;
            doc.Text = txt;

            return doc;
        }

        public void AddIdiomsDocument(TextDocument doc)
        {
            this.idiomsDocuments.Add(doc);
            SaveIdiomsDocuments();
        }

        public void RemoveIdiomsDocument(TextDocument doc)
        {
            idiomsDocuments.Remove(doc);
            SaveIdiomsDocuments();
        }

        // pronunciation documents
        public void SaveSoundDocuments()
        {
            string filePath = _userDataRoot + SOUND_FILE;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                int i = 0;
                foreach (TextDocument doc in soundDocuments)
                {
                    i++;
                    if ( i > numSounds )
                    file.WriteLine(doc.append2String(doc.FileName, doc.Subject, ","));
                }
            }
        }

        public IAsyncResult BeginGetSoundDocuments(AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<IEnumerable<TextDocument>>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    asyncResult.SetComplete(new ReadOnlyCollection<TextDocument>(this.soundDocuments), false);
                });

            return asyncResult;
        }

        public IEnumerable<TextDocument> EndGetSoundDocuments(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<IEnumerable<TextDocument>>.End(asyncResult);

            return localAsyncResult.Result;
        }

        public IAsyncResult BeginSendSoundDocument(TextDocument text, AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<object>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    Thread.Sleep(500);
                    asyncResult.SetComplete(null, false);
                });

            return asyncResult;
        }

        public void EndSendSoundDocument(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<object>.End(asyncResult);
        }

        public TextDocument GetSoundDocument(Guid id)
        {
            TextDocument doc = this.soundDocuments.FirstOrDefault(e => e.Id == id);

            if (string.IsNullOrEmpty(doc.Text))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(doc.FileName))
                    {
                        if (doc.FileName.ToUpper().Contains(SOUND))
                        {
                            bool isFirst = false;
                            int txtType = 0;
                            string line;
                            string pageLines = null;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.ToUpper().Contains(TITLE_KEY))
                                {
                                    string[] col = Regex.Split(line, TTService.SEP_CHAR);
                                    doc.Subject = col[1];
                                    continue;
                                }
                               
                                if (line.ToUpper().Contains(PAGE_KEY))
                                {
                                    isFirst = true;
                                    txtType = 2;
                                    if (pageLines != null) doc.addTxtList(pageLines);
                                    string[] col = Regex.Split(line, TTService.SEP_CHAR);
                                    doc.addMyAudio(col[1]);
                                    doc.addImage(col[2]);
                                    doc.addSubject(col[3]);
                                    pageLines = null;
                                }

                                if (txtType == 2)
                                {
                                    if (isFirst) isFirst = false;
                                    else pageLines = pageLines + line + "\n";
                                }

                            }

                            doc.addTxtList(pageLines);

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            string txt = "", subj = "";
            if (doc.TxtList.Count > 0)
            {
                txt = doc.TxtList[0];
                subj = doc.SubjectList[0];
            }
                
            doc.Idx = 0;
            doc.Text = txt;
            doc.SubSubject = subj;

            return doc;
        }

        public void AddSoundDocument(TextDocument doc)
        {
            this.soundDocuments.Add(doc);
            SaveSoundDocuments();
        }

        public void RemoveSoundDocument(TextDocument doc)
        {
            soundDocuments.Remove(doc);
            SaveSoundDocuments();
        }

        // grammar documents
        public void SaveGrammarDocuments()
        {
            string filePath = _userDataRoot + GRAMMAR_FILE;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                int i = 0;
                foreach (TextDocument doc in grammarDocuments)
                {
                    i++;
                    if ( i > numGrammars )
                    file.WriteLine(doc.append2String(doc.FileName, doc.Subject, ","));
                }
            }
        }

        public IAsyncResult BeginGetGrammarDocuments(AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<IEnumerable<TextDocument>>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    asyncResult.SetComplete(new ReadOnlyCollection<TextDocument>(this.grammarDocuments), false);
                });

            return asyncResult;
        }

        public IEnumerable<TextDocument> EndGetGrammarDocuments(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<IEnumerable<TextDocument>>.End(asyncResult);

            return localAsyncResult.Result;
        }

        public IAsyncResult BeginSendGrammarDocument(TextDocument text, AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<object>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    Thread.Sleep(500);
                    asyncResult.SetComplete(null, false);
                });

            return asyncResult;
        }

        public void EndSendGrammarDocument(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<object>.End(asyncResult);
        }

        public TextDocument GetGrammarDocument(Guid id)
        {
            TextDocument doc = this.grammarDocuments.FirstOrDefault(e => e.Id == id);

            if (string.IsNullOrEmpty(doc.Text))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(doc.FileName))
                    {
                        if (doc.FileName.ToUpper().Contains(GRAMMAR))
                            doc.Text = sr.ReadToEnd();
                        else if (doc.FileName.Contains(".dic"))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                string[] col = line.Split(new char[] { ' ' });
                                doc.Text += col[0] + "\n";
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            return doc;
        }


        public void AddGrammarDocument(TextDocument doc)
        {
            this.grammarDocuments.Add(doc);
            SaveGrammarDocuments();
        }

        public void RemoveGrammarDocument(TextDocument doc)
        {
            grammarDocuments.Remove(doc);
            SaveGrammarDocuments();
        }

    }
}

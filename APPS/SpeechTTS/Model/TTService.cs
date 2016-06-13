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

        public static string MISSED_IMG = "DataFiles\\missing.jpg";
        public static string MISSED_MP3 = "DataFiles\\missing.mp3";

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
        private string _userDefaultRoot = "";

        public TTService()
        {
            bool newFile = false;
            string userRoot = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _userDataRoot = userRoot + @"\ECTData\";
            if (!Directory.Exists(_userDataRoot))
            {
                Directory.CreateDirectory(_userDataRoot);
            }

            _userDefaultRoot = _userDataRoot + @"ECT\";
            if (!Directory.Exists(_userDefaultRoot))
            {
                Directory.CreateDirectory(_userDefaultRoot);
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
            bool editable = false;
            if (File.Exists(personPath))
            {
                Personal person = Personal.read(personPath);
                studentId = person.StudentId;
                studentName = person.StudentName;
                editable = person.Editable;
            }

            // Cartoons documents
            numCarts = 1;
            this.cardsDocuments = new List<TextDocument>();
            string _path = _appDataRoot + "Cartoons\\";
            TextDocument doc = new TextDocument();
            doc.Editable = editable;
            doc.FileName = _path + "LM.cartoon";
            doc.Subject = "The Lion and The Mouse";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.cardsDocuments.Add(doc);

            doc = new TextDocument();
            doc.Editable = editable;
            doc.FileName = _path + "2Goose.cartoon";
            doc.Subject = "The Goose that laid the golden eggs";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.cardsDocuments.Add(doc);


            // Class documents
            this.activitiesDocuments = new List<TextDocument>();
            _path = _appDataRoot + "Classes\\";
            doc = new TextDocument();
            doc.Editable = editable;
            doc.FileName = _path + "classsample.class";
            doc.Subject = "The Body Parts";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.activitiesDocuments.Add(doc);
            numClasses = 1;

            // Idiom documents
            numIdioms = 3;
            this.idiomsDocuments = new List<TextDocument>();
            _path = _appDataRoot + "Idioms\\";
            doc = new TextDocument();
            doc.Editable = editable;
            doc.FileName = _path + "idiomsample.idiom";
            doc.Subject = "1-10 Commom Idioms";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.idiomsDocuments.Add(doc);

            doc = new TextDocument();
            doc.Editable = editable;
            doc.FileName = _path + "idiomsample2.idiom";
            doc.Subject = "11-20 Commom Idioms";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.idiomsDocuments.Add(doc);

            doc = new TextDocument();
            doc.Editable = editable;
            doc.FileName = _path + "idiomsample3.idiom";
            doc.Subject = "21-30 Commom Idioms";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.idiomsDocuments.Add(doc);


            // pronunciation documents
            this.soundDocuments = new List<TextDocument>();
            _path = _appDataRoot + "Sounds\\";
            doc = new TextDocument();
            doc.Editable = editable;
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
            doc.Editable = editable;
            doc.FileName = _path + "singular.grammar";
            doc.Subject = "Singular and Plurals";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.grammarDocuments.Add(doc);

            doc = new TextDocument();
            doc.Editable = editable;
            doc.FileName = _path + "countNouns.grammar";
            doc.Subject = "Count and Non-Count Nouns";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.grammarDocuments.Add(doc);
            numGrammars = 2;

            // Notes documents
            this.funDocuments = new List<TextDocument>(); 
            string fileName = _userDefaultRoot + "myVocab.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\myVocab.notes";
            }

            numNotes = 10;
            // 1 ok
            doc = new TextDocument();
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Minimum Vocaburary (2600) 英语听说最基本词汇";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            fileName = _userDefaultRoot + "sentences1.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\sentences1.notes";
            }
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Commonly Used Phrases 英语听说最常用短语 Part-I";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            fileName = _userDefaultRoot + "sentences2.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\sentences2.notes";
            }
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Commonly Used Phrases 英语听说最常用短语 Part-2";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            fileName = _userDefaultRoot + "sentences3.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\sentences3.notes";
            }
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Commonly Used Phrases 英语听说最常用短语 Part-3";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            // 3 ok
            doc = new TextDocument();
            fileName = _userDefaultRoot + "irregularVerb.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\irregularVerb.notes";
            }
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Irregular Verb 常用不规则动词";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            // ok
            doc = new TextDocument();
            fileName = _userDefaultRoot + "plurals.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\plurals.notes";
            }
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Plural Nouns 常用不规则名词复数";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            fileName = _userDefaultRoot + "synonyms.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\synonyms.notes";
            }
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Synonyms 常用近义词";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            fileName = _userDefaultRoot + "antonyms.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\antonyms.notes";
            }
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Antonyms 常用反义词";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            // ok
            doc = new TextDocument();
            fileName = _userDefaultRoot + "famous.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\famous.notes";
            }
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Famous Epigram 警句名言";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);

            doc = new TextDocument();
            fileName = _userDefaultRoot + "soundHard.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\soundHard.notes";
            }
            doc.Editable = editable;
            doc.FileName = fileName;
            doc.Subject = "Words that are hard to pronounce correctly 难发音的单词";
            doc.StudentId = studentId;
            doc.From = studentName;
            this.funDocuments.Add(doc);   

            // ok
            doc = new TextDocument();
            fileName = _userDefaultRoot + "sample.notes";
            if (!File.Exists(fileName))
            {
                fileName = _appDataRoot + "Notes\\sample.notes";
            }
            doc.FileName = fileName;
            doc.Subject = "My Notes 我的英文笔记";
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
            return getDocument(id, this.funDocuments, STORY);
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
            return getDocument(id, this.cardsDocuments, CARTOON);
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
            return getDocument(id, this.activitiesDocuments, CLASS);
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
            return getDocument(id, this.idiomsDocuments, IDIOM);
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
            return getDocument(id, this.soundDocuments, SOUND);
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

        private TextDocument getDocument(Guid id, List<TextDocument> list, string mType)
        {
            TextDocument doc = list.FirstOrDefault(e => e.Id == id);

            if (string.IsNullOrEmpty(doc.Text))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(doc.FileName))
                    {
                        if (doc.FileName.ToUpper().Contains(mType))
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
                                    int num = col.Length;
                                    if (num > 1)
                                        doc.SubjectList.Add(col[1]);
                                    if (num > 2)
                                        doc.addImage(col[2]);
                                    if (num > 3)
                                        doc.addMedia(col[3]);

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
                                        if (line.Contains(A) || line.Contains(B)
                                            || line.ToUpper().Contains(SUB))
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

    }
}

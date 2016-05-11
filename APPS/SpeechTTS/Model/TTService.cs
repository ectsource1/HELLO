// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using SpeechInfrastructure;

namespace SpeechTTS.Model
{
    [Export(typeof(ITTService))]
    public class TTService : ITTService
    {
        private readonly List<TextDocument> funDocuments;
        private readonly List<TextDocument> cardsDocuments;
        private readonly List<TextDocument> activitiesDocuments;
        private string _audioPath = "";

        public TTService()
        {
            bool newFile = false;
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _audioPath = rootPath + @"\ECTAudio\";
            if (!Directory.Exists(_audioPath))
            {
                Directory.CreateDirectory(_audioPath);
            }

            rootPath = rootPath + @"\ECTData\";
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            // fun documents
            this.funDocuments = new List<TextDocument>();
            string filePath = rootPath + "funStories.txt";
            if (!File.Exists(filePath))
            {
                newFile = true;
                File.Create(filePath);
            }

            if (!newFile)
            {
                string[] lines = File.ReadAllLines(filePath);
                int i = 0;
                string studentId="", studentName="";
                foreach (string line in lines)
                {
                    string[] col = line.Split(new char[] { ',' });
                    if (i == 0 )
                    {
                        studentId   = col[0];
                        studentName = col[1];
                        i++;
                    } else
                    {
                        TextDocument doc = new TextDocument();
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
            this.cardsDocuments = new List<TextDocument>();
            filePath = rootPath + "cardStories.txt";
            if (!File.Exists(filePath))
            {
                newFile = true;
                File.Create(filePath);
            }

            if (!newFile)
            {
                string[] lines = File.ReadAllLines(filePath);
                int i = 0;
                string studentId = "", studentName = "";
                foreach (string line in lines)
                {
                    string[] col = line.Split(new char[] { ',' });
                    if (i == 0)
                    {
                        studentId = col[0];
                        studentName = col[1];
                        i++;
                    }
                    else
                    {
                        TextDocument doc = new TextDocument();
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
            this.activitiesDocuments = new List<TextDocument>();
            filePath = rootPath + "activities.txt";
            if (!File.Exists(filePath))
            {
                newFile = true;
                File.Create(filePath);
            }

            if (!newFile)
            {
                string[] lines = File.ReadAllLines(filePath);
                int i = 0;
                string studentId = "", studentName = "";
                foreach (string line in lines)
                {
                    string[] col = line.Split(new char[] { ',' });
                    if (i == 0)
                    {
                        studentId = col[0];
                        studentName = col[1];
                        i++;
                    }
                    else
                    {
                        TextDocument doc = new TextDocument();
                        doc.FileName = col[0];
                        doc.Subject = col[1];
                        doc.StudentId = studentId;
                        doc.From = studentName;
                        this.activitiesDocuments.Add(doc);
                    }
                } // foreach
            }// new file
        }

        public string getAudioPath()
        {
             return this._audioPath;
        }

        public void SaveFunDocuments()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            filePath = filePath + @"\ECTData\funStories.txt";
            int i = 0;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                foreach (TextDocument doc in funDocuments)
                {
                    if (i == 0)
                    {
                        file.WriteLine(doc.append2String(doc.StudentId, doc.From, ","));
                        i++;
                    }
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
                       if (doc.FileName.Contains(".txt"))
                           doc.Text = sr.ReadToEnd(); 
                       else if (doc.FileName.Contains(".dic"))
                        {
                            string line;
                            while ( (line = sr.ReadLine()) != null )
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

        
        public void AddFunDocument(TextDocument doc)
        {
            this.funDocuments.Add(doc);
            SaveFunDocuments();
        }

        public void RemoveFunDocument(TextDocument doc)
        {
            funDocuments.Remove(doc);
            SaveFunDocuments();
        }

        // cartoon documents
        public void SaveCardsDocuments()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            filePath = filePath + @"\ECTData\cardStories.txt";
            int i = 0;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                foreach (TextDocument doc in cardsDocuments)
                {
                    if (i == 0)
                    {
                        file.WriteLine(doc.append2String(doc.StudentId, doc.From, ","));
                        i++;
                    }
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
                        if (doc.FileName.Contains(".txt"))
                        {
                            string line;
                            string multiLines = null;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.Contains("PAGE_"))
                                {
                                    if (multiLines != null) doc.addTxtList(multiLines);
                                    string[] col = line.Split(new char[] { ':' });
                                    doc.addImage(col[1]);
                                    multiLines = null;
                                }
                                else
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
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            filePath = filePath + @"\ECTData\activities.txt";
            int i = 0;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                foreach (TextDocument doc in activitiesDocuments)
                {
                    if (i == 0)
                    {
                        file.WriteLine(doc.append2String(doc.StudentId, doc.From, ","));
                        i++;
                    }
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
                        if (doc.FileName.Contains(".txt"))
                        {
                            bool isFirst = false;
                            int txtType = 0;
                            string line;
                            string pageLines = null;
                            string vocabLines = null;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.Contains("VOCAB_"))
                                {
                                    isFirst = true;
                                    txtType = 0;
                                }

                                if (line.Contains("DIALOG_"))
                                {
                                    isFirst = true;
                                    txtType = 1;
                                }

                                if (line.Contains("PAGE_"))
                                {
                                    isFirst = true;
                                    txtType = 2;
                                    if (pageLines != null) doc.addTxtList(pageLines);
                                    string[] col = line.Split(new char[] { ':' });
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
                                        if (line.Contains("M:") || line.Contains("F:") )
                                        {
                                            string[] col = line.Split(new char[] { ':' });
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
            SaveCardsDocuments();
        }

        public void RemoveActivitiesDocument(TextDocument doc)
        {
            activitiesDocuments.Remove(doc);
            SaveActivitiesDocuments();
        }

    }
}

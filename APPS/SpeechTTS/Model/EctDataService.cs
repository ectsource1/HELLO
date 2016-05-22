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
    [Export(typeof(IEctDataService))]
    public class EctDataService : IEctDataService
    {
        public static string PRJ_FILE     = "projects.txt";
    
        private readonly List<TextDocument> projects;
        private string _projDataRoot = "";

        public EctDataService()
        {
            this.projects = new List<TextDocument>();
            bool newFile = false;
            _projDataRoot =  @"C:\ECTProjData\";
            if (!Directory.Exists(_projDataRoot))
            {
                Directory.CreateDirectory(_projDataRoot);
            }

            string filePath = _projDataRoot + PRJ_FILE;
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

                    if ( col.Length < 3 ) return;
                    if ( File.Exists(col[0]) )
                    {
                        TextDocument doc = new TextDocument();
                        doc.FileName  = col[0];
                        doc.Subject   = col[1];
                        doc.Type = col[2];
                        this.projects.Add(doc);
                    }
                    
                } // foreach
            }// new file
        }

        public int ProjIdx(TextDocument doc)
        {
            int idx = projects.IndexOf(doc);
            return idx;
        }

        public void SaveProjDocuments()
        {
            string filePath = _projDataRoot + PRJ_FILE;
            using (StreamWriter file = new StreamWriter(filePath))
            {
                foreach (TextDocument doc in projects)
                {
                       file.WriteLine(doc.append3String(doc.FileName, doc.Subject, doc.Type, ","));  
                }
            }
        }


        public IAsyncResult BeginGetProjDocuments(AsyncCallback callback, object userState)
        {
            var asyncResult = new AsyncResult<IEnumerable<TextDocument>>(callback, userState);
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    asyncResult.SetComplete(new ReadOnlyCollection<TextDocument>(this.projects), false);
                });

            return asyncResult;
        }

        public IEnumerable<TextDocument> EndGetProjDocuments(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<IEnumerable<TextDocument>>.End(asyncResult);

            return localAsyncResult.Result;
        }

        public IAsyncResult BeginSendProjDocument(TextDocument text, AsyncCallback callback, object userState)
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

        public void EndSendProjDocument(IAsyncResult asyncResult)
        {
            var localAsyncResult = AsyncResult<object>.End(asyncResult);
        }

        public EctModule GetProjDocument(Guid id)
        {
            EctModule module = null;
            /*
            TextDocument doc = this.projects.FirstOrDefault(e => e.Id == id);

            if (string.IsNullOrEmpty(doc.Text) ) {
               try
               {
                   using (StreamReader sr = new StreamReader(doc.FileName))
                   {
                       
                        if (doc.FileName.ToUpper().Contains(".xml"))
                        {
                            string line;
                            while ( (line = sr.ReadLine()) != null )
                            {
                                //if (!line.Contains(TITLE_KEY))
                                //    doc.Text += line + "\n";
                            }
                        }
                   }
               }
               catch (Exception e)
               {
                   Console.WriteLine("The file could not be read:");
                   Console.WriteLine(e.Message);
                }
            }*/

            return module;
        }

        
        public void AddProjDocument(TextDocument doc)
        {
            this.projects.Add(doc);
            SaveProjDocuments();
        } 

        public void RemoveProjDocument(TextDocument doc)
        {
            projects.Remove(doc);
            SaveProjDocuments();
        }  

    }
}

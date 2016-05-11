// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace SpeechInfrastructure
{
    public class TextDocument : ICloneable
    {
        public static string TXT = "TXT";
        public static string RTF = "RTF";
        public static string RTE = "RTE";
        public static string RTE_KEY = "H_FRMDESSUB";

        private List<string> imgList;
        private List<string> txtList;

        private List<string> genderList;
        private List<string> sentenceList;

        private string dialogs = "";

        public TextDocument(bool create = true)
            : this(Guid.NewGuid())
        {
            if (create)
            {
                this.Id = Guid.NewGuid();
                this.Type = "TXT";
                this.Text = "";
            }

            imgList = new List<string>();
            txtList = new List<string>();

            genderList = new List<string>();
            sentenceList = new List<string>();
        }

        public TextDocument(Guid id)
        {
            this.Id = id;   
        }

        public string FileName { get; set; }

        public string    Type { get; set; }

        public string From { get; set;}

        public string StudentId { get; set; }

        public string ClassLevel { get; set;}

        public string Created { get; set; }

        public string Updated { get; set; }

        public bool NotPassed { get; set; }

        public string Subject { get; set;}

        public string Text { get; set;}

        public string Vocalburay { get; set; }

        public String Dialogs
        {
            get
            {
                return this.dialogs;
            }

        }

        public void MergeSentences(string s1, string s2)
        {
            dialogs += s1 + ": " + s2 + "\n";
        }

        public string Description { get; set; }

        public Guid Id { get; set;}

        public int Idx { get; set; }

        public List<string> ImgList
        {
            get { return this.imgList; }
        }

        public void addImage(string str)
        {
            imgList.Add(str);
        }

        public List<string> TxtList
        {
            get { return this.txtList; }
        }

        public void addTxtList(string str)
        {
            txtList.Add(str);
        }

        public List<string> GenderList
        {
            get { return this.genderList; }
        }

        public void addGender(string str)
        {
            genderList.Add(str);
        }

        public List<string> SentenceList
        {
            get { return this.sentenceList; }
        }

        public void addSentence(string str)
        {
            sentenceList.Add(str);
        }

        public object Clone()
        {
            TextDocument doc = (TextDocument)this.MemberwiseClone();
            return doc;
        }

        public string append2String(string s1, string s2, string delimiter)
        {
            return (s1 + delimiter + s2);
        }

    }
}

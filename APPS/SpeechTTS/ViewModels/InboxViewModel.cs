// Copyright (c) Microsoft Corporation. All rights reserved.
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Input;
using System.Text.RegularExpressions;
using SpeechTTS.Model;
using SpeechTTS.Auth;
using SpeechInfrastructure;
using System.Speech.Synthesis;
using System.Windows.Threading;

namespace SpeechTTS.ViewModels
{
    [Export]
    public class InboxViewModel : BindableBase
    {
        private const string TTSViewKey = "TTSView";
        private const string TextIdKey = "TextId";

        SpeechSynthesizer voice;

        private readonly SynchronizationContext synchronizationContext;
        private readonly ITTService ttsService;
        private readonly IRegionManager regionManager;
        private readonly DelegateCommand<TextDocument> loadTextCommand;
        private readonly DelegateCommand<TextDocument> deleteTextCommand;
        private readonly DelegateCommand addNewCommand;
        private readonly ObservableCollection<TextDocument> textCollection;

        private string readme = "";
        private string intro0 = "";
        private string intro1 = "";
        private string intro2 = "";
        private bool showIsChecked = false;
        private bool notShowIsChecked = true;
        private bool speaking = false;

        private int step = 0;

        DispatcherTimer timer;

        [ImportingConstructor]
        public InboxViewModel(ITTService ttsService, IRegionManager regionManager)
        {
            voice = new SpeechSynthesizer();
            voice.SpeakCompleted += OnSpeakCompleted;

            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
            this.loadTextCommand = new DelegateCommand<TextDocument>(this.loadFile);
            this.deleteTextCommand = new DelegateCommand<TextDocument>(this.deleteFile);
            this.addNewCommand = new DelegateCommand(this.addNewStory);
            this.textCollection = new ObservableCollection<TextDocument>();
            this.Messages = new ListCollectionView(this.textCollection);

            this.ttsService = ttsService;
            this.regionManager = regionManager;

            this.ttsService.BeginGetFunDocuments(
                r =>
                {
                    var messages = this.ttsService.EndGetFunDocuments(r);

                    this.synchronizationContext.Post(
                        s =>
                        {
                            foreach (var message in messages)
                            {
                                this.textCollection.Add(message);
                            }
                        },
                        null);
                },
                null);

            addReadme();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5000);
            timer.Tick += new EventHandler(Timer_tick);
            timer.Start();
            //voice.SpeakAsync(intro);

        }

        void Timer_tick(Object sender, EventArgs e)
        {
            if (step == 0 && !speaking)
            {
                speaking = true;
                voice.SpeakAsync(intro0);
                timer.Interval = TimeSpan.FromMilliseconds(50000);
                step += 1;
            }

            if (step == 1 && !speaking)
            {
                CustomPrincipal customPrincipal 
               = Thread.CurrentPrincipal as CustomPrincipal;

                if (customPrincipal == null) return;
         
                string id = customPrincipal.Identity.Name;
                if (string.IsNullOrEmpty(id))
                {
                        speaking = true;
                        voice.SpeakAsync(intro1);
                } else {
                        step += 1;
                }    
            }

            if (step == 2 && !speaking)
            {
                string fileName = AppDomain.CurrentDomain.BaseDirectory;
                fileName = fileName + "DataFiles\\" + Personal.PERSON_BIN;
                Personal person = null;
                bool filled = false;
                if (File.Exists(fileName))
                {
                    person = Personal.read(fileName);
                    filled = person.isFilled();
                }
                if (!filled)
                {
                    speaking = true;
                    voice.SpeakAsync(intro2);
                    timer.Stop();
                }
                   
            }

        }

        void OnSpeakCompleted(object sender, EventArgs e)
        {
            speaking = false;
        }

        public ICollectionView Messages { get; private set; }

        public bool ShowIsChecked
        {
            get
            {
                return this.showIsChecked;
            }

            set
            {
                this.SetProperty(ref this.showIsChecked, value);
            }
        }

        public bool NotShowIsChecked
        {
            get
            {
                return this.notShowIsChecked;
            }

            set
            {
                this.SetProperty(ref this.notShowIsChecked, value);
            }
        }

        public ICommand LoadTextCommand
        {
            get { return this.loadTextCommand; }
        }

        public ICommand DeleteTextCommand
        {
            get { return this.deleteTextCommand; }
        }

        public ICommand AddNewCommand
        {
            get { return this.addNewCommand; }
        }

        public String Readme
        {
            get
            {
                return this.readme;
            }
        }

        private void loadFile(TextDocument document)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add(TextIdKey, document.Id.ToString("N"));

            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(TTSViewKey + parameters, UriKind.Relative));
        }

        private void deleteFile(TextDocument document)
        {
            int idx = ttsService.FunIdx(document);
            if (idx < ttsService.getNumNotes()) return;

            textCollection.Remove(document);
            this.ttsService.RemoveFunDocument(document);
        }

        private void addNewStory()
        {
            TextDocument doc = new TextDocument();
            TextDocument doc1 = null;
            if (textCollection.Count > 0)
                doc1 = textCollection[0];
            else
                doc1 = new TextDocument();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Notes|*" + TTService.STORY;
            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select Notes File";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fname = dialog.FileName;
                doc.FileName = fname;
                doc.StudentId = doc1.StudentId;
                doc.From = doc1.From;

                string[] lines = File.ReadAllLines(fname);
                foreach (string line in lines)
                {
                    if (line.Contains(TTService.TITLE_KEY))
                    {
                        string[] col = Regex.Split(line, TTService.SEP_CHAR);
                        doc.Subject = col[1];
                        break;
                    }

                }

                this.ttsService.AddFunDocument(doc);
                textCollection.Add(doc);
            }
        }

        void addReadme()
        {
            readme = "学英语和学其它新学科一样，刚开始都会觉得很难，因为接触的所有内容都是新的，很陌生.\n\n";
            readme += "英语不是靠短时间突击出来的，需要一定时间听说读写的历练，包括日常口语句型和单词的积累，";
            readme += "英语发音和语感的习惯以及基本语法的掌握。\n快速学好英语的办法就是每天消化所学的新词新句，";
            readme += "把暂时消化不了的记录下来，反复练习。\n\nMy Notes 学习模块就是提供方便，让学生可以记录新学的句型和单词,";
            readme += "难发音的词句，不规则动词等等你认为需要加强练习的地方,\n由软件帮助学生正确发音和反复练习。";
            readme += "所有的新词新句都必须先听标准发音并通过耳朵过滤几遍，把新词的发音正确地固定起来。\n\n";
            readme += "千万不要跟着不标准的发音练习。发音口音一旦定型了，很难纠正";

            intro0 = "Hello My Friend, Welcome to ECT English School!\n";
            intro0 += "ECT wants you to be sucessfull in speaking fluent English!\n";
            intro0 += "Lets work together to achieve what you want to be !!\n";
            intro0 += "You need to login before you use this software.\n";
            intro0 += "Please send Charlie or Andy a message if you don't have your login account yet!";

            intro1 = "Looked like you still not logged in yet.\n";
            intro1 += "Did you forgot your username and password?\n";
            intro1 += "Ask your daddy or Mommy whether they know your account or not?\n";
            intro1 += "Then try one more time!\n";

            intro2 = "Congradulations, you have successfully logged into the system.\n";
            intro2 += "Would you please setup your personal information?\n";
            intro2 += "You need to introduce yourself in software, so that I know who you are!\n";
        }
    }     
}


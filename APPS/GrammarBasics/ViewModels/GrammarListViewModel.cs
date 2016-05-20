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
using SpeechInfrastructure;

namespace GrammarBasics.ViewModels
{
    [Export]
    public class GrammarListViewModel : BindableBase
    {
        private const string TTSViewKey = "GrammarView";
        private const string TextIdKey = "TextId";

        private readonly SynchronizationContext synchronizationContext;
        private readonly ITTService ttsService;
        private readonly IRegionManager regionManager;
        private readonly DelegateCommand<TextDocument> loadTextCommand;
        private readonly DelegateCommand<TextDocument> deleteTextCommand;
        private readonly DelegateCommand addNewCommand;
        private readonly ObservableCollection<TextDocument> textCollection;

        private string readme = "";
        private bool showIsChecked = false;
        private bool notShowIsChecked = true;

        [ImportingConstructor]
        public GrammarListViewModel(ITTService ttsService, IRegionManager regionManager)
        {
            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
            this.loadTextCommand = new DelegateCommand<TextDocument>(this.loadFile);
            this.deleteTextCommand = new DelegateCommand<TextDocument>(this.deleteFile);
            this.addNewCommand = new DelegateCommand(this.addNewStory);
            this.textCollection = new ObservableCollection<TextDocument>();
            this.Messages = new ListCollectionView(this.textCollection);

            this.ttsService = ttsService;
            this.regionManager = regionManager;
            
            this.ttsService.BeginGetGrammarDocuments(
                r =>
                {
                    var messages = this.ttsService.EndGetGrammarDocuments(r);

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

            readme = "语法很重要，但刚开始学英文的孩子不能过早的学习语法。模仿的英语才是纯正的口语。\n\n";
            readme += "当能基本口语交流，并掌握一定常用词汇后，语法就显得很必要了。因为英文阅读和写作\n";
            readme += "都离不开基本的语法知识，另外语法也能进一步提高口语交流的能力。";
        }

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

        public String Readme
        {
            get
            {
                return this.readme;
            }
        }

        public ICollectionView Messages { get; private set; }

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


        private void loadFile(TextDocument document)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add(TextIdKey, document.Id.ToString("N"));

            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(TTSViewKey + parameters, UriKind.Relative));
        }

        private void deleteFile(TextDocument document)
        {
            int idx = ttsService.GrammarIdx(document);
            if (idx < ttsService.getNumGrammars()) return;

            textCollection.Remove(document);
            this.ttsService.RemoveGrammarDocument(document);
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
            dialog.Filter ="Grammar|*" + TTService.GRAMMAR;
            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select a grammar file";
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

                this.ttsService.AddGrammarDocument(doc);
                textCollection.Add(doc);
            }
        }
    }
}


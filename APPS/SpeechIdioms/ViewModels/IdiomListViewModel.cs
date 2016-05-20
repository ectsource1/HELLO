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

namespace SpeechIdioms.ViewModels
{
    [Export]
    public class IdiomListViewModel : BindableBase
    {
        private const string ViewKey = "IdiomsView";
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
        public IdiomListViewModel(ITTService ttsService, IRegionManager regionManager)
        {
            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
            this.loadTextCommand = new DelegateCommand<TextDocument>(this.loadFile);
            this.deleteTextCommand = new DelegateCommand<TextDocument>(this.deleteFile);
            this.addNewCommand = new DelegateCommand(this.addNewStory);
            this.textCollection = new ObservableCollection<TextDocument>();
            this.Messages = new ListCollectionView(this.textCollection);

            this.ttsService = ttsService;
            this.regionManager = regionManager;
            this.ttsService.BeginGetIdiomsDocuments(
                r =>
                {
                    var messages = this.ttsService.EndGetIdiomsDocuments(r);

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

            readme = "俚语也就是英语成语，在美国日常生活中，在电影电视里，英文歌词中，以及每天开车\n";
            readme += "收音机里的节目中经常出现。掌握一定数量的俚语对于提高听力讲一口地道口语很有帮助。\n\n";
            readme += "俚语模块收集300句最常用的俚语句法，供学生练习应用。每个俚语除了定义外，\n";
            readme += "还配有图像，发音，例句和情景对话。课堂上也会有场景练习，以便加深理解。\n";
            readme += "学生能在很轻松的环境中不知不觉地掌握主要常用俚语。";
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

            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(ViewKey + parameters, UriKind.Relative));
        }

        private void deleteFile(TextDocument document)
        {
            int idx = ttsService.IdiomIdx(document);
            if (idx < ttsService.getNumIdioms()) return;

            textCollection.Remove(document);
            this.ttsService.RemoveIdiomsDocument(document);
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
            dialog.Filter ="Idiom|*" + TTService.IDIOM;
            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select a Idiom file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fname = dialog.FileName;
                doc.FileName = fname;
                doc.Subject = Path.GetFileNameWithoutExtension(fname);
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
                this.ttsService.AddIdiomsDocument(doc);
                textCollection.Add(doc);
            }
        }
    }
}

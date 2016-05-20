// Copyright (c) Microsoft Corporation. All rights reserved.
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Data;
using System.Text.RegularExpressions;
using SpeechTTS.Model;
using SpeechInfrastructure;

namespace Pronunciation.ViewModels
{
    [Export]
    public class SoundListViewModel : BindableBase
    {
        private const string ViewKey = "SoundView";
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
        public SoundListViewModel(ITTService ttsService, IRegionManager regionManager)
        {
            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
            this.loadTextCommand = new DelegateCommand<TextDocument>(this.loadFile);
            this.deleteTextCommand = new DelegateCommand<TextDocument>(this.deleteFile);
            this.addNewCommand = new DelegateCommand(this.addNewStory);
            this.textCollection = new ObservableCollection<TextDocument>();
            this.Messages = new ListCollectionView(this.textCollection);

            this.ttsService = ttsService;
            this.regionManager = regionManager;
            this.ttsService.BeginGetSoundDocuments(
                r =>
                {
                    var messages = this.ttsService.EndGetSoundDocuments(r);

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

            readme = "所谓英语口语过关必须是英语流利并且发音正确。要想口音标准，必须从娃娃抓起。\n\n";
            readme += "英语发音正确不是靠学音标拼读，而是要依靠模仿，照着标准发音跟读。ECT 软件可以\n";
            readme += "对每个单词和句子正确发声，并且可以调节语速，是很好的学习工具.\n\n";
            readme += "但是学生还是要掌握英语的基本发音规则，这样容易做到举一反三，触类旁通";
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
            int idx = ttsService.SoundIdx(document);
            if (idx < ttsService.getNumSounds()) return;

            textCollection.Remove(document);
            this.ttsService.RemoveSoundDocument(document);
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
            dialog.Filter ="Sound|*" + TTService.SOUND;
            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select a pronunciation file";
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

                this.ttsService.AddSoundDocument(doc);
                textCollection.Add(doc);
            }
        }
    }
}

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

namespace SpeechWords.ViewModels
{
    [Export]
    public class StoryListViewModel : BindableBase
    {
        private const string ViewKey = "WordsView";
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
        public StoryListViewModel(ITTService ttsService, IRegionManager regionManager)
        {
            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
            this.loadTextCommand = new DelegateCommand<TextDocument>(this.loadFile);
            this.deleteTextCommand = new DelegateCommand<TextDocument>(this.deleteFile);
            this.addNewCommand = new DelegateCommand(this.addNewStory);
            this.textCollection = new ObservableCollection<TextDocument>();
            this.Messages = new ListCollectionView(this.textCollection);

            this.ttsService = ttsService;
            this.regionManager = regionManager;
            this.ttsService.BeginGetCardsDocuments(
                r =>
                {
                    var messages = this.ttsService.EndGetCardsDocuments(r);

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

            readme = "对于小年龄的孩子来说，看图识字，看图听故事和看图重复讲故事都是有效的学习方法，因为有视觉，听觉和口音的反复印记。\n\n";
            readme += "卡通书模块除了提供课堂用的动画书外，学生也可以自己制作卡通书，配有自己写的简单英文，自己的画和自己的英文录音.\n";
            readme += "学生可以选择用标准美国发音，或用自己的录音读自己写的作文。让学生感觉学英文可以自己编写卡通书，有一种成就感。";
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

        private void loadFile(TextDocument document)
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add(TextIdKey, document.Id.ToString("N"));

            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(ViewKey + parameters, UriKind.Relative));
        }

        private void deleteFile(TextDocument document)
        {
            int idx = ttsService.CardIdx(document);
            if (idx < ttsService.getNumCarts()) return;

            textCollection.Remove(document);
            this.ttsService.RemoveCardsDocument(document);
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
            dialog.Filter = "TXT|*" + TTService.CARTOON;
            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select a cartoon file";
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

                this.ttsService.AddCardsDocument(doc);
                textCollection.Add(doc);
            }
        }
    }
}

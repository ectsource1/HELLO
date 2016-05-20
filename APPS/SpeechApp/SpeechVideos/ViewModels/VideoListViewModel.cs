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

namespace SpeechVideos.ViewModels
{
    [Export]
    public class VideoListViewModel : BindableBase
    {
        private const string ViewKey = "VideosView";
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
        public VideoListViewModel(ITTService ttsService, IRegionManager regionManager)
        {
            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
            this.loadTextCommand = new DelegateCommand<TextDocument>(this.loadFile);
            this.deleteTextCommand = new DelegateCommand<TextDocument>(this.deleteFile);
            this.addNewCommand = new DelegateCommand(this.addNewStory);
            this.textCollection = new ObservableCollection<TextDocument>();
            this.Messages = new ListCollectionView(this.textCollection);

            this.ttsService = ttsService;
            this.regionManager = regionManager;
            this.ttsService.BeginGetActivitiesDocuments(
                r =>
                {
                    var messages = this.ttsService.EndGetActivitiesDocuments(r);

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

            readme  = "课堂活动模块记录每节课或每个星期的教学内容，包括视频，句型，对话和生词。\n";
            readme += "学生不仅能课堂跟老师学习，家里也可以反复练习，做到每星期的教学内容能100%消化。\n\n";
            readme += "学生光凭课堂一点时间学习英语是远远不够的，课外必须有学习英语的环境，ECT的软件系统给学生\n";
            readme += "提供良好的课外语言环境，只要有心每天坚持学习，不需要出国，也一定能全面学好英文。";
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
            int idx = ttsService.ClassIdx(document);
            if (idx < ttsService.getNumClasses()) return;

            textCollection.Remove(document);
            this.ttsService.RemoveActivitiesDocument(document);
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
            dialog.Filter = "TXT|*" + TTService.CLASS;
            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select a class file";
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
                this.ttsService.AddActivitiesDocument(doc);
                textCollection.Add(doc);
            }
        }
    }
}

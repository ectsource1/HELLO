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
using SpeechTTS.Model;
using SpeechInfrastructure;

namespace SpeechTTS.ViewModels
{
    [Export]
    public class InboxViewModel : BindableBase
    {
        private const string TTSViewKey = "TTSView";
        private const string TextIdKey = "TextId";

        private readonly SynchronizationContext synchronizationContext;
        private readonly ITTService ttsService;
        private readonly IRegionManager regionManager;
        private readonly DelegateCommand<TextDocument> loadTextCommand;
        private readonly DelegateCommand<TextDocument> deleteTextCommand;
        private readonly DelegateCommand addNewCommand;
        private readonly ObservableCollection<TextDocument> textCollection;

        [ImportingConstructor]
        public InboxViewModel(ITTService ttsService, IRegionManager regionManager)
        {
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
            dialog.Filter =
               "TXT|*.txt|DIC|*.dic";
            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select a text file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fname = dialog.FileName;
                doc.FileName = fname;
                doc.Subject = Path.GetFileNameWithoutExtension(fname);
                doc.StudentId = doc1.StudentId;
                doc.From = doc1.From;
                this.ttsService.AddFunDocument(doc);
                textCollection.Add(doc);
            }
            //textCollection.Remove(document);
        }
    }
}


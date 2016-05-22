// Copyright (c) Microsoft Corporation. 
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using SpeechInfrastructure;
using SpeechTTS.Model;

namespace ClassProject.ViewModels
{
    [Export]
    public class ProjectViewModel : BindableBase, INavigationAware
    {
        private readonly IEctDataService dataService;
        private readonly IRegionManager regionManager;
        private readonly SynchronizationContext synchronizationContext;

        private List<string> typeOptions;
        private bool showCreated    = true;
        private bool noShowCreated  = false;
        private string message = "";

        private int currPageIdx;
        private string pageStrIdx = "";
        private EctPage currPage;   
        private EctModule ectModule;
        private readonly ObservableCollection<TextDocument> projects;

        [ImportingConstructor]
        public ProjectViewModel(IEctDataService service, IRegionManager regionManager)
        {
            currPageIdx = 0;
            pageStrIdx = "Page 0";
            currPage = new EctPage();
            currPage.Txt = "Not Assigned";
            currPage.ImgFile = "Not Assigned";
            currPage.PlayFile = "Not Assigned";
            currPage.SubTitle = "Not Assigned";

            ectModule = new EctModule();
            ectModule.Pages.Add(currPage);
            ectModule.Type = "Class With Video";
            ectModule.Title = "Title 1";
            KeyValuePair<string, string> kp 
                = new KeyValuePair<string, string>("A", "Test Me");
            ectModule.Dialogs.Add(kp);
            ectModule.Vocab = "This is New words";

            this.CreatedPages = new ListCollectionView(ectModule.Pages);

            this.projects = new ObservableCollection<TextDocument>();
            this.CreatedProjects = new ListCollectionView(this.projects);
            TextDocument doc = new TextDocument();
            doc.FileName = "TEST.txt";
            doc.Type = "Notes";
            doc.Subject = "Subject Test";
            projects.Add(doc);

            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();

            this.dataService = service;
            this.regionManager = regionManager;

            this.dataService.BeginGetProjDocuments(
                r =>
                {
                    var projs = this.dataService.EndGetProjDocuments(r);

                    this.synchronizationContext.Post(
                        s =>
                        {
                            foreach (var proj in projs)
                            {
                                this.projects.Add(proj);
                            }
                        },
                        null);
                },
                null);

            typeOptions = new List<string>
            {
                "Notes",
                "Cartoons",
                "Class With Video",
                "Idioms",
                "Pronunciations",
                "Grammars"
            };


        }

        public EctModule CurrModule
        {
            get
            {
                return this.ectModule;
            }

            set
            {
                this.SetProperty(ref this.ectModule, value);
            }
        }

        public ICollectionView CreatedPages { get; private set; }
        public ICollectionView CreatedProjects { get; private set; }

        public List<string> TypeOptions
        {
            get
            {
                return this.typeOptions;
            }
        }

        public int CurrPageIdx
        {
            get
            {
                return this.currPageIdx;
            }

            set
            {
                this.SetProperty(ref this.currPageIdx, value);
                PageStrIdx = "Page " + currPageIdx;
            }
        }

        public string PageStrIdx
        {
            get
            {
                return this.pageStrIdx;
            }

            set
            {
                this.SetProperty(ref this.pageStrIdx, value);
            }
        }

        public EctPage CurrPage
        {
            get
            {
                return this.currPage;
            }

            set
            {
                this.SetProperty(ref this.currPage, value);
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                this.SetProperty(ref this.message, value);
            }
        }


        public bool ShowCreated
        {
            get
            {
                return this.showCreated;
            }

            set
            {
                this.SetProperty(ref this.showCreated, value);
            }
        }

        public bool NoShowCreated
        {
            get
            {
                return this.noShowCreated;
            }

            set
            {
                this.SetProperty(ref this.noShowCreated, value);
            }
        }

        #region INavigationAware

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Intentionally not implemented.
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            //this.navigationJournal = navigationContext.NavigationService.Journal;
        }

        #endregion
    }
}

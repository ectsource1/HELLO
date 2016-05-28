// Copyright (c) Microsoft Corporation. 
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using SpeechInfrastructure;
using SpeechTTS.Model;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.IO;

namespace ClassProject.ViewModels
{
    [Export]
    public class ProjectViewModel : BindableBase, INavigationAware
    {
        private readonly DelegateCommand addPageCommand;
        private readonly DelegateCommand<EctPage> removePageCommand;
        private readonly DelegateCommand updatePageCommand;
        private readonly DelegateCommand addModuleCommand;
        private readonly DelegateCommand<EctModule> removeProjCommand;
        private readonly DelegateCommand updateModuleCommand;

        private readonly IEctDataService dataService;
        private readonly IRegionManager regionManager;
        private readonly SynchronizationContext synchronizationContext;

        private List<string> typeOptions;
        private string selectedType;
        private bool showCreated    = true;
        private bool noShowCreated  = false;
        private bool showDialogTxt;
        private string message = "";

        private int currPageIdx;
        private string pageStrIdx = "";
        private EctPage currPage;   
        private EctModule ectModule;
        
        private ComboBox _tType;
        private TextBox _tTitle, _tDialogs, _tVocab;
        private TextBox _tTxt, _tImgFile, _tPlayFile, _tSubtitle;

        private readonly ObservableCollection<EctModule> projects;

        [ImportingConstructor]
        public ProjectViewModel(IEctDataService service, IRegionManager regionManager)
        {
            this.synchronizationContext =
            SynchronizationContext.Current ?? new SynchronizationContext();
            this.dataService = service;
            this.regionManager = regionManager;

            this.addPageCommand = new DelegateCommand(this.addPage);
            this.updatePageCommand = new DelegateCommand(this.updatePage);
            this.removePageCommand = new DelegateCommand<EctPage>(this.removePage);
            this.addModuleCommand  = new DelegateCommand(this.addModule);
            this.updateModuleCommand = new DelegateCommand(this.updateModule);
            this.removeProjCommand = new DelegateCommand<EctModule>(this.removeProj);

            this.projects = new ObservableCollection<EctModule>();

            pageStrIdx = "Page 0";
            ectModule = new EctModule();
            this.CreatedPages = new ListCollectionView(ectModule.Pages);
            this.CreatedProjects = new ListCollectionView(this.projects);
            this.CreatedProjects.CurrentChanged += CreatedProjects_CurrentChanged;

            /*
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
                null); */

            typeOptions = new List<string>
            {
                "Notes",
                "Cartoons",
                "Class With Video",
                "Idioms"
            };

            SelectedType = typeOptions[0];


        }

        
        public ICommand AddPageCommand
        {
            get { return this.addPageCommand; }
        }

        public ICommand UpdatePageCommand
        {
            get { return this.updatePageCommand; }
        }

        public ICommand RemovePageCommand
        {
            get { return this.removePageCommand; }
        } 

        public ICommand AddModuleCommand
        {
            get { return this.addModuleCommand; }
        }

        public ICommand RemoveProjCommand
        {
            get { return this.removeProjCommand; }
        }

        public ICommand UpdateModuleCommand
        {
            get { return this.updateModuleCommand; }
        }

        private void addPage()
        {
            currPage = new EctPage();
            currPage.Txt = _tTxt.Text;
            currPage.ImgFile = _tImgFile.Text;
            currPage.PlayFile = _tPlayFile.Text;
            currPage.SubTitle = _tSubtitle.Text;

            EctModule m = (EctModule)CreatedProjects.CurrentItem;
            if ( m != null )
                m.Pages.Add(currPage);

            ectModule.Pages.Add(currPage);
            int cnt = ectModule.Pages.Count;

            CreatedPages.MoveCurrentToPosition(cnt - 1);
        }

        private void updatePage()
        {
            EctPage cItem = (EctPage)CreatedPages.CurrentItem;
            int pos = CreatedPages.CurrentPosition;
            EctPage citem = ectModule.Pages[pos];
            cItem.ImgFile = _tImgFile.Text;
            cItem.PlayFile = _tPlayFile.Text;
            cItem.SubTitle = _tSubtitle.Text;
            CreatedPages.MoveCurrentToPosition(-1);
        }

        private void addModule()
        {
            ectModule.Title = _tTitle.Text;
            ectModule.Type = selectedType;
            ectModule.Vocab = _tVocab.Text;
            ectModule.Dialogs = _tDialogs.Text;

            projects.Add(ectModule.clone());
            int cnt = projects.Count;
            CreatedProjects.MoveCurrentToPosition(cnt - 1);

            XmlDocument myXml = new XmlDocument();
            XPathNavigator xNav = myXml.CreateNavigator();

            XmlSerializer x = new XmlSerializer(ectModule.GetType());
            using (var xs = xNav.AppendChild())
            {
                x.Serialize(xs, ectModule);
            }
            string sXml = myXml.OuterXml;

            // deserialize
            EctModule em = (EctModule)x.Deserialize(new StringReader(sXml));


        }

        private void updateModule()
        {
            EctModule m = (EctModule)CreatedProjects.CurrentItem;
            m.Title = _tTitle.Text;
            m.Type = selectedType;
            m.Dialogs = _tDialogs.Text;
            m.Vocab = _tVocab.Text;
            CreatedProjects.MoveCurrentToPosition(-1);
        }

        private void CreatedProjects_CurrentChanged(object sender, System.EventArgs e)
        {

            if (CreatedProjects.CurrentItem != null)
            {
                EctModule md = (EctModule)CreatedProjects.CurrentItem;
                EctModule.copy(md, ectModule);
                SelectedType = ectModule.Type;
            }

        }

        private void removePage(EctPage page)
        {
            ectModule.Pages.Remove(page);
        }

        private void removeProj(EctModule proj)
        {
            projects.Remove(proj);
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

        public string  SelectedType
        {
            get
            {
                return this.selectedType;
            }

            set
            {
                this.SetProperty(ref this.selectedType, value);
                if (selectedType.Equals(typeOptions[0]))
                {
                    ShowDialogTxt = false;
                } else
                {
                    ShowDialogTxt = true;
                }
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

        public bool ShowDialogTxt
        {
            get
            {
                return this.showDialogTxt;
            }

            set
            {
                this.SetProperty(ref this.showDialogTxt, value);
            }
        }

        public void setPageElements(TextBox tTxt, TextBox tImgFile,
            TextBox tPlayFile, TextBox tSubtitle)
        {
            _tTxt = tTxt;
            _tImgFile = tImgFile;
            _tPlayFile = tPlayFile;
            _tSubtitle = tSubtitle;
        }

        public void setModuleElemets(ComboBox tType, TextBox tTitle,
            TextBox tDialogs, TextBox tVocab)
        {
            _tType = tType;
            _tTitle = tTitle;
            _tDialogs = tDialogs;
            _tVocab = tVocab;
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

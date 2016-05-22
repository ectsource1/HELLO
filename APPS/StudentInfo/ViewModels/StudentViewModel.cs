// Copyright (c) Microsoft Corporation. All rights reserved
// http://englishharmony.com/phrases-to-use-at-home/
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System.Threading;
using SpeechTTS.Auth;
using System.Windows.Input;
using SpeechInfrastructure;

namespace StudentInfo.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StudentViewModel : BindableBase, INavigationAware
    {
       

        private readonly DelegateCommand saveCommand;

        private List<string> sexOptions;
        private List<string> hobbyOptions;
        private string selectedSex = "Male";
        private string selectedHobby = "Drawing";
        private string message = "";
        private Personal personData;

        [ImportingConstructor]
        public StudentViewModel()
        {
            this.saveCommand = new DelegateCommand(this.Save);

            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            CustomIdentity id = null;
            if (customPrincipal != null)
            {
                id = (CustomIdentity)customPrincipal.Identity;
            }

            string fileName = AppDomain.CurrentDomain.BaseDirectory;
            fileName = fileName + "DataFiles\\" + Personal.PERSON_BIN;
            Personal person = null;

            if (!File.Exists(fileName))
            {
                person = new Personal();
            } else
            {
                person = Personal.read(fileName);
            } 

            if ( id != null)
            {
                person.StudentId = id.Name;
                person.StudentName = id.Fullname;
            }

            this.PersonData = person;

            sexOptions = new List<string>
            {
                "Male",
                "Female"
            };

            hobbyOptions = new List<string>
            {
                "Drawing",
                "Dancing",
                "Chess",
                "Sports",
                "Piano"
            };
        }

        public ICommand SaveCommand
        {
            get { return this.saveCommand; }
        }

        private void Save()
        {
            Message = "";
            string fileName = AppDomain.CurrentDomain.BaseDirectory;
            fileName = fileName + "DataFiles\\" + Personal.PERSON_BIN;
            Personal.write(personData, fileName);
            Message = "Saved Successfully !";
        }

        public Personal PersonData
        {
            get
            {
                return this.personData;
            }

            set
            {
                this.SetProperty(ref this.personData, value);
            }
        }

        public List<string> SexOptions
        {
            get
            {
                return this.sexOptions;
            }
        }

        public String SelectedSex
        {
            get
            {
                return this.selectedSex;
            }

            set
            {
                this.SetProperty(ref this.selectedSex, value);
            }
        }

        public List<string> HobbyOptions
        {
            get
            {
                return this.hobbyOptions;
            }
        }

        public String SelectedHobby
        {
            get
            {
                return this.selectedHobby;
            }

            set
            {
                this.SetProperty(ref this.selectedHobby, value);
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

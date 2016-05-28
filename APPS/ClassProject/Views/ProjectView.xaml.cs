// Copyright (c) Microsoft Corporation.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using ClassProject.ViewModels;

namespace ClassProject.Views
{
    [Export("ProjectView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
        }

        [Import]
        public ProjectViewModel ViewModel
        {
            set {
                this.DataContext = value;
                ProjectViewModel model = (ProjectViewModel)this.DataContext;
                model.setPageElements(tTxt, tImgFile, tPlayFile, tSubtitle);
                model.setModuleElemets(tType, tTitle, tDialogs, tVocab);
            }
        }

    }
}

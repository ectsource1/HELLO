// Copyright (c) Microsoft Corporation. All rights reserved
using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechIdioms.ViewModels;
using System.Security.Permissions;

namespace SpeechIdioms.Views
{
    [Export("IdiomsView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class IdiomsView : UserControl
    {
        public IdiomsView()
        {
            InitializeComponent();
        }

        [Import]
        public IdiomsViewModel ViewModel
        {
            set {
                this.DataContext = value;
                IdiomsViewModel model = (IdiomsViewModel)this.DataContext;
                model.setImage(ImageViewer1);
                model.setAudio(AudioViewer);
                model.setEditbox(EditBox);
            }
        }

        private void MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            IdiomsViewModel model = (IdiomsViewModel)this.DataContext;
            model.MediaEnded();
        }

        private void DialogDbClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IdiomsViewModel model = (IdiomsViewModel)this.DataContext;
            model.SelectedText2 = MyDialog.SelectedText;
        }

        private void SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            IdiomsViewModel model = (IdiomsViewModel)this.DataContext;
            model.SelectedText2 = MyDialog.SelectedText;
        }

        private void IsVisibleChanged1(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            IdiomsViewModel model = (IdiomsViewModel)this.DataContext;
            model.changeVisible();
        }
    }
}

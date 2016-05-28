// Copyright (c) Microsoft Corporation. All rights reserved
using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechWords.ViewModels;
using System.Security.Permissions;

namespace SpeechWords.Views
{
    [Export("WordsView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class WordsView : UserControl
    {
        public WordsView()
        {
            InitializeComponent();
            
            
            //ImageViewer1.Source = new BitmapImage(new Uri("C:\\ECTImages\\Creek.jpg"));
        }

        [Import]
        public WordsViewModel ViewModel
        {
            set {
                this.DataContext = value;
                WordsViewModel model = (WordsViewModel)this.DataContext;
                model.setImage(ImageViewer1);
                model.setAudio(AudioViewer);
                model.setEditbox(EditBox);
            }
        }

        private void MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            WordsViewModel model = (WordsViewModel)this.DataContext;
            model.MediaEnded();
        }

        private void VocabDbClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WordsViewModel model = (WordsViewModel)this.DataContext;
            model.SelectedText2 = MyVocab.SelectedText;
        }

        private void DialogDbClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WordsViewModel model = (WordsViewModel)this.DataContext;
            model.SelectedText2 = MyDialog.SelectedText;
        }


    }
}

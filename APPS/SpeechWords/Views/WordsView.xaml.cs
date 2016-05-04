// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechWords.ViewModels;
using System.Security.Permissions;
using System.Windows.Media.Imaging;

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
            }
        }
    }
}

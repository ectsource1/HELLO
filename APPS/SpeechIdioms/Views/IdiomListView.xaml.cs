// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechIdioms.ViewModels;

namespace SpeechIdioms.Views
{
    [Export("IdiomListView")]
    public partial class IdiomListView : UserControl
    {
        public IdiomListView()
        {
            InitializeComponent();
        }

        [Import]
        public IdiomListViewModel ViewModel
        {
            get { return this.DataContext as IdiomListViewModel; }
            set { this.DataContext = value; }
        }
    }
}

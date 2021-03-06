// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechWords.ViewModels;
using System.Security.Permissions;

namespace SpeechWords.Views
{
    [Export("StoryListView")]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class StoryListView : UserControl
    {
        public StoryListView()
        {
            InitializeComponent();
        }

        [Import]
        public StoryListViewModel ViewModel
        {
            get { return this.DataContext as StoryListViewModel; }
            set { this.DataContext = value; }
        }
    }
}

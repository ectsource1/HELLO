// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechVideos.ViewModels;
using System.Security.Permissions;

namespace SpeechVideos.Views
{
    [Export("VideoListView")]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class VideoListView : UserControl
    {
        public VideoListView()
        {
            InitializeComponent();
        }

        [Import]
        public VideoListViewModel ViewModel
        {
            get { return this.DataContext as VideoListViewModel; }
            set { this.DataContext = value; }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechVideos.ViewModels;

namespace SpeechVideos.Views
{
    [Export("VideoListView")]
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

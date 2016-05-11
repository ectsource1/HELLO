// Copyright (c) Microsoft Corporation. All rights reserved
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using SpeechInfrastructure;

namespace SpeechVideos.Views
{
    [Export]
    [ViewSortHint("03")]
    public partial class VideosNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri videoViewUri = new Uri("/VideoListView", UriKind.Relative);

        [Import]
        public IRegionManager regionManager;

        public VideosNavigationItemView()
        {
            InitializeComponent();
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            IRegion mainContentRegion = this.regionManager.Regions[RegionNames.MainContentRegion];
            if (mainContentRegion != null && mainContentRegion.NavigationService != null)
            {
                mainContentRegion.NavigationService.Navigated += this.MainContentRegion_Navigated;
            }
        }

        public void MainContentRegion_Navigated(object sender, RegionNavigationEventArgs e)
        {
            this.UpdateNavigationButtonState(e.Uri);
        }

        private void UpdateNavigationButtonState(Uri uri)
        {
            this.NavigateToVideosRadioButton.IsChecked = (uri == videoViewUri);
        }

        private void NavigateToVideosRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, videoViewUri);
        }
    }
}

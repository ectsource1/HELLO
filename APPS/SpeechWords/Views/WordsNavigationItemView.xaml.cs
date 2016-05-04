// Copyright (c) Microsoft Corporation. All rights reserved
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using SpeechInfrastructure;

namespace SpeechWords.Views
{
    [Export]
    [ViewSortHint("02")]
    public partial class WordsNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri storyViewUri = new Uri("/StoryListView", UriKind.Relative);

        [Import]
        public IRegionManager regionManager;

        public WordsNavigationItemView()
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
            this.NavigateToWordsRadioButton.IsChecked = (uri == storyViewUri);
        }

        private void NavigateToWordsRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, storyViewUri);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using SpeechInfrastructure;

namespace Pronunciation.Views
{
    [Export]
    [ViewSortHint("08")]
    public partial class SoundNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri soundViewUri = new Uri("/SoundListView", UriKind.Relative);

        [Import]
        public IRegionManager regionManager;

        public SoundNavigationItemView()
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
            this.NavigateToSoundRadioButton.IsChecked = (uri == soundViewUri);
        }

        private void NavigateToSoundRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, soundViewUri);
        }
    }
}

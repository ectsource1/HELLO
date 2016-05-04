// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using SpeechInfrastructure;

namespace SpeechWritting.Views
{
    [Export]
    [ViewSortHint("05")]
    public partial class WrittingNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri ttsViewUri = new Uri("/WrittingView", UriKind.Relative);

        [Import]
        public IRegionManager regionManager;

        public WrittingNavigationItemView()
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
            this.NavigateToWrittingRadioButton.IsChecked = (uri == ttsViewUri);
        }

        private void NavigateToWrittingRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, ttsViewUri);
        }
    }
}

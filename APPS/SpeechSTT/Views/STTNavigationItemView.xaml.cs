// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using SpeechInfrastructure;

namespace SpeechSTT.Views
{
    [Export]
    [ViewSortHint("03")]
    public partial class STTNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri sttViewUri = new Uri("/STTView", UriKind.Relative);

        [Import]
        public IRegionManager regionManager;

        public STTNavigationItemView()
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
            this.NavigateToSTTRadioButton.IsChecked = (uri == sttViewUri);
        }

        private void NavigateToSTTRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, sttViewUri);
        }
    }
}

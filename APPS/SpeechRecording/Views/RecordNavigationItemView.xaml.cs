// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using SpeechInfrastructure;

namespace SpeechRecording.Views
{
    [Export]
    [ViewSortHint("12")]
    public partial class RecordNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri recordViewUri = new Uri("/RecordView", UriKind.Relative);

        [Import]
        public IRegionManager regionManager;

        public RecordNavigationItemView()
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
            this.NavigateToRecordRadioButton.IsChecked = (uri == recordViewUri);
        }

        private void NavigateToRecordRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, recordViewUri);
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using SpeechInfrastructure;

namespace SpeechIdioms.Views
{
    [Export]
    [ViewSortHint("06")]
    public partial class IdiomsNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri idiomViewUri = new Uri("/IdiomListView", UriKind.Relative);

        [Import]
        public IRegionManager regionManager;

        public IdiomsNavigationItemView()
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
            this.NavigateToIdiomsRadioButton.IsChecked = (uri == idiomViewUri);
        }

        private void NavigateToIdiomsRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, idiomViewUri);
        }
    }
}

// Copyright (c) Microsoft Corporation. 
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using SpeechInfrastructure;

namespace GrammarBasics.Views
{
    [Export]
    [ViewSortHint("10")]
    public partial class GrammarNavigationItemView : UserControl, IPartImportsSatisfiedNotification
    {
        private static Uri ttsViewUri = new Uri("/GrammarListView", UriKind.Relative);

        [Import]
        public IRegionManager regionManager;

        public GrammarNavigationItemView()
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
            this.NavigateToGrammarRadioButton.IsChecked = (uri == ttsViewUri);
        }

        private void NavigateToGrammarRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, ttsViewUri);
        }
    }
}

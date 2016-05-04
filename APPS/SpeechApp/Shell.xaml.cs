// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using SpeechInfrastructure;
using System.Windows;
using System.Security.Permissions;

namespace SpeechApp
{
    [Export]
    public partial class Shell : Window, IPartImportsSatisfiedNotification
    {
        private const string TTSModuleName = "TTSModule";
        private static Uri InboxViewUri = new Uri("/InboxView", UriKind.Relative);
        private static Uri LoginViewUri = new Uri("/LoginView", UriKind.Relative);
        public Shell()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogPresenter.Show(new ChildView(), "Hello Child");
        }

        [Import(AllowRecomposition = false)]
        public IModuleManager ModuleManager;

        [Import(AllowRecomposition = false)]
        public IRegionManager RegionManager;

        public void OnImportsSatisfied()
        {
            this.ModuleManager.LoadModuleCompleted +=
                (s, e) =>
                {
                    // spash window
                    // https://www.youtube.com/watch?v=HONa6ARX0Zc
                    if (e.ModuleInfo.ModuleName == TTSModuleName)
                    {
                        this.RegionManager.RequestNavigate(
                            RegionNames.LoginRegion,
                            LoginViewUri);

                        this.RegionManager.RequestNavigate(
                            RegionNames.MainContentRegion,
                            InboxViewUri);
                    }
                };
        }
    }
}

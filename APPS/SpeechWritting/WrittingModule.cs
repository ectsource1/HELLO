// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using SpeechWritting.Views;
using SpeechInfrastructure;

namespace SpeechWritting
{
    [ModuleExport(typeof(WrittingModule))]
    public class WrittingModule : IModule
    {
        [Import]
        public IRegionManager regionManager;

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(WrittingNavigationItemView));
        }
    }
}

using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Pronunciation.Views;
using SpeechInfrastructure;

namespace Pronunciation
{
    [ModuleExport(typeof(SoundModule))]
    public class SoundModule : IModule
    {
        [Import]
        public IRegionManager regionManager;

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(SoundNavigationItemView));
        }
    }
}

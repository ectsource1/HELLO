using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using SpeechIdioms.Views;
using SpeechInfrastructure;

namespace SpeechIdioms
{
    [ModuleExport(typeof(IdiomsModule))]
    public class IdiomsModule : IModule
    {
        [Import]
        public IRegionManager regionManager;

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(IdiomsNavigationItemView));
        }
    }
}

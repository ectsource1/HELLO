using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using GrammarBasics.Views;
using SpeechInfrastructure;

namespace GrammarBasics
{
    [ModuleExport(typeof(GrammarModule))]
    public class GrammarModule : IModule
    {
        [Import]
        public IRegionManager regionManager;

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(GrammarNavigationItemView));
        }
    }
}

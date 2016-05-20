using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Parents.Views;
using SpeechInfrastructure;

namespace Parents
{
    [ModuleExport(typeof(ParentsModule))]
    public class ParentsModule : IModule
    {
        [Import]
        public IRegionManager regionManager;

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(ParentNavigationItemView));
        }
    }
}

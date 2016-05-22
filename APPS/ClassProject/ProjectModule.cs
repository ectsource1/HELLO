using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using ClassProject.Views;
using SpeechInfrastructure;

namespace ClassProject
{
   
    [ModuleExport(typeof(ProjectModule))]
    public class ProjectModule : IModule
    {
        [Import]
        public IRegionManager regionManager;

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(ProjectNavigationItemView));
        }
    }
}

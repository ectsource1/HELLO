using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using StudentInfo.Views;
using SpeechInfrastructure;

namespace StudentInfo
{
    [ModuleExport(typeof(StudentModule))]
    public class StudentModule : IModule
    {
        [Import]
        public IRegionManager regionManager;

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(StudentNavigationItemView));
        }
    }
}

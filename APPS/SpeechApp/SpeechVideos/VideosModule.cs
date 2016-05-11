using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using SpeechVideos.Views;
using SpeechInfrastructure;

namespace SpeechVideos
{
    [ModuleExport(typeof(VideosModule))]
    public class VideosModule : IModule
    {
        [Import]
        public IRegionManager regionManager;

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(VideosNavigationItemView));
        }
    }
}

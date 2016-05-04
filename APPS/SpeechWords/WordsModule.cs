using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using SpeechWords.Views;
using SpeechInfrastructure;

namespace SpeechWords
{
    [ModuleExport(typeof(WordsModule))]
    public class WordsModule : IModule
    {
        [Import]
        public IRegionManager regionManager;

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(WordsNavigationItemView));
        }
    }
}

// Copyright (c) Microsoft Corporation.
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechVideos.ViewModels;
using System.Security.Permissions;
using System.Windows.Threading;

namespace SpeechVideos.Views
{
    [Export("VideosView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class VideosView : UserControl
    {
        DispatcherTimer timer;

        public VideosView()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += new EventHandler(Timer_tick);

        }

        [Import]
        public VideosViewModel ViewModel
        {
            set {
                this.DataContext = value;
                VideosViewModel model = (VideosViewModel)this.DataContext;
                model.setVideoPlayer(VideoViewer);
                model.setEditbox(EditBox, EditBox);
            }
        }

        void Timer_tick(Object sender, EventArgs e)
        {
            VideosViewModel model = (VideosViewModel)this.DataContext;
            model.VideoProgress = VideoViewer.Position.TotalSeconds;
        }

        private void MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            TimeSpan ts = VideoViewer.NaturalDuration.TimeSpan;
            VideosViewModel model = (VideosViewModel)this.DataContext;
            model.MaxTime = ts.TotalSeconds;
            timer.Start();
        }

        private void MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            VideosViewModel model = (VideosViewModel)this.DataContext;
            model.MediaEnded();
        }

        private void VocabDbClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VideosViewModel model = (VideosViewModel)this.DataContext;
            model.SelectedText2 = MyVocab.SelectedText;
        }

        private void DialogDbClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VideosViewModel model = (VideosViewModel)this.DataContext;
            model.SelectedText2 = MyDialog.SelectedText;
        }

        private void SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            VideosViewModel model = (VideosViewModel)this.DataContext;
            model.SelectedText2 = MyDialog.SelectedText;
        }

        private void SelectionVChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            VideosViewModel model = (VideosViewModel)this.DataContext;
            model.SelectedText2 = MyVocab.SelectedText;
        }

        private void IsVisibleChanged1(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            VideosViewModel model = (VideosViewModel)this.DataContext;
            model.changeVisible();
        }
    }
}

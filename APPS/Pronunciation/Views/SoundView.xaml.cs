// Copyright (c) Microsoft Corporation. All rights reserved
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Pronunciation.ViewModels;
using System.Security.Permissions;
using System.Windows.Threading;

namespace Pronunciation.Views
{
    [Export("SoundView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class SoundView : UserControl
    {
        DispatcherTimer timer;

        public SoundView()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += new EventHandler(Timer_tick);
        }

        void Timer_tick(Object sender, EventArgs e)
        {
            SoundViewModel model = (SoundViewModel)this.DataContext;
            model.MediaProgress = MediaBox.Position.TotalSeconds;
        }

        [Import]
        public SoundViewModel ViewModel
        {
            set {
                this.DataContext = value;
                SoundViewModel model = (SoundViewModel)this.DataContext;
                model.setImage(ImageBox);
                model.setAudio(MediaBox);
                model.setEditbox(EditBox);
            }
        }

        private void MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            SoundViewModel model = (SoundViewModel)this.DataContext;
            model.MediaEnded();
        }

        private void MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            TimeSpan ts = MediaBox.NaturalDuration.TimeSpan;
            SoundViewModel model = (SoundViewModel)this.DataContext;
            model.MaxTime = ts.TotalSeconds;
            timer.Start();
        }
    }
}

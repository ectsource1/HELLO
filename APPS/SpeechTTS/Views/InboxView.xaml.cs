// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechTTS.ViewModels;

namespace SpeechTTS.Views
{
    [Export("InboxView")]
    public partial class InboxView : UserControl
    {
        public InboxView()
        {
            InitializeComponent();
        }

        [Import]
        public InboxViewModel ViewModel
        {
            get { return this.DataContext as InboxViewModel; }
            set { this.DataContext = value; }
        }
    }
}

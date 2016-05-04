// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechTTS.ViewModels;
using System.Security.Permissions;

namespace SpeechTTS.Views
{
    [Export("TTSView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class TTSView : UserControl
    {
        public TTSView()
        {
            InitializeComponent();
        }

        [Import]
        public TTSViewModel ViewModel
        {
            set { this.DataContext = value; }
        }
    }
}

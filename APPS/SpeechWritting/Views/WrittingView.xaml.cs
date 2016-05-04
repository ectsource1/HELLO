// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechWritting.ViewModels;
using System.Security.Permissions;

namespace SpeechWritting.Views
{
    [Export("WrittingView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class WrittingView : UserControl
    {
        public WrittingView()
        {
            InitializeComponent();
        }

        [Import]
        public WrittingViewModel ViewModel
        {
            set { this.DataContext = value; }
        }
    }
}

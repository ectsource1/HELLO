// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechSTT.ViewModels;
using System.Security.Permissions;

namespace SpeechSTT.Views
{
    [Export("STTView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class STTView : UserControl
    {
        public STTView()
        {
            InitializeComponent();
        }

        [Import]
        public STTViewModel ViewModel
        {
            set { this.DataContext = value; }
        }
    }
}

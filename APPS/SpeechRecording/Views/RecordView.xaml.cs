// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using SpeechRecording.ViewModels;
using System.Security.Permissions;

namespace SpeechRecording.Views
{
    [Export("RecordView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class RecordView : UserControl
    {
        public RecordView()
        {
            InitializeComponent();
        }

        [Import]
        public RecordViewModel ViewModel
        {
            set { this.DataContext = value; }
        }
    }
}

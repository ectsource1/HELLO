// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using Pronunciation.ViewModels;
using System.Security.Permissions;

namespace Pronunciation.Views
{
    [Export("SoundListView")]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class SoundListView : UserControl
    {
        public SoundListView()
        {
            InitializeComponent();
        }

        [Import]
        public SoundListViewModel ViewModel
        {
            get { return this.DataContext as SoundListViewModel; }
            set { this.DataContext = value; }
        }
    }
}

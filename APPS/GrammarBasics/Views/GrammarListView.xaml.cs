// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using GrammarBasics.ViewModels;
using System.Security.Permissions;

namespace GrammarBasics.Views
{
    [Export("GrammarListView")]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class GrammarListView : UserControl
    {
        public GrammarListView()
        {
            InitializeComponent();
        }

        [Import]
        public GrammarListViewModel ViewModel
        {
            get { return this.DataContext as GrammarListViewModel; }
            set { this.DataContext = value; }
        }
    }
}

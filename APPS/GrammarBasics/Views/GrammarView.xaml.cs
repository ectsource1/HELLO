// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using GrammarBasics.ViewModels;
using System.Security.Permissions;

namespace GrammarBasics.Views
{
    [Export("GrammarView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class GrammarView : UserControl
    {
        public GrammarView()
        {
            InitializeComponent();
        }

        [Import]
        public GrammarViewModel ViewModel
        {
            set {
                this.DataContext = value;
                GrammarViewModel model = (GrammarViewModel)this.DataContext;
                model.setEditbox(EditBox);
            }
        }
    }
}

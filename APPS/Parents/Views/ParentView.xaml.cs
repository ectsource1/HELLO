// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using Parents.ViewModels;

namespace Parents.Views
{
    [Export("ParentView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ParentView : UserControl
    {
        public ParentView()
        {
            InitializeComponent();
        }

        [Import]
        public ParentViewModel ViewModel
        {
            set { this.DataContext = value; }
        }

        private void MouseDbClick20(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ParentViewModel model = (ParentViewModel)this.DataContext;
            model.SelectedText = Dialog20.SelectedText;
        }

        private void MouseDbClick30(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ParentViewModel model = (ParentViewModel)this.DataContext;
            model.SelectedText = Dialog30.SelectedText;
        }
    }
}

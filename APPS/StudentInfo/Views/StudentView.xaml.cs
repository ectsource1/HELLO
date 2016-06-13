// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using StudentInfo.ViewModels;
using System.Security.Permissions;
using System.Windows.Input;

namespace StudentInfo.Views
{
    [Export("StudentView")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class StudentView : UserControl
    {
        public StudentView()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(keyDownEventHandler);
        }

        private void keyDownEventHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                StudentViewModel vmodel = (StudentViewModel)this.DataContext;
                if (vmodel.CheckVisible)
                    vmodel.CheckVisible = false;
                else
                    vmodel.CheckVisible = true;
            }
        }

        [Import]
        public StudentViewModel ViewModel
        {
            set { this.DataContext = value; }
        }

    }
}

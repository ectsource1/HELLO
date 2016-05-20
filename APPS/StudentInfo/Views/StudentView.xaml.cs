// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.ComponentModel.Composition;
using System.Windows.Controls;
using StudentInfo.ViewModels;
using System.Security.Permissions;

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
        }

        [Import]
        public StudentViewModel ViewModel
        {
            set { this.DataContext = value; }
        }

    }
}

using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.Composition;
using SpeechTTS.ViewModels;

namespace SpeechTTS.Views
{
    [Export("LoginView")]
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(keyDownEventHandler);
        }

        private void keyDownEventHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                LoginViewModel vmodel = (LoginViewModel)this.DataContext;

                // display update only when logged in
                if (!vmodel.FocusPoint) return;

                if (vmodel.FocusPoint) {
                    vmodel.NeedUpdate = true;
                    vmodel.FocusPoint = false;
                    
                }
                else
                {
                    vmodel.FocusPoint = false;
                }

                vmodel.NeedAuth = false;
                vmodel.NeedForce = false;

            } else if (e.Key == Key.A && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                LoginViewModel vmodel = (LoginViewModel)this.DataContext;

                // display authorize only when not logged in
                if (vmodel.FocusPoint) return;

                vmodel.NeedAuth = true;
                vmodel.NeedUpdate = false;
                vmodel.NeedForce = false;
                vmodel.FocusPoint = false;
                vmodel.NotAuthenticated = false;
            }
            else if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                LoginViewModel vmodel = (LoginViewModel)this.DataContext;

                // display authorize only when not logged in
                if (vmodel.FocusPoint) return;

                vmodel.NeedForce = true;
                vmodel.NeedAuth = false;
                vmodel.NeedUpdate = false;
                vmodel.FocusPoint = false;
                vmodel.NotAuthenticated = false;
            }
            else if (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                LoginViewModel vmodel = (LoginViewModel)this.DataContext;

                // display authorize only when not logged in
                if (vmodel.FocusPoint) return;

                vmodel.NeedForce = false;
                vmodel.NeedAuth = false;
                vmodel.NeedUpdate = false;
                vmodel.FocusPoint = false;
                vmodel.NotAuthenticated = true;
            }

        }

        [Import]
        public LoginViewModel ViewModel
        {
            get { return this.DataContext as LoginViewModel; }
            set { this.DataContext = value; }
        }
    }

    
}

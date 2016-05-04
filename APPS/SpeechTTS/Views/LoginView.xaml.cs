using System.Windows.Controls;
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
        }

        
        [Import]
        public LoginViewModel ViewModel
        {
            get { return this.DataContext as LoginViewModel; }
            set { this.DataContext = value; }
        }
    }
}

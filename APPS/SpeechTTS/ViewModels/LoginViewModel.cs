using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Input;
using SpeechTTS.Auth;

namespace SpeechTTS.ViewModels
{
    [Export]
    public class LoginViewModel : BindableBase, INavigationAware
    {
        private readonly IAuthenticationService _authService;
        private readonly DelegateCommand logoutCommand;
        private readonly DelegateCommand<object> loginCommand;

        private bool isAuthenticated;
        private bool notAuthenticated;

        private int id;
        private string _status;

        [ImportingConstructor]
        public LoginViewModel(IAuthenticationService authService)
        {
            this.loginCommand = new DelegateCommand<object>(this.Login);
            this.logoutCommand = new DelegateCommand(this.Logout);
            this._authService = authService;
            isAuthenticated = false;
            notAuthenticated = true;
        }

        public ICommand LoginCommand
        {
            get { return this.loginCommand; }
        }

        public ICommand LogoutCommand
        {
            get { return this.logoutCommand; }
        }

        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.SetProperty(ref this.id, value);
            }
        }

        public string Status
        {
            get { return this._status; }
            set { this.SetProperty(ref this._status, value); }
        }

        public bool IsAuthenticated
        {
            get { return this.isAuthenticated; }
            set { this.SetProperty(ref this.isAuthenticated, value); }
        }

        public bool NotAuthenticated
        {
            get { return this.notAuthenticated; }
            set { this.SetProperty(ref this.notAuthenticated, value); }
        }

        private void Login(object control)
        {
            PasswordBox passwordBox = control as PasswordBox;
            string clearTextPassword = passwordBox.Password;
            try
            {
                //Validate credentials through the authentication service
                if (Id == 0) Id = -1;
                User user = _authService.AuthenticateUser(Id, clearTextPassword);

                if (user.Id == -1)
                {
                    MessageBox.Show(user.Name + "\n" +
                                    user.Macid);
                    return;
                }

                //Get the current principal object
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("Must set CustomPrincipal object on startup.");

                //Authenticate the user
                string userId = "ECT" + user.Location + user.Id;
                string[] roles = user.Roles.Split(',');
                customPrincipal.Identity = new CustomIdentity(userId, user.Name, user.Level, user.Email, roles);
                IsAuthenticated = true;
                NotAuthenticated = false;

                //Update UI
                //NotifyPropertyChanged("AuthenticatedUser");
                //NotifyPropertyChanged("IsAuthenticated");
                //_loginCommand.RaiseCanExecuteChanged();
                //_logoutCommand.RaiseCanExecuteChanged();
                //Username = string.Empty; //reset
                //passwordBox.Password = string.Empty; //reset
                //Status = string.Empty;
            }
            catch (UnauthorizedAccessException)
            {
                Status = "Login failed!";
            }
            catch (Exception ex)
            {
                Status = string.Format("ERROR: {0}", ex.Message);
            }
        }

        private void Logout()
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                IsAuthenticated = false;
                NotAuthenticated = true;
                Status = string.Empty;
            }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return string.Format("Signed in as {0}. {1}",
                          Thread.CurrentPrincipal.Identity.Name,
                          Thread.CurrentPrincipal.IsInRole("Administrators") ? "You are an administrator!"
                              : "You are NOT a member of the administrators group.");

                return "Not authenticated!";
            }
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
                return true;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Intentionally not implemented.
        }


    }
}

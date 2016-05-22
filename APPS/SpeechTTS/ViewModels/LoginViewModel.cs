using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using System;
using System.IO;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Input;
using SpeechTTS.Auth;
using SpeechInfrastructure;
namespace SpeechTTS.ViewModels
{
    [Export]
    public class LoginViewModel : BindableBase, INavigationAware
    {
        private readonly IAuthenticationService _authService;
        private readonly DelegateCommand logoutCommand;
        private readonly DelegateCommand<object> authCommand;
        private readonly DelegateCommand<object> loginCommand;
        private readonly DelegateCommand<object> updatePasswd;

        private bool needAuth;
        private bool needUpdate;
        private bool focusPoint;
        private bool notAuthenticated;
        private string userName = "ECT";
        private string _status;
        private bool notLogin = true;
         
        [ImportingConstructor]
        public LoginViewModel(IAuthenticationService authService)
        {
            this.logoutCommand = new DelegateCommand(this.Logout);
            this.authCommand  = new DelegateCommand<object>(this.Authorize);
            this.loginCommand = new DelegateCommand<object>(this.Login);
            this.updatePasswd = new DelegateCommand<object>(this.UpdatePw);

            this._authService = authService;
            needAuth = false;
            needUpdate = false;
            focusPoint = false;
            notAuthenticated = true;

            string fileName = AppDomain.CurrentDomain.BaseDirectory;
            fileName = fileName + "DataFiles\\" + Personal.PERSON_BIN;

            if (File.Exists(fileName))
            {
                Personal person = Personal.read(fileName);
                UserName = person.StudentId;
            }
            
        }

        public ICommand LoginCommand
        {
            get { return this.loginCommand; }
        }

        public ICommand LogoutCommand
        {
            get { return this.logoutCommand; }
        }

        public ICommand AuthCommand
        {
            get { return this.authCommand; }
        }

        public ICommand UpdatePasswd
        {
            get { return this.updatePasswd; }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                this.SetProperty(ref this.userName, value);
            }
        }

        public string Status
        {
            get { return this._status; }
            set { this.SetProperty(ref this._status, value); }
        }

        public bool NeedAuth
        {
            get { return this.needAuth; }
            set { this.SetProperty(ref this.needAuth, value); }
        }

        public bool NeedUpdate
        {
            get { return this.needUpdate; }
            set { this.SetProperty(ref this.needUpdate, value); }
        }

        public bool FocusPoint
        {
            get { return this.focusPoint; }
            set { this.SetProperty(ref this.focusPoint, value); }
        }

        public bool NotAuthenticated
        {
            get { return this.notAuthenticated; }
            set { this.SetProperty(ref this.notAuthenticated, value); }
        }

        private void Authorize(object control)
        {
            NotLogin = false;
            Status = "";
            NotLogin = false;
            PasswordBox passwordBox = control as PasswordBox;
            string password = passwordBox.Password;

            if (userName.Length < 6) return;
            string sub = userName.Substring(5);
            int id = -1;
            if (!Int32.TryParse(sub, out id))
            {
                Status = "Invalid User Name !";
                return;
            }

            try
            {
                User user = _authService.AuthorizeComputer(id, password);
                if (user.Id == -1) {
                   NotLogin = true;
                   Status = user.Name + "--" + user.Macid;
                   return;
                }

                setIdentity(user);

            } catch (UnauthorizedAccessException) {
                Status = "Login failed!";
            } catch (Exception ex) {
                Status = string.Format("ERROR: {0}", ex.Message);
            }

            NotLogin = true;
        }

        private void UpdatePw(object control)
        {
            PasswordBox passwordBox = control as PasswordBox;
            string passwd = passwordBox.Password;

            if (userName.Length < 6) return;
            string sub = userName.Substring(5);
            int id = -1;
            if (!Int32.TryParse(sub, out id))
            {
                Status = "Invalid User Name !";
                return;
            }

            _authService.UpdatePasswd(id, passwd);
            FocusPoint = true;
            NotAuthenticated = false;
            NeedUpdate = false;
            NeedAuth = false;
        }

        public bool NotLogin
        {
            get { return this.notLogin; }
            set { this.SetProperty(ref this.notLogin, value); }
        }

        private void Login(object control)
        {
            Status = "";
            NotLogin = false;
            PasswordBox passwordBox = control as PasswordBox;
            string clearTextPassword = passwordBox.Password;

            if (userName.Length < 6) return;
            string sub = userName.Substring(5);
            int id = -1;
            if (!Int32.TryParse(sub, out id))
            {
                Status = "Invalid User Name !";
                return;
            }       

            try
            {
                //Validate credentials through the authentication service
                User user = _authService.AuthenticateUser(id, clearTextPassword);

                if (user.Id == -1)
                {
                    NotLogin = true;
                    Status = user.Name + "--" + user.Macid;
                    return;
                }

                setIdentity(user);
                
            }
            catch (UnauthorizedAccessException)
            {
                Status = "Login failed!";
            }
            catch (Exception ex)
            {
                Status = string.Format("ERROR: {0}", ex.Message);   
            }

            NotLogin = true;
        }

        private void setIdentity(User user)
        {
            //Get the current principal object
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal == null)
                throw new ArgumentException("Must set CustomPrincipal object on startup.");

            //Authenticate the user
            string userId = "ECT" + user.Location + user.Id;
            string[] roles = user.Roles.Split(',');
            customPrincipal.Identity = new CustomIdentity(userId, user.Name, user.Level, user.Email, roles);
            FocusPoint = true;
            NotAuthenticated = false;
            NeedUpdate = false;
            NeedAuth = false;
        }

        private void Logout()
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
                FocusPoint = false;
                NeedUpdate = false;
                NotAuthenticated = true;
                Status = string.Empty;
            }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (FocusPoint)
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

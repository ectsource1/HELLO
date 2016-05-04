using System.Linq;
using System.Security.Principal;

namespace SpeechTTS.Auth
{
    public class CustomIdentity : IIdentity
    {
        public CustomIdentity(string name, string fullname, int level,  string email, string[] roles)
        {
            Name = name;
            Fullname = fullname;
            Level = level;
            Email = email;
            Roles = roles;
        }

        public string Name { get; private set; }
        public string Fullname { get; private set; }
        public int Level { get; private set; }
        public string Email { get; private set; }
        public string[] Roles { get; private set; }

        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion
    }

    public class AnonymousIdentity : CustomIdentity
    {
        public AnonymousIdentity()
            : base(string.Empty, string.Empty, 0, string.Empty, new string[] { })
        { }
    }

    public class CustomPrincipal : IPrincipal
    {
        private CustomIdentity _identity;

        public CustomIdentity Identity
        {
            get { return _identity ?? new AnonymousIdentity(); }
            set { _identity = value; }
        }

        #region IPrincipal Members
        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }

        public bool IsInRole(string role)
        {
            return _identity.Roles.Contains(role);
        }
        #endregion
    }
}

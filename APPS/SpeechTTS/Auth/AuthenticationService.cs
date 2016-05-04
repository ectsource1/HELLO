using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ComponentModel.Composition;
using Newtonsoft.Json;
using System.Management;

namespace SpeechTTS.Auth
{
    public interface IAuthenticationService
    {
        //User AuthenticateUser(string username, string password);
        User AuthenticateUser(int id, string password);
    }

    [Export(typeof(IAuthenticationService))]
    public class AuthenticationService : IAuthenticationService
    {
        private class InternalUserData
        {
            public InternalUserData(string username, string email, 
                                    string hashedPassword, string[] roles)
            {
                Username = username;
                Email = email;
                HashedPassword = hashedPassword;
                Roles = roles;
            }
            public string Username
            {
                get;
                private set;
            }

            public string Email
            {
                get;
                private set;
            }

            public string HashedPassword
            {
                get;
                private set;
            }

            public string[] Roles
            {
                get;
                private set;
            }
        }

        private static readonly List<InternalUserData> _users = new List<InternalUserData>()
        {
            new InternalUserData("Mark", "mark@company.com",
            "MB5PYIsbI2YzCUe34Q5ZU2VferIoI4Ttd+ydolWV0OE=", new string[] { "Administrators" }),
            new InternalUserData("John", "john@company.com",
            "hMaLizwzOQ5LeOnMuj+C6W75Zl5CXXYbwDSHWW9ZOXc=", new string[] { })
        };

        public AuthenticationService()
        {
        }

        public User AuthenticateUser(int id, string inputTxt)
        {
            //InternalUserData userData = _users.FirstOrDefault(u => u.Username.Equals(username)
            //    && u.HashedPassword.Equals(CalculateHash(clearTextPassword, u.Username)));
            string json;
            WebClient client = new WebClient();
            string url = string.Format("http://www.ectedu.com/api2.php/students/{0}", id);
            string value = client.DownloadString(url);
            //string value = client.OpenRead(url);
            json = value;
            //User user = new JavaScriptSerializer().Deserialize<User>(json);
            dynamic d = JsonConvert.DeserializeObject<dynamic>(json);
            string macid = GetMACAddress();
            User user = new User();
            if (d == null)
            {
                user.Id = -1;
                user.Name = "No such account";
                user.Macid = macid;
                return user;
            }

            user.Id = d.id;
            user.Name = d.name;
            user.Passwd = d.passwd;
            user.Macid = d.macid;
            user.Level = d.level;
            user.Location = d.location;
            user.Status = d.status;
            user.Created = d.created;
            user.Email = d.email;
            user.Roles = d.roles;
       
            
            if (!user.Passwd.Equals(inputTxt))
            {
                user.Id = -1;
                user.Name = "Invalid account";
            } else if (!user.Macid.Equals(macid))
            {
                user.Id = -1;
                user.Name = "Invalid Computer";
            }
                
            return user;
        }

        private string CalculateHash(string clearTextPassword, string salt)
        {
            // Convert the salted password to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
            // Use the hash algorithm to calculate the hash
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // Return the hash as a base64 encoded string to be compared to the stored password
            return Convert.ToBase64String(hash);
        }

        public static string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }

            //MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }
    }

    ///https://blog.magnusmontin.net/2013/03/24/custom-authorization-in-wpf/
    /// If you want to show the LoginDialog BEFORE the Shell is loaded, 
    /// you have to create your own Bootstrapper which inhertis from 
    /// the prism Bootstrapper and call the LoginDialog.ShowDialog() method 
    /// in the CreateShell() (or InitializeShell()) method of the Bootstrapper

    public class Users
    {
        public List<User> data { get; set; }
    }

    public class User
    {
        public User() { }

        public User(int id, string name, string passwd, string macid, int level, string loc, int status, 
                    DateTime date, string email, string roles)
        {
            Id = id;
            Name = name;
            Passwd = passwd;
            Macid = macid;
            Level = level;
            Location = loc;
            Status = status;
            Created = date;
            Email = email;
            Roles = roles;
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Passwd
        {
            get;
            set;
        }

        public string Macid
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public int Status
        {
            get;
            set;
        }

        public DateTime Created
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Roles
        {
            get;
            set;
        }
    }
}

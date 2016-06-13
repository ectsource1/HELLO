using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ComponentModel.Composition;
using Newtonsoft.Json;
using System.Management;

namespace SpeechTTS.Auth
{
    public interface IAuthenticationService
    {
        string GetDbLink();
        void UpdatePasswd(int id, string pw);
        User AuthorizeComputer(int id, string pw);
        User AuthenticateUser(int id, string pw);
    }

    [Export(typeof(IAuthenticationService))]
    public class AuthenticationService : IAuthenticationService
    {
        private string dbLink = "";

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

        public AuthenticationService()
        {
            string userRoot = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string userDataRoot = userRoot + @"\ECTData\";
            if (!Directory.Exists(userDataRoot))
            {
                Directory.CreateDirectory(userDataRoot);
            }

            string userDefaultRoot = userDataRoot + @"ECT\";
            if (!Directory.Exists(userDefaultRoot))
            {
                Directory.CreateDirectory(userDefaultRoot);
            }

            string fileName = userDefaultRoot + "goLink.txt";
            dbLink = "http://www.ectedu.com/apiTest.php";
            if (File.Exists(fileName))
            {
                string[] lines = File.ReadAllLines(fileName);
                foreach (string line in lines)
                {
                    if (line.Length > 10 )
                    {
                        dbLink = line + "apiTest.php";
                        break;
                    }  
                } // foreach
            }
        }

        public string GetDbLink()
        {
            return dbLink;
        }

        public void UpdatePasswd(int id, string pw)
        {
            updateStatus(id, "PPP");

            WebClient client = new WebClient();
            string linkStr = dbLink + "/students/{0}/PPP/{1}/p";
            string url = string.Format(linkStr, id, pw);
            string value = client.DownloadString(url);
        }

        public User AuthorizeComputer(int id, string pw)
        {
            updateStatus(id, "PPM");

            string macid = GetMACAddress();
            WebClient client = new WebClient();

            User user = validateUser(id, pw);
            if (user.Id < 0) return user;
            string linkStr = dbLink + "/students/{0}/PPM/{1}/ppm";
            user.Macid = macid; 
            string url = string.Format(linkStr, id, macid);
            string value = client.DownloadString(url);
            return user;
        }

        public User AuthenticateUser(int id, string pw)
        { 
            updateStatus(id, "PG");

            User user = validateUser(id, pw);
            if (user.Id < 0) return user;

            string macid = GetMACAddress();
            
            if (!user.Macid.Equals(macid))
            {
                user.Id = -1;
                user.Macid = macid;
                user.Name = "Invalid Computer";
            }
                
            return user;
        }

        private void updateStatus(int id, string action)
        {
            string macid = GetMACAddress();
            WebClient client = new WebClient();
            string linkStr = dbLink + "/loginstats/{0}/ADD/{1}/{2}";
            string url = string.Format(linkStr, id, macid, action);
            string value = client.DownloadString(url);
        }

        private User validateUser(int id, string pw)
        {
            string json;
            WebClient client = new WebClient();
            string linkStr = dbLink + "/students/{0}/PG/{1}/v";
            string url = string.Format(linkStr, id, pw);
            string value = client.DownloadString(url);
            json = value;
            dynamic d = JsonConvert.DeserializeObject<dynamic>(json);

            User user = new User();
            if (d == null)
            {
                user.Id = -1;
                user.Name = "Invalid Account";
                return user;
            }
 
            user.Id = d.id;
            user.Name = d.name;
            user.Macid = d.macid;
            user.Level = d.level;
            user.Location = d.location;
            user.Status = d.status;
            user.Created = d.created;
            user.Email = d.email;
            user.Roles = d.roles;

            return user;
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

            if (MACAddress.Length > 15)
               MACAddress = MACAddress.Substring(0, 15);

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

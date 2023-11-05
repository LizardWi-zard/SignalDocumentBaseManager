using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDocumentBaseManager.Classes
{
    public class User
    {
        private string _login;
        private string _password;
        private int _accessLevel = 3;

        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                if(String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException();   
                }
                else
                {
                    _login = value;
                }
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException();
                }
                else
                {
                    _password = value;
                }
            }
        }

        public int AccessLevel
        {
            get
            {
                return _accessLevel;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    _accessLevel = value;
                }
            }
        }
    }
}

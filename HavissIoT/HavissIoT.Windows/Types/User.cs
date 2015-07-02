using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavissIoT
{
    class User
    {
        private string name;
        private bool OP = false;
        private bool Protected = false;
        private bool PasswordProtected = false;

        public User(string name, bool OP, bool isProtected, bool isPasswordProtected) {
            this.name = name;
            this.OP = OP;
            this.Protected = isProtected;
            this.PasswordProtected = isPasswordProtected;
        }

        public string getName()
        {
            return this.name;
        }

        public bool isOP()
        {
            return this.OP;
        }

        public bool isProtected()
        {
            return this.Protected;
        }

        public bool isPasswordProtected()
        {
            return this.PasswordProtected;
        }
    }
}

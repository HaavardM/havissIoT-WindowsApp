using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavissIoT
{
    class UserHandler
    {
        private List<User> availableUsers = new List<User>();

        public bool addUser(User user)
        {
            foreach(User u in this.availableUsers) {
                if(u.getName().CompareTo(user.getName()) == 0) {
                    return false;
                }
            }
            availableUsers.Add(user);
            return true;
        }

        public bool removeUser(string name)
        {
            foreach (User u in this.availableUsers)
            {
                if (u.getName().CompareTo(name) == 0)
                {
                    this.availableUsers.Remove(u);
                    return true;
                }
            }
            return false;
        }

        public User getUser(string name)
        {
            foreach (User u in this.availableUsers)
            {
                if (u.getName().CompareTo(name) == 0)
                {
                    return u;
                }
            }
            return null;
        }

        public void clearUsers()
        {
            this.availableUsers.Clear();
        }
    }
}

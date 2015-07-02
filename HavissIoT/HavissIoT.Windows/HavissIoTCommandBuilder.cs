using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HavissIoT
{
    class HavissIoTCommandBuilder
    {
        private JObject jsonObject;
        private JObject arguments;
        private string username;
        private string password;
        private bool hasCommad = false;
        private bool hasArgs = false;

        public HavissIoTCommandBuilder()
        {
            this.jsonObject = new JObject();
            this.arguments = new JObject();
            this.username = null;
            this.password = null;
        }

        //Adding user
        public void addUser(string username)
        {
            this.username = username;
            this.password = null;
            this.jsonObject.Remove("user");
            this.jsonObject.Add("user", this.username);
        }

        //Overloaded function - includes password
        public void addUser(string username, string password)
        {
            this.username = username;
            this.password = password;
            this.jsonObject.Remove("user");
            this.jsonObject.Add("user", this.username);
            this.jsonObject.Remove("password");
            this.jsonObject.Add("password", this.password);
        }

        //List all sensors
        public void listSensors()
        {
            
            this.jsonObject.Remove("cmd");
            this.jsonObject.Add("cmd", "sensor");
            this.jsonObject.Remove("args");
            this.arguments = new JObject();
            this.arguments.Add("intent", "list");
            this.jsonObject.Add("args", this.arguments);
            

        }

        //Create a new sensor
        public void createSensor(string name, string topic, string type, bool toStore)
        {
            this.jsonObject.Remove("cmd");
            this.jsonObject.Remove("args");
            this.jsonObject.Add("cmd", "sensor");
            this.arguments = new JObject();
            this.arguments.Add("intent", "create");
            this.arguments.Add("name", name);
            this.arguments.Add("topic", topic);
            this.arguments.Add("type", type);
            this.arguments.Add("toStore", toStore);
            this.jsonObject.Add("args", this.arguments);
        }

        //Remove a sensor by name
        public void removeSensor(string name)
        {
            this.jsonObject.Remove("cmd");
            this.jsonObject.Remove("args");
            this.jsonObject.Add("cmd", "sensor");
            this.arguments = new JObject();
            this.arguments.Add("intent", "remove");
            this.arguments.Add("name", name);
            this.jsonObject.Add("args", this.arguments);
        }

        //Remove a sensor by name
        public void saveSensors()
        {
            this.jsonObject.Remove("cmd");
            this.jsonObject.Remove("args");
            this.jsonObject.Add("cmd", "sensor");
            this.arguments = new JObject();
            this.arguments.Add("intent", "save");
            this.jsonObject.Add("args", this.arguments);
        }

        //Close server application remotely 
        public void serverExit()
        {
            this.jsonObject.Remove("cmd");
            this.jsonObject.Remove("args");
            this.jsonObject.Add("cmd", "exit");
        }

        //Creates new user without password authentication
        public void newUser(string username)
        {
            this.jsonObject.Remove("cmd");
            this.jsonObject.Remove("args");
            this.jsonObject.Add("cmd", "user");
            this.arguments = new JObject();
            this.arguments.Add("intent", "create");
            this.arguments.Add("name", username);
            this.jsonObject.Add("args", this.arguments);
        }

        //Creates new user with password authentication
        public void newUser(string username, string password)
        {
            this.jsonObject.Remove("cmd");
            this.jsonObject.Remove("args");
            this.jsonObject.Add("cmd", "user");
            this.arguments = new JObject();
            this.arguments.Add("intent", "create");
            this.arguments.Add("name", username);
            this.arguments.Add("password", password);
            this.jsonObject.Add("args", this.arguments);
        }

        //Creates new OP user - must have password authentication and must be used by user with OP access
        public void newOPUser(string username, string password)
        {
            this.jsonObject.Remove("cmd");
            this.jsonObject.Remove("args");
            this.jsonObject.Add("cmd", "user");
            this.arguments = new JObject();
            this.arguments.Add("intent", "create");
            this.arguments.Add("name", username);
            this.arguments.Add("password", password);
            this.arguments.Add("isOP", true);
            this.jsonObject.Add("args", this.arguments);
        }

        public void getUsers()
        {
            this.jsonObject.Remove("cmd");
            this.jsonObject.Add("cmd", "user");
            this.jsonObject.Remove("args");
            this.arguments = new JObject();
            this.arguments.Add("intent", "list");
            this.jsonObject.Add("args", this.arguments);
        }

        public void getConfig()
        {
            this.jsonObject.Remove("cmd");
            this.jsonObject.Remove("args");
            this.jsonObject.Add("cmd", "config");
            this.arguments = new JObject();
            this.arguments.Add("intent", "get");
            this.jsonObject.Add("args", this.arguments);
        }

        //Get the finished json string
        public string getJsonString()
        {
            return this.jsonObject.ToString(Formatting.None);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Data;
using Newtonsoft.Json.Linq;

namespace HavissIoT
{
    class Config
    {
        public static volatile string serverAddress = "";
        public static volatile int serverPort = 0;
        public static volatile string brokerAddress = "";
        public static volatile int brokerPort = 0;
        public static volatile int mqttQOS = 0;
        public static volatile string username = "";
        public static volatile string password = "";
        
        static Config() {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("server_address"))
            {
                serverAddress = (string)ApplicationData.Current.LocalSettings.Values["server_address"];
            }
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("server_port"))
            {
                serverPort = (int)ApplicationData.Current.LocalSettings.Values["server_port"];
            }
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("broker_address"))
            {
                brokerAddress = (string)ApplicationData.Current.LocalSettings.Values["broker_address"];
            }
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("broker_port"))
            {
                brokerPort = (int)ApplicationData.Current.LocalSettings.Values["broker_port"];
            }
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("mqtt_qos"))
            {
                mqttQOS = (int)ApplicationData.Current.LocalSettings.Values["mqtt_qos"];
            }
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("username"))
            {
                username = (string)ApplicationData.Current.LocalSettings.Values["username"];
            }
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("password"))
            {
                password = (string)ApplicationData.Current.LocalSettings.Values["password"];
            }
        }
        public static void saveSettings()
        {
            ApplicationData.Current.LocalSettings.Values["server_address"] = serverAddress;
            ApplicationData.Current.LocalSettings.Values["server_port"] = serverPort;
            ApplicationData.Current.LocalSettings.Values["broker_address"] = brokerAddress;
            ApplicationData.Current.LocalSettings.Values["broker_port"] = brokerPort;
            ApplicationData.Current.LocalSettings.Values["mqtt_qos"] = mqttQOS;
            ApplicationData.Current.LocalSettings.Values["username"] = username;
            ApplicationData.Current.LocalSettings.Values["password"] = password;
        }

        public async static Task requestConfig()
        {
            HavissIoTCommandBuilder commandBuilder = new HavissIoTCommandBuilder();
            if (Config.password.Length > 0 && Config.username.Length > 0)
            {
                commandBuilder.addUser(Config.username, Config.password);
            }
            else if(Config.username.Length > 0)
            {
                commandBuilder.addUser(Config.username);
            }
            commandBuilder.getConfig();
            if (SharedVariables.client.isConnected())
            {
                string response = await SharedVariables.client.request(commandBuilder.getJsonString());
                JObject jsonObject = JObject.Parse(response);
                try
                {
                    Config.brokerAddress = (string) jsonObject.GetValue("brokerAddress");
                    Config.brokerPort = (int)jsonObject.GetValue("brokerPort");
                    Config.mqttQOS = (int)jsonObject.GetValue("qos");
                }
                catch (Exception ex)
                {
                    //TODO: Handle exception
                }
            }
                        
        }
    }
}

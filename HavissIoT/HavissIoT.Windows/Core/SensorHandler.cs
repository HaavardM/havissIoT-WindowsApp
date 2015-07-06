using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HavissIoT
{
    class SensorHandler
    {
        private List<IoTSensor> availableSensors = new List<IoTSensor>();

        public void addSensor(IoTSensor sensor) {
            this.availableSensors.Add(sensor);
        }

        //Remove sensorByname
        public void removeSensorByName(string name)
        {
            foreach (IoTSensor s in this.availableSensors)
            {
                if(s.getName().CompareTo(name) == 0)
                {
                    this.availableSensors.Remove(s);
                    break;
                }
            }
        }

        public IoTSensor getSensorByName(string name)
        {
            foreach (IoTSensor s in this.availableSensors)
            {
                if (s.getName().CompareTo(name) == 0)
                {
                    return s;
                }
            }
            return null;
        }

        public IoTSensor getSensorByTopic(string topic)
        {
            foreach (IoTSensor s in this.availableSensors)
            {
                if (s.getTopic().CompareTo(topic) == 0)
                {
                    return s;
                }
            }
            return null; 
        }

        public List<IoTSensor> getSensors()
        {
            return this.availableSensors;
        }

        public void clearSensors()
        {
            this.availableSensors.Clear();
        }

        public async void refreshSensors()
        {
            if (SharedVariables.client.isConnected())
            {
                HavissIoTCommandBuilder commandBuilder = new HavissIoTCommandBuilder();
                if (Config.username.Length > 0)
                {
                    if (Config.password.Length > 0)
                    {
                        commandBuilder.addUser(Config.username, Config.password);
                    }
                    else
                    {
                        commandBuilder.addUser(Config.username);
                    }
                }
                commandBuilder.listSensors();
                String response = await SharedVariables.client.request(commandBuilder.getJsonString());
                JObject jsonObject = null;
                JArray jsonArray = null;
                try
                {
                    jsonObject = JObject.Parse(response);
                    jsonArray = jsonObject.GetValue("r") as JArray;
                }
                catch (Exception ex)
                {
                    jsonObject = null;
                    jsonArray = null;
                }
                if (jsonArray != null)
                {
                    foreach (JObject s in jsonArray)
                    {
                        try
                        {
                            string sensorName = (string)s.GetValue("name");
                            string sensorTopic = (string)s.GetValue("topic");
                            string sensorType = (string)s.GetValue("type");
                            bool toStore = (bool)s.GetValue("toStore");
                            IoTSensor sensor = new IoTSensor(sensorName, sensorTopic, sensorType, toStore);
                            SharedVariables.sensorHandler.addSensor(sensor);
                        }
                    }
                }

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

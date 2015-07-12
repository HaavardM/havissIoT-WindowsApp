using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace HavissIoT
{
    class IoTSensor
    {
        private string name;
        private string topic;
        private string lastValue;
        private string type;
        private bool storage;
        private bool isActive;
        /*
        private Stopwatch timer;
        private int firstLog = -1;
        private List<string> values = new List<string>();
         * */

        //Constructor
        public IoTSensor(String name, String topic, String type, bool toStore)
        {
            //this.timer = new Stopwatch();
            //this.timer.Start();
            this.topic = topic;
            this.name = name;
            this.type = type;
            this.storage = toStore;
        }

        //Updates last value
        public void updateValue(string value)
        {
            this.lastValue = value;
            this.isActive = true;
        }

        //Get list of values
        /*public List<ChartEntry> getValues()
        {
            return this.values;
        }
        */
        //Change sensor topic
        public void changeTopic(String topic)
        {
            this.topic = topic;
        }

        //Change sensor name
        public void changeName(String name)
        {
            this.name = name;
        }

        //Set state of toStore
        public void setStorage(bool state)
        {
            this.storage = state;
        }

        //Get sensor name
        public String getName()
        {
            return this.name;
        }

        //Get sensor topic
        public String getTopic()
        {
            return this.topic;
        }

        //Get sensortype
        public String getType()
        {
            return this.type;
        }

        //Get latest value
        public string getLastValue()
        {
            return this.lastValue;
        }

        //Check if sensordata is to be stored
        public bool getStorage()
        {
            return this.storage;
        }


        //Checks if sensor is inactive
        /*public bool checkActive()
        {
            if ((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - this.lastUpdated > this.timeout)
            {
                this.isActive = false;
                return false;
            }
            else
            {
                return true;
            }
        }
         * */

        //Return isActive
        public bool getIsActive()
        {
            return this.isActive;
        }
    }
}

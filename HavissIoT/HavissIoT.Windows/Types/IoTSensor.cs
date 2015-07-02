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
        private int lastValue;
        private string type;
        private bool storage;
        private bool isActive;
        private long lastUpdated;
        private Stopwatch timer;
        private int firstLog = -1;
        private string dataType;
        private List<ChartEntry> values = new List<ChartEntry>();

        //Constructor
        public IoTSensor(String name, String topic, String type, bool toStore)
        {
            this.timer = new Stopwatch();
            this.timer.Start();
            this.topic = topic;
            this.name = name;
            this.type = type;
            this.storage = toStore;
            Random random = new Random();
        }

        //Updates last value
        public void updateValue(int value)
        {
            this.timer.Stop();
            this.values.Add(new ChartEntry() { Value = value, Time =  (int)(this.timer.ElapsedMilliseconds/1000) });
            this.timer.Start();
            this.lastValue = value;
            this.isActive = true;
        }

        //Get list of values
        public List<ChartEntry> getValues()
        {
            return this.values;
        }
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
        public int getLastValue()
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

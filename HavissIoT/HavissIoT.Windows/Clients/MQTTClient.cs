using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace HavissIoT
{
    class MQTTClient
    {
        private MqttClient mClient;
        private string brokerAddress;
        private int brokerPort;
        private string clientID;

        //Constructor - set variables
        public MQTTClient(string clientID, string address, int port)
        {
            this.clientID = clientID;
            this.brokerAddress = address;
            this.brokerPort = port;
            this.connect(brokerAddress, brokerPort);
            this.mClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }
        //Event - when messages is recieved
        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            IoTSensor sensor = SharedVariables.sensorHandler.getSensorByTopic(e.Topic);
            if (sensor != null)
            {
                sensor.updateValue(Encoding.UTF8.GetString(e.Message, 0, e.Message.Length));
            }
        } 
        //Connect to MQTT broker
        public void connect(string address, int port)
        {
            this.brokerAddress = address;
            this.brokerPort = port;
            this.mClient = new MqttClient(this.brokerAddress, this.brokerPort, false);
        }
        //Reconnect to broker
        public void reconnect(string address, int port)
        {
            if (mClient != null)
            {
                mClient.Disconnect();
                connect(address, port);
            }
        }
        //Disconnect from broker
        public void disconnect()
        {
            mClient.Disconnect();
        }
        //Subscribe to a MQTT topic
        public void subscribeToTopic(string topic)
        {
            switch (Config.mqttQOS)
            {
                case 0:
                    mClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                    break;
                case 1:
                    mClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                    break;
                case 2:
                    mClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    break;
            }
        }
        //Check if client is connected to broker
        public bool isConnected()
        {
            return this.mClient.IsConnected;
        }

    }
}

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
        private int qos;

        public MQTTClient(string clientID, string address, int port)
        {
            this.clientID = clientID;
            this.connect(address, port);
            this.mClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            IoTSensor sensor = SharedVariables.sensorHandler.getSensorByTopic(e.Topic);
            sensor.updateValue(int.Parse(Encoding.UTF8.GetString(e.Message, 0, e.Message.Length)));
        }

        

        public void connect(string address, int port)
        {
            this.brokerAddress = address;
            this.brokerPort = port;
            this.mClient = new MqttClient(this.brokerAddress, this.brokerPort, false);
        }

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

        public bool isConnected()
        {
            return this.mClient.IsConnected;
        }

    }
}

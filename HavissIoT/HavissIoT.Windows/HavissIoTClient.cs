using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft;
using System.Threading.Tasks;
using Windows.System.UserProfile;

namespace HavissIoT
{
    class HavissIoTClient
    {
        private StreamSocket socket;
        private HostName hostname;
        private int port;
        private DataWriter writer;
        private DataReader reader;
        private MQTTClient mClient;

        private volatile bool connected = false;

        //Connects to server
        public async Task connect(string address, int port)
        {
            Exception e = null;
            if (!this.connected)
            {
                try
                {
                    this.socket = new StreamSocket();
                    this.hostname = new HostName(address);
                    this.port = port;
                    await this.socket.ConnectAsync(this.hostname, port.ToString());
                    this.writer = new DataWriter(this.socket.OutputStream);
                    this.reader = new DataReader(this.socket.InputStream);
                    this.reader.InputStreamOptions = InputStreamOptions.Partial;
                    connected = true;
                }
                catch (Exception ex)
                {
                    connected = false;
                }
                if (!connected)
                {
                    MessageDialog message = new MessageDialog("Couldn't connect to havissIoT server, check if server address and port is correct", "Connection error");
                    await message.ShowAsync();
                    new Settings().Show();
                }
                else
                {
                    try
                    {
                        string clientID = await getClientID();
                        mClient = new MQTTClient(clientID, Config.brokerAddress, Config.brokerPort);
                    } 
                    catch(Exception ex) 
                    {
                        e = ex;
                        
                    }
                    if(e != null) {
                        await Config.requestConfig();
                        string clientID = await getClientID();
                        mClient = new MQTTClient(clientID, Config.brokerAddress, Config.brokerPort);
                    }
                }
            }
            else
            {
                await new MessageDialog("Already connected", "Information").ShowAsync();
                connected = true;
                
            }
        }

        private async Task<string> getClientID()
        {
            string systemUsername = await UserInformation.GetDisplayNameAsync();
            string userFirstName = await UserInformation.GetFirstNameAsync();
            string userLastName = await UserInformation.GetLastNameAsync();
            return (userFirstName + userLastName);
        }

        //Reconnects to server
        public async Task reconnect(string address, int port)
        {
            if (!this.connected)
            {
                await this.connect(address, port);
            }
            else
            {
                try
                {
                    await this.disconnect();
                    await this.connect(Config.serverAddress, Config.serverPort);
                }
                catch (Exception e)
                {
                    connected = false;
                }
            }
        }
        //Check if client is connected
        public bool isConnected()
        {
            return this.connected;
        }

        //Disconnect from server
        public async Task disconnect()
        {
            if (this.connected)
            {
                this.writer.WriteString("close" + Environment.NewLine);
                await this.writer.StoreAsync();
                await this.writer.FlushAsync();
                this.closeConnection();
                SharedVariables.mainPage.onClientDisconnect();
            }
            
        }

        public void closeConnection()
        {
            this.socket.Dispose();
            this.writer.Dispose();
            this.reader.Dispose();
            this.connected = false;
        }

        public async void write(string message)
        {
            Exception e = null;
            try
            {
                byte[] toWrite = Encoding.UTF8.GetBytes(message + Environment.NewLine);
                this.writer.WriteBytes(toWrite);
                await this.writer.StoreAsync();
                await this.writer.FlushAsync();
            }
            catch (Exception ex)
            {
                e = ex;
            }
            if (e != null)
            {
                MessageDialog errorMessage = new MessageDialog("Error writing to socket - " + e.Message, "ERROR");
                await errorMessage.ShowAsync();
                this.closeConnection();
            }
        }

        private async Task<string> getResponse()
        {
            Exception e = null;
            try
            {
                await this.reader.LoadAsync(8192);
                string message = this.reader.ReadString(reader.UnconsumedBufferLength);
                if (message.EndsWith("\n"))
                {
                    return message.Substring(0, message.Length - 1); //Remove line end
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                e = ex;
            }
            if (e != null)
            {
                MessageDialog errorMessage = new MessageDialog("Error reading from socket - " + e.Message, "ERROR");
                await errorMessage.ShowAsync();
                this.closeConnection();
            }
            return null;
        }

        //Make a request
        public async Task<string> request(string message)
        { 
            Exception e = null;
            try
            {
                this.write(message);
                string msg = await this.getResponse();

                if (msg != null && msg.Contains('\n'))
                {
                    if (msg.IndexOf('\n') == msg.Length)
                    {
                        msg.Remove(msg.IndexOf('\n'));
                    }
                }
                return msg;
            }
            catch (Exception ex)
            {
                e = ex;
            }
            if (e != null)
            {
                MessageDialog errorMessage = new MessageDialog("Error on socket - " + e.Message);
                await errorMessage.ShowAsync();
            }
            return null;
        }
    }
}

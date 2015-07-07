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
using System.Diagnostics;

namespace HavissIoT
{
    class HavissIoTClient
    {
        private StreamSocket socket;
        private HostName hostname;
        private int port;
        private DataWriter writer;
        private DataReader reader;
        private volatile bool connected = false;

        //Connects to server
        public async Task connect(string address, int port)
        {
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
                catch (Exception e)
                {
                    connected = false;
                    Debug.WriteLine(e.Message);
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
            try
            {
                if (!this.connected)
                {
                    await this.connect(address, port);
                }
                else
                {
                    await this.disconnect();
                    await this.connect(Config.serverAddress, Config.serverPort);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
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
            try
            {
                byte[] toWrite = Encoding.UTF8.GetBytes(message + Environment.NewLine);
                this.writer.WriteBytes(toWrite);
                await this.writer.StoreAsync();
                await this.writer.FlushAsync();
            }
            catch (Exception ex)
            {
                //TODO Handle exception
                Debug.WriteLine(ex.Message);
            }
        }

        //Load response from server
        private async Task<string> getResponse()
        {
            try
            {
                await this.reader.LoadAsync(8192);
                string message = this.reader.ReadString(reader.UnconsumedBufferLength);
                if (message.EndsWith("\n"))
                {
                    //Return message revcieved from server - without newline character
                    return message.Substring(0, message.Length - 1); //Remove line end
                }
                else
                {
                    //Return null if message doesent end with newline - not properly formatted
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        //Make a request
        public async Task<string> request(string message)
        { 
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
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }
}

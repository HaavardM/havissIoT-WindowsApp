using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Data;
using Windows.Storage;
using System.Threading.Tasks;
// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace HavissIoT
{
    public sealed partial class Settings : SettingsFlyout
    {
        private bool userButtonPressed = false;
        public Settings()
        {
            this.InitializeComponent();
            this.server_address.Text = Config.serverAddress;
            this.server_port.Text = Config.serverPort.ToString();
            this.broker_address.Text = Config.brokerAddress;
            this.broker_port.Text = Config.brokerPort.ToString();
            this.username.Text = Config.username;
            this.password.Password = Config.username;
            this.mqtt_qos.ItemsSource = new List<int>() { 0, 1, 2 };
            this.mqtt_qos.SelectedIndex = this.mqtt_qos.Items.IndexOf(Config.mqttQOS);
            users_button.IsEnabled = SharedVariables.client.isConnected();
            disconnect_button.IsEnabled = SharedVariables.client.isConnected();
        }

        private void broker_manual_settings_Toggled(object sender, RoutedEventArgs e)
        {
            broker_address.IsEnabled = broker_manual_settings.IsOn;
            broker_port.IsEnabled = broker_manual_settings.IsOn;
            mqtt_qos.IsEnabled = broker_manual_settings.IsOn;
        }

        private async void SettingsFlyout_Unloaded(object sender, RoutedEventArgs e)
        {
            Config.serverAddress = this.server_address.Text;
            Config.serverPort = int.Parse(this.server_port.Text);
            Config.username = this.username.Text;
            Config.password = this.password.Password;
            //Config.password = this.password
            if (broker_manual_settings.IsOn)
            {
                Config.brokerAddress = this.broker_address.Text;
                Config.brokerPort = int.Parse(this.broker_port.Text);
                Config.mqttQOS = (int) this.mqtt_qos.SelectedItem;
            }
            Config.saveSettings();
            if (!userButtonPressed)
            {
                await toReconnect();
            }
        }

        private async Task toReconnect()
        {
            if (SharedVariables.client.isConnected())
            {
                MessageDialog message = new MessageDialog("Reconnect - Reconnects to server, Disconnect - Disconnects from server and close application, Keep connection - Keep connection alive, Settings - go back to settings", "Reconnect to server?");
                message.Commands.Add(new UICommand("Reconnect", new UICommandInvokedHandler(toReconnectHandler)));
                message.Commands.Add(new UICommand("Keep connection", new UICommandInvokedHandler(toReconnectHandler)));
                message.Commands.Add(new UICommand("Settings", new UICommandInvokedHandler(toReconnectHandler)));
                await message.ShowAsync();
            }
            else
            {
                MessageDialog message = new MessageDialog("Connect - Connects to server, No - Close application, Settings - Change application settings", "To connect?");
                message.Commands.Add(new UICommand("Connect", new UICommandInvokedHandler(toReconnectHandler)));
                message.Commands.Add(new UICommand("No", new UICommandInvokedHandler(toReconnectHandler)));
                message.Commands.Add(new UICommand("Settings", new UICommandInvokedHandler(toReconnectHandler)));
                await message.ShowAsync();
            }
        }

        private async void toReconnectHandler(IUICommand command)
        {
            switch (command.Label)
            {
                case "Reconnect":
                    await SharedVariables.client.reconnect(Config.serverAddress, Config.serverPort);
                    SharedVariables.mainPage.refreshSensors();
                    break;
                case "Keep connection":
                    //Do nothing
                    break;
                case "Connect":
                    await SharedVariables.client.connect(Config.serverAddress, Config.serverPort);
                    if (SharedVariables.client.isConnected())
                    {
                        SharedVariables.mainPage.refreshSensors();
                    }
                    
                    break;
                case "No":
                    Application.Current.Exit();
                    break;
                case "Settings":
                    this.Show();
                    break;
                    
            }
        }

        private void exit_button_Click(object sender, RoutedEventArgs e)
        {
            HavissIoTCommandBuilder commandBuilder = new HavissIoTCommandBuilder();
            if (Config.password.Length > 0)
            {
                commandBuilder.addUser(Config.username, Config.password);
            }
            else
            {
                commandBuilder.addUser(Config.username);
            }
            commandBuilder.serverExit();
            SharedVariables.client.write(commandBuilder.getJsonString());
        }

        private void users_button_Click(object sender, RoutedEventArgs e)
        {
            this.userButtonPressed = true;
            UserFlyout users = new UserFlyout();
            users.Show();
        }

        private async void disconnect_button_Click(object sender, RoutedEventArgs e)
        {
            await SharedVariables.client.disconnect();
            if (!SharedVariables.client.isConnected())
            {
                this.disconnect_button.IsEnabled = false;
            }
        }

        
    }
}

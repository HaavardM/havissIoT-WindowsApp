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
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI;
using Newtonsoft.Json.Linq;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HavissIoT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    
    public sealed partial class MainPage : Page
    {
        private List<string> sensorNames = new List<string>();
        private List<int> chartSource = new List<int>();
        private IoTSensor currentSensor = null;

        public MainPage()
        {
            this.InitializeComponent();
            SharedVariables.client = new HavissIoTClient();
            SharedVariables.userHandler = new UserHandler();
            SharedVariables.sensorHandler = new SensorHandler();
            SharedVariables.mainPage = this;
            firstConnect();
        }
        private async void firstConnect()
        {
            await toConnect();
        }

        

        private async Task toConnect()
        {
            MessageDialog message = new MessageDialog("Connect - Connects to server, No - Closes application, Settings - Change application settings", "To connect?");
            message.Commands.Add(new UICommand("Connect", new UICommandInvokedHandler(toConnectResponse)));
            message.Commands.Add(new UICommand("No", new UICommandInvokedHandler(toConnectResponse)));
            message.Commands.Add(new UICommand("Settings", new UICommandInvokedHandler(toConnectResponse)));
            await message.ShowAsync();

        }

        private async void toConnectResponse(IUICommand command)
        {
            switch (command.Label)
            {
                case "Connect":
                    await SharedVariables.client.connect(Config.serverAddress, Config.serverPort);
                    if (SharedVariables.client.isConnected())
                    {
                        connection_status.Text = "Connected";
                        refreshSensors();
                    }
                    break;
                case "No":
                    Application.Current.Exit();
                    break;
                case "Settings":
                    new Settings().Show();
                    break;
            }
        }

        //Refresh sensors when button is pressed
        private void refresh_button_Click(object sender, RoutedEventArgs e)
        {
            if (SharedVariables.client.isConnected())
            {
                refreshSensors();
            }
        }

        //Request a list of all sensors and refresh local list
        public async void refreshSensors()
        {
            await SharedVariables.sensorHandler.refreshSensors();
            foreach (IoTSensor s in SharedVariables.sensorHandler.getSensors())
            {
                if (!sensorNames.Contains(s.getName()))
                {
                    sensorNames.Add(s.getName());
                }
            }

            foreach (String s in sensorNames)
            {
                bool remove = true;
                foreach (IoTSensor i in SharedVariables.sensorHandler.getSensors())
                {
                    if (s.CompareTo(i.getName()) == 0)
                    {
                        remove = false;
                        break;
                    }
                }
                if (remove)
                {
                    sensorNames.Remove(s);
                }

            }
            sensor_select.ItemsSource = sensorNames;
        }

        //When a new sensor is selected
        private void sensor_select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.sensor_select.Items.Count > 0)
            {
                string sensorName = sensorNames[sensor_select.SelectedIndex];
                this.currentSensor = SharedVariables.sensorHandler.getSensorByName(sensorName);
                //(values_chart.Series[0] as LineSeries).ItemsSource = this.currentSensor.getValues();
                this.sensor_name.Text = this.currentSensor.getName();
            }
        }

        //Method to run when the client is disconnecting from server - some UI elements must change
        public void onClientDisconnect()
        {
            this.connection_status.Text = "Not connected";
            this.sensorNames.Clear();
            this.sensor_select.Items.Clear();
            (values_chart.Series[0] as LineSeries).ItemsSource = new List<ChartEntry>();


        }

        //Open new page on button press
        private void add_remove_button_Click(object sender, RoutedEventArgs e)
        {
            addRemoveSensors addRemove = new addRemoveSensors();
            this.Frame.Navigate(typeof(addRemoveSensors), null);

        }

        //Show a message dialog with a message and a title. 
        public async void showMessageDialog(string message, string title)
        {
           await new MessageDialog(message, title).ShowAsync();
        }
     
    }
}

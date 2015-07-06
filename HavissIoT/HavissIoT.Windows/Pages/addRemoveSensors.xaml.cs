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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HavissIoT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class addRemoveSensors : Page
    {

        private List<string> sensorNames;

        public addRemoveSensors()
        {
            this.InitializeComponent();
            this.sensorNames = new List<string>();
            this.getSensorNames();
            this.sensor_select.ItemsSource = this.sensorNames;
        }

        //Get sensor names from sensorhandler
        private void getSensorNames()
        {
            sensorNames.Clear();
            foreach (IoTSensor s in SharedVariables.sensorHandler.getSensors())
            {
                sensorNames.Add(s.getName());
            }
        }
    }
}

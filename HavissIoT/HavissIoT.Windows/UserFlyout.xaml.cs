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
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.UI.Popups;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace HavissIoT
{
    public sealed partial class UserFlyout : SettingsFlyout
    {
        public UserFlyout()
        {
            this.InitializeComponent();
            available_users.IsEnabled = SharedVariables.client.isConnected();
            if (SharedVariables.client.isConnected())
            {
                refreshUsers();

            }
        }

        private async void refreshUsers() {
            HavissIoTCommandBuilder commandBuilder =new HavissIoTCommandBuilder();
            commandBuilder.getUsers();
            if (Config.username.Length > 0)
            {
                if (Config.password.Length > 0)
                {
                    commandBuilder.addUser(Config.username, Config.password);
                }
                else
                {
                    commandBuilder.addUser(Config.username);
                }
            }
            string response = await SharedVariables.client.request(commandBuilder.getJsonString());
            JObject jsonObject = null;
            JArray jsonArray = null;
            try
            {
                jsonObject = JObject.Parse(response);
                jsonArray = (JArray)jsonObject.GetValue("r");
            }
            catch (Exception ex)
            {
                MessageDialog message = new MessageDialog(ex.Message, "Request parse error");
                return;
            }
            List<string> usernames = new List<string>();
            SharedVariables.userHandler.clearUsers();
            foreach (JObject j in jsonArray)
            {
                try
                {
                    string username = (string)j.GetValue("name");
                    bool OP = (bool)j.GetValue("isOP");
                    bool isProtected = (bool)j.GetValue("isProtected");
                    bool isPasswordProtected = (bool)j.GetValue("isPasswordProtected");
                    User user = new User(username, OP, isProtected, isPasswordProtected);
                    SharedVariables.userHandler.addUser(user);
                    usernames.Add(username);
                }
                catch (Exception ex)
                {
                    MessageDialog message = new MessageDialog(ex.Message, "Request parse error");
                    return;
                }
            }
            available_users.ItemsSource = usernames;
        }

        private async void SettingsFlyout_Unloaded(object sender, RoutedEventArgs e)
        {
            if (available_users.SelectedIndex != -1)
            {
                Config.username = available_users.Items[available_users.SelectedIndex].ToString();
                Config.password = "";
                Config.saveSettings();
                User user = SharedVariables.userHandler.getUser(Config.username);
                if (user != null && user.isPasswordProtected())
                {
                    MessageDialog message = new MessageDialog("User need password authentication, enter password in settings section", "Password needed");
                    await message.ShowAsync();
                }
                
            }
            Settings settings = new Settings();
            settings.Show();
        }
    }
}

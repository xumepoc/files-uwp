﻿using Files.DataModels;
using Files.View_Models;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Files.SettingsPages
{
    public sealed partial class Preferences : Page
    {
        public SettingsViewModel AppSettings => App.AppSettings;
        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public Preferences()
        {
            this.InitializeComponent();

            StorageFolder.GetFolderFromPathAsync(App.AppSettings.OneDrivePath).AsTask()
                    .ContinueWith((t) =>
                {
                    App.AppSettings.PinOneDriveToSideBar = false;
                    OneDrivePin.IsEnabled = false;
                }, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TerminalApplicationsComboBox.SelectedItem = App.AppSettings.TerminalsModel.GetDefaultTerminal();
        }

        private void EditTerminalApplications_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LaunchTerminalsConfigFile();
        }

        private async void LaunchTerminalsConfigFile()
        {
            await Launcher.LaunchFileAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/settings/terminal.json")));
        }

        private void TerminalApp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;

            var selectedTerminal = (TerminalModel)comboBox.SelectedItem;

            App.AppSettings.TerminalsModel.DefaultTerminalId = selectedTerminal.Id;

            SaveTerminalSettings();
        }

        private async void SaveTerminalSettings()
        {
            await FileIO.WriteTextAsync(App.AppSettings.TerminalsModelFile,
                JsonConvert.SerializeObject(App.AppSettings.TerminalsModel, Formatting.Indented));
        }
    }
}
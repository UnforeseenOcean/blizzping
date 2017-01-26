using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BlizzPing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        string org_title;
        List<int> MS_Values = new List<int>();

        ObservableCollection<PingData> pingDataCollection = new ObservableCollection<PingData>();
        Dictionary<string, Dictionary<string, List<string>>> all_Games = new Dictionary<string, Dictionary<string, List<string>>>();

        public MainWindow()
        {
            InitializeComponent();
            org_title = Title;
            pingData.ItemsSource = pingDataCollection;

            dynamic jsonObj = JsonConvert.DeserializeObject(File.ReadAllText("ips.json"));

            foreach (var obj in jsonObj)
            {
                Console.WriteLine(obj.name);

                var ips = obj.ips.ToObject<Dictionary<string, List<string>>>();
                all_Games.Add((string)obj.name, new Dictionary<string, List<string>>());
                AllGames.Items.Add(new ComboBoxItem()
                {
                    Tag = obj.name,
                    Content = obj.name
                });


                if (obj.has_realms != null && !(bool)obj.has_realms)
                {
                    foreach (var region in ips)
                    {
                        all_Games[(string)obj.name][region.Key] = region.Value;
                    }
                }
            }
        }

        private void AllGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string str = ((ComboBoxItem)AllGames.SelectedItem).Tag.ToString();

            var ips_regions = all_Games[str];

            foreach (var gameIP in ips_regions)
            {
                for (var i = 0; i < gameIP.Value.Count; i++)
                {
                    SelectRegion.Items.Add(new ComboBoxItem()
                    {
                        Content = $"{gameIP.Key} ( {gameIP.Value[i]} )",
                        Tag = gameIP.Value[i]
                    });
                }
            }
        }

        public void PingServer(string ip, string region)
        {
            MS_Values.Clear();
            Ping pingSender = new Ping();
            IPAddress address = IPAddress.Parse(ip);

            Dispatcher.Invoke(() =>
            {
                pingDataCollection.Insert(0, new PingData()
                {
                    ServerRegion = region,
                    DataOut = $"Average Travel Time: N/A MS",
                    BackGroundBrush = new SolidColorBrush()
                    {
                        Color = (Color)ColorConverter.ConvertFromString("#FFBBDEFB")
                    }
                });
            });

            for (var i = 0; i < 10; i++)
            {
                PingData ping = new PingData() { ServerRegion = region };
                Dispatcher.Invoke(() =>
                {
                   ping.BackGroundBrush = new SolidColorBrush()
                   {
                       Color = (Color)ColorConverter.ConvertFromString("#FFFFFFFF")
                   };
                    ping.DataOut = "Checking ping please wait...";
                    pingDataCollection.Add(ping);
                });

                PingReply reply = pingSender.Send(address);
                if (reply.Status == IPStatus.Success)
                {
                    MS_Values.Add((int)reply.RoundtripTime);

                    Dispatcher.Invoke(() => {
                        pingDataCollection[pingDataCollection.Count - 1].DataOut = $"Travel time {reply.RoundtripTime} MS";

                        if(reply.RoundtripTime > 250)
                        {
                            pingDataCollection[pingDataCollection.Count - 1].BackGroundBrush = new SolidColorBrush()
                            {
                                Color = (Color)ColorConverter.ConvertFromString("#FFFFF59D")
                            };
                        }
                    });
                }
                else
                {
                    string message = "Error time out reached while trying to connect";
                    Dispatcher.Invoke(() => {
                        pingDataCollection[pingDataCollection.Count - 1].DataOut = message;
                        pingDataCollection[pingDataCollection.Count - 1].BackGroundBrush = new SolidColorBrush()
                        {
                            Color = (Color)ColorConverter.ConvertFromString("#FFEF9A9A")
                        };
                    });
                }
            }

            Dispatcher.Invoke(() =>
            {
                pingDataCollection[0].DataOut = $"Average Travel Time: {MS_Values.Average().ToString("N0")} MS";
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            Title = $"{org_title} - {((ComboBoxItem)AllGames.SelectedItem).Tag.ToString()} - Running";

            WaitingBox.Visibility = Visibility.Collapsed;
            pingData.Visibility = Visibility.Visible;
            pingDataCollection.Clear();

            if (((ComboBoxItem)SelectRegion.SelectedItem) != null)
            {
                string ip = ((ComboBoxItem)SelectRegion.SelectedItem).Tag.ToString();
                string region = ((ComboBoxItem)SelectRegion.SelectedItem).Content.ToString();

                await Task.Run(() => PingServer(ip, region));
            }

            ((Button)sender).IsEnabled = true;
            Title = $"{org_title} - {((ComboBoxItem)AllGames.SelectedItem).Tag.ToString()} - Finished";
        }
    }
}

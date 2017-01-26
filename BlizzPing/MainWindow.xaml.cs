using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlizzPing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, Dictionary<string, List<string>>> all_Games = new Dictionary<string, Dictionary<string, List<string>>>();

        public MainWindow()
        {
            InitializeComponent();

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


                foreach (var region in ips)
                {
                    all_Games[(string)obj.name][region.Key] = region.Value;
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

        private async void SelectRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string str = ((ComboBoxItem)SelectRegion.SelectedItem).Tag.ToString();

            await Task.Run(() => PingServer(str));
        }

        public PingReply PingServer(string ip)
        {
            // Ping's the local machine.
            Ping pingSender = new Ping();
            IPAddress address = IPAddress.Parse(ip);
            PingReply reply = pingSender.Send(address);

            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("Address: {0}", reply.Address.ToString());
                Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
            }
            else
            {
                Console.WriteLine(reply.Status);
            }

            return reply;
        }

    }
}

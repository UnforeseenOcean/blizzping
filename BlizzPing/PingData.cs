using System.ComponentModel;
using System.Windows.Media;

namespace BlizzPing
{
    public class PingData : INotifyPropertyChanged
    {
        private string _serverRegion;
        public string ServerRegion {
            get
            {
                return _serverRegion;
            }
            set
            {
                _serverRegion = value;
                NotifyPropertyChanged("ServerRegion");
            }
        }

        private string _dataOut;
        public string DataOut
        {
            get
            {
                return _dataOut;
            }
            set
            {
                _dataOut = value;
                NotifyPropertyChanged("DataOut");
            }
        }
        public SolidColorBrush BackGroundBrush { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}

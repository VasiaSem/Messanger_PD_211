using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MessangerClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPEndPoint serverPoint;
        //const string serverAddress = "127.0.0.1";
        //const short serverPort = 4040;
        UdpClient client ;
        ObservableCollection<MessageInfo> messages ;   
        public MainWindow()
        {
            InitializeComponent();
            client = new UdpClient();
            string address = ConfigurationManager.AppSettings["ServerAddress"]!;
            short port = short.Parse(ConfigurationManager.AppSettings["ServerPort"]!);
            serverPoint = new IPEndPoint(IPAddress.Parse(address), port);
            messages = new ObservableCollection<MessageInfo>();
            this.DataContext = messages;
            
        }
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {           
            string message = msgTextBox.Text;
            SendMessage(message);
        }
        private async void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "$<join>";
            SendMessage(message);
            Listen();
        }
        private async void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(data, serverPoint);
        }
        private async void Listen()
        {
            while (true)
            {
                //IPEndPoint remoreIpServer = null;
                var result = await client.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                messages.Add(new MessageInfo(message));
            }
           
        }
    }
    public class MessageInfo
    {
        public string Text { get; set; }
        public DateTime Time { get; set; }

        public MessageInfo(string text)
        {
            this.Text = text;   
            Time = DateTime.Now;                
        }
        public override string ToString()
        {
            return $"{Text}  {Time.ToShortDateString()}";
        }

    }
}
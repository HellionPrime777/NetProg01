using Client;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;

namespace Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
  public partial class MainWindow : Window
  {
    private Service service;
    private string login;
    private string password; // Add password field
    private DispatcherTimer timer;

    public MainWindow()
    {
        InitializeComponent();
        service = new Service("127.0.0.1", 5000);

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password)) // Check if login and password are set
        {
            int messageCount = service.GetMessageCount(login, password);

            if (messageCount > 0)
            {
                List<Message> messages = service.GetMessages(login, password);
                messagesListView.ItemsSource = messages;
            }
        }
    }

    private void GetMessage_Click(object sender, RoutedEventArgs e)
    {
        login = loginTextBox.Text;
        password = passwordTextBox.Text; // Store the password entered
        int messageCount = service.GetMessageCount(login, password);
        MessageBox.Show($"Message count: {messageCount}");

        if (messageCount > 0)
        {
            List<Message> messages = service.GetMessages(login, password);
            messagesListView.ItemsSource = messages;
        }

        timer.Start();
    }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string toLogin = toLoginTextBox.Text;
            string text = messageTextBox.Text;

            var message = new Message
            {
                From = new CommonLibrary.Client { Login = login },
                To = new CommonLibrary.Client { Login = toLogin },
                Text = text,
                CreatedAt = DateTime.Now,
            };

            bool success = service.SendMessage(message);

            if (success)
            {
                MessageBox.Show("Message sent successfully");
            }
            else
            {
                MessageBox.Show("Failed to send message");
            }
        }
   }
}

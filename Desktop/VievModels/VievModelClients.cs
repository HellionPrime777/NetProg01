using DesktopClient.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonLibrary;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Desktop.ViewModels
{
    public class ViewModelClient
    {
        private MainWindow _clientWindow;
        private Service _service;
        public string Login { get; set; }
        public string Password { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string Info { get; set; }
        public string ForMessages { get; set; }
        public ViewModelClient(MainWindow window)
        {
            _clientWindow = window;
            _service = new Service("127.0.0.1", 5000); ;
        }

        private BaceCommand _LoginCommand;
        public BaceCommand LoginCommand
        {
            get
            {

                return _LoginCommand ?? new BaceCommand(obj =>
                {
                    Window wnd = obj as Window;
                    //var temp = _userService.Find(new UserDTO() { Login = this.Login, Password = this.Password });
                    //_clientWindow.tbInfo.Text += 555;
                    var test = _service.GetMessageCount(Login, Password);
                    if (test > 0)
                    {
                        _clientWindow.tbForMessage.Text += "Message count: " + test + "\n";
                        //Console.WriteLine("Message count: {0}", messageCount);
                        _service.GetMessages(Login).ForEach(m =>
                        {
                            _clientWindow.tbForMessage.Text += $"From:  {m.From.Login}\t\t";
                            _clientWindow.tbForMessage.Text += $"To: {m.To.Login}\n";
                            _clientWindow.tbForMessage.Text += m.CreatedAt.ToString("G") + "\n";
                            _clientWindow.tbForMessage.Text += (m.Text) + "\n\n";

                        });
                    }
                    if (test != -1)
                    {
                        _clientWindow.btnLogin.IsEnabled = false;
                        _clientWindow.btnSend.IsEnabled = true;
                    }
                    else
                    {
                        _clientWindow.tbInfo.Text = "Error password";
                        MessageBox.Show("Error password");
                        ClearFields();
                    }

                });
            }
        }

        private BaceCommand _SendCommand;
        public BaceCommand SendCommand
        {
            get
            {

                return _SendCommand ?? new BaceCommand(obj =>
                {
                    var message = new Message
                    {
                        From = new CommonLibrary.Client { Login = Login },
                        To = new CommonLibrary.Client { Login = To },
                        Text = Message,
                        CreatedAt = DateTime.Now,
                    };
                    if (_service.SendMessage(message))
                    {
                        MessageBox.Show("Send succes");
                    }
                    _clientWindow.btnLogin.IsEnabled = true;
                    _clientWindow.btnSend.IsEnabled = false;
                    ClearFields();
                });
            }
        }
        private void ClearFields()
        {
            _clientWindow.tbForMessage.Text = "";
            _clientWindow.tbMessage.Text = "";
            _clientWindow.tbLogin.Text = "";
            _clientWindow.tbPassword.Text = "";
            _clientWindow.tbInfo.Text = "";
            _clientWindow.tbTo.Text = "";
        }
    }
}


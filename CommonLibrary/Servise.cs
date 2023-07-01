using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary;
using System.Text.Json;
using CommonLibrary.Requests;
using CommonLibrary.Responses;

namespace CommonLibrary;

public class Service
{
    public Service(string host, int port)
    {
        _ep = new IPEndPoint(IPAddress.Parse(host), 5000);
    }

    private IPEndPoint _ep;

    private Data Send(Data request)
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        s.Connect(_ep);
        s.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)));

        var buffer = new byte[4096];
        int read = s.Receive(buffer);

        string incoming = Encoding.UTF8.GetString(buffer, 0, read);
        return JsonSerializer.Deserialize<Data>(incoming);
    }

    private T ToResponse<T>(Data response) where T : class
    {
        return
            JsonSerializer.Deserialize<T>(response.Content);
    }

    public int GetMessageCount(string login, string password)
    {
        var response = ToResponse<LoginResponse>(
            Send(Data.Create(new LoginRequest
            {
                Me = new CommonLibrary.Client
                {
                    Login = login,
                    Password = password
                }
            })));
        if (response.Success == true)
        {
            return response.MessagesCount;
        }
        else
        {
            return -1;
        }

    }

    public List<Message> GetMessages(string login)
    {
        var response = ToResponse<GetMessagesResponse>(
            Send(Data.Create(new GetMessagesRequest
            {
                Me = new CommonLibrary.Client
                {
                    Login = login
                }
            })));
        return response.Messages;
    }

    public bool SendMessage(Message message)
    {
        var response = ToResponse<SendMessageResponse>(
            Send(Data.Create(new SendMessageRequest
            {
                Message = message
            })));
        return response.Success;
    }



}

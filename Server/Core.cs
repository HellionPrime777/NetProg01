using CommonLibrary;
using CommonLibrary.Requests;
using CommonLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Server;

public class Core
{
    public Core()
    {

    }

    private List<Message> _messages = new List<Message>();
    private List<Client> _clients = new List<Client>();

    private T Deserialize<T>(Data data) where T : class
    {
        return JsonSerializer.Deserialize<T>(data.Content);
    }

    private Client FindClient(LoginRequest request)
    {

        foreach (var client in _clients)
        {
            if (client.Login == request.Me.Login)
            {
                if (client.Password == request.Me.Password)
                {
                    return client;
                }
                else
                {
                    return null;
                }
            }
        }
        _clients.Add(new Client() { Login = request.Me.Login, Password = request.Me.Password });
        return _clients[_clients.Count - 1];
    }

    private Data HandleLoginRequest(Data data)
    {
        var request = Deserialize<LoginRequest>(data);
        if (FindClient(request) != null)
        {
            return Data.Create(new LoginResponse
            {
                Success = true,
                MessagesCount = _messages.Where(x => x.To.Login == request.Me.Login).Count(),
            });
        }
        else
        {
            return Data.Create(new LoginResponse
            {
                Success = false,
            });
        }

    }

    private Data HandleSendMessageRequest(Data data)
    {
        var request = Deserialize<SendMessageRequest>(data);

        _messages.Add(request.Message);
        return Data.Create(new SendMessageResponse { Success = true });
    }

    private Data HandleGetMessagesRequest(Data data)
    {
        var request = Deserialize<GetMessagesRequest>(data);

        var clientMessages = _messages.Where(x => x.To.Login == request.Me.Login).ToList();
        _messages.RemoveAll(x => x.To.Login == request.Me.Login);

        return Data.Create(new GetMessagesResponse
        {
            Messages = clientMessages
        });
    }

    public Data Handle(Data request)
    {
        switch (request.Type)
        {
            case DataType.LoginRequest:
                return HandleLoginRequest(request);
            case DataType.SendMessageRequest:
                return HandleSendMessageRequest(request);
            case DataType.GetMessageRequest:
                return HandleGetMessagesRequest(request);
            default:
                return Data.Create(new ErrorResponse
                {
                    Error = "Unknown request"
                });
        }
    }
}

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

    private T Deserialize<T>(Data data) where T : class
    {
        return JsonSerializer.Deserialize<T>(data.Content);
    }

    private Dictionary<string, string> loginCredentials = new Dictionary<string, string>();

    private Data HandleLoginRequest(Data data)
    {
        var request = Deserialize<LoginRequest>(data);

        if (!loginCredentials.ContainsKey(request.Me.Login))
        {
            loginCredentials[request.Me.Login] = request.Me.Password;
        }
        else
        {
            if (loginCredentials[request.Me.Login] != request.Me.Password)
            {
                return Data.Create(new LoginResponse
                {
                    Success = false,
                    MessagesCount = 0
                });
            }
        }

        return Data.Create(new LoginResponse
        {
            Success = true,
            MessagesCount = _messages.Count(x => x.To.Login == request.Me.Login)
        });
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

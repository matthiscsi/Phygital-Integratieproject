using Microsoft.AspNetCore.SignalR;

namespace UI.MVC.Hub.voorbeeld;

public sealed class Chathub: Hub<IChatClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.ReceiveMessage($"{Context.ConnectionId} joined the chat");
    }
    public async Task SendMessage(string message)
    {
        await Clients.All.ReceiveMessage( $"{Context.ConnectionId}, {message})");
    }
}
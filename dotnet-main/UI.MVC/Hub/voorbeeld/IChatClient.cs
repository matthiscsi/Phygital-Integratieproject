namespace UI.MVC.Hub.voorbeeld;

public interface IChatClient
{
    Task ReceiveMessage(string message);
}
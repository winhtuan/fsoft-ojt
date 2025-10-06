namespace Plantpedia.Service;

using Microsoft.AspNetCore.SignalR;

public class MyNotificationHub : Hub
{
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}

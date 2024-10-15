using Ahmed_mart.Dtos.v1.NotificationDtos;
using Microsoft.AspNetCore.SignalR;

namespace Ahmed_mart.Hubs.v1
{
    public class NotificationHub : Hub
    {
        [HubMethodName("sendMessage")]
        public void SendMessage(NotificationDto notificationDto)
        {
            Clients.All.SendAsync("receiveMessage", notificationDto);
        }

        [HubMethodName("refreshMessaging")]
        public void RefreshMessaging()
        {
            Clients.All.SendAsync("receiveMessaging");
        }
    }
}

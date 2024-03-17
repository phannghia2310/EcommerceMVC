using Microsoft.AspNetCore.SignalR;

namespace EcommerceMVC.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            var group = FindGroupForUser(user);
            await Clients.Group(group).SendAsync("ReceiveMessage", user, message);
        }

        private string FindGroupForUser(string user)
        {
            // Thực hiện logic tìm kiếm nhóm tại đây
            // Ví dụ: trả về tên nhóm dựa trên tên người dùng
            return "groupFor" + user;
        }
    }
}

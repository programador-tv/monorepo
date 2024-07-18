// using Domain.Models;
// using Domain.Redis;
// using Infrastructure.Redis;
// using Microsoft.AspNetCore.SignalR;

// namespace Presentation.Hubs;

// public sealed class ChatHub(ChatRedisRepository repository) : Hub
// {
//     private static Dictionary<Guid, int> ConnectedIds = new();

//     public async Task SendMessage(CreateChatMessageRequest message)
//     {
//         await repository.SaveMessageAsync(message);
//         await Clients.Group(message.LiveId.ToString()).SendAsync("ReceiveMessage", message);
//     }

//     public async Task DeleteMessage(string messageId, Guid liveId)
//     {
//         await repository.DeleteMessageAsync(messageId, liveId);
//         await Clients.Group(liveId.ToString()).SendAsync("RemoveMessage", messageId);
//     }

//     public override async Task OnConnectedAsync()
//     {
//         var liveIdStr = Context.GetHttpContext().Request.Query["liveId"].ToString();
//         var liveId = Guid.Parse(liveIdStr);
//         await Groups.AddToGroupAsync(Context.ConnectionId, liveId.ToString());

//         // Enviar mensagens anteriores para o cliente que acabou de se conectar
//         var messages = await repository.GetMessageAsync(liveId);
//         foreach (var message in messages)
//         {
//             await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", message);
//         }

//         if (!ConnectedIds.ContainsKey(liveId))
//         {
//             ConnectedIds[liveId] = ConnectedIds[liveId]++;
//         }

//         await Clients
//             .Group(liveId.ToString())
//             .SendAsync("UpdateLiveUserCount", ConnectedIds[liveId]);

//         await base.OnConnectedAsync();
//     }

//     public override async Task OnDisconnectedAsync(Exception exception)
//     {
//         var liveIdStr = Context.GetHttpContext().Request.Query["liveId"].ToString();
//         var liveId = Guid.Parse(liveIdStr);
//         await Groups.RemoveFromGroupAsync(Context.ConnectionId, liveId.ToString());

//         ConnectedIds[liveId] = ConnectedIds[liveId]--;
//         await Clients
//             .Group(liveId.ToString())
//             .SendAsync("UpdateLiveUserCount", ConnectedIds[liveId]);
//         await base.OnDisconnectedAsync(exception);
//     }
// }

// using System.Text.Json;
// using Domain.Models;
// using Domain.Redis;
// using Microsoft.AspNetCore.Mvc;
// using StackExchange.Redis;

// namespace Infrastructure.Redis;

// public sealed class ChatRedisRepository(IRedisContext _redis) : IChatRedisRepository
// {
//     public async Task DeleteMessageAsync(string messageId, Guid liveId)
//     {
//         var key = $"messages:{liveId}";
//         var messages = await GetMessageAsync(liveId);
//         var messageToRemove = messages.Find(message => message.Id.ToString() == messageId);
//         if (messageToRemove != null)
//         {
//             messages.Remove(messageToRemove);
//             var jsonMessages = JsonSerializer.Serialize(messages);
//             await _redis.SetValueAsync(key, jsonMessages);
//         }
//     }

//     public async Task<List<CreateChatMessageRequest>> GetMessageAsync(Guid liveId)
//     {
//         var key = $"messages:{liveId}";
//         var result = await _redis.GetValueAsync(key);
//         if (string.IsNullOrEmpty(result))
//         {
//             return new();
//         }
//         var messages = JsonSerializer.Deserialize<List<CreateChatMessageRequest>>(result);
//         if (messages == null)
//         {
//             return new();
//         }
//         return messages;
//     }

//     public async Task SaveMessageAsync(CreateChatMessageRequest message)
//     {
//         var key = $"messages:{message.LiveId}";
//         await _redis.SaveMessageAsync(key, message);
//     }
// }

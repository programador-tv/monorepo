// using Domain.Contracts;
// using Domain.Entities;
// using Domain.Enumerables;
// using Domain.Repositories;
// using Queue;

// namespace Application.Logic;

// public sealed class ChatBusinessLogic(IChatRepository _repository, IMessagePublisher _publisher)
//     : IChatBusinessLogic
// {
//     public async Task SaveAsync(ChatMessage chatMessage)
//     {
//         using var client = new HttpClient();
//         var response = await client.GetAsync(
//             $"https://bs.programador.tv/api/v1/validate?id={chatMessage.PerfilId}&text={chatMessage.Content}"
//         );

//         var result = await response.Content.ReadAsStringAsync();

//         if (result.Contains("503 Service Temporarily Unavailable") || result == "true")
//         {
//             chatMessage.Validate();
//             try
//             {
//                 await _publisher.PublishAsync("validChatMessage", chatMessage);
//             }
//             catch (Exception e)
//             {
//                 Console.WriteLine("Falha ao enviar mensagem para o consumidor: " + e.Message);
//             }
//         }

//         await _repository.SaveAsync(chatMessage);
//     }
// }

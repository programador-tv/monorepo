namespace Domain.Models.ViewModels
{
    public sealed class ChatMessagesViewModel
    {
        public HashSet<ChatMessageRequestModel> Messages { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLib.Schedule.Models.Interfaces
{
    public interface IPublishChatGpt
    {
        Task<string> GenerateResponse(
            string prompt,
            int maxTokens,
            double temperature,
            int topP,
            int frequencyPenalty,
            int presencePenalty
        );
    }
}

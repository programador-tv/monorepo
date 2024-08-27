using ClassLib.Schedule.Models.Interfaces;
using Domain.Models.ViewModels;
using Domain.WebServices;

namespace APP.Platform.Services
{
    public class OpenAiService(ILiveWebService _liveWebService)
    {
        public async Task<LiveTitleAndDescriptionViewModel?> GetTitleAndDescriptionSugestion(
            string tags
        )
        {
            var response = await _liveWebService.GetTitleAndDescriptionSugestion(tags);
            if (!response.Contains("[$$]"))
            {
                return new LiveTitleAndDescriptionViewModel
                {
                    Titulo = string.Empty,
                    Descricao = string.Empty,
                };
            }

            string[] FraseDescricao = response.Split(
                new string[] { "[$$]" },
                StringSplitOptions.RemoveEmptyEntries
            );

            FraseDescricao[0] = FraseDescricao[0].Replace("\n", ""); // Remove quebra de linha
            FraseDescricao[0] = FraseDescricao[0].Replace(".", ""); // Remove ponto
            string titulo = FraseDescricao[0].Trim().Substring(7).Trim(); // Obtém o trecho até o primeiro ponto
            string descricao = FraseDescricao[1].Trim().Substring(10).Trim(); // Obtém o trecho após o primeiro ponto, removendo espaços em branco no início

            var resultado = new LiveTitleAndDescriptionViewModel
            {
                Titulo = titulo,
                Descricao = descricao,
            };
            return resultado;
        }

        // private async Task<string> TryGetGptResponse(string tags)
        // {
        //     int maxTokens = 1500;
        //     double temperature = 0.5;
        //     int topP = 1;
        //     int frequencyPenalty = 0;
        //     int presencePenalty = 0;

        //     string prompt =
        //         "Olá, especialista em geração de textos atrativos e criativos. Estou procurando criar um título e uma descrição para uma live de um site de programação de software, com base nas seguintes palavras-chave: {0}. Por favor, utilize sua expertise para criar um título e uma descrição impactantes, que atraiam a atenção do público. Separe o título e a descrição pelo conjunto de caracteres [$$]. Exemplo:Título: título. [$$] Descrição:descricao";
        //     var input = string.Format(prompt, tags);

        //     string response = await _publishChatGpt.GenerateResponse(
        //         input,
        //         maxTokens,
        //         temperature,
        //         topP,
        //         frequencyPenalty,
        //         presencePenalty
        //     );

        //     return response;
        // }
    }
}

using Domain.Entities;
using Domain.Models.ViewModels;
using Domain.WebServices;
using Infrastructure.Data.Contexts;

namespace APP.Platform.Pages
{
    public sealed class Search
    {
        private readonly ApplicationDbContext _context;
        private readonly PerfilDbContext _perfilContext;
        private IPerfilWebService _perfilWebService { get; set; }
        public List<Live> Lives = new List<Live>();
        public List<Domain.Entities.Perfil> Perfils = new();
        public IHttpClientFactory _httpClientFactory { get; set; }

        public Search(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IPerfilWebService perfilWebService,
            string key
        )
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
            _perfilWebService = perfilWebService;

            foreach (var word in key.Trim().Split(' '))
            {
                ProcessKeyword(word);
                ProcessKeywordForChannels(word).Wait();
            }
        }

        public void ProcessKeyword(string keyword)
        {
            Lives.AddRange(SearchBySpecificTitle(keyword));
            Lives.AddRange(SearchByTitleContaining(keyword));
            Lives.AddRange(SearchByDescriptionContaining(keyword));
            Lives.AddRange(SearchByTagContaining(keyword));
        }

        public async Task ProcessKeywordForChannels(string keyword)
        {
            var perfilsResponse = await _perfilWebService.GetByKeyword(keyword);

            foreach (var perfil in perfilsResponse)
            {
                var perfilLegacy = new Domain.Entities.Perfil
                {
                    Id = perfil.Id,
                    Nome = perfil.Nome,
                    Foto = perfil.Foto,
                    Token = perfil.Token,
                    UserName = perfil.UserName,
                    Linkedin = perfil.Linkedin,
                    GitHub = perfil.GitHub,
                    Bio = perfil.Bio,
                    Email = perfil.Email,
                    Descricao = perfil.Descricao,
                    Experiencia = (Domain.Entities.ExperienceLevel)perfil.Experiencia
                };
                Perfils.Add(perfilLegacy);
            }

            foreach (var perfil in Perfils)
            {
                Lives.AddRange(SearchByUserId(perfil.Id));
            }
        }

        private List<Live> SearchByUserId(Guid perfilId)
        {
            var lives = _context
                ?.Lives?.Where(e => e.PerfilId == perfilId && e.Visibility)
                .ToList();
            if (lives == null)
            {
                lives = new List<Live> { };
            }
            return lives;
        }

        private List<Live> SearchBySpecificTitle(string keyword)
        {
            var lives = _context
                ?.Lives?.Where(e => (e.Titulo ?? "").ToUpper() == keyword.ToUpper() && e.Visibility)
                .ToList();
            if (lives == null)
            {
                lives = new List<Live> { };
            }
            return lives;
        }

        private List<Live> SearchByTitleContaining(string keyword)
        {
            var lives = _context
                ?.Lives?.Where(e =>
                    (e.Titulo ?? string.Empty).ToUpper().Contains(keyword.ToUpper()) && e.Visibility
                )
                .ToList();
            if (lives == null)
            {
                lives = new List<Live> { };
            }
            return lives;
        }

        private List<Live> SearchByDescriptionContaining(string keyword)
        {
            var lives = _context
                ?.Lives?.Where(e =>
                    (e.Descricao ?? string.Empty).ToUpper().Contains(keyword.ToUpper())
                    && e.Visibility
                )
                .ToList();
            if (lives == null)
            {
                lives = new List<Live> { };
            }
            return lives;
        }

        private List<Live> SearchByTagContaining(string keyword)
        {
            var tagsLivesRelation = GetTagRelationByKeyword(keyword);

            return GetLivesByTagRelation(tagsLivesRelation);
        }

        private List<Live> GetLivesByTagRelation(List<Tag> tagRelationList)
        {
            var tempLivesList = new List<Live>();

            foreach (var tag in tagRelationList)
            {
                var lives = _context?.Lives?.Where(e =>
                    e.Id.ToString() == tag.LiveRelacao && e.Visibility
                );
                if (lives != null)
                {
                    tempLivesList.AddRange(lives);
                }
            }

            return tempLivesList;
        }

        private List<Tag> GetTagRelationByKeyword(string keyword)
        {
            var tags = _context
                ?.Tags?.Where(e =>
                    keyword.ToUpper().Contains((e.Titulo ?? "").ToUpper()) && e.LiveRelacao != null
                )
                .ToList();
            if (tags == null)
            {
                tags = new List<Tag> { };
            }
            return tags;
        }
    }
}

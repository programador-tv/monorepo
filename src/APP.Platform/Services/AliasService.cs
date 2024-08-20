using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APP.Platform
{
    public partial class AliasService : IAliasService
    {
        private readonly ApplicationDbContext _context;

        public AliasService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string NormalizeTitle(string title)
        {
            title = title.ToLower().Replace(" ", "-");
            title = HttpUtility.UrlEncode(title);
            return title;
        }

        public async Task<bool> AliasExists(string urlAlias)
        {
            var live = await _context
                .Lives.Where(e => e.UrlAlias == urlAlias)
                .FirstOrDefaultAsync();

            if (live != null)
            {
                return true;
            }

            return false;
        }

        public async Task<string> AliasGeneratorAsync(string title)
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(7));
            var cancellationToken = cts.Token;

            var alias = string.Concat(
                NormalizeTitle(title),
                "-",
                Guid.NewGuid().ToString().AsSpan(0, 5)
            );
            var aliasExists = AliasExists(alias);

            while (await aliasExists)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException("Tempo de execução excedido.");
                }
                alias = string.Concat(
                    NormalizeTitle(title),
                    "-",
                    Guid.NewGuid().ToString().AsSpan(0, 5)
                );
                aliasExists = AliasExists(alias);
            }

            return alias;
        }
    }
}

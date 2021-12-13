using Pandora.Core.Interfaces;
using Pandora.Core.Migrations;

namespace Pandora.Services
{
    public abstract class PandoraService : IPandoraService
    {
        public PandoraDbContext dbContext { get; set; }
        protected PandoraService(PandoraDbContext context)
        {
            dbContext = context;
        }
    }
}

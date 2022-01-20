using Pandora.Core.Migrations;

namespace Pandora.Core.Interfaces
{
    public interface IPandoraService
    {
        PandoraDbContext dbContext { get; set; }
    }
}

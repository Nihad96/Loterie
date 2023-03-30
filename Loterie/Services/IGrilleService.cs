using Loterie.Models;

namespace Loterie.Services
{
    public interface IGrilleService
    {
        GrilleViewModel GetGrille();

        string ValiderParticipation(string participation);
    }

}

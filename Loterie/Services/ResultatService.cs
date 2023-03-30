using DataLayer.Model;
using Loterie.Models;
using DataLayer; // JAMAIS DANS UN CONTROLLEUR 

namespace Loterie.Services
{
    public class ResultatService : IResultatService
    {
        private readonly ILogger<ResultatService> _logger;
        private readonly AppDbContext dbContext;

        public ResultatService(ILogger<ResultatService> logger, AppDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }
        public ResultatViewModel GetResultat(string user_guid)
        {

            // on récupère la participation grâce au guid
            var participation = dbContext.Participations.FirstOrDefault(p => p.guid_participation == user_guid);

            // on récupère le tirage grâce à l'id_tirage de la participation
            var tirage = dbContext.Tirages.FirstOrDefault(t => t.id_tirage == participation.id_tirage);


            // on crée un objet ResultatViewModel contenant toutes les informations nécessaires à l'affichage du résultat du tirage

            var resultat = new ResultatViewModel()
            {
                id_tirage = tirage.id_tirage,
                tirage_aleatoire = tirage.tirage_aleatoire,
                nombre_choux_total = tirage.nombre_choux_total,
                date_tirage = tirage.date_tirage,

                tirage_utilisateur = participation.tirage_utilisateur,
                nombres_trouves = participation.nombres_trouves,
                nombre_choux_remportes = participation.nombre_choux_remportes
            };

            return resultat;
        }
    }
}

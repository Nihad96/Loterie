using CSharpVitamins;
using Loterie.Models;
using DataLayer; // JAMAIS DANS UN CONTROLLEUR 
using DataLayer.Model;

namespace Loterie.Services
{
    public class HelpersService : IHelpersService 
    {
        private readonly ILogger<HelpersService> _logger; // permet de récupérer la chaine de connexion dans appsettings.json
        private readonly AppDbContext dbContext;  // permet d'effectuer des requêtes à la base de données

        public HelpersService(ILogger<HelpersService> logger, AppDbContext dbContext) 
        {
            _logger = logger;
            this.dbContext = dbContext;
        }


        // fonction permettant de checker si le guid est reconnu dans la base de données
        public string CheckGuid(IndexViewModel model)
        {
            var guid = new IndexViewModel  // on crée un objet IndexViewModel avec le guid
            {
                User_guid = model.User_guid
            };

            string string_user_guid = guid.User_guid;

            // on récupère la participation si elle existe 
            var participation = dbContext.Participations.Where(p => p.guid_participation == string_user_guid).FirstOrDefault();

            // si elle n'existe pas, elle est nulle alors on renvoie l'erreur
            if(participation == null)
            {
                return "Ce code ne correspond a aucune participation.";
            }

            // si elle existe, on récupère le tirage lié à la participation
            var tirage = dbContext.Tirages.Where(t => t.id_tirage == participation.id_tirage).FirstOrDefault();

            // ensuite on récupère le dernier tirage en cours
            var dernier_tirage = dbContext.Tirages.OrderByDescending(t => t.id_tirage)
                .FirstOrDefault();


            // si le tirage concerné par la participation est le dernier en date alors on renvoie une erreur car le tirage n'est pas fini
            if (tirage.id_tirage == dernier_tirage.id_tirage) 
            {
                return "Il est trop tôt pour consulter ce tirage.";  // on redirige vers l'accueil
            }

            return "";
        }
    }
}

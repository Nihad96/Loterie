using CSharpVitamins;
using DataLayer;
using DataLayer.Model;
using Loterie.Models;

namespace Loterie.Services
{
    public class GrilleService : IGrilleService
    {
        private readonly ILogger<GrilleService> _logger; // permet de récupérer la chaine de connexion dans appsettings.json
        private readonly AppDbContext dbContext;  // permet d'effectuer des requêtes à la base de données

        public GrilleService(ILogger<GrilleService> logger, AppDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        public GrilleViewModel GetGrille()
        {
            var tirage = dbContext.Tirages.OrderByDescending(t => t.id_tirage)
                    .FirstOrDefault();
            var grille = new GrilleViewModel()
            {
                nombre_choux_total = tirage.nombre_choux_total,
                date_tirage = tirage.date_tirage
            };
            return grille;
        }

       public string ValiderParticipation(string tirage_utilisateur)
        {
            bool erreur = false;
            // ici il faut checker si la participation est au bon format (6 nombres de 1 à 49, sinon on renvoie un message d'erreur)

            // on transforme le tirage en liste de 6 nombres
            var liste_tirage_utilisateur = tirage_utilisateur.Split(';').ToList();
            
            // s'il y a plus ou moins de 6 nombres
            if(liste_tirage_utilisateur.Count != 6)
            {
                return "";
            }

            foreach(var nombre in liste_tirage_utilisateur)
            {
                // variable qui va contenir le nombre après conversion en entier
                int resultat; 

                // variable qui va contenir vrai si nombre est convertible, ou faux s'il ne l'est pas
                bool conversion = int.TryParse(nombre, out resultat);

                // si nombre n'est pas convertible on admet une erreur donc on arrête le programme
                if (!conversion)
                {
                    return "";
                }
                else  // sinon s'il est convertible mais qu'il est inférieur à 1 ou supérieur à 49 on arrête aussi
                {
                    if(resultat < 1 ||resultat > 49)
                    {
                        return "";
                    }
                }
            }

            // si on se retrouve ici c'est que la participation contient bien 6 nombres de 1 à 49.


            // on récupère le tirage concerné par la participation (le dernier créé puisque c'est celui en cours)
            var tirage = dbContext.Tirages
                .OrderByDescending(t => t.id_tirage)
                .FirstOrDefault();

            // s'il ne reste plus qu'une minute pour participer on arrête la fonction
            DateTime date_tirage = tirage.date_tirage;
            DateTime maintenant = DateTime.Now;
            if ( (date_tirage - maintenant).TotalSeconds < 60) 
            {
                return "";
            }

            // si on est ici c'est qu'il reste plus d'une minute alors on va procéder à la création de la participation

            // il faut tout d'abord augmenter le lot du tirage de 1 chou
            int nombre_choux_total = tirage.nombre_choux_total;

            tirage.nombre_choux_total = nombre_choux_total + 1;
            dbContext.Tirages.UpdateRange(tirage);
            dbContext.SaveChanges();

            // on va créer un short guid à transmettre à l'utilisateur pour accéder au résultat
            ShortGuid guid = ShortGuid.NewGuid();


            var participation = new Participation()
            {
                guid_participation = guid.ToString(),
                id_tirage = tirage.id_tirage,
                tirage_utilisateur = tirage_utilisateur
            };

            dbContext.Participations.Add(participation);
            // dbcontext add
            dbContext.SaveChanges();

            // on renvoie maintenant le GUID que l'on va afficher au participant via le controleur Validation
            return guid.ToString();
        }
    }
}

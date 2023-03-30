using DataLayer;
using DataLayer.Model;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

public class BackgroundWorkerService : BackgroundService
{
    readonly ILogger<BackgroundWorkerService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        // ici c'est un peu différent que d'habitude car on est dans le cas d'un hosted service
        // réponse trouvée ici : https://www.thecodebuzz.com/cannot-consume-scoped-service-from-singleton-ihostedservice/
        // https://bartwullems.blogspot.com/2019/11/using-scoped-service-inside.html
        // this.dbContext = factory.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
    }


    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // _logger.LogInformation("Worker running at:{time}", DateTimeOffset.Now);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // récupérer toutes les infos du tirage

                var tirage = dbContext.Tirages.OrderByDescending(t => t.id_tirage)
                    .FirstOrDefault();
                _logger.LogInformation("------------------ Tirage numéro :{id}", tirage.id_tirage);
                // _logger.LogInformation("Tirage numéro :{id}", tirage.id_tirage);

                // lot de base si on crée un tirage à partir de 0
                int lot_tirage_suivant = 10;

                // s'il n'y a pas encore de tirage
                if (tirage == null)
                {
                    // on en crée un 
                    CreerTirage(lot_tirage_suivant);
                }
                else  // sinon on cloture le tirage en attribuant les prix et on crée un nouveau tirage
                {
                    // récupérer toutes les participations de ce tirage
                    var participations = dbContext.Participations.Where(p => p.id_tirage == tirage.id_tirage).ToList();

                    // _logger.LogInformation("Nombre de participations :{count}", participations.Count());

                    // si il n'y a pas de participation, supprimer le tirage (pq pas ajouter le lot du tirage précédent au nouveau tirage
                    if (participations.Count == 0)
                    {
                        // supprimer le tirage
                        // attribuer le lot au tirage suivant
                        lot_tirage_suivant = tirage.nombre_choux_total;

                        CreerTirage(lot_tirage_suivant);

                        SupprimerTirage(tirage.id_tirage);
                    }
                    else
                    {

                        // on transforme le tirage en liste de 6 nombres
                        var liste_tirage = tirage.tirage_aleatoire.Split(';').ToList();

                        // on va classer les participations par rang de nombres trouvés
                        // par exemple rang 6 = 6 nombres trouvés
                        // le lot est remporté par la ou les personnes qui ont trouvé le plus de nombres (plus haut rang atteint)
                        // pour chaque participation on va déterminer le nombre de bons résultats

                        lot_tirage_suivant = tirage.nombre_choux_total;
                        _logger.LogInformation("------------------- 0 : lot_tirage_suivant : {lot_tirage_suivant}", lot_tirage_suivant);

                        foreach (var participation in participations)
                        {
                            var liste_participation = participation.tirage_utilisateur.Split(";").ToList();
                            int nombres_trouves = 0;

                            // on va comparer le tirage aléatoire et la participation pour voir le nombre de nombres trouvés
                            foreach (string nombre in liste_tirage)
                            {
                                if (liste_participation.Contains(nombre))
                                {
                                    nombres_trouves++;
                                }
                            }
                            // s'il y en a au moins un on modifie la participation dans la bdd
                            if (nombres_trouves > 0)
                            {
                                participation.nombres_trouves = nombres_trouves;
                            }


                            dbContext.Participations.UpdateRange(participations);
                            dbContext.SaveChanges();
                        }

                        // Maintenant il faut passer à la répartition des lots
                        // on va chercher à découper les participations en groupes 
                        lot_tirage_suivant = tirage.nombre_choux_total;
                        _logger.LogInformation("------------------- 1 : lot_tirage_suivant : {lot_tirage_suivant}", lot_tirage_suivant);

                        for (int i = 6; i > 3; i--)
                        {
                            // on récupère les participations de ce tirage, ayant trouvé i nombres de bons résultatqs
                            participations = dbContext.Participations.Where(p => p.nombres_trouves == i &&
                            p.id_tirage == tirage.id_tirage).ToList();


                            int nombre_vainceurs = participations.Count();
                            _logger.LogInformation("-------------------- nombre vainceurs : {nombre_vainceurs}", nombre_vainceurs);

                            // si on a une ou plusieurs participations 
                            // alors on distribue le lot parmi la/les participations ayant trouvé le plus grand nombre de nombres
                            // 6 nombres justes = 60% | 5 nombres justes = 20% | 4 nombres justes = 20% 
                            // le reste est ajouté au prochain tirage (si 3 bons nombres, alors 51% du lot est ajouté au prochain tirage)
                            if (nombre_vainceurs != 0)
                            {
                                int lot_final;

                                if (i == 6)
                                {
                                    lot_final = (int)Math.Round(lot_tirage_suivant * 0.6, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    lot_final = (int)Math.Round(lot_tirage_suivant * 0.2, MidpointRounding.AwayFromZero);
                                }

                                lot_tirage_suivant = lot_tirage_suivant - lot_final;

                                foreach (var participation in participations)
                                {
                                    participation.nombre_choux_remportes = (lot_final / nombre_vainceurs);
                                }

                                // si on a eu des vainceurs pour ce rang, on stoppe la boucle car les rangs inférieurs n'auront rien
                                // mais d'abord on passe le lot_tirage_suivant à 0 car le lot a été remporté et on modifie la bdd
                                dbContext.Participations.UpdateRange(participations);
                                dbContext.SaveChanges();
                                break;
                            }
                        }

                        _logger.LogInformation("-------------------- 2 : lot_tirage_suivant : {lot_tirage_suivant}", lot_tirage_suivant);

                        // une fois qu'on a effectué la répartition des lots, on crée un nouveau tirage
                        CreerTirage(lot_tirage_suivant);
                    }
                }

            }

            // _logger.LogInformation("Id du tirage : {id}", tirage.id_tirage);

            // on répète toutes les cinq minutes
            await Task.Delay(5 * 60000, stoppingToken);
        }
    }
    // fonction permettant de créer un nouveau tirage, prend en paramètre le lot précédent si personne ne l'a remporté
    public void CreerTirage(int lot_tirage_suivant)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();


            // _logger.LogInformation("Fonction : lot_tirage_suivant : {lot_tirage_suivant}", lot_tirage_suivant);
            // on va générer une chaine de 6 nombres aléatoires de 1 à 49, séparés par des ;
            List<int> liste_aleatoire = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                Random aleatoire = new Random();
                int nombre_aleatoire = aleatoire.Next(1, 49);

                if (liste_aleatoire.Contains(nombre_aleatoire))
                {
                    i--;
                }
                else
                {
                    liste_aleatoire.Add(nombre_aleatoire);
                }
            }
            // maintenant qu'on a une liste de 6 nombres aléatoires on va les concaténer dans une variable de type string
            string tirage_aleatoire = string.Join(";", liste_aleatoire.ToArray());

            var tirage = new Tirage()
            {
                date_tirage = DateTime.Now.AddMinutes(5),
                tirage_aleatoire = tirage_aleatoire,
                nombre_choux_total = lot_tirage_suivant
            };

            // _logger.LogInformation("Fonction : lot_tirage_suivant : {lot_tirage_suivant}", tirage.nombre_choux_total);
            // _logger.LogInformation("Fonction : aleatoire : {aleatoire}", tirage.tirage_aleatoire);

            dbContext.Tirages.Add(tirage);
            dbContext.SaveChanges();
        }
    }


    public void SupprimerTirage(int id_tirage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // on récupère le tirage à supprimer dans une variable
            var tirage_a_supprimer = dbContext.Tirages.FirstOrDefault(t => t.id_tirage == id_tirage);

            // on supprime le tirage
            dbContext.Tirages.Remove(tirage_a_supprimer);
            dbContext.SaveChanges();
        }
    }
}
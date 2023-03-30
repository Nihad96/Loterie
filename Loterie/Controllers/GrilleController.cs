using System;
using System.Diagnostics;
using DataLayer; // JAMAIS DANS UN CONTROLLEUR 
using Loterie.Models;
using Loterie.Services;
using Microsoft.AspNetCore.Mvc;

namespace Loterie.Controllers
{
    public class GrilleController : Controller
    {
        private readonly ILogger<GrilleController> _logger;
        private readonly IGrilleService _grilleService;
        private readonly AppDbContext dbContext;  // ajout de la bdd au controlleur -- JAMAIS DANS UN CONTROLLEUR


        public GrilleController(
            ILogger<GrilleController> logger,
            IGrilleService grilleService,
            AppDbContext dbContext
            )
             
        {
            _logger = logger;
            _grilleService = grilleService;
            this.dbContext = dbContext;
        }

        public IActionResult Grille()
        {
            var model = _grilleService.GetGrille();
            return View(model);
        }

        [HttpPost]
        public IActionResult Grille(string participation)
        {

            var guid = _grilleService.ValiderParticipation(participation);

            // si le guid est vide c'est que le format de la participation n'est pas valide
            // alors on l'indique au participant
            if (guid == "")
            {
                // rediriger vers la page d'erreur
                return RedirectToAction("Error", "Grille");
            }
            else  // sinon on le redirige vers la page de validation de participation
            {
                // rediriger vers la page de validation
                TempData["guid"] = guid;
                return RedirectToAction("Confirmation", "Grille");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var model = new ErrorViewModel {};
            return View(model);
        }

        public IActionResult Confirmation()
        {
            string guid = (string)TempData["guid"]; // ici on récupère le guid entré par l'utilisateur
            var model = new ConfirmationViewModel() { guid = guid };
            return View(model);
        }
    }
}
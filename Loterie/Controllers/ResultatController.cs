using System.Diagnostics;
using CSharpVitamins;
using Loterie.Models;
using Loterie.Services;
using Microsoft.AspNetCore.Mvc;

namespace Loterie.Controllers
{
    public class ResultatController : Controller
    {
        private readonly IResultatService _resultatService;


        public ResultatController(IResultatService resultatService)
        {
            _resultatService = resultatService;
        }
         
        public IActionResult Resultat()
        {

            if (TempData["user_guid"] == null) // si on essaye d'accéder à la page des résultats par l'URL on redirige vers l'accueil
            {
                return RedirectToAction("Index", "Home");
            }

            string user_guid = (string)TempData["user_guid"]; // ici on récupère le guid entré par l'utilisateur

            // on crée le model à partir de la méthode GetResultat de ResultatService
            var model = _resultatService.GetResultat(user_guid);
            return View(model);
        }
    }
}
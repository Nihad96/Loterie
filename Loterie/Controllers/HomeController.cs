using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Loterie.Models;
using Loterie.Services;
using Microsoft.AspNetCore.Mvc;

namespace Loterie.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHelpersService _helpersService;


        public HomeController(IHelpersService helpersService)
        {
            _helpersService = helpersService;
        }

        public IActionResult Index() // page d'accueil de base
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IndexViewModel model)  // lorsque l'utilisateur souhaite consulter un résultat
        {
            string guid = model.User_guid;  // guid saisi par l'utilisateur
            
            if(!ModelState.IsValid)  // s'il ne respecte pas les contraintes fixées par le modèle on renvoie un message d'erreur
            {
                return View(model);
            }
            else  // sinon on effectue des contrôles supplémentaires (on check dans la bdd si le guid existe)
            {
                string message_erreur = _helpersService.CheckGuid(model);
                
                // CheckGuid envoie vrai si le guid est reconnu, faux s'il n'est pas reconnu
                if (message_erreur == "")
                {
                    // si le guid est reconnu on redirige vers le controleur de resultat en passant le guid en paramètre dans TempData
                    TempData["user_guid"] = guid;
                    return RedirectToAction("Resultat", "Resultat");
                }
                else // sinon on renvoie un message d'erreur sur la page d'accueil
                {
                    ModelState.AddModelError("UserGuid", message_erreur);
                    // afficher page d'accueil AVEC le message d'erreur
                    return View(); 
                }
            }
        }
    }
}
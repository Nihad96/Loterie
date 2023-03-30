using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Loterie.Models
{
    public class IndexViewModel
    {
        [MinLength(22, ErrorMessage = "Code d'authentification trop court !")]
        [MaxLength(22, ErrorMessage = "Code d'identification trop long !")]  // on peut ajouter des messages d'erreur de cette façon
        [Required(ErrorMessage = "Veuillez entrer un code d'identification !")]
        public string? User_guid { get; set; }
    }
}

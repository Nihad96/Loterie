using System.ComponentModel.DataAnnotations;

namespace Loterie.Models
{
    public class GrilleViewModel
    {

        [MinLength(11)]
        [MaxLength(17)]  // on peut ajouter des messages d'erreur de cette façon
        [Required]
        public string Participation { get; set; }

        public int nombre_choux_total { get; set; }
        public DateTime date_tirage { get; set; }
    }
}
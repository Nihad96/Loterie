using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Model
{
    public class Tirage
    {
        [Key]
        public int id_tirage { get; set; }

        public DateTime date_tirage { get; set; }

        [MaxLength(17)]
        public string tirage_aleatoire { get; set; }   // Varchar(50)

        [MaxLength(6)]
        [DefaultValue(0)]
        public int nombre_choux_total { get; set; }
    }
}

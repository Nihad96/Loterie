using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Model
{
    public class Participation
    {
        [Key]
        public int id_participation { get; set; }

        [MaxLength(36)]
        public string guid_participation { get; set; }

        [MaxLength(11)]
        public int id_tirage { get; set; }

        [MaxLength(17)]
        public string tirage_utilisateur { get; set; }

        [MaxLength(1)]
        [DefaultValue(0)]
        public int nombres_trouves { get; set; }

        [MaxLength(6)]
        [DefaultValue(0)]
        public int nombre_choux_remportes { get; set; }
    }
}

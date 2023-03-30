namespace Loterie.Models
{
    public class ResultatViewModel
    {
        public int? id_tirage { get; set; }

        public string? tirage_aleatoire { get; set; }

        public int? nombre_choux_total { get; set; }

        public DateTime date_tirage { get; set; }



        public string tirage_utilisateur { get; set; }

        public int? nombres_trouves { get; set; }

        public int? nombre_choux_remportes { get; set; }

    }
}
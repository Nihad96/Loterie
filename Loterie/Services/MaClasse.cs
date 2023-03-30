namespace Loterie.Services
{
    public class MaClasse : IMaClasse
    {
        public string DisSalut()
        {
            return "Salut toi !";
        }


        public string DisSalut(string nom)
        {
            return "Salut " + nom + " !";
        }
    }
}

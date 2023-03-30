using DataLayer.Model;
using Loterie.Models;

namespace Loterie.Services
{
    public interface IResultatService
    {
        ResultatViewModel GetResultat(string user_guid);
    }
}

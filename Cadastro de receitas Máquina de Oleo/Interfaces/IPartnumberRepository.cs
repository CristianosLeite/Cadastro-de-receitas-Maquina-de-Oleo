using Cadastro_de_receitas_Máquina_de_Oleo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;

namespace Cadastro_de_receitas_Máquina_de_Oleo.Interfaces
{
    public interface IPartnumberRepository
    {
        Task InsertOrUpdatePartnumber(SqlConnection connection, Partnumber partnumber);
        Task<bool> SavePartnumber(Partnumber partnumber);
        ObservableCollection<Partnumber> LoadPartnumberList();
        Task<bool> DeletePartnumber(string partnumber);
    }
}

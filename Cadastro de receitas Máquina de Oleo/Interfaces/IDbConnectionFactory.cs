using Microsoft.Data.SqlClient;

namespace Cadastro_de_receitas_Máquina_de_Oleo.Interfaces
{
    public interface IDbConnectionFactory
    {
        SqlConnection GetConnection();
    }
}

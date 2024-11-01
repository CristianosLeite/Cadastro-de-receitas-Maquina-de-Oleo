using Cadastro_de_receitas_Máquina_de_Oleo.Interfaces;
using Microsoft.Data.SqlClient;

namespace Cadastro_de_receitas_Máquina_de_Oleo.Database
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public SqlConnection GetConnection()
        {
            var connectionString = Environment.GetEnvironmentVariable("MAQOLEO_DB_CONNECTION");
            return new SqlConnection(connectionString);
        }
    }
}

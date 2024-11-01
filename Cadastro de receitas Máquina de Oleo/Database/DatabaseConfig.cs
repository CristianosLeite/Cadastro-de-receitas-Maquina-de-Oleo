using Cadastro_de_receitas_Máquina_de_Oleo.Interfaces;

namespace Cadastro_de_receitas_Máquina_de_Oleo.Database
{
    public class DatabaseConfig : IDatabaseConfig
    {
        public static string? ConnectionString { get; set; } =
            Environment.GetEnvironmentVariable("MAQOLEO_DB_CONNECTION");
    }
}

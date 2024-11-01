namespace Cadastro_de_receitas_Máquina_de_Oleo.Interfaces
{
    internal interface IDatabaseConfig
    {
        public static string? ConnectionString { get; } =
            Environment.GetEnvironmentVariable("PICKBYOPEN_DB_CONNECTION");
    }
}

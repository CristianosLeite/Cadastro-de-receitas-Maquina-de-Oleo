using Cadastro_de_receitas_Máquina_de_Oleo.Interfaces;
using Cadastro_de_receitas_Máquina_de_Oleo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;

namespace Cadastro_de_receitas_Máquina_de_Oleo.Database
{
    public class Db : DatabaseConfig
    {
        protected readonly IDbConnectionFactory _connectionFactory;
        protected readonly IPartnumberRepository _partnumberRepository;

        public Db(
            IDbConnectionFactory connectionFactory
        )
        {
            _connectionFactory = connectionFactory;
            _partnumberRepository = new PartnumberRepository(connectionFactory);
        }

        public static SqlConnection GetConnection()
        {
            try
            {
                var connectionString = ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "Database connection string is not configured."
                    );
                }

                return new SqlConnection(connectionString);
            }
            catch (ArgumentNullException ex)
            {
                ShowErrorMessage("Erro ao obter a string de conexão: " + ex.Message);
                App.Current.Shutdown();
                throw;
            }
            catch (InvalidOperationException ex)
            {
                ShowErrorMessage("Erro de configuração: " + ex.Message);
                App.Current.Shutdown();
                throw;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Erro ao criar a conexão com o banco de dados: " + ex.Message);
                App.Current.Shutdown();
                throw;
            }
        }

        // <summary>
        // Show an error message
        // </summary>
        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public async Task<bool> SavePartnumber(Partnumber partnumber)
        {
            return await _partnumberRepository.SavePartnumber(partnumber);
        }

        public async Task<bool> DeletePartnumber(string partnumber)
        {
            return await _partnumberRepository.DeletePartnumber(partnumber);
        }

        public ObservableCollection<Partnumber> LoadPartnumberList()
        {
            return _partnumberRepository.LoadPartnumberList();
        }
    }
}

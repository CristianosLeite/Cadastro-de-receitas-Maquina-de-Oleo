using Cadastro_de_receitas_Máquina_de_Oleo.Interfaces;
using Cadastro_de_receitas_Máquina_de_Oleo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;

namespace Cadastro_de_receitas_Máquina_de_Oleo.Database
{
    public class PartnumberRepository(IDbConnectionFactory connectionFactory)
        : IPartnumberRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        // <summary>
        // Insert a new partnumber into the partnumbers table
        // </summary>
        public async Task InsertOrUpdatePartnumber(SqlConnection connection, Partnumber partnumber)
        {
            string partnumberTypeOil = (
                partnumber.TypeOil == "Tanque 01" ? "OLEO LUBRIFICANTE"
                : partnumber.TypeOil == "Tanque 02" ? "OLEO MOTOR SAE 15W40 CLASS TFE-ACEA E7/0"
                : partnumber.TypeOil == "Tanque 03" ? "OLEO LUBRIFICANTE TLS CK4-SAE10W40"
                : throw new Exception("Tipo de óleo inválido.")
            );
            try
            {
                var mergeCommand = new SqlCommand(
                    "MERGE INTO dbo.receitas AS target "
                        + "USING (SELECT @codigoReceita AS codigoReceita, @nomeMotor AS nomeMotor, @idMotor AS idMotor, @volume AS volume, @tempoDosagem AS tempoDosagem) AS source "
                        + "ON target.codigoReceita = source.codigoReceita "
                        + "WHEN MATCHED THEN "
                        + "UPDATE SET nomeMotor = source.nomeMotor, idMotor = source.idMotor, volume = source.volume, tempoDosagem = source.tempoDosagem "
                        + "WHEN NOT MATCHED THEN "
                        + "INSERT (codigoReceita, nomeMotor, idMotor, volume, data, tempoDosagem) "
                        + "VALUES (source.codigoReceita, source.nomeMotor, source.idMotor, source.volume, CURRENT_TIMESTAMP, source.tempoDosagem);",
                    connection
                );

                mergeCommand.Parameters.AddWithValue("@codigoReceita", partnumber.Code);
                mergeCommand.Parameters.AddWithValue("@nomeMotor", partnumber.Description);
                mergeCommand.Parameters.AddWithValue("@idMotor", partnumberTypeOil);
                mergeCommand.Parameters.AddWithValue("@volume", partnumber.Volume);
                mergeCommand.Parameters.AddWithValue("@tempoDosagem", partnumber.DosingTime);

                await mergeCommand.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                MessageBox.Show("Não foi possível cadastrar o partnumber." + e);
                throw;
            }
        }

        // <summary>
        // Insert a new partnumber into the database
        // </summary>
        public async Task<bool> SavePartnumber(Partnumber partnumber)
        {
            using var connection = _connectionFactory.GetConnection();
            await connection.OpenAsync();

            await InsertOrUpdatePartnumber(connection, partnumber);

            return true;
        }

        public ObservableCollection<Partnumber> LoadPartnumberList()
        {
            try
            {
                using var connection = _connectionFactory.GetConnection();
                connection.Open();

                var partnumberList = new ObservableCollection<Partnumber>();

                using var command = new SqlCommand(
                    "SELECT codigoReceita, nomeMotor, idMotor, volume, tempoDosagem FROM dbo.receitas ORDER BY data DESC;",
                    connection
                );
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string tank = string.Empty;
                    if (reader.GetString(2) == "OLEO LUBRIFICANTE")
                    {
                        tank = "Tanque 01";
                    }
                    else if (reader.GetString(2) == "OLEO MOTOR SAE 15W40 CLASS TFE-ACEA E7/0")
                    {
                        tank = "Tanque 02";
                    }
                    else if (reader.GetString(2) == "OLEO LUBRIFICANTE TLS CK4-SAE10W40")
                    {
                        tank = "Tanque 03";
                    }

                    partnumberList.Add(
                        new Partnumber(
                            reader.GetString(0), // partnumber
                            reader.GetString(1), // description
                            tank, // oil
                            reader.GetString(3), // volume
                            reader.GetInt32(4) // dosingTime
                        )
                    );
                }

                return partnumberList;
            }
            catch (SqlException e)
            {
                throw new Exception("Erro ao carregar a lista de partnumbers." + e);
            }
        }

        // <summary>
        // Delete a partnumber from the database
        // </summary>
        public async Task<bool> DeletePartnumber(string partnumber)
        {
            try
            {
                using var connection = _connectionFactory.GetConnection();
                connection.Open();

                var deleteCommand = new SqlCommand(
                    "DELETE FROM dbo.receitas WHERE codigoReceita = @partnumber;",
                    connection
                );
                deleteCommand.Parameters.AddWithValue("@partnumber", partnumber);
                await deleteCommand.ExecuteNonQueryAsync();

                return true;
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "Não foi possível deletar o partnumber. Verifique se o partnumber está associado à alguma porta e desfaça a associação."
                );
                return false;
                throw;
            }
        }
    }
}

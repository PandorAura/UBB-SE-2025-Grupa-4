using App1.Infrastructure;
using App1.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace App1.Repositories
{
    public class UpgradeRequestsRepository : IUpgradeRequestsRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly ISqlDataAdapter _dataAdapter;

        private const string SELECT_ALL_UPGRADE_REQUESTS_QUERY = "SELECT RequestId, RequestingUserId, RequestingUserName FROM UpgradeRequests";
        private const string SELECT_UPGRADE_REQUEST_BY_IDENTIFIER_QUERY = "SELECT RequestId, RequestingUserId, RequestingUserName FROM UpgradeRequests WHERE RequestId = @upgradeRequestIdentifier";
        private const string DELETE_UPGRADE_REQUEST_QUERY = "DELETE FROM UpgradeRequests WHERE RequestId=@upgradeRequestIdentifier";

        public UpgradeRequestsRepository(ISqlConnectionFactory connectionFactory, ISqlDataAdapter dataAdapter)
        {
            _connectionFactory = connectionFactory;
            _dataAdapter = dataAdapter;
        }

        // Legacy constructor for backward compatibility
        public UpgradeRequestsRepository(string databaseConnectionString)
            : this(new SqlConnectionFactory(databaseConnectionString), new SqlDataAdapterWrapper(new SqlDataAdapter()))
        {
        }

        public List<UpgradeRequest> RetrieveAllUpgradeRequests()
        {
            List<UpgradeRequest> upgradeRequestsList = new List<UpgradeRequest>();

            using (ISqlConnection connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    connection.Open();
                    ISqlCommand selectUpgradeRequestsCommand = connection.CreateCommand();
                    selectUpgradeRequestsCommand.CommandText = SELECT_ALL_UPGRADE_REQUESTS_QUERY;

                    using (ISqlDataReader upgradeRequestsDataReader = selectUpgradeRequestsCommand.ExecuteReader())
                    {
                        while (upgradeRequestsDataReader.Read())
                        {
                            UpgradeRequest upgradeRequest = new UpgradeRequest(
                                upgradeRequestsDataReader.GetInt32(0),
                                upgradeRequestsDataReader.GetInt32(1),
                                upgradeRequestsDataReader.GetString(2));

                            upgradeRequestsList.Add(upgradeRequest);
                        }
                        upgradeRequestsDataReader.Close();
                    }
                }
                catch (Exception databaseException)
                {
                    Console.WriteLine("Database Error: " + databaseException.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return upgradeRequestsList;
        }

        public void RemoveUpgradeRequestByIdentifier(int upgradeRequestIdentifier)
        {
            using (ISqlConnection connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    connection.Open();
                    ISqlCommand deleteCommand = connection.CreateCommand();
                    deleteCommand.CommandText = DELETE_UPGRADE_REQUEST_QUERY;
                    deleteCommand.Parameters.AddWithValue("@upgradeRequestIdentifier", upgradeRequestIdentifier);
                    deleteCommand.ExecuteNonQuery();
                }
                catch (Exception databaseException)
                {
                    Console.WriteLine("Database Error: " + databaseException.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public UpgradeRequest RetrieveUpgradeRequestByIdentifier(int upgradeRequestIdentifier)
        {
            UpgradeRequest retrievedUpgradeRequest = null;

            using (ISqlConnection connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    connection.Open();
                    ISqlCommand selectUpgradeRequestCommand = connection.CreateCommand();
                    selectUpgradeRequestCommand.CommandText = SELECT_UPGRADE_REQUEST_BY_IDENTIFIER_QUERY;
                    selectUpgradeRequestCommand.Parameters.AddWithValue("@upgradeRequestIdentifier", upgradeRequestIdentifier);

                    using (ISqlDataReader upgradeRequestDataReader = selectUpgradeRequestCommand.ExecuteReader())
                    {
                        if (upgradeRequestDataReader.Read())
                        {
                            retrievedUpgradeRequest = new UpgradeRequest(
                                upgradeRequestDataReader.GetInt32(0),
                                upgradeRequestDataReader.GetInt32(1),
                                upgradeRequestDataReader.GetString(2)
                            );
                        }
                        upgradeRequestDataReader.Close();
                    }
                }
                catch (Exception databaseException)
                {
                    Console.WriteLine("Database Error: " + databaseException.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return retrievedUpgradeRequest;
        }
    }
}
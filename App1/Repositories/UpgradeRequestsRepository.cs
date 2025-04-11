using App1.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace App1.Repositories
{
    public class UpgradeRequestsRepository : IUpgradeRequestsRepository
    {
        private readonly SqlDataAdapter databaseDataAdapter;
        private readonly SqlConnection databaseConnection;

        private const string SELECT_ALL_UPGRADE_REQUESTS_QUERY = "SELECT RequestId, RequestingUserId, RequestingUserName FROM UpgradeRequests";
        private const string SELECT_UPGRADE_REQUEST_BY_IDENTIFIER_QUERY = "SELECT RequestId, RequestingUserId, RequestingUserName FROM UpgradeRequests WHERE RequestId = @upgradeRequestIdentifier";
        private const string DELETE_UPGRADE_REQUEST_QUERY = "DELETE FROM UpgradeRequests WHERE RequestId=@upgradeRequestIdentifier";

        public UpgradeRequestsRepository(string databaseConnectionString)
        {
            databaseConnection = new SqlConnection(databaseConnectionString);
            databaseDataAdapter = new SqlDataAdapter();
        }

        public List<UpgradeRequest> RetrieveAllUpgradeRequests()
        {
            List<UpgradeRequest> upgradeRequestsList = new List<UpgradeRequest>();
            
            try
            {
                databaseConnection.Open();
                SqlCommand selectUpgradeRequestsCommand = new SqlCommand(SELECT_ALL_UPGRADE_REQUESTS_QUERY, databaseConnection);
                SqlDataReader upgradeRequestsDataReader = selectUpgradeRequestsCommand.ExecuteReader();
                
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
            catch (Exception databaseException)
            {
                Console.WriteLine("Database Error: " + databaseException.Message);
            }
            finally
            {
                databaseConnection.Close();
            }
            
            return upgradeRequestsList;
        }

        public void RemoveUpgradeRequestByIdentifier(int upgradeRequestIdentifier)
        {
            databaseDataAdapter.DeleteCommand = new SqlCommand(DELETE_UPGRADE_REQUEST_QUERY, databaseConnection);
            databaseDataAdapter.DeleteCommand.Parameters.Add("@upgradeRequestIdentifier", System.Data.SqlDbType.Int).Value = upgradeRequestIdentifier;
            
            databaseConnection.Open();
            databaseDataAdapter.DeleteCommand.ExecuteNonQuery();
            databaseConnection.Close();
        }

        public UpgradeRequest RetrieveUpgradeRequestByIdentifier(int upgradeRequestIdentifier)
        {
            UpgradeRequest retrievedUpgradeRequest = null;

            try
            {
                databaseConnection.Open();
                SqlCommand selectUpgradeRequestCommand = new SqlCommand(SELECT_UPGRADE_REQUEST_BY_IDENTIFIER_QUERY, databaseConnection);
                selectUpgradeRequestCommand.Parameters.AddWithValue("@upgradeRequestIdentifier", upgradeRequestIdentifier);

                SqlDataReader upgradeRequestDataReader = selectUpgradeRequestCommand.ExecuteReader();

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
            catch (Exception databaseException)
            {
                Console.WriteLine("Database Error: " + databaseException.Message);
            }
            finally
            {
                databaseConnection.Close();
            }

            return retrievedUpgradeRequest;
        }
    }
}

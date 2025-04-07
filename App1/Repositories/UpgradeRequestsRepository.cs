using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
namespace App1.Repositories
{
    public class UpgradeRequestsRepository: IUpgradeRequestsRepository
    {
        private SqlDataAdapter da;
        private SqlConnection cs;
        public UpgradeRequestsRepository(string connectionString) {
            cs = new SqlConnection(connectionString);
            da = new SqlDataAdapter();
        }

        public List<UpgradeRequest> getAllRequests()
        {
            List<UpgradeRequest> requests = new List<UpgradeRequest>();
            string query = "SELECT RequestId, RequestingUserId, RequestingUserName FROM UpgradeRequests";
            try
            {
                cs.Open();
                SqlCommand selectCommand = new SqlCommand(query, cs);
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    UpgradeRequest request = new UpgradeRequest(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2));
                    
                    requests.Add(request);
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
            }
            return requests;
        }

        public void deleteRequestBasedOnRequestId(int requestId)
        {
            da.DeleteCommand = new SqlCommand("DELETE FROM UpgradeRequests WHERE RequestId=@requestId",cs);
            da.DeleteCommand.Parameters.Add("@requestId", System.Data.SqlDbType.Int).Value = requestId;
            cs.Open();
            da.DeleteCommand.ExecuteNonQuery();
            cs.Close();
        }
        public UpgradeRequest getUpgradeRequest(int requestId)
        {
            UpgradeRequest request = null;
            string query = "SELECT RequestId, RequestingUserId, RequestingUserName FROM UpgradeRequests WHERE RequestId = @id";

            try
            {
                cs.Open();
                SqlCommand cmd = new SqlCommand(query, cs);
                cmd.Parameters.AddWithValue("@id", requestId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) 
                {
                    request = new UpgradeRequest(
                        reader.GetInt32(0),      
                        reader.GetInt32(1),       
                        reader.GetString(2)       
                    );
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
            }

            return request;
        }
    
    }
}

namespace App1.Models
{
    public class UpgradeRequest
    {
        public UpgradeRequest(int newRequestId, int newRequestingUserId, string newRequestingUserName) { 
            RequestId = newRequestId;
            RequestingUserId = newRequestingUserId;
            RequestingUserName = newRequestingUserName;
        }
        public int RequestId { get; set; }

        public int RequestingUserId { get; set; }

        public string RequestingUserName {  get; set; }

        public override string ToString()
        {
            return this.RequestingUserName;
        }
    }
}

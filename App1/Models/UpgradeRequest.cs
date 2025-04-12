namespace App1.Models
{
    public class UpgradeRequest
    {
        public UpgradeRequest(int upgradeRequestId, int requestingUserIdentifier, string requestingUserDisplayName)
        {
            UpgradeRequestId = upgradeRequestId;
            RequestingUserIdentifier = requestingUserIdentifier;
            RequestingUserDisplayName = requestingUserDisplayName;
        }

        public int UpgradeRequestId { get; set; }
        public int RequestingUserIdentifier { get; set; }
        public string RequestingUserDisplayName { get; set; }

        public override string ToString()
        {
            return RequestingUserDisplayName;
        }
    }
}

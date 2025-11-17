namespace Web_Application_Part3.Models
{
    public class HRDashboardModel
    {
        public string HRName { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<ClaimHR> PendingHRClaims { get; set; }
        public List<ClaimHR> ApprovedByHR { get; set; }
        public List<ClaimHR> DeclinedByHR { get; set; }
    }

    // Notification class for HR
    public class NotificationHR
    {
        public string Type { get; set; }  // e.g., "NewClaim", "AdminApproved", "Warning"
        public string Message { get; set; }
    }

    // Claim class (reuse from your existing claim class)
    public class ClaimHR
    {
        public int claimID { get; set; }
        public string FacallyName { get; set; }
        public string ModuleName { get; set; }
        public int Hourworked { get; set; }
        public int Hourlyrate { get; set; }
        public int TotalAmount { get; set; }
        public string SupportingDocuments { get; set; }
        public string claim_status { get; set; }
        public DateTime creating_date { get; set; }
    }
}

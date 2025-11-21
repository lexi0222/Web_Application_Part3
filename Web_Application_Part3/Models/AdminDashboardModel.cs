namespace Web_Application_Part3.Models
{
    public class AdminDashboardModel
    {
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public List<Claim> PendingClaims { get; set; }
        public List<Claim> RecentClaims { get; set; }
        public List<Claim> ApprovedClaims { get; set; }
        public List<Claim> DeclinedClaims { get; set; }
        public List<Claim> Claims { get; set; }
      

    }
}

namespace Web_Application_Part3.Models
{
    public class DashboardModel
    {

     
    }
    public class LecturerDashboardModel
    {
        public List<Notification> Notifications { get; set; }
        public List<Claim> RecentClaims { get; set; }
        public List<Claim> ApprovedClaims { get; set; }
        public List<Claim> DeclinedClaims { get; set; }
        public List<Claim> MyClaims { get; set; }
        public string LecturerName { get; set; }
    }


    public class Notification
    {
        public int NotificationID { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string UserEmail { get; set; }
    }


    public class RecentUpdate
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Claim
    {
        public int claimID { get; set; }
        public string claim_status { get; set; }

        public DateTime creating_date { get; set; }


        public string FacallyName { get; set; }



        public string ModuleName { get; set; }


        public int Hourworked { get; set; }


        public int Hourlyrate { get; set; }


        public int TotalAmount { get; set; }

        public List<Claim> RecentClaims { get; set; }


        public byte[] SupportingDocuments { get; set; }
    }

   
}

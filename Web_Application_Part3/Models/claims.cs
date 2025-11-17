using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
namespace Web_Application_Part3.Models
{
    public class claims
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string FacallyName { get; set; }

        [Required]
        public string ModuleName { get; set; }

        [Required]
        public int Hourworked { get; set; }

        [Required]
        public int Hourlyrate { get; set; }

        [Required]
        public int TotalAmount { get; set; }

        public DateTime creating_date { get; set; }
        public string claim_status { get; set; }

        public string user_claims(string FacallyName, string ModuleName, int Hourworked, int Hourlyrate, int TotalAmount, string Email)
        {
            string message = "No claims were made";

            try
            {
                using (SqlConnection connects = new SqlConnection(@"Data Source=(localdb)\claim_system;Initial Catalog=claims_database;Integrated Security=True;"))
                {
                    connects.Open();

                    // Get the user ID
                    string getUserIDQuery = "SELECT userID FROM Users WHERE Email = @Email";
                    int userID;
                    using (SqlCommand cmd = new SqlCommand(getUserIDQuery, connects))
                    {
                        cmd.Parameters.AddWithValue("@Email", Email);
                        object result = cmd.ExecuteScalar();
                        if (result == null)
                            return "User not found.";
                        userID = (int)result;
                    }

                    // Insert claim (no SupportingDocuments)
                    string query = @"INSERT INTO Claims 
                        (FacallyName, ModuleName, Hourworked, Hourlyrate, TotalAmount, claim_status, creating_date, userID)
                        VALUES (@FacallyName, @ModuleName, @Hourworked, @Hourlyrate, @TotalAmount, @claim_status, @creating_date, @userID)";

                    using (SqlCommand cmd = new SqlCommand(query, connects))
                    {
                        cmd.Parameters.AddWithValue("@FacallyName", FacallyName);
                        cmd.Parameters.AddWithValue("@ModuleName", ModuleName);
                        cmd.Parameters.AddWithValue("@Hourworked", Hourworked);
                        cmd.Parameters.AddWithValue("@Hourlyrate", Hourlyrate);
                        cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                        cmd.Parameters.AddWithValue("@claim_status", "Pending");
                        cmd.Parameters.AddWithValue("@creating_date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@userID", userID);

                        cmd.ExecuteNonQuery();
                    }

                    message = "Claim successfully submitted!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                message = "Error submitting claim.";
            }

            return message;
        }
    }
}

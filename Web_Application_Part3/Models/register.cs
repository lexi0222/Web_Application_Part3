using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Web_Application_Part3.Models
{
    public class register
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string FullName { get; set; }


        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // "Admin" or "HR"or "Lecturer"



        // creating a method 
        public string register_user(string FullName, string Email, string Role, string Password)
        {
            connectionString connectString = new connectionString();
            string Message = "Not registered";
            try
            {
                using (SqlConnection connects = new SqlConnection(connectString.connects()))
                {
                    connects.Open();
                    string query = @"insert into Users values('" + FullName + "','" + Email + "','" + Role + "','" + Password + "')";

                    using (SqlCommand execute_query = new SqlCommand(query, connects))
                    {
                        execute_query.ExecuteNonQuery();

                        Message = "yes";


                    }






                    connects.Close();
                }

            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);

            }
            return Message;
        }

    }
}


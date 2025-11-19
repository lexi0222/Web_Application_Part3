using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Mvc;
using Web_Application_Part3.Models;
using static Web_Application_Part3.Models.DashboardModel;

namespace Web_Application_Part3.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = @"Server=(localdb)\claim_system;Database=claims_database";
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Login user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill in all required fields.";
                return View(user);
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password AND Role = @Role";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Role", user.Role);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) // <-- check if a matching user exists
                {
                    // Set session
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("Role", user.Role);

                    // Redirect based on role from DB
                    string role = reader["Role"].ToString();
                    if (role == "Admin")
                        return RedirectToAction("Dashboard_Admin", "Home");
                    else if (role == "Lecturer")
                        return RedirectToAction("Dashboard_L", "Home");
                    else if (role == "HR")
                        return RedirectToAction("Dashboard_HR", "Home");
                }
                else
                {
                    ViewBag.Error = "Invalid email, password, or role.";
                }

                con.Close();
            }

            return View(user);
        }


        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult register()
        {

            return View();
        }
        [HttpPost]
        public IActionResult register(register user)
        {
            if (ModelState.IsValid)
            {

                if (user.register_user(user.FullName, user.Email, user.Role, user.Password) == "yes")
                {
                    return RedirectToAction("index");
                }
                else
                {
                    Console.WriteLine("Failed to register the user");
                }






            }
            else

            {
                Console.WriteLine("All fields are required");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Claims()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index"); // Not logged in

            // Load claim history
            var claimList = GetUserClaims(email);

            ViewBag.ClaimHistory = claimList;

            return View();
        }

        
        [HttpPost]
        [HttpPost]
        public IActionResult Claims(claims info)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "You must log in first.";
                return RedirectToAction("Index");
            }

            // Calculate total before inserting
            info.TotalAmount = info.Hourworked * info.Hourlyrate;

            // Submit claim
            string result = info.user_claims(
                info.FacallyName,
                info.ModuleName,
                info.Hourworked,
                info.Hourlyrate,
                info.TotalAmount,
                email
            );

            ViewBag.Message = result;
            ViewBag.ClaimHistory = GetUserClaims(email);

            return View();
        }



        private List<claims> GetUserClaims(string email)
        {
            var list = new List<claims>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT c.* 
                         FROM Claims c 
                         JOIN Users u ON c.userID = u.userID 
                         WHERE u.Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new claims
                            {
                                FacallyName = reader["FacallyName"].ToString(),
                                ModuleName = reader["ModuleName"].ToString(),
                                Hourworked = Convert.ToInt32(reader["Hourworked"]),
                                Hourlyrate = Convert.ToInt32(reader["Hourlyrate"]),
                                TotalAmount = Convert.ToInt32(reader["TotalAmount"]),

                                creating_date = Convert.ToDateTime(reader["creating_date"]),
                                claim_status = reader["claim_status"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }






        public IActionResult Dashboard_L()
        {
            var email = HttpContext.Session.GetString("Email");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(email) || role != "Lecturer")
            {
                return RedirectToAction("Index");
            }

            var claims = GetLecturerClaims(email);
            var model = new LecturerDashboardModel
            {
                RecentClaims = claims
            };

            return View(model);
        }

        private List<Claim> GetLecturerClaims(string email)
        {
            var claims = new List<Claim>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT c.* FROM Claims c INNER JOIN Users u ON c.userID = u.userID WHERE u.email = @email";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    claims.Add(new Claim
                    {
                        claimID = Convert.ToInt32(reader["claimID"]),
                        FacallyName = reader["FacallyName"].ToString(),
                        ModuleName = reader["ModuleName"].ToString(),
                        creating_date = Convert.ToDateTime(reader["creating_date"]),
                        claim_status = reader["claim_status"].ToString()
                    });
                }
            }

            return claims;
        }

        private List<Notification> GetLecturerNotifications(string email)
        {
            // populate notifications data here
            // for example:
            var notifications = new List<Notification>();
            // add notifications to the list
            return notifications;
        }

        private List<Claim> GetLecturerRecentClaims(string email)
        {
            // populate recent claims data here
            // for example:
            var recentClaims = new List<Claim>();
            // add recent claims to the list
            return recentClaims;
        }

        private List<Claim> GetLecturerApprovedClaims(string email)
        {
            // populate approved claims data here
            // for example:
            var approvedClaims = new List<Claim>();
            // add approved claims to the list
            return approvedClaims;
        }

        private List<Claim> GetLecturerDeclinedClaims(string email)
        {
            // populate declined claims data here
            // for example:
            var declinedClaims = new List<Claim>();
            // add declined claims to the list
            return declinedClaims;
        }

        public IActionResult Profile_L()
        {

            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index"); // Not logged in

            Login user = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT Email, Role FROM Users WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new Login
                            {
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
                con.Close();
            }

            if (user == null)
                return RedirectToAction("Index");

            return View(user);
        }

        public IActionResult Profile_A()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index"); // Not logged in

            Login user = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT Email, Role FROM Users WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new Login
                            {
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
                con.Close();
            }

            if (user == null)
                return RedirectToAction("Index");

            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public User GetUser(string email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Users WHERE Email = @Email";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        Name = reader["full_names"].ToString(),
                        Email = reader["Email"].ToString()
                    };
                }
                return null;
            }
        }

        private List<Claim> GetClaims(string email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Claims WHERE userID = (SELECT userID FROM Users WHERE Email = @Email)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                var reader = command.ExecuteReader();
                var claims = new List<Claim>();
                while (reader.Read())
                {
                    claims.Add(new Claim
                    {
                        claimID = Convert.ToInt32(reader["claimID"]),
                        claim_status = reader["claim_status"].ToString()
                    });
                }
                return claims;
            }
        }

        public IActionResult Dashboard_Admin()
        {
            var claims = GetAllClaims();


            var email = HttpContext.Session.GetString("Email");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(email) || role != "Admin")
                return RedirectToAction("Index"); // Not logged in or not admin

            var allClaims = GetAllClaims(); // List<Claim>

            var model = new AdminDashboardModel
            {
                Claims = claims,
                RecentClaims = allClaims.OrderByDescending(c => c.creating_date).Take(10).ToList(),
                PendingClaims = allClaims.Where(c => c.claim_status == "Pending").ToList(),
                ApprovedClaims = allClaims.Where(c => c.claim_status == "Approved").ToList(),
                DeclinedClaims = allClaims.Where(c => c.claim_status == "Declined").ToList(),
                Notifications = GetNotifications(email)
            };

            return View(model); // Pass the correct model
        }


        [HttpGet]
        public IActionResult claims_A()
        {
            var email = HttpContext.Session.GetString("Email");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(email) || !string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index");
            }

            var allClaims = GetAllClaims();
            var model = new AdminDashboardModel
            {
                RecentClaims = allClaims.OrderByDescending(c => c.creating_date).Take(10).ToList(),
                PendingClaims = allClaims.Where(c => c.claim_status == "Pending").ToList(),
                ApprovedClaims = allClaims.Where(c => c.claim_status == "Approved").ToList(),
                DeclinedClaims = allClaims.Where(c => c.claim_status == "Declined").ToList(),
            };
            return View(model);

        }


        private List<Claim> GetAllClaims()
        {
            var claims = new List<Claim>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Claims";
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    claims.Add(new Claim
                    {
                        claimID = Convert.ToInt32(reader["claimID"]),
                        FacallyName = reader["FacallyName"].ToString(),
                        ModuleName = reader["ModuleName"].ToString(),
                        Hourworked = Convert.ToInt32(reader["Hourworked"]),
                        Hourlyrate = Convert.ToInt32(reader["Hourlyrate"]),
                        TotalAmount = Convert.ToInt32(reader["TotalAmount"]),
                        claim_status = reader["claim_status"].ToString(),
                        creating_date = Convert.ToDateTime(reader["creating_date"])
                    });
                }
            }
            return claims;
        }





        [HttpPost]
        public IActionResult UpdateClaimStatus(int claimID, string claim_status)
        {
            if (string.IsNullOrEmpty(claim_status))
            {
                ModelState.AddModelError("claim_status", "Status is required");
                return RedirectToAction("claims_A");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 1️⃣ Get lecturer email from the claim
                string lecturerEmail = "";
                string getEmailQuery = @"
            SELECT u.Email
            FROM Claims c
            JOIN Users u ON c.userID = u.userID
            WHERE c.claimID = @claimID";

                using (var command = new SqlCommand(getEmailQuery, connection))
                {
                    command.Parameters.AddWithValue("@claimID", claimID);
                    lecturerEmail = command.ExecuteScalar()?.ToString();
                }

                // 2️⃣ Update claim status as selected by Admin
                string updateQuery = "UPDATE Claims SET claim_status = @claim_status WHERE claimID = @claimID";
                using (var cmd = new SqlCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@claimID", claimID);
                    cmd.Parameters.AddWithValue("@claim_status", claim_status);
                    cmd.ExecuteNonQuery();
                }

                // 3 ADMIN NOTIFICATIONS (Lecturer ONLY)
                if (!string.IsNullOrEmpty(lecturerEmail))
                {
                    if (claim_status == "Approved")
                    {
                        AddNotification(
                            lecturerEmail,
                            $"Your claim #{claimID} has been approved by Admin.",
                            "AdminApproved"
                        );

                        // Set status to move to HR queue
                        string hrStatusQuery = "UPDATE Claims SET claim_status = 'PendingHR' WHERE claimID = @claimID";
                        using (var cmd = new SqlCommand(hrStatusQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@claimID", claimID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (claim_status == "Declined")
                    {
                        AddNotification(
                            lecturerEmail,
                            $"Your claim #{claimID} has been declined by Admin.",
                            "AdminDeclined"
                        );
                    }
                }
            }

            return RedirectToAction("claims_A");
        }



        public IActionResult ClaimsHistory()
        {
            var email = HttpContext.Session.GetString("email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index");
            }

            var claimsHistory = GetClaimsHistory(email);
            ViewBag.ClaimsHistory = claimsHistory;

            return View();
        }

        private List<Claim> GetClaimsHistory(string email)
        {
            var claimsHistory = new List<Claim>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Claims WHERE userID = (SELECT userID FROM Users WHERE email = @email)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    claimsHistory.Add(new Claim
                    {
                        FacallyName = reader["FacallyName"].ToString(),
                        ModuleName = reader["ModuleName"].ToString(),
                        Hourworked = Convert.ToInt32(reader["Hourworked"]),
                        Hourlyrate = Convert.ToInt32(reader["Hourlyrate"]),
                        TotalAmount = Convert.ToInt32(reader["TotalAmount"]),
                        creating_date = Convert.ToDateTime(reader["creating_date"]),
                        claim_status = reader["claim_status"].ToString()
                    });
                }

                Console.WriteLine("Claims History Count: " + claimsHistory.Count);
            }

            return claimsHistory;
        }

        private List<Notification> GetNotifications(string email)
        {
            // implement this method to retrieve notifications from the database
            return new List<Notification>();
        }

        private List<RecentUpdate> GetRecentUpdates(string email)
        {
            // implement this method to retrieve recent updates from the database
            return new List<RecentUpdate>();
        }

        [HttpGet]
        public IActionResult Dashboard_HR()
        {
            var email = HttpContext.Session.GetString("Email");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(email) || role != "HR")
                return RedirectToAction("Index"); // Not logged in or not HR

            // Get HR name from database
            string hrName = GetUser(email)?.Name ?? email;

            // Prepare HR dashboard model
            var model = new HRDashboardModel
            {
                HRName = hrName,
                Notifications = GetHRNotifications(email),
                PendingHRClaims = GetPendingHRClaims(),
                ApprovedByHR = GetApprovedHRClaims(),
                DeclinedByHR = GetDeclinedHRClaims()
            };

            return View(model);
        }

        private List<Notification> GetHRNotifications(string hrEmail)
        {
            var notifications = new List<Notification>();

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
            SELECT c.claimID, c.FacallyName, c.ModuleName, c.claim_status, c.creating_date, u.Email AS LecturerEmail
            FROM Claims c
            JOIN Users u ON c.userID = u.userID
            WHERE c.claim_status IN ('PendingHR', 'Approved', 'Declined')
            ORDER BY c.creating_date DESC";

                using (var cmd = new SqlCommand(query, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string type = reader["claim_status"].ToString() switch
                        {
                            "PendingHR" => "NewClaim",
                            "Approved" => "AdminApproved",
                            "Declined" => "Warning",
                            _ => "Info"
                        };

                        string message = reader["claim_status"].ToString() switch
                        {
                            "PendingHR" => $"New claim from {reader["FacallyName"]} for module {reader["ModuleName"]}",
                            "Approved" => $"Claim by {reader["FacallyName"]} approved by Admin",
                            "Declined" => $"Claim by {reader["FacallyName"]} declined by Admin",
                            _ => "Unknown notification"
                        };

                        notifications.Add(new Notification
                        {
                            Type = type,
                            Message = message
                        });
                    }
                }
            }

            return notifications;
        }


        private List<ClaimHR> GetPendingHRClaims()
        {
            var list = new List<ClaimHR>();
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT c.*, u.full_names AS LecturerName
                         FROM Claims c
                         JOIN Users u ON c.userID = u.userID
                         WHERE claim_status = 'PendingHR'";
                using (var cmd = new SqlCommand(query, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ClaimHR
                        {
                            claimID = Convert.ToInt32(reader["claimID"]),
                            FacallyName = reader["FacallyName"].ToString(),
                            ModuleName = reader["ModuleName"].ToString(),
                            Hourworked = Convert.ToInt32(reader["Hourworked"]),
                            Hourlyrate = Convert.ToInt32(reader["Hourlyrate"]),
                            TotalAmount = Convert.ToInt32(reader["TotalAmount"]),
                            claim_status = reader["claim_status"].ToString(),
                            creating_date = Convert.ToDateTime(reader["creating_date"]),
                            SupportingDocuments = reader["SupportingDocuments"].ToString()
                        });
                    }
                }
            }
            return list; // now List<ClaimHR>
        }



        private List<ClaimHR> GetApprovedHRClaims()
        {
            var list = new List<ClaimHR>();
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT c.*, u.full_names AS LecturerName
                         FROM Claims c
                         JOIN Users u ON c.userID = u.userID
                         WHERE claim_status = 'ApprovedByHR'";
                using (var cmd = new SqlCommand(query, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ClaimHR
                        {
                            claimID = Convert.ToInt32(reader["claimID"]),
                            FacallyName = reader["FacallyName"].ToString(),
                            ModuleName = reader["ModuleName"].ToString(),
                            Hourworked = Convert.ToInt32(reader["Hourworked"]),
                            Hourlyrate = Convert.ToInt32(reader["Hourlyrate"]),
                            TotalAmount = Convert.ToInt32(reader["TotalAmount"]),
                            claim_status = reader["claim_status"].ToString(),
                            creating_date = Convert.ToDateTime(reader["creating_date"]),
                            SupportingDocuments = reader["SupportingDocuments"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        private List<ClaimHR> GetDeclinedHRClaims()
        {
            var list = new List<ClaimHR>();
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT c.*, u.full_names AS LecturerName
                         FROM Claims c
                         JOIN Users u ON c.userID = u.userID
                         WHERE claim_status = 'DeclinedByHR'";
                using (var cmd = new SqlCommand(query, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ClaimHR
                        {
                            claimID = Convert.ToInt32(reader["claimID"]),
                            FacallyName = reader["FacallyName"].ToString(),
                            ModuleName = reader["ModuleName"].ToString(),
                            Hourworked = Convert.ToInt32(reader["Hourworked"]),
                            Hourlyrate = Convert.ToInt32(reader["Hourlyrate"]),
                            TotalAmount = Convert.ToInt32(reader["TotalAmount"]),
                            claim_status = reader["claim_status"].ToString(),
                            creating_date = Convert.ToDateTime(reader["creating_date"]),
                            SupportingDocuments = reader["SupportingDocuments"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        private void AddNotification(string userEmail, string message, string type)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"INSERT INTO Notifications (UserEmail, Message, Type)
                         VALUES (@UserEmail, @Message, @Type)";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserEmail", userEmail);
                    cmd.Parameters.AddWithValue("@Message", message);
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}

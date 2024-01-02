using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;  // Importez l'espace de noms correct
using MySql.Data.MySqlClient;
using System.Diagnostics;

using Gestion_bibliotheque.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Gestion_biblio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;  // Importez l'espace de noms correct
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Gestion_biblio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Gestion_biblio.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;



        public IActionResult Index()
        {
            // Transmettez les données à la vue
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("YourAuthenticationScheme");

            // Redirigez l'utilisateur vers la page d'accueil après la déconnexion
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Index(User model)
        {
            string yourConnectionString = "Server=localhost;User ID=root;Password=;Database=registered;port=3036;SslMode=none";
            using var connection = new MySqlConnection(yourConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT email, password FROM user WHERE email = @Email AND password = @Password;", connection);
            command.Parameters.AddWithValue("@Email", model.Email);
            command.Parameters.AddWithValue("@Password", model.Password);

            using var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
               
                return RedirectToAction("Acceuil", "Acceuil"); 
            }
            else
            {
            
                ViewBag.ErrorMessage = "Email ou mot de passe incorrect.";
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;  // Importez l'espace de noms correct
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Gestion_bibliotheque.Models;
using Microsoft.Extensions.Configuration;
using Gestion_biblio.Models;
using Microsoft.Extensions.Configuration;  // Importez l'espace de noms correct

using Microsoft.Extensions.Configuration;


namespace Gestion_bibliotheque.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        // GET: AcceuilController
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
       

        // GET: AcceuilController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AcceuilController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AcceuilController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AcceuilController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AcceuilController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AcceuilController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AcceuilController/Delete/5
        [HttpPost]
        public IActionResult Index(User model)
        {
            try
            {
                string yourConnectionString = "Server=localhost;User ID=root;Password=;Database=registered;port=3036;SslMode=none";
                using var connection = new MySqlConnection(yourConnectionString);
                connection.Open();

                // Utilisez une commande paramétrée pour éviter les injections SQL
                using var command = new MySqlCommand("INSERT INTO user (email, password) VALUES (@Email, @Password)", connection);
                command.Parameters.AddWithValue("@Email", model.Email);
                command.Parameters.AddWithValue("@Password", model.Password);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Insertion réussie, rediriger vers une page de confirmation ou une autre page
                    return View();
                }
                else
                {
                    // Insertion échouée, afficher un message d'erreur
                    ViewBag.ErrorMessage = "Erreur lors de l'enregistrement de l'utilisateur.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Gérer les exceptions, afficher un message d'erreur ou effectuer d'autres actions nécessaires
                ViewBag.ErrorMessage = "Une erreur s'est produite : " + ex.Message;
                return View();
            }
        }

        public IActionResult RegistrationSuccess()
        {
            // Vous pouvez afficher une page de confirmation ici
            return View();
        }
    }
}
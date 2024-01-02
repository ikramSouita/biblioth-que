using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;  // Add this line for SelectListItem
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gestion_bibliotheque.Models;
using NuGet.Protocol.Plugins;

namespace Gestion_biblio.Controllers
{
    public class LivreController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;



        [HttpPost]
        public IActionResult Livre()
        {
            try
            {
                // Vous n'avez plus besoin du modèle, car la valeur est statique
                string genreValue = Request.Form["SelectedGenre"];

                // Connectez-vous à la base de données en utilisant votre chaîne de connexion
                string yourConnectionString = "Server=localhost;User ID=root;Password=;Database=registered;port=3036;SslMode=none";
                using var connection = new MySqlConnection(yourConnectionString);
                connection.Open();

                // Utilisez la variable de connexion dans votre requête SQL
                using var command = new MySqlCommand("SELECT Id, Nom, Genre, Description,CheminImage FROM Produits WHERE genre=@Genre;", connection);
                command.Parameters.AddWithValue("@Genre", genreValue);

                using var reader = command.ExecuteReader();

                List<Produit> produits = new List<Produit>();

                while (reader.Read())
                {
                    Produit Produit = new Produit
                    {
                        Nom = reader.GetString("nom"),
                        Genre = reader.GetString("genre"),
                        Description = reader.GetString("Description"),
                        CheminImage = reader.GetString("CheminImage"),
                        Id = reader.GetInt32("Id")


                    };

                    produits.Add(Produit);
                }

                return View("Livre", produits);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur : {ex.Message}");
                throw;
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
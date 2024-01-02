using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Gestion_bibliotheque.Models;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Diagnostics;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace Gestion_bibliotheque.Controllers
{
    public class AcceuilController : Controller
    {
        private readonly IConfiguration _configuration;

        public AcceuilController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Acceuil()
        {
            List<Produit> produits = new List<Produit>();

            string yourConnectionString = "Server=localhost;User ID=root;Password=;Database=registered;port=3036;SslMode=none";

            using var connection = new MySqlConnection(yourConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT * FROM Produits", connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Produit produit = new Produit
                {
                    Id = reader.GetInt32("Id"),
                    Nom = reader.GetString("Nom"),
                    Description = reader.GetString("Description"),
                    Genre = reader.GetString("Genre"),
                    CheminImage = reader.GetString("CheminImage")
                };

                produits.Add(produit);
            }

            return View(produits);
        }

        public IActionResult Details(int id)
        {
            Produit produit = null;

            string yourConnectionString = "Server=localhost;User ID=root;Password=;Database=registered;port=3036;SslMode=none";

            using var connection = new MySqlConnection(yourConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT * FROM Produits WHERE Id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                produit = new Produit
                {
                    Id = reader.GetInt32("Id"),
                    Nom = reader.GetString("Nom"),
                    Description = reader.GetString("Description"),
                    Genre = reader.GetString("Genre"),
                    CheminImage = reader.GetString("CheminImage")
                };
            }

            return View(produit);
        }
        [HttpGet]
        public ActionResult Recherche(string searchTerm)
        {
            try
            {
                string yourConnectionString = "Server=localhost;User ID=root;Password=;Database=registered;port=3036;SslMode=none";
                using var connection = new MySqlConnection(yourConnectionString);
                connection.Open();

                // Utilisez la variable de connexion dans votre requête SQL
                var query = "SELECT Nom, Genre,Description,CheminImage  FROM Produits WHERE Nom LIKE @SearchTerm OR Genre LIKE @SearchTerm";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                using var reader = command.ExecuteReader();

                List<Produit> livres = new List<Produit>();

                while (reader.Read())
                {
                    Produit livre = new Produit
                    {
                        Nom = reader.GetString("Nom"),
                        Genre = reader.GetString("Genre"),
                        Description = reader.GetString("Description"),
                        CheminImage = reader.GetString("CheminImage"),
                    };

                    livres.Add(livre);
                }

                return View("Acceuil", livres);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur : {ex.Message}");
                throw;
            }
        }
        [HttpPost]
        public IActionResult AjouterAuPanier(int produitId, string DateReservation, string DateExpiration)
        {
            string yourConnectionString = "Server=localhost;User ID=root;Password=;Database=registered;port=3036;SslMode=none";

            using var connection = new MySqlConnection(yourConnectionString);
            connection.Open();

            Produit produit = null;
            DateTime parsedDateReservation;
            DateTime parsedDateExpiration;

            if (!DateTime.TryParseExact(DateReservation, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateReservation))
            {
                // Gérer l'échec de la conversion de la date de réservation
                // Par exemple, afficher un message d'erreur ou gérer cette situation
            }

            if (!DateTime.TryParseExact(DateExpiration, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateExpiration))
            {
                // Gérer l'échec de la conversion de la date d'expiration
                // Par exemple, afficher un message d'erreur ou gérer cette situation
            }

            // Sélection des données à partir de la table Produits en fonction de l'ID passé en paramètre
            using (var selectCommand = new MySqlCommand("SELECT * FROM Produits WHERE Id = @id", connection))
            {
                selectCommand.Parameters.AddWithValue("@id", produitId);

                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        produit = new Produit
                        {
                            Id = reader.GetInt32("Id"),
                            Nom = reader.GetString("Nom"),
                            Description = reader.GetString("Description"),
                            Genre = reader.GetString("Genre"),
                            CheminImage = reader.GetString("CheminImage"),
                        };
                    }
                    reader.Close(); // Fermeture du DataReader pour libérer la connexion
                }
            }

            if (produit != null)
            {
                // Vérification si le produit est déjà dans le panier
                using (var checkCommand = new MySqlCommand("SELECT COUNT(*) FROM LivreReserve WHERE Id_produit = @idProduit", connection))
                {
                    checkCommand.Parameters.AddWithValue("@idProduit", produitId);
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count == 0)
                    {
                        var emailMessage = new MimeMessage();

                        emailMessage.From.Add(new MailboxAddress("Bibliothèque", "bibliothequedotnet@gmail.com"));
                        emailMessage.To.Add(new MailboxAddress("reda", "redajord19123@gmail.com")); // Mettez ici l'adresse de l'utilisateur concerné
                        emailMessage.Subject = "Confirmation de réservation";
                        emailMessage.Body = new TextPart("plain")
                        {
                            Text = "Êtes-vous sûr de vouloir réserver cet article ?"
                        };

                        using var client = new SmtpClient();
                        client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                        client.Authenticate("bibliothequedotnet@gmail.com", "biblio1234");

                        client.Send(emailMessage);
                        client.Disconnect(true);

                        using var insertCommand = new MySqlCommand("INSERT INTO LivreReserve (Id, Nom, Description, Genre, Id_produit, DateReservation, DateExpiration) VALUES (@Id_pr, @Nom, @Description, @Genre, @Id, @DateReservation, @DateExpiration)", connection);
                        insertCommand.Parameters.AddWithValue("@Id_pr", produit.Id);
                        insertCommand.Parameters.AddWithValue("@Nom", produit.Nom);
                        insertCommand.Parameters.AddWithValue("@Description", produit.Description);
                        insertCommand.Parameters.AddWithValue("@Genre", produit.Genre);
                        insertCommand.Parameters.AddWithValue("@Id", produitId);
                        insertCommand.Parameters.AddWithValue("@DateReservation", parsedDateReservation); // Utilisation des valeurs converties
                        insertCommand.Parameters.AddWithValue("@DateExpiration", parsedDateExpiration); // Utilisation des valeurs converties

                        insertCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        // Le produit est déjà réservé, vérifier la date d'expiration
                        using (var dateCheckCommand = new MySqlCommand("SELECT DateExpiration FROM LivreReserve WHERE Id_produit = @idProduit", connection))
                        {
                            dateCheckCommand.Parameters.AddWithValue("@idProduit", produitId);
                            DateTime existingExpirationDate = Convert.ToDateTime(dateCheckCommand.ExecuteScalar());

                            if (parsedDateReservation < existingExpirationDate)
                            {
                                ViewBag.ReservationMessage = "Ce produit est déjxà réservé.";
                                return View("Details", produit);
                            }
                            else
                            {
                                var emailMessage = new MimeMessage();

                                emailMessage.From.Add(new MailboxAddress("Bibliothèque", "bibliothequedotnet@gmail.com"));
                                emailMessage.To.Add(new MailboxAddress("Utilisateur", "redajord19123@gmail.com")); // Mettez ici l'adresse de l'utilisateur concerné
                                emailMessage.Subject = "Confirmation de réservation";
                                emailMessage.Body = new TextPart("plain")
                                {
                                    Text = "Êtes-vous sûr de vouloir réserver cet article ?"
                                };

                                using var client = new SmtpClient();
                                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                                client.Authenticate("bibliothequedotnet@gmail.com", "biblio1234");

                                client.Send(emailMessage);
                                client.Disconnect(true);

                                // Mettre à jour la réservation si la date de réservation est ultérieure à la date d'expiration actuelle
                                using var updateCommand = new MySqlCommand("UPDATE LivreReserve SET DateReservation = @DateReservation, DateExpiration = @DateExpiration WHERE Id_produit = @idProduit", connection);
                                updateCommand.Parameters.AddWithValue("@DateReservation", parsedDateReservation);
                                updateCommand.Parameters.AddWithValue("@DateExpiration", parsedDateExpiration);
                                updateCommand.Parameters.AddWithValue("@idProduit", produitId);

                                updateCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            connection.Close(); // Fermeture de la connexion après l'exécution des commandes

            return RedirectToAction("Acceuil");
        }

        public void NettoyerReservationsExpirées()
        {
            string yourConnectionString = "Server=localhost;User ID=root;Password=;Database=registered;port=3036;SslMode=none";

            using var connection = new MySqlConnection(yourConnectionString);
            connection.Open();

            // Supprimez les enregistrements expirés de LivreReserve
            var now = DateTime.Now;
            using (var deleteCommand = new MySqlCommand("DELETE FROM LivreReserve WHERE DateExpiration <= @Now", connection))
            {
                deleteCommand.Parameters.AddWithValue("@Now", now);
                deleteCommand.ExecuteNonQuery();
            }

            connection.Close();
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
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
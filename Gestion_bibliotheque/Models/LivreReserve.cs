namespace Gestion_bibliotheque.Models
{
    
    public class LivreReserve
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public int Id_produit { get; set; }
        public DateTime DateReservation { get; set; }
        public DateTime DateExpiration { get; set; }

        // Autres propriétés...

        public LivreReserve(int id, string nom, string description, string genre, int id_produit, DateTime dateReservation, DateTime dateExpiration)
        {
            Id = id;
            Nom = nom;
            Description = description;
            Genre = genre;
            Id_produit = id_produit;
            DateReservation = dateReservation;
            DateExpiration = dateExpiration;

            // Initialisation d'autres propriétés...
        }
    }

}

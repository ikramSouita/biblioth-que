using System;

namespace Gestion_bibliotheque.Models
{
    public class Produit
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public string? CheminImage { get; set; }
       
    }
}

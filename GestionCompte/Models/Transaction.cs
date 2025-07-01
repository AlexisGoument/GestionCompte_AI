namespace GestionCompte.Models
{
    public record Transaction
    {
        public required DateOnly Date { get; init; }
        public required decimal Montant { get; init; }
        public required string Devise { get; init; }
        public required string Categorie { get; init; }
    }
} 
namespace GestionCompte.Models
{
    public record DonneesCompte
    {
        public decimal? BalanceReference { get; set; }
        public DateOnly? DateBalanceReference { get; set; }
        public TauxDeChange? TauxDeChange { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
    }
} 
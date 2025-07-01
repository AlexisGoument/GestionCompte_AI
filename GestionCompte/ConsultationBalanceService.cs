using GestionCompte.Models;

namespace GestionCompte
{
    public class ConsultationBalanceService(decimal? balanceReference, DateOnly? dateBalanceReference, List<Transaction> transactions, TauxDeChange? tauxDeChange)
    {
        public decimal? BalanceReference { get; } = balanceReference;
        public DateOnly? DateBalanceReference { get; } = dateBalanceReference;
        public List<Transaction> Transactions { get; } = transactions;
        public TauxDeChange? TauxDeChange { get; } = tauxDeChange;

        public decimal ObtenirBalancePour(DateOnly date)
        {
            decimal balancePrecedante = BalanceReference ?? 0m;
            decimal balanceMoisEnCours = Transactions
                .Where(t => t.Date <= date)
                .Sum(t =>
                {
                    if (t.Devise != "EUR" && TauxDeChange is null)
                        throw new InvalidOperationException($"Taux de change {t.Devise} manquant.");

                    return t.Devise switch
                    {
                        "USD" => t.Montant * TauxDeChange!.USD,
                        "JPY" => t.Montant * TauxDeChange!.JPY,
                        "EUR" => t.Montant,
                        _ => throw new InvalidOperationException($"Devise inconnue : {t.Devise}")
                    };
                });

            return balancePrecedante + balanceMoisEnCours;
        }
    }
} 
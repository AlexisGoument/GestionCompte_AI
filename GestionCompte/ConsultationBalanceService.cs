using GestionCompte.Models;

namespace GestionCompte
{
    public static class ConsultationBalanceService
    {
        public static decimal ObtenirBalancePour(
            DateOnly date,
            decimal? balanceReference,
            List<Transaction> transactions,
            TauxDeChange? tauxDeChange)
        {
            decimal balancePrecedante = balanceReference ?? 0m;
            decimal balanceMoisEnCours = transactions
                .Where(t => t.Date <= date)
                .Sum(t =>
                {
                    if (t.Devise != "EUR" && tauxDeChange is null)
                        throw new InvalidOperationException($"Taux de change {t.Devise} manquant.");

                    return t.Devise switch
                    {
                        "USD" => t.Montant * tauxDeChange!.USD,
                        "JPY" => t.Montant * tauxDeChange!.JPY,
                        "EUR" => t.Montant,
                        _ => throw new InvalidOperationException($"Devise inconnue : {t.Devise}")
                    };
                });

            return balancePrecedante + balanceMoisEnCours;
        }
    }
} 
using GestionCompte.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GestionCompte
{
    public class CompteParser : ICompteParser
    {
        public DonneesCompte Parser(string contenuCsv)
        {
            var donnees = new DonneesCompte();
            decimal usd = 1m, jpy = 1m;
            var lignes = contenuCsv.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            int headerIndex = -1;
            for (int i = 0; i < lignes.Length; i++)
            {
                var ligne = lignes[i];
                if (ligne.StartsWith("Compte au "))
                    ParseBalanceReference(ligne, donnees);
                else if (ligne.Contains("/EUR"))
                    ParseTauxDeChange(ligne, ref usd, ref jpy);
                else if (ligne.StartsWith("Date;Montant;Devise;Categorie"))
                {
                    headerIndex = i;
                    break;
                }
            }
            bool hasHeader = headerIndex >= 0;
            if (!hasHeader)
                throw new ArgumentException("L'en-tête CSV 'Date;Montant;Devise;Categorie' est requis.");
            
            lignes.Skip(headerIndex + 1)
                  .ToList()
                  .ForEach(ligne => ParseTransaction(ligne, donnees.Transactions));
            donnees.TauxDeChange = new TauxDeChange(usd, jpy);
            return donnees;
        }

        private void ParseBalanceReference(string ligne, DonneesCompte donnees)
        {
            var match = Regex.Match(ligne, @"Compte au (\d{2}/\d{2}/\d{4}) ?: ?(-?\d+(?:[.,]\d+)?) EUR");
            if (match.Success)
            {
                donnees.DateBalanceReference = DateOnly.ParseExact(match.Groups[1].Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                donnees.BalanceReference = decimal.Parse(match.Groups[2].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
            }
        }

        private void ParseTauxDeChange(string ligne, ref decimal usd, ref decimal jpy)
        {
            var match = Regex.Match(ligne, @"(\w{3})/EUR ?: ?(-?\d+(?:[.,]\d+)?)");
            if (match.Success)
            {
                var devise = match.Groups[1].Value;
                var taux = decimal.Parse(match.Groups[2].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                if (devise == "USD") usd = taux;
                if (devise == "JPY") jpy = taux;
            }
        }

        private void ParseTransaction(string ligne, List<Transaction> transactions)
        {
            var champs = ligne.Split(';');
            if (champs.Length >= 4)
            {
                var date = DateOnly.ParseExact(champs[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var montant = decimal.Parse(champs[1].Replace(',', '.'), CultureInfo.InvariantCulture);
                var devise = champs[2];
                var categorie = champs[3];
                transactions.Add(new Transaction
                {
                    Date = date,
                    Montant = montant,
                    Devise = devise,
                    Categorie = categorie
                });
            }
        }
    }
} 
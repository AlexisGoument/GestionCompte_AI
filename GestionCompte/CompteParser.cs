using GestionCompte.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GestionCompte
{
    public class CompteParser : ICompteParser
    {
        public DonneesCompte Parser(string contenuCsv)
        {
            var lignes = string.IsNullOrWhiteSpace(contenuCsv) 
                ? Array.Empty<string>() 
                : contenuCsv.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            if (lignes.Length == 0)
                throw new ArgumentException("Le contenu CSV ne peut pas être vide.");
                
            var donnees = new DonneesCompte();
            decimal usd = 1m, jpy = 1m;
                
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
                try
                {
                    donnees.DateBalanceReference = DateOnly.ParseExact(match.Groups[1].Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    throw new FormatException($"Format de date invalide pour la balance de référence : {match.Groups[1].Value}");
                }
                
                try
                {
                    donnees.BalanceReference = decimal.Parse(match.Groups[2].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    throw new FormatException($"Format de montant invalide pour la balance de référence : {match.Groups[2].Value}");
                }
            }
            else
            {
                // Vérifier si c'est un problème de format de date ou de montant
                var dateMatch = Regex.Match(ligne, @"Compte au (\S+)");
                if (dateMatch.Success)
                {
                    var dateStr = dateMatch.Groups[1].Value;
                    if (!Regex.IsMatch(dateStr, @"\d{2}/\d{2}/\d{4}"))
                    {
                        throw new FormatException($"Format de date invalide pour la balance de référence : {dateStr}");
                    }
                }
                
                var montantMatch = Regex.Match(ligne, @"Compte au \d{2}/\d{2}/\d{4} ?: ?(\S+) EUR");
                if (montantMatch.Success)
                {
                    var montantStr = montantMatch.Groups[1].Value;
                    if (!Regex.IsMatch(montantStr, @"-?\d+(?:[.,]\d+)?"))
                    {
                        throw new FormatException($"Format de montant invalide pour la balance de référence : {montantStr}");
                    }
                }
            }
        }

        private void ParseTauxDeChange(string ligne, ref decimal usd, ref decimal jpy)
        {
            var match = Regex.Match(ligne, @"(\w{3})/EUR ?: ?(-?\d+(?:[.,]\d+)?)");
            if (match.Success)
            {
                var devise = match.Groups[1].Value;
                var tauxStr = match.Groups[2].Value;
                
                try
                {
                    var taux = decimal.Parse(tauxStr.Replace(',', '.'), CultureInfo.InvariantCulture);
                    if (devise == "USD") usd = taux;
                    if (devise == "JPY") jpy = taux;
                }
                catch (FormatException)
                {
                    throw new FormatException($"Format de taux de change invalide : {tauxStr}");
                }
            }
            else
            {
                // Vérifier si c'est un problème de format de taux
                var tauxMatch = Regex.Match(ligne, @"\w{3}/EUR ?: ?(\S+)");
                if (tauxMatch.Success)
                {
                    var tauxStr = tauxMatch.Groups[1].Value;
                    if (!Regex.IsMatch(tauxStr, @"-?\d+(?:[.,]\d+)?"))
                    {
                        throw new FormatException($"Format de taux de change invalide : {tauxStr}");
                    }
                }
            }
        }

        private void ParseTransaction(string ligne, List<Transaction> transactions)
        {
            var champs = ligne.Split(';');
            
            // Fail fast : transaction incomplète
            if (champs.Length < 4)
            {
                throw new ArgumentException($"Transaction incomplète : {ligne}. Attendu 4 champs (Date;Montant;Devise;Categorie).");
            }
            
            try
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
            catch (FormatException ex)
            {
                throw new FormatException($"Format invalide dans la transaction : {ligne}. {ex.Message}");
            }
        }
    }
} 
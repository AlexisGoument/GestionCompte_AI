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

            var headerFound = false;
            for (int i = 0; i < lignes.Length; i++)
            {
                var ligne = lignes[i];
                var lineNumber = i + 1; // Les lignes commencent à 1
                
                if (ligne.StartsWith("Compte au"))
                {
                    ParseBalanceReference(ligne, donnees, lineNumber);
                }
                else if (ligne.Contains("/EUR"))
                {
                    ParseTauxDeChange(ligne, ref usd, ref jpy);
                }
                else if (ligne.Contains("Date;Montant;Devise;Categorie"))
                {
                    headerFound = true;
                }
                else if (ligne.Contains(";") && headerFound)
                {
                    ParseTransaction(ligne, donnees.Transactions, lineNumber);
                }
            }

            if (!headerFound && donnees.Transactions.Count == 0)
            {
                // Pas d'en-tête trouvé et aucune transaction
                throw new ArgumentException("En-tête CSV manquant (Date;Montant;Devise;Categorie).");
            }

            donnees.TauxDeChange = new TauxDeChange(usd, jpy);

            return donnees;
        }

        private void ParseBalanceReference(string ligne, DonneesCompte donnees, int lineNumber)
        {
            var match = Regex.Match(ligne, @"Compte au (\d{2}/\d{2}/\d{4}) ?: ?(-?\d+(?:[.,]\d+)?) EUR");
            if (match.Success)
            {
                donnees.DateBalanceReference = ParseDate(match.Groups[1].Value, lineNumber);
                donnees.BalanceReference = ParseDecimal(match.Groups[2].Value, lineNumber);
            }
            else
            {
                throw new FormatException($"Format de balance de référence invalide à la ligne {lineNumber} : {ligne}");
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
                    
                    if (devise == "USD")
                        usd = taux;
                    else if (devise == "JPY")
                        jpy = taux;
                }
                catch (FormatException)
                {
                    throw new FormatException($"Format de taux de change invalide : {tauxStr}");
                }
            }
            else
            {
                throw new FormatException($"Format de taux de change invalide dans la ligne : {ligne}");
            }
        }

        private void CheckTransactionFormat(string[] champs, string ligne, int lineNumber)
        {
            if (champs.Length < 4)
            {
                throw new ArgumentException($"Transaction incomplète à la ligne {lineNumber} : {ligne}. Attendu 4 champs (Date;Montant;Devise;Categorie).");
            }
        }

        private void ParseTransaction(string ligne, List<Transaction> transactions, int lineNumber)
        {
            var champs = ligne.Split(';');
            
            CheckTransactionFormat(champs, ligne, lineNumber);
            
            try
            {
                var date = ParseDate(champs[0], lineNumber);
                var montant = ParseDecimal(champs[1], lineNumber);
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
                throw new FormatException($"Format invalide dans la transaction à la ligne {lineNumber} : {ligne}. {ex.Message}");
            }
        }

        private static DateOnly ParseDate(string dateStr, int lineNumber)
        {
            try
            {
                return DateOnly.ParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new FormatException($"Format de date invalide à la ligne {lineNumber} : {dateStr}");
            }
        }

        private static decimal ParseDecimal(string decimalStr, int lineNumber)
        {
            try
            {
                return decimal.Parse(decimalStr.Replace(',', '.'), CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new FormatException($"Format de montant invalide à la ligne {lineNumber} : {decimalStr}");
            }
        }
    }
} 
using NUnit.Framework;
using GestionCompte;
using GestionCompte.Models;

namespace GestionCompteTests
{
    public class CompteParserTests
    {
        [Test]
        public void Parser_RetourneDonneesCorrectes_PourCsvMinimal()
        {
            var contenu =
                """
                Compte au 28/02/2023 : 8300.00 EUR
                JPY/EUR : 0.482
                USD/EUR : 1.445
                Date;Montant;Devise;Categorie
                06/10/2022;-504.61;EUR;Loisir
                """;
            var parser = new CompteParser();
            var donnees = parser.Parser(contenu);
            Assert.That(donnees.BalanceReference, Is.EqualTo(8300.00m));
            Assert.That(donnees.DateBalanceReference, Is.EqualTo(new DateOnly(2023, 2, 28)));
            Assert.That(donnees.TauxDeChange, Is.Not.Null);
            Assert.That(donnees.TauxDeChange.JPY, Is.EqualTo(0.482m));
            Assert.That(donnees.TauxDeChange.USD, Is.EqualTo(1.445m));
            Assert.That(donnees.Transactions, Has.Count.EqualTo(1));
            var t = donnees.Transactions[0];
            Assert.That(t.Date, Is.EqualTo(new DateOnly(2022, 10, 6)));
            Assert.That(t.Montant, Is.EqualTo(-504.61m));
            Assert.That(t.Devise, Is.EqualTo("EUR"));
            Assert.That(t.Categorie, Is.EqualTo("Loisir"));
        }
    }
} 
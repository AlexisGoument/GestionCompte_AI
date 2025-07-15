using NUnit.Framework;
using GestionCompte;
using GestionCompte.Models;

namespace GestionCompteTests
{
    public class CompteParserTests
    {
        [Test]
        public void Parser_AvecFichierMinimal()
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
            var transaction = donnees.Transactions[0];
            Assert.That(transaction.Date, Is.EqualTo(new DateOnly(2022, 10, 6)));
            Assert.That(transaction.Montant, Is.EqualTo(-504.61m));
            Assert.That(transaction.Devise, Is.EqualTo("EUR"));
            Assert.That(transaction.Categorie, Is.EqualTo("Loisir"));
        }

        [Test]
        public void Parser_SansBalanceReference()
        {
            var contenu = """
            JPY/EUR : 0.482
            USD/EUR : 1.445
            Date;Montant;Devise;Categorie
            06/10/2022;-504.61;EUR;Loisir
            """;
            var parser = new CompteParser();
            
            var donnees = parser.Parser(contenu);

            Assert.That(donnees.BalanceReference, Is.Null);
            Assert.That(donnees.DateBalanceReference, Is.Null);
            Assert.That(donnees.TauxDeChange, Is.Not.Null);
            Assert.That(donnees.TauxDeChange.JPY, Is.EqualTo(0.482m));
            Assert.That(donnees.TauxDeChange.USD, Is.EqualTo(1.445m));
            Assert.That(donnees.Transactions, Has.Count.EqualTo(1));
            var transaction = donnees.Transactions[0];
            Assert.That(transaction.Date, Is.EqualTo(new DateOnly(2022, 10, 6)));
            Assert.That(transaction.Montant, Is.EqualTo(-504.61m));
            Assert.That(transaction.Devise, Is.EqualTo("EUR"));
            Assert.That(transaction.Categorie, Is.EqualTo("Loisir"));
        }

        [Test]
        public void Parser_SansTransactions()
        {
            var contenu = """
            Compte au 28/02/2023 : 8300.00 EUR
            JPY/EUR : 0.482
            USD/EUR : 1.445
            Date;Montant;Devise;Categorie
            """;
            var parser = new CompteParser();

            var donnees = parser.Parser(contenu);

            Assert.That(donnees.BalanceReference, Is.EqualTo(8300.00m));
            Assert.That(donnees.DateBalanceReference, Is.EqualTo(new DateOnly(2023, 2, 28)));
            Assert.That(donnees.TauxDeChange, Is.Not.Null);
            Assert.That(donnees.TauxDeChange.JPY, Is.EqualTo(0.482m));
            Assert.That(donnees.TauxDeChange.USD, Is.EqualTo(1.445m));
            Assert.That(donnees.Transactions, Is.Empty);
        }

        [Test]
        public void Parser_SansTauxJPY()
        {
            var contenu = """
            Compte au 28/02/2023 : 8300.00 EUR
            USD/EUR : 1.445
            Date;Montant;Devise;Categorie
            06/10/2022;-504.61;EUR;Loisir
            """;
            var parser = new CompteParser();

            var donnees = parser.Parser(contenu);
            
            Assert.That(donnees.BalanceReference, Is.EqualTo(8300.00m));
            Assert.That(donnees.DateBalanceReference, Is.EqualTo(new DateOnly(2023, 2, 28)));
            Assert.That(donnees.TauxDeChange, Is.Not.Null);
            Assert.That(donnees.TauxDeChange.JPY, Is.EqualTo(1m));
            Assert.That(donnees.TauxDeChange.USD, Is.EqualTo(1.445m));
            Assert.That(donnees.Transactions, Has.Count.EqualTo(1));
            var transaction = donnees.Transactions[0];
            Assert.That(transaction.Date, Is.EqualTo(new DateOnly(2022, 10, 6)));
            Assert.That(transaction.Montant, Is.EqualTo(-504.61m));
            Assert.That(transaction.Devise, Is.EqualTo("EUR"));
            Assert.That(transaction.Categorie, Is.EqualTo("Loisir"));
        }

        [Test]
        public void Parser_SansHeader_ThrowArgumentException()
        {
            var contenu = """
            Compte au 28/02/2023 : 8300.00 EUR
            JPY/EUR : 0.482
            USD/EUR : 1.445
            06/10/2022;-504.61;EUR;Loisir
            """;
            var parser = new CompteParser();
            
            Assert.Throws<ArgumentException>(() => parser.Parser(contenu));
        }

        [Test]
        public void Parser_AvecVirgulesDansLesMontants()
        {
            var contenu = """
            Compte au 28/02/2023 : 8300,50 EUR
            JPY/EUR : 0,482
            USD/EUR : 1,445
            Date;Montant;Devise;Categorie
            06/10/2022;-504,61;EUR;Loisir
            07/10/2022;1200,75;USD;Salaire
            """;
            var parser = new CompteParser();

            var donnees = parser.Parser(contenu);

            Assert.That(donnees.BalanceReference, Is.EqualTo(8300.50m));
            Assert.That(donnees.TauxDeChange.JPY, Is.EqualTo(0.482m));
            Assert.That(donnees.TauxDeChange.USD, Is.EqualTo(1.445m));
            Assert.That(donnees.Transactions, Has.Count.EqualTo(2));
            Assert.That(donnees.Transactions[0].Montant, Is.EqualTo(-504.61m));
            Assert.That(donnees.Transactions[1].Montant, Is.EqualTo(1200.75m));
        }

        [Test]
        public void Parser_FormatDateInvalideBalanceReference_ThrowsException()
        {
            var contenu = """
            Compte au 2023/02/28 : 8300.00 EUR
            JPY/EUR : 0.482
            USD/EUR : 1.445
            Date;Montant;Devise;Categorie
            06/10/2022;-504.61;EUR;Loisir
            """;
            var parser = new CompteParser();

            Assert.Throws<FormatException>(() => parser.Parser(contenu));
        }

        [Test]
        public void Parser_FormatDateInvalideTransaction_ThrowsException()
        {
            var contenu = """
            Compte au 28/02/2023 : 8300.00 EUR
            JPY/EUR : 0.482
            USD/EUR : 1.445
            Date;Montant;Devise;Categorie
            2022/10/06;-504.61;EUR;Loisir
            """;
            var parser = new CompteParser();

            Assert.Throws<FormatException>(() => parser.Parser(contenu));
        }

        [Test]
        public void Parser_FormatMontantInvalideBalanceReference_ThrowsException()
        {
            var contenu = """
            Compte au 28/02/2023 : abc EUR
            JPY/EUR : 0.482
            USD/EUR : 1.445
            Date;Montant;Devise;Categorie
            06/10/2022;-504.61;EUR;Loisir
            """;
            var parser = new CompteParser();

            Assert.Throws<FormatException>(() => parser.Parser(contenu));
        }

        [Test]
        public void Parser_FormatMontantInvalideTransaction_ThrowsException()
        {
            var contenu = """
            Compte au 28/02/2023 : 8300.00 EUR
            JPY/EUR : 0.482
            USD/EUR : 1.445
            Date;Montant;Devise;Categorie
            06/10/2022;abc;EUR;Loisir
            """;
            var parser = new CompteParser();

            Assert.Throws<FormatException>(() => parser.Parser(contenu));
        }

        [Test]
        public void Parser_FormatTauxInvalide_ThrowsException()
        {
            var contenu = """
            Compte au 28/02/2023 : 8300.00 EUR
            JPY/EUR : abc
            USD/EUR : 1.445
            Date;Montant;Devise;Categorie
            06/10/2022;-504.61;EUR;Loisir
            """;
            var parser = new CompteParser();

            Assert.Throws<FormatException>(() => parser.Parser(contenu));
        }

        [Test]
        public void Parser_TransactionIncomplete_ThrowsException()
        {
            var contenu = """
            Compte au 28/02/2023 : 8300.00 EUR
            JPY/EUR : 0.482
            USD/EUR : 1.445
            Date;Montant;Devise;Categorie
            06/10/2022;-504.61;EUR;Loisir
            07/10/2022;100.00;USD
            08/10/2022;-50.00;EUR;Nourriture
            """;
            var parser = new CompteParser();

            Assert.Throws<ArgumentException>(() => parser.Parser(contenu));
        }

        [Test]
        public void Parser_ContenuVide_ThrowsException()
        {
            var contenu = "";
            var parser = new CompteParser();

            Assert.Throws<ArgumentException>(() => parser.Parser(contenu));
        }

        [Test]
        public void Parser_SeulementLignesVides_ThrowsException()
        {
            var contenu = "";
            var parser = new CompteParser();

            Assert.Throws<ArgumentException>(() => parser.Parser(contenu));
        }

        [Test]
        public void Parser_SansAucunTauxDeChange()
        {
            var contenu = """
            Compte au 28/02/2023 : 8300.00 EUR
            Date;Montant;Devise;Categorie
            06/10/2022;-504.61;EUR;Loisir
            """;
            var parser = new CompteParser();

            var donnees = parser.Parser(contenu);
            
            Assert.That(donnees.BalanceReference, Is.EqualTo(8300.00m));
            Assert.That(donnees.DateBalanceReference, Is.EqualTo(new DateOnly(2023, 2, 28)));
            Assert.That(donnees.TauxDeChange, Is.Not.Null);
            Assert.That(donnees.TauxDeChange.JPY, Is.EqualTo(1m));
            Assert.That(donnees.TauxDeChange.USD, Is.EqualTo(1m));
            Assert.That(donnees.Transactions, Has.Count.EqualTo(1));
        }
    }
} 
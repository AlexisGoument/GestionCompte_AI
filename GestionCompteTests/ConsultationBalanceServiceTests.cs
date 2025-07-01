using NUnit.Framework;
using System;
using System.Collections.Generic;
using GestionCompte;
using GestionCompte.Models;

namespace GestionCompte.Tests
{
    public class ConsultationBalanceServiceTests
    {
        private decimal? balanceReference;
        private DateOnly dateBalanceReference;
        private TauxDeChange tauxDeChange;
        private List<Transaction> transactions;
        private DateOnly dateCible;

        [SetUp]
        public void SetUp()
        {
            balanceReference = 1000m;
            dateBalanceReference = new DateOnly(2023, 1, 1);
            tauxDeChange = new TauxDeChange(1.5m, 0.5m);
            transactions = new List<Transaction>();
            dateCible = new DateOnly(2023, 2, 1);
        }

        [Test]
        public void Doit_retourner_la_balance_de_reference_si_aucune_transaction()
        {
            var service = new ConsultationBalanceService(balanceReference, dateBalanceReference, transactions, tauxDeChange);

            var balance = service.ObtenirBalancePour(dateCible);

            Assert.That(balance, Is.EqualTo(1000.00m));
        }

        [Test]
        public void Doit_ajouter_les_transactions_EUR_a_la_balance_reference()
        {
            transactions = new List<Transaction>
            {
                new() { Date = new DateOnly(2023, 1, 2), Montant = -100m, Devise = "EUR", Categorie = "Test" },
                new() { Date = new DateOnly(2023, 1, 3), Montant = 50m, Devise = "EUR", Categorie = "Test" }
            };
            var service = new ConsultationBalanceService(balanceReference, dateBalanceReference, transactions, tauxDeChange);

            var balance = service.ObtenirBalancePour(dateCible);

            Assert.That(balance, Is.EqualTo(950m));
        }

        [Test]
        public void Doit_convertir_et_ajouter_les_transactions_USD_et_JPY()
        {
            transactions = new List<Transaction>
            {
                new() { Date = new DateOnly(2023, 1, 2), Montant = 100m, Devise = "USD", Categorie = "Test" },
                new() { Date = new DateOnly(2023, 1, 3), Montant = -200m, Devise = "JPY", Categorie = "Test" }
            };
            var service = new ConsultationBalanceService(balanceReference, dateBalanceReference, transactions, tauxDeChange);

            var balance = service.ObtenirBalancePour(dateCible);

            Assert.That(balance, Is.EqualTo(1050m));
        }

        [Test]
        public void Doit_retourner_la_somme_des_transactions_si_pas_de_solde_de_reference()
        {
            balanceReference = null;
            transactions = new List<Transaction>
            {
                new() { Date = new DateOnly(2023, 1, 2), Montant = 100m, Devise = "EUR", Categorie = "Test" },
                new() { Date = new DateOnly(2023, 1, 3), Montant = -50m, Devise = "EUR", Categorie = "Test" }
            };
            var service = new ConsultationBalanceService(balanceReference, dateBalanceReference, transactions, tauxDeChange);

            var balance = service.ObtenirBalancePour(dateCible);

            Assert.That(balance, Is.EqualTo(50m));
        }

        [Test]
        public void Doit_retourner_0_si_aucun_solde_ni_transaction()
        {
            balanceReference = null;
            var service = new ConsultationBalanceService(balanceReference, dateBalanceReference, transactions, tauxDeChange);

            var balance = service.ObtenirBalancePour(dateCible);

            Assert.That(balance, Is.EqualTo(0m));
        }

        [Test]
        public void Doit_ignorer_les_transactions_posterieures_a_la_date_cible()
        {
            transactions = new List<Transaction>
            {
                new() { Date = new DateOnly(2023, 1, 2), Montant = 100m, Devise = "EUR", Categorie = "Test" },
                new() { Date = new DateOnly(2023, 3, 1), Montant = 999m, Devise = "EUR", Categorie = "Test" }
            };
            var service = new ConsultationBalanceService(balanceReference, dateBalanceReference, transactions, tauxDeChange);

            var balance = service.ObtenirBalancePour(dateCible);

            Assert.That(balance, Is.EqualTo(1100m));
        }

        [Test]
        public void Exception_si_devise_inconnue()
        {
            transactions = new List<Transaction>
            {
                new() { Date = new DateOnly(2023, 1, 2), Montant = 100m, Devise = "GBP", Categorie = "Test" }
            };
            var service = new ConsultationBalanceService(balanceReference, dateBalanceReference, transactions, tauxDeChange);

            Assert.Throws<InvalidOperationException>(() => service.ObtenirBalancePour(dateCible));
        }

        [Test]
        public void Exception_si_taux_de_change_manquant_pour_USD()
        {
            transactions = new List<Transaction>
            {
                new() { Date = new DateOnly(2023, 1, 2), Montant = 100m, Devise = "USD", Categorie = "Test" }
            };
            TauxDeChange? tauxDeChangeAbsent = null;
            var service = new ConsultationBalanceService(balanceReference, dateBalanceReference, transactions, tauxDeChangeAbsent);

            Assert.Throws<InvalidOperationException>(() => service.ObtenirBalancePour(dateCible));
        }

        [Test]
        public void Doit_inclure_la_transaction_du_jour_dans_le_calcul()
        {
            transactions = new List<Transaction>
            {
                new() { Date = new DateOnly(2023, 2, 1), Montant = 100m, Devise = "EUR", Categorie = "Test" }
            };
            var service = new ConsultationBalanceService(balanceReference, dateBalanceReference, transactions, tauxDeChange);

            var balance = service.ObtenirBalancePour(dateCible);

            Assert.That(balance, Is.EqualTo(1100m));
        }
    }
} 
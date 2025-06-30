using NUnit.Framework;
using System;
using System.Collections.Generic;
using GestionCompte;
using GestionCompte.Models;

namespace GestionCompte.Tests
{
    public class ConsultationBalanceServiceTests
    {
        [Test]
        public void Doit_retourner_la_balance_de_reference_si_aucune_transaction()
        {
            var dateCible = new DateOnly(2022, 1, 1);
            var balanceReference = 8300.00m;
            var dateBalanceReference = new DateOnly(2023, 2, 28);
            var transactions = new List<Transaction>();
            var tauxDeChange = new TauxDeChange(1.445m, 0.482m);
            var service = new ConsultationBalanceService(
                balanceReference, dateBalanceReference, transactions, tauxDeChange
            );

            var balance = service.ObtenirBalancePour(dateCible);

            Assert.AreEqual(8300.00m, balance);
        }
    }
} 
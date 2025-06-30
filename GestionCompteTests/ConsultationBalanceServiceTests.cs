using NUnit.Framework;
using System;
using System.Collections.Generic;
using GestionCompte;

namespace GestionCompte.Tests
{
    public class ConsultationBalanceServiceTests
    {
        [Test]
        public void Doit_retourner_la_balance_de_reference_si_aucune_transaction()
        {
            // Arrange
            var dateCible = new DateOnly(2022, 1, 1);
            var balanceReference = 8300.00m;
            var dateBalanceReference = new DateOnly(2023, 2, 28);
            var transactions = new List<Transaction>(); // aucune transaction
            var tauxDeChange = new TauxDeChange { USD = 1.445m, JPY = 0.482m };

            var service = new ConsultationBalanceService(
                balanceReference, dateBalanceReference, transactions, tauxDeChange
            );

            // Act
            var balance = service.ObtenirBalancePour(dateCible);

            // Assert
            Assert.AreEqual(8300.00m, balance);
        }
    }
} 
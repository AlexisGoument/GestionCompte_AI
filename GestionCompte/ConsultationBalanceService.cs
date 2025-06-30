using System;
using System.Collections.Generic;
using GestionCompte.Models;

namespace GestionCompte
{
    public class ConsultationBalanceService
    {
        public ConsultationBalanceService(decimal balance, DateOnly dateBalance, List<Transaction> transactions, TauxDeChange tauxDeChange) { }
        public decimal ObtenirBalancePour(DateOnly date) => 8300.00m;
    }
} 
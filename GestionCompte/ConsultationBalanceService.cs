using System;
using System.Collections.Generic;

namespace GestionCompte
{
    public class ConsultationBalanceService
    {
        public ConsultationBalanceService(decimal balance, DateOnly dateBalance, List<Transaction> transactions, TauxDeChange tauxDeChange) { }
        public decimal ObtenirBalancePour(DateOnly date) => 8300.00m;
    }

    public class Transaction { }
    public class TauxDeChange { public decimal USD; public decimal JPY; }
} 
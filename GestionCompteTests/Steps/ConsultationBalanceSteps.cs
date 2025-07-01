using Reqnroll;
using System.Globalization;
using GestionCompte;
using GestionCompte.Models;

namespace GestionCompteTests.Steps;

[Binding]
public class ConsultationBalanceSteps
{
    private decimal? _balanceReference;
    private DateOnly? _dateBalanceReference;
    private List<Transaction> _transactions = new();
    private TauxDeChange? _tauxDeChange;
    private decimal? _balanceResultat;

    [Given(@"^le solde de référence au (\d+\/\d+\/\d+) est de (-?\d+(?:.\d+)?) EUR$")]
    public void GivenSoldeReference(string date, string balanceStr)
    {
        _dateBalanceReference = DateOnly.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        _balanceReference = decimal.Parse(balanceStr, CultureInfo.GetCultureInfo("en-US"));
    }

    [Given("les taux de change sont les suivants:")]
    public void GivenTauxDeChange(Table table)
    {
        decimal usd = 1m, jpy = 1m;
        foreach (var row in table.Rows)
        {
            if (row["Devise"] == "USD") usd = decimal.Parse(row["Taux"], CultureInfo.InvariantCulture);
            if (row["Devise"] == "JPY") jpy = decimal.Parse(row["Taux"], CultureInfo.InvariantCulture);
        }
        _tauxDeChange = new TauxDeChange(usd, jpy);
    }

    [Given("les transactions suivantes existent:")]
    public void GivenTransactions(Table table)
    {
        foreach (var row in table.Rows)
        {
            var date = DateOnly.ParseExact(row["Date"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var montant = decimal.Parse(row["Montant"], CultureInfo.InvariantCulture);
            var devise = row["Devise"];
            var categorie = row["Catégorie"];
            _transactions.Add(new Transaction
            {
                Date = date,
                Montant = montant,
                Devise = devise,
                Categorie = categorie
            });
        }
    }

    [Given("il n'existe pas de solde de référence")]
    public void GivenNoSoldeReference()
    {
        _balanceReference = null;
        _dateBalanceReference = null;
    }

    [Given("il n'existe aucune transaction")]
    public void GivenNoTransaction()
    {
        _transactions.Clear();
    }

    [When("je consulte la balance du compte au {DateTime}")]
    public void WhenJeConsulteLaBalance(DateTime date)
    {
        var dateCible = DateOnly.FromDateTime(date);
        var service = new ConsultationBalanceService(
            _balanceReference,
            _dateBalanceReference,
            _transactions,
            _tauxDeChange
        );
        _balanceResultat = service.ObtenirBalancePour(dateCible);
    }

    [Then(@"^la balance affichée est (-?\d+(?:.\d+)?) EUR$")]
    public void ThenLaBalanceAfficheeEst(string balanceStr)
    {
        var balanceAttendue = decimal.Parse(balanceStr, CultureInfo.GetCultureInfo("en-US"));
        Assert.That(_balanceResultat, Is.EqualTo(balanceAttendue));
    }
} 
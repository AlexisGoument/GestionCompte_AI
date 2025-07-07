using Reqnroll;
using System.Globalization;
using GestionCompte;
using GestionCompte.Models;
using System.Text;

namespace GestionCompteTests.Steps;

[Binding]
public class ConsultationBalanceSteps
{
    private decimal? _balanceResultat;
    private readonly ICompteParser _parser;
    private StringBuilder _fileBuilder = new StringBuilder();

    public ConsultationBalanceSteps()
    {
        _parser = new CompteParser();
    }

    [Given(@"^le solde de référence au (\d+\/\d+\/\d+) est de (-?\d+(?:.\d+)?) EUR$")]
    public void GivenSoldeReference(string date, string balanceStr)
    {
        _fileBuilder.AppendLine($"Compte au {date} : {balanceStr} EUR");
    }

    [Given("les taux de change sont les suivants:")]
    public void GivenTauxDeChange(Table table)
    {
        foreach (var row in table.Rows)
        {
            _fileBuilder.AppendLine($"{row["Devise"]}/EUR : {row["Taux"]}");
        }
    }

    [Given("les transactions suivantes existent:")]
    public void GivenTransactions(Table table)
    {
        _fileBuilder.AppendLine("Date;Montant;Devise;Categorie");
        foreach (var row in table.Rows)
        {
            _fileBuilder.AppendLine($"{row["Date"]};{row["Montant"]};{row["Devise"]};{row["Catégorie"]}");
        }
    }

    [Given("il n'existe pas de solde de référence")]
    public void GivenNoSoldeReference()
    {
        // Ne rien ajouter au CSV
    }

    [Given("il n'existe aucune transaction")]
    public void GivenNoTransaction()
    {
        // Ajoute juste l'en-tête si besoin
        _fileBuilder.AppendLine("Date;Montant;Devise;Categorie");
    }

    [When("je consulte la balance du compte au {DateTime}")]
    public void WhenJeConsulteLaBalance(DateTime date)
    {
        var donnees = _parser.Parser(_fileBuilder.ToString());
        var dateCible = DateOnly.FromDateTime(date);
        _balanceResultat = ConsultationBalanceService.ObtenirBalancePour(
            dateCible,
            donnees.BalanceReference,
            donnees.Transactions,
            donnees.TauxDeChange
        );
    }

    [Then(@"^la balance affichée est (-?\d+(?:.\d+)?) EUR$")]
    public void ThenLaBalanceAfficheeEst(string balanceStr)
    {
        var balanceAttendue = decimal.Parse(balanceStr, CultureInfo.GetCultureInfo("en-US"));
        Assert.That(_balanceResultat, Is.EqualTo(balanceAttendue));
    }
} 
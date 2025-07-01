using Reqnroll;
using System.Globalization;

namespace GestionCompteTests.Steps;

[Binding]
public class ConsultationBalanceSteps
{
    [Given(@"^le solde de référence au (\d+\/\d+\/\d+) est de (-?\d+(?:.\d+)?) EUR$")]
    public void GivenSoldeReference(string date, string balanceStr)
    {
        var balance = decimal.Parse(balanceStr, CultureInfo.GetCultureInfo("en-US"));
        // À implémenter
    }

    [Given("les taux de change sont les suivants:")]
    public void GivenTauxDeChange(Table table)
    {
        // À implémenter
    }

    [Given("les transactions suivantes existent:")]
    public void GivenTransactions(Table table)
    {
        // À implémenter
    }

    [Given("il n'existe pas de solde de référence")]
    public void GivenNoSoldeReference()
    {
        // À implémenter
    }

    [Given("il n'existe aucune transaction")]
    public void GivenNoTransaction()
    {
        // À implémenter
    }

    [When("je consulte la balance du compte au {DateTime}")]
    public void WhenJeConsulteLaBalance(DateTime date)
    {
        // À implémenter
    }

    [Then(@"^la balance affichée est (-?\d+(?:.\d+)?) EUR$")]
    public void ThenLaBalanceAfficheeEst(string balanceStr)
    {
        var balanceAttendue = decimal.Parse(balanceStr, CultureInfo.GetCultureInfo("en-US"));
        // À implémenter
    }
} 
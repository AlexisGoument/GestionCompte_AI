using Reqnroll;
using System.Globalization;

namespace GestionCompteTests.Steps;

[Binding]
public class ConsultationBalanceSteps
{
    [Given("le solde de référence au {DateTime} est de {int}.{int} EUR")]
    public void EtantDonneSoldeReference(DateTime date, int unite, int @decimal)
    {
        var balanceStr = $"{unite}.{@decimal}";
        var balance = decimal.Parse(balanceStr, CultureInfo.GetCultureInfo("en-US"));
    }    

    [Given("les taux de change sont les suivants:")]
    public void GivenTauxDeChange(Table table)
    {
        // À implémenter
    }

    [Given("que les transactions suivantes existent:")]
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

    [When("je consulte la balance du compte au {int}\\/{int}\\/{int}")]
    public void WhenJeConsulteLaBalance(int jour, int mois, int annee)
    {
        // À implémenter
    }

    [Then("la balance affichée est {int}.{int} EUR")]
    public void ThenLaBalanceAfficheeEst(int unite, int @decimal)
    {
        var balanceStr = $"{unite}.{@decimal}";
        var balanceAttendue = decimal.Parse(balanceStr, CultureInfo.GetCultureInfo("en-US"));
        // À implémenter
    }

    [Then("la balance affichée est {int} EUR")]
    public void ThenLaBalanceAfficheeEst(int balanceAttendue)
    {
        // À implémenter
    }
} 
using GestionCompte.Models;

namespace GestionCompte
{
    public interface ICompteParser
    {
        DonneesCompte Parser(string contenuCsv);
    }
} 
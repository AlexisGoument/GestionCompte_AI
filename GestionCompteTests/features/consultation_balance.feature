Fonctionnalité: Consultation de la balance du compte à une date donnée

  En tant qu'utilisateur
  Je veux connaître la balance de mon compte à une date précise
  Afin de suivre l'évolution de mes finances

  Contexte:
    Étant donné que le solde de référence au 28/02/2023 est de 8300.00 EUR
    Et que les taux de change sont les suivants:
      | Devise | Taux  |
      | JPY    | 0.482 |
      | USD    | 1.445 |
    Et que les transactions suivantes existent:
      | Date       | Montant  | Devise | Catégorie  |
      | 06/10/2022 | -504.61  | EUR    | Loisir     |
      | 15/01/2023 | 1200.00  | USD    | Salaire    |
      | 01/02/2023 | -30000   | JPY    | Alimentation |

  Scénario: Afficher la balance à une date antérieure à toutes les transactions
    Quand je consulte la balance du compte au 01/01/2022
    Alors la balance affichée est 8300.00 EUR

  Scénario: Afficher la balance à une date entre deux transactions
    Quand je consulte la balance du compte au 10/10/2022
    Alors la balance affichée est 7795.39 EUR

  Scénario: Afficher la balance à une date après plusieurs transactions en devises
    Quand je consulte la balance du compte au 02/02/2023
    Alors la balance affichée est -4930.61 EUR

  Scénario: Afficher la balance à une date sans solde de référence ni transaction
    Étant donné qu'il n'existe pas de solde de référence
    Et qu'il n'existe aucune transaction
    Quand je consulte la balance du compte au 01/01/2022
    Alors la balance affichée est 0 EUR 
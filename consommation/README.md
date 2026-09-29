# Consommation – suivi de la consommation électrique

Application console en **C#** connectée à une base de données **MySQL**, permettant de suivre la consommation électrique d'un bâtiment : zones, types d'appareils, appareils et périodes d'utilisation.

Réalisée comme **projet d'examen** du cours de Gestion et exploitation de bases de données (IPAMC La Louvière).

## Fonctionnalités (13, réparties sur 4 menus)

**Zones**
- Lister, ajouter, modifier et supprimer une zone
- Refus de supprimer une zone qui contient encore des appareils

**Types d'appareil**
- Lister, ajouter, modifier et supprimer un type
- Chaque type peut définir **soit une puissance (W), soit une consommation directe, soit aucune des deux**
- Refus de supprimer un type utilisé par des appareils

**Appareils**
- Lister les appareils par zone, avec consommation unitaire et consommation maximale (× quantité)
- Ajouter un appareil (valeurs propres ou héritées de son type)
- Supprimer un appareil et ses utilisations (avec confirmation)
- Enregistrer une utilisation : quantité utilisée, pourcentage d'utilisation, période de début et de fin

**Rapport**
- Consommation d'une zone sur une période : détail par appareil (quantité, pourcentage, durée) et **total global**

## Points techniques

- **Requêtes SQL paramétrées** (`?`) pour se protéger des injections SQL
- **Jointures** sur 3 tables (`utilisation` → `appareil` → `type_appareil`) et filtrage par période
- **Gestion des valeurs `NULL`** (`DBNull`) : la consommation d'un appareil est déterminée par ordre de priorité
  1. consommation propre de l'appareil
  2. puissance propre de l'appareil × facteur de marche
  3. consommation définie par son type
  4. puissance définie par son type × facteur de marche
- **Calculs en `decimal`** pour éviter les erreurs d'arrondi des nombres à virgule flottante
- **Validation des saisies** : bornes numériques, format de date strict (`yyyy-MM-dd HH:mm:ss`), fin de période postérieure au début
- **Intégrité référentielle** vérifiée avant chaque suppression
- Affichage en colonnes alignées, avec troncature des textes trop longs

## Structure de la base de données

| Table | Rôle | Colonnes principales |
|---|---|---|
| `zone` | Zones du bâtiment | `id`, `nom`, `commentaire` |
| `type_appareil` | Types d'appareils | `id`, `nom`, `commentaire`, `facteur_de_marche`, `puissance`, `consommation` |
| `appareil` | Appareils installés | `id`, `denomination`, `ref_type`, `ref_zone`, `quantite`, `puissance`, `consommation` |
| `utilisation` | Périodes d'utilisation | `id`, `ref_appareil`, `quantite`, `pourcentage`, `debut_periode`, `fin_periode` |

## Lancer le projet

1. Dans MySQL (phpMyAdmin > Importer), exécuter `sql/1-consommation-structure.sql` : crée la base, les tables et l'utilisateur `u_conso` (mot de passe `MDP`). La base démarre vide : zones, types, appareils et utilisations s'encodent depuis l'application.
2. Ouvrir le projet dans Visual Studio et le lancer : les paramètres de connexion de `Program.cs` correspondent déjà à l'utilisateur créé.

> Le projet s'ouvre via la solution `CSharpMySqlConsoleApps.sln` à la racine du dépôt, qui inclut la bibliothèque GEBD.

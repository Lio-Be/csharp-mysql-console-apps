# Journal de quêtes

Application console en **C#** connectée à une base de données **MySQL**, permettant de gérer le journal de quêtes d'un jeu de rôle : régions, entités (objets, personnages…), caractéristiques et quêtes détaillées.

Réalisée dans le cadre du cours de Gestion et exploitation de bases de données (IPAMC La Louvière).

## Fonctionnalités

**Régions, entités, caractéristiques et quêtes**
- Lister, ajouter, modifier/renommer et supprimer chaque élément
- Contrôle des doublons avant chaque ajout ou modification
- Refus de supprimer un élément encore utilisé (région contenant des quêtes, entité ou caractéristique utilisée dans un détail, quête possédant des détails)

**Détails de quête**
- Chaque détail associe une quête à une caractéristique de l'un des 3 types :
  - **T** – texte descriptif (ex. : objectif)
  - **M** – montant (ex. : récompense en pièces d'or)
  - **Q** – quantité d'une entité (ex. : 5 × Peau de loup)
- Afficher, ajouter, modifier et supprimer les détails d'une quête

**Recherche**
- Recherche par mot-clé dans le nom et la description des quêtes, les valeurs des détails et les noms des entités

## Points techniques

- **Requêtes SQL paramétrées** (`?`) pour se protéger des injections SQL, y compris pour les recherches `LIKE`
- **Jointures** (`INNER JOIN`, `LEFT JOIN`) sur 4 tables, **agrégats** (`COUNT`, `COUNT(DISTINCT)`) et `GROUP BY`
- **Clé étrangère facultative** (`ref_entite` à `NULL` sauf pour les détails de type Quantité)
- **Intégrité référentielle** vérifiée avant chaque suppression
- **Validation des saisies** : longueurs de texte, bornes numériques

## Structure de la base de données

| Table | Rôle | Colonnes principales |
|---|---|---|
| `region` | Régions du monde | `id`, `nom` |
| `entite` | Objets, créatures, personnages… | `id`, `nom` |
| `caracteristique` | Types d'informations d'une quête | `id`, `nom`, `type` (T / M / Q) |
| `quete` | Quêtes | `id`, `nom`, `description`, `ref_region` |
| `quete_detail` | Détails d'une quête | `id`, `ref_quete`, `ref_caracteristique`, `valeur`, `ref_entite` |

## Lancer le projet

1. Dans MySQL (phpMyAdmin > Importer), exécuter dans cet ordre :
   - `sql/1-quetes-structure.sql` : crée la base, les tables et l'utilisateur `u_quetes` (mot de passe `MDP`)
   - `sql/2-quetes-donnees.sql` : ajoute des données de démonstration
2. Ouvrir le projet dans Visual Studio et le lancer : les paramètres de connexion de `Program.cs` correspondent déjà à l'utilisateur créé.

> Le projet s'ouvre via la solution `CSharpMySqlConsoleApps.sln` à la racine du dépôt, qui inclut la bibliothèque GEBD.

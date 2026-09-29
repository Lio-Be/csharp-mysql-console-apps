# Multimédia – gestion de médiathèque

Application console en **C#** connectée à une base de données **MySQL**, permettant de gérer une médiathèque : catégories, médias et playlists.

Projet réalisé dans le cadre du **Bachelier en informatique – orientation développement d'applications** (IPAMC La Louvière).

## Fonctionnalités

**Catégories**
- Lister les catégories avec le nombre de médias associés
- Ajouter, renommer et supprimer une catégorie
- Refus de supprimer une catégorie qui contient encore des médias

**Médias**
- Lister les médias avec leur catégorie, leur durée et leur fichier
- Ajouter un média (saisie de la durée en secondes, convertie au format `HH:MM:SS`)
- Renommer et supprimer un média
- Refus de supprimer un média utilisé dans une playlist

**Playlists**
- Créer, renommer et supprimer une playlist (avec confirmation)
- Ajouter un média à une playlist : la position est calculée automatiquement
- Retirer un média : les positions suivantes sont réorganisées automatiquement
- Consulter une playlist : liste ordonnée des médias, nombre de médias et durée totale

## Points techniques

- **Requêtes SQL paramétrées** (`?`) pour se protéger des injections SQL
- **Jointures** (`INNER JOIN`, `LEFT JOIN`) et **agrégats** (`COUNT`, `MAX`, `SUM`)
- Calcul de la durée totale directement en SQL avec `SUM(TIME_TO_SEC(...))`
- **Contrôles d'intégrité** : vérification des doublons et des dépendances avant chaque suppression
- **Réorganisation des positions** d'une playlist avec `UPDATE ... ORDER BY`, pour respecter la contrainte d'unicité `(ref_liste, ordre)` pendant le décalage

## Structure de la base de données

| Table       | Rôle                                   | Colonnes principales                          |
|-------------|----------------------------------------|-----------------------------------------------|
| `categorie` | Catégories de médias                   | `id`, `nom`                                   |
| `media`     | Médias (morceaux, vidéos…)             | `id`, `titre`, `nomfichier`, `duree`, `ref_categorie` |
| `liste`     | Playlists                              | `id`, `nom`                                   |
| `detail`    | Contenu des playlists (table de liaison) | `id`, `ref_liste`, `ref_media`, `ordre`     |

## Technologies

- C# / .NET
- MySQL
- Bibliothèque `GEBD` (accès MySQL et interface console, fournie dans le cadre du cours)

## Lancer le projet

1. Dans MySQL (phpMyAdmin > Importer), exécuter dans cet ordre :
   - `sql/1-multimedia-structure.sql` : crée la base, les tables et l'utilisateur `u_multimedia` (mot de passe `MDP`)
   - `sql/2-multimedia-donnees.sql` : ajoute des données de démonstration
2. Ouvrir le projet dans Visual Studio et le lancer : les paramètres de connexion de `Program.cs` correspondent déjà à l'utilisateur créé.

> Le projet s'ouvre via la solution `CSharpMySqlConsoleApps.sln` à la racine du dépôt, qui inclut la bibliothèque GEBD.

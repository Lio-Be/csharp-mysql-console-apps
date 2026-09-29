# Applications console C# / MySQL

Applications de gestion en mode console, développées en **C#** avec une base de données **MySQL**, dans le cadre du cours de **Gestion et exploitation de bases de données** du Bachelier en informatique – orientation développement d'applications (IPAMC La Louvière).

Chaque application met en œuvre un CRUD complet sur plusieurs tables liées, avec une attention particulière à la **sécurité** et à l'**intégrité des données**.

## Applications

| Application | Description | Tables |
|---|---|---|
| [**Multimédia**](multimedia/) | Gestion d'une médiathèque : catégories, médias et playlists ordonnées | 4 |
| [**Journal de quêtes**](quetes/) | Gestion de quêtes, régions, entités et caractéristiques | 5 |
| [**Consommation**](consommation/) | Suivi de la consommation électrique d'un bâtiment – projet d'examen (13 fonctionnalités, 4 menus) | 4 |

## Compétences mises en œuvre

- **Requêtes SQL paramétrées** partout, pour se protéger des injections SQL
- **Jointures** (`INNER JOIN`, `LEFT JOIN`), **agrégats** (`COUNT`, `SUM`, `MAX`) et `GROUP BY`
- **Intégrité référentielle** : vérification des dépendances avant chaque suppression
- **Unicité** : contrôle des doublons avant chaque ajout ou modification
- **Validation des saisies** (longueurs, bornes numériques, formats)
- Gestion des valeurs `NULL` (`DBNull`) et des types nullables en C#
- Conversions de types C# ↔ MySQL (dates, durées, décimaux au format belge)

## Technologies

- C# / .NET Framework 4.8.1
- MySQL 8
- Visual Studio 2022
- Bibliothèque **GEBD** (accès MySQL et interface console), fournie dans le cadre du cours – voir ci-dessous

## Bibliothèque GEBD

Les applications s'appuient sur la bibliothèque **GEBD**, fournie par M. Mahieu dans le cadre du cours et publiée ici avec son accord :

| Projet | Rôle |
|---|---|
| `GEBD` | Outils génériques |
| `GEBD_MySql` | Accès à la base de données (connexion, requêtes paramétrées, lecture des résultats) |
| `GEBD_UI` | Affichage et saisies contrôlées en console |

Dans le cadre du cours, j'ai complété moi-même une partie de `GEBD_MySql` : l'exécution des requêtes (`Execute`), la préparation des requêtes paramétrées (`?` et `@nom`) et la lecture des résultats (`GetValue`, `GetRow`).

`GEBD_MySql` utilise le connecteur officiel **MySQL Connector/NET** (`MySql.Data.dll`, © Oracle, licence GPLv2 avec exception FOSS).

## Lancer une application

1. Importer les scripts SQL de l'application dans MySQL (phpMyAdmin > Importer) : structure puis données de démonstration. Le README de chaque dossier précise l'ordre.
2. Ouvrir `CSharpMySqlConsoleApps.sln` dans Visual Studio 2022.
3. Clic droit sur l'application voulue > **Définir comme projet de démarrage**, puis lancer.

Les scripts créent un utilisateur MySQL dédié avec le mot de passe `MDP`, déjà configuré dans le code.

## Auteur

**Lionel Zinque** – [LinkedIn](https://www.linkedin.com/in/lionel-zinque-5026a92a2) · [GitHub](https://github.com/Lio-Be)

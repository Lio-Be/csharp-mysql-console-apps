using System;
using System.Collections.Generic;
using GEBD.MySql;
using GEBD.UI;

namespace Multimedia
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connexion à la base de données
            var parametres = DbSettings.Create("localhost", "u_multimedia", "MDP", "multimedia");
            Db bd = new Db(parametres);

            // Boucle du menu principal
            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION MULTIMEDIA ===\n\n");
                UI.DisplayPrompt("1. Gestion des Catégories\n");
                UI.DisplayPrompt("2. Gestion des Médias\n");
                UI.DisplayPrompt("3. Gestion des Playlists\n");
                UI.DisplayPrompt("4. Consulter une Playlist\n");
                UI.DisplayPrompt("0. Quitter\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 4);

                switch (choix)
                {
                    case 1:
                        MenuCategories(bd);
                        break;
                    case 2:
                        MenuMedias(bd);
                        break;
                    case 3:
                        MenuListes(bd);
                        break;
                    case 4:
                        ConsulterPlaylists(bd);
                        break;
                    case 0:
                        continuer = false;
                        UI.DisplayPrompt("\n Au revoir !\n");
                        break;
                }
            }

            UI.WaitKey(ConsoleKey.Enter, "fermer", true);
        }

        // ========================================
        // MENU CATÉGORIES
        // ========================================
        private static void MenuCategories(Db bd)
        {
            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION DES CATÉGORIES ===\n\n");
                UI.DisplayPrompt("1. Lister les catégories\n");
                UI.DisplayPrompt("2. Ajouter une catégorie\n");
                UI.DisplayPrompt("3. Renommer une catégorie\n");
                UI.DisplayPrompt("4. Supprimer une catégorie\n");
                UI.DisplayPrompt("0. Retour au menu principal\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 4);

                switch (choix)
                {
                    case 1:
                        ListerCategories(bd);
                        break;
                    case 2:
                        AjouterCategorie(bd);
                        break;
                    case 3:
                        RenommerCategorie(bd);
                        break;
                    case 4:
                        SupprimerCategorie(bd);
                        break;
                    case 0:
                        continuer = false;
                        break;
                }
            }
        }

        // --- Lister les catégories ---
        private static void ListerCategories(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== LISTE DES CATÉGORIES ===\n\n");

            string sql = @"
                SELECT c.id, c.nom, COUNT(m.id) AS nb_medias
                FROM categorie c
                LEFT JOIN media m ON c.id = m.ref_categorie
                GROUP BY c.id, c.nom
                ORDER BY c.nom";

            int compteur = 0;
            foreach (var row in bd.GetRows(sql))
            {
                if (compteur == 0)
                {
                    UI.DisplayPrompt("ID    | Nom de la catégorie                  | Nb médias\n");
                    UI.DisplayPrompt("------|--------------------------------------|----------\n");
                }
                compteur++;

                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                long nbMedias = Convert.ToInt64(row["nb_medias"]);

                UI.DisplayPrompt($"{id,-5} | {nom,-36} | {nbMedias}\n");
            }

            if (compteur == 0)
            {
                UI.DisplayError("Aucune catégorie trouvée.\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // --- Ajouter une catégorie ---
        private static void AjouterCategorie(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== AJOUTER UNE CATÉGORIE ===\n\n");

            string nom = UI.InputText("Nom de la catégorie : ", 2, 60);

            // Vérifier l'unicité
            long existe = bd.GetLong("SELECT COUNT(*) FROM categorie WHERE nom = ?", nom);
            if (existe > 0)
            {
                UI.DisplayError($"\nLa catégorie '{nom}' existe déjà !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Insérer
            var result = bd.Execute("INSERT INTO categorie (nom) VALUES (?)", nom);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nCatégorie '{nom}' ajoutée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // --- Renommer une catégorie ---
        private static void RenommerCategorie(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== RENOMMER UNE CATÉGORIE ===\n\n");

            // Récupérer les catégories dans une liste manuelle
            var categories = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM categorie ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();

                categories.Add(Tuple.Create(id, nom));
                UI.DisplayPrompt($"{numero}. {nom}\n");
            }

            if (categories.Count == 0)
            {
                UI.DisplayError("Aucune catégorie disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la catégorie à renommer : ", 1, categories.Count);
            long idCategorie = categories[choix - 1].Item1;
            string ancienNom = categories[choix - 1].Item2;

            string nouveauNom = UI.InputText($"\nNouveau nom (actuel : '{ancienNom}') : ", 2, 60);

            // Vérifier l'unicité (en excluant l'ID actuel)
            long existe = bd.GetLong(
                "SELECT COUNT(*) FROM categorie WHERE nom = ? AND id != ?",
                nouveauNom, idCategorie);

            if (existe > 0)
            {
                UI.DisplayError($"\nLe nom '{nouveauNom}' est déjà utilisé !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Mettre à jour
            var result = bd.Execute(
                "UPDATE categorie SET nom = ? WHERE id = ?",
                nouveauNom, idCategorie);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nCatégorie renommée : '{ancienNom}' → '{nouveauNom}'\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // --- Supprimer une catégorie ---
        private static void SupprimerCategorie(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== SUPPRIMER UNE CATÉGORIE ===\n\n");

            // Récupérer les catégories
            var categories = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM categorie ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();

                categories.Add(Tuple.Create(id, nom));
                UI.DisplayPrompt($"{numero}. {nom}\n");
            }

            if (categories.Count == 0)
            {
                UI.DisplayError("Aucune catégorie disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la catégorie à supprimer : ", 1, categories.Count);
            long idCategorie = categories[choix - 1].Item1;
            string nomCategorie = categories[choix - 1].Item2;

            // Vérifier les médias liés
            long nbMedias = bd.GetLong(
                "SELECT COUNT(*) FROM media WHERE ref_categorie = ?",
                idCategorie);

            if (nbMedias > 0)
            {
                UI.DisplayError($"\nImpossible de supprimer '{nomCategorie}' : {nbMedias} média(s) lié(s) !\n");
                UI.DisplayError("Supprimez d'abord les médias de cette catégorie.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Confirmation
            UI.DisplayPrompt($"\nSupprimer définitivement la catégorie '{nomCategorie}' ?\n");
            if (!UI.AnswerYes("Confirmer la suppression"))
            {
                UI.DisplayPrompt("\nSuppression annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Supprimer
            var result = bd.Execute("DELETE FROM categorie WHERE id = ?", idCategorie);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nCatégorie '{nomCategorie}' supprimée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // ========================================
        // MENU MÉDIAS
        // ========================================
        private static void MenuMedias(Db bd)
        {
            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION DES MÉDIAS ===\n\n");
                UI.DisplayPrompt("1. Lister les médias\n");
                UI.DisplayPrompt("2. Ajouter un média\n");
                UI.DisplayPrompt("3. Renommer un média\n");
                UI.DisplayPrompt("4. Supprimer un média\n");
                UI.DisplayPrompt("0. Retour au menu principal\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 4);

                switch (choix)
                {
                    case 1:
                        ListerMedias(bd);
                        break;
                    case 2:
                        AjouterMedia(bd);
                        break;
                    case 3:
                        RenommerMedia(bd);
                        break;
                    case 4:
                        SupprimerMedia(bd);
                        break;
                    case 0:
                        continuer = false;
                        break;
                }
            }
        }

        // --- Lister les médias ---
        private static void ListerMedias(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== LISTE DES MÉDIAS ===\n\n");

            string sql = @"
                SELECT 
                    m.id, 
                    m.titre, 
                    m.nomfichier, 
                    m.duree, 
                    c.nom AS categorie
                FROM media m
                INNER JOIN categorie c ON m.ref_categorie = c.id
                ORDER BY m.titre ASC";

            int compteur = 0;
            foreach (var row in bd.GetRows(sql))
            {
                if (compteur == 0)
                {
                    UI.DisplayPrompt("ID    | Titre                                    | Catégorie            | Durée    | Nom de fichier\n");
                    UI.DisplayPrompt("------|------------------------------------------|----------------------|----------|----------------------------------\n");
                }
                compteur++;

                long id = Convert.ToInt64(row["id"]);
                string titre = row["titre"].ToString();
                string categorie = row["categorie"].ToString();
                string duree = row["duree"].ToString();
                string nomfichier = row["nomfichier"].ToString();

                UI.DisplayPrompt($"{id,-5} | {titre,-40} | {categorie,-20} | {duree,-8} | {nomfichier}\n");
            }

            if (compteur == 0)
            {
                UI.DisplayError("Aucun média trouvé.\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // --- Ajouter un média ---
        private static void AjouterMedia(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== AJOUTER UN MÉDIA ===\n\n");

            // 1. Sélection de la catégorie
            var categories = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM categorie ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();

                categories.Add(Tuple.Create(id, nom));
                UI.DisplayPrompt($"{numero}. {nom}\n");
            }

            if (categories.Count == 0)
            {
                UI.DisplayError("Aucune catégorie disponible. Créez d'abord une catégorie !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choixCategorie = UI.InputInt("\nNuméro de la catégorie : ", 1, categories.Count);
            long idCategorie = categories[choixCategorie - 1].Item1;

            // 2. Saisie du titre
            string titre = UI.InputText("\nTitre du média : ", 1, 120);

            // Vérifier l'unicité du titre
            long existe = bd.GetLong("SELECT COUNT(*) FROM media WHERE titre = ?", titre);
            if (existe > 0)
            {
                UI.DisplayError($"\nLe titre '{titre}' existe déjà !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // 3. Saisie du nom de fichier
            string nomfichier = UI.InputText("Nom du fichier : ", 5, 1000);

            // 4. Saisie de la durée (en secondes, puis conversion)
            int dureeSecondes = UI.InputInt("Durée en secondes (ex: 225 pour 3min45s) : ", 1, 86399);

            // Convertir en format HH:MM:SS
            int heures = dureeSecondes / 3600;
            int minutes = (dureeSecondes % 3600) / 60;
            int secondes = dureeSecondes % 60;
            string duree = $"{heures:D2}:{minutes:D2}:{secondes:D2}";

            UI.DisplayPrompt($"\nDurée enregistrée : {duree}\n");

            // 5. Insérer le média avec TOUS les champs
            var result = bd.Execute(
                "INSERT INTO media (titre, nomfichier, duree, ref_categorie) VALUES (?, ?, ?, ?)",
                titre, nomfichier, duree, idCategorie
            );

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\n Média '{titre}' ajouté avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\n Erreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // --- Renommer un média ---
        private static void RenommerMedia(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== RENOMMER UN MÉDIA ===\n\n");

            // Récupérer les médias
            var medias = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, titre FROM media ORDER BY titre"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string titre = row["titre"].ToString();

                medias.Add(Tuple.Create(id, titre));
                UI.DisplayPrompt($"{numero}. {titre}\n");
            }

            if (medias.Count == 0)
            {
                UI.DisplayError("Aucun média disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Sélection du média à renommer
            int choix = UI.InputInt("\nNuméro du média à renommer : ", 1, medias.Count);
            long idMedia = medias[choix - 1].Item1;
            string ancienTitre = medias[choix - 1].Item2;

            // Saisie du nouveau titre
            string nouveauTitre = UI.InputText($"\nNouveau titre (actuel : '{ancienTitre}') : ", 1, 120);

            // Vérifier l'unicité (en excluant le média actuel)
            long existe = bd.GetLong(
                "SELECT COUNT(*) FROM media WHERE titre = ? AND id != ?",
                nouveauTitre, idMedia
            );

            if (existe > 0)
            {
                UI.DisplayError($"\nLe titre '{nouveauTitre}' est déjà utilisé !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Mettre à jour le titre
            var result = bd.Execute(
                "UPDATE media SET titre = ? WHERE id = ?",
                nouveauTitre, idMedia
            );

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nMédia renommé : '{ancienTitre}' → '{nouveauTitre}'\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // --- Supprimer un média ---
        private static void SupprimerMedia(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== SUPPRIMER UN MÉDIA ===\n\n");

            // Récupérer les médias
            var medias = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, titre FROM media ORDER BY titre"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string titre = row["titre"].ToString();

                medias.Add(Tuple.Create(id, titre));
                UI.DisplayPrompt($"{numero}. {titre}\n");
            }

            if (medias.Count == 0)
            {
                UI.DisplayError("Aucun média disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Sélection du média à supprimer
            int choix = UI.InputInt("\nNuméro du média à supprimer : ", 1, medias.Count);
            long idMedia = medias[choix - 1].Item1;
            string titreMedia = medias[choix - 1].Item2;

            //  VÉRIFICATION CRITIQUE : Vérifier si le média est dans des playlists
            long nbDetails = bd.GetLong(
                "SELECT COUNT(*) FROM detail WHERE ref_media = ?",
                idMedia
            );

            if (nbDetails > 0)
            {
                UI.DisplayError($"\nImpossible de supprimer '{titreMedia}' : utilisé dans {nbDetails} playlist(s) !\n");
                UI.DisplayError("Retirez d'abord ce média des playlists.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Confirmation
            UI.DisplayPrompt($"\nSupprimer définitivement le média '{titreMedia}' ?\n");
            if (!UI.AnswerYes("Confirmer la suppression"))
            {
                UI.DisplayPrompt("\nSuppression annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Supprimer
            var result = bd.Execute("DELETE FROM media WHERE id = ?", idMedia);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nMédia '{titreMedia}' supprimé avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }
        // ========================================
        // MENU LISTES (PLAYLISTS) 
        // ========================================
        private static void MenuListes(Db bd)
         {
            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION DES LISTE ===\n\n");
                UI.DisplayPrompt("1. Lister les listes\n");
                UI.DisplayPrompt("2. Ajouter une liste\n");
                UI.DisplayPrompt("3. Renommer une liste\n");
                UI.DisplayPrompt("4. Supprimer une liste\n");
                UI.DisplayPrompt("5. Ajouter un média dans une liste\n");
                UI.DisplayPrompt("6. Supprimer un média dans une liste\n");
                UI.DisplayPrompt("0. Retour au menu principal\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 6);

                switch (choix)
                {
                    case 1:
                        ListerListe(bd);
                        break;
                    case 2:
                        AjouterListe(bd);
                        break;
                    case 3:
                        RenommerListe(bd);
                        break;
                    case 4:
                        SupprimerListe(bd);
                        break;
                    case 5:
                        AjouterMediaListe(bd);
                        break;
                    case 6:
                        SupprimerMediaListe(bd);
                        break;
                    case 0:
                        continuer = false;
                        break;
                }
            }
        }
        
       private static void ListerListe(Db bd)
{
    UI.Clear();
    UI.DisplayStrong("=== LISTE DES LISTES ===\n\n");

    string sql = @"
        SELECT l.id, l.nom, COUNT(d.id) AS nb_medias
        FROM liste l
        LEFT JOIN detail d ON l.id = d.ref_liste
        GROUP BY l.id, l.nom
        ORDER BY l.nom";

    int compteur = 0;
    foreach (var row in bd.GetRows(sql))
    {
        if (compteur == 0)
        {
            UI.DisplayPrompt("ID    | Nom de la liste                   | Médias\n");
            UI.DisplayPrompt("------|-----------------------------------|-------\n");
        }
        compteur++;

        long id = Convert.ToInt64(row["id"]);
        string nom = row["nom"].ToString();
        long nbMedias = Convert.ToInt64(row["nb_medias"]);

        UI.DisplayPrompt($"{id,-5} | {nom,-33} | {nbMedias,-6}\n");
    }

    if (compteur == 0)
    {
        UI.DisplayError("Aucune liste trouvée.\n");
    }
    else
    {
        UI.DisplayPrompt($"\nTotal : {compteur} liste(s)\n");
    }

    UI.WaitKey(ConsoleKey.Enter, "continuer", true);
}

        // --- Ajouter une liste ---
        private static void AjouterListe(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== AJOUTER UNE LISTE ===\n\n");

            string nom = UI.InputText("Nom de la Liste : ", 1, 60);

            // Vérifier l'unicité
            long existe = bd.GetLong("SELECT COUNT(*) FROM liste WHERE nom = ?", nom);
            if (existe > 0)
            {
                UI.DisplayError($"\nLa liste '{nom}' existe déjà !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Insérer
            var result = bd.Execute("INSERT INTO liste (nom) VALUES (?)", nom);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nListe '{nom}' ajoutée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }
      // --- Renommer une liste ---
        private static void RenommerListe(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== RENOMMER UNE LISTE ===\n\n");

            // Récupérer les liste dans une liste manuelle
            var listes = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM liste ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();

                listes.Add(Tuple.Create(id, nom));
                UI.DisplayPrompt($"{numero}. {nom}\n");
            }

            if (listes.Count == 0)
            {
                UI.DisplayError("Aucune liste disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la liste à renommer : ", 1, listes.Count);
            long idListe = listes[choix - 1].Item1;
            string ancienNom = listes[choix - 1].Item2;

            string nouveauNom = UI.InputText($"\nNouveau nom (actuel : '{ancienNom}') : ", 1, 60);

            // Vérifier l'unicité (en excluant l'ID actuel)
            long existe = bd.GetLong(
                "SELECT COUNT(*) FROM liste WHERE nom = ? AND id != ?",
                nouveauNom, idListe);

            if (existe > 0)
            {
                UI.DisplayError($"\nLe nom '{nouveauNom}' est déjà utilisé !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Mettre à jour
            var result = bd.Execute(
                "UPDATE liste SET nom = ? WHERE id = ?",
                nouveauNom, idListe);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nListe renommée : '{ancienNom}' → '{nouveauNom}'\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // --- Supprimer une liste ---
        private static void SupprimerListe(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== SUPPRIMER UNE LISTE ===\n\n");

            // Récupérer les listes
            var listes = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM liste ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();

                listes.Add(Tuple.Create(id, nom));
                UI.DisplayPrompt($"{numero}. {nom}\n");
            }

            if (listes.Count == 0)
            {
                UI.DisplayError("Aucune liste disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la liste à supprimer : ", 1, listes.Count);
            long idListe = listes[choix - 1].Item1;
            string nomListe = listes[choix - 1].Item2;

            // Vérifier les détails liés
            long nbDetails = bd.GetLong(
                "SELECT COUNT(*) FROM detail WHERE ref_liste = ?",
                idListe);

            if (nbDetails > 0)
            {
                UI.DisplayPrompt($"\nCette liste contient {nbDetails} média(s).\n");
                UI.DisplayPrompt("Ils seront retirés de la liste si vous confirmez la suppression.\n");
            }

            // Confirmation
            UI.DisplayPrompt($"\n  Supprimer définitivement la liste '{nomListe}' ?\n");
            if (!UI.AnswerYes("Confirmer la suppression"))
            {
                UI.DisplayPrompt("\nSuppression annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            // Retirer d'abord les médias de la liste (seulement après confirmation)
            if (nbDetails > 0)
            {
                var resultDetails = bd.Execute("DELETE FROM detail WHERE ref_liste = ?", idListe);
                if (!resultDetails.IsSuccess)
                {
                    UI.DisplayError($"\n Erreur : {resultDetails.ErrorMessage}\n");
                    UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                    return;
                }
            }

            // Supprimer
            var result = bd.Execute("DELETE FROM liste WHERE id = ?", idListe);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\n Liste '{nomListe}' supprimée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\n Erreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }
       
        private static void AjouterMediaListe(Db bd)
{
    UI.Clear();
    UI.DisplayStrong("=== AJOUTER UN MÉDIA À UNE LISTE ===\n\n");
    
    // ========== ÉTAPE 1 : Sélectionner une LISTE ==========
    UI.DisplayStrong("Sélectionnez une liste :\n\n");
    var listes = new List<Tuple<long, string>>();
    int numero = 0;
    
    foreach (var row in bd.GetRows("SELECT id, nom FROM liste ORDER BY nom"))
    {
        numero++;
        
        // D'ABORD déclarer les variables
        long id = Convert.ToInt64(row["id"]);
        string nom = row["nom"].ToString();
        
        // ENSUITE ajouter dans la List<Tuple>
        listes.Add(Tuple.Create(id, nom));
        
        // ENFIN afficher (avec le NUMÉRO, pas l'ID)
        UI.DisplayPrompt($"{numero}. {nom}\n");
    }
    
    if (listes.Count == 0)
    {
        UI.DisplayError("Aucune liste trouvée.\n");
        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        return;
    }
    
    int choixListe = UI.InputInt("\nNuméro de la liste : ", 1, listes.Count);
    long idListe = listes[choixListe - 1].Item1;
    string nomListe = listes[choixListe - 1].Item2;  
    
    // ========== ÉTAPE 2 : Sélectionner un MÉDIA ==========
    UI.DisplayStrong("\nSélectionnez un média :\n\n");
    var medias = new List<Tuple<long, string>>();
    int numeroMedia = 0;
    
    foreach (var row in bd.GetRows("SELECT id, titre FROM media ORDER BY titre"))
    {
        numeroMedia++;
        
        // D'ABORD déclarer les variables
        long id = Convert.ToInt64(row["id"]);
        string titre = row["titre"].ToString();
        
        // ENSUITE ajouter dans la List<Tuple>
        medias.Add(Tuple.Create(id, titre));
        
        // ENFIN afficher (avec le NUMÉRO, pas l'ID)
        UI.DisplayPrompt($"{numeroMedia}. {titre}\n");
    }
    
    if (medias.Count == 0)
    {
        UI.DisplayError("Aucun média trouvé.\n");
        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        return;
    }
    
    int choixMedia = UI.InputInt("\nNuméro du média : ", 1, medias.Count);
    long idMedia = medias[choixMedia - 1].Item1;
    string titreMedia = medias[choixMedia - 1].Item2;  
    
    // ========== ÉTAPE 3 : Calculer l'ORDRE ==========
    object maxOrdre = bd.GetValue(
        "SELECT MAX(ordre) FROM detail WHERE ref_liste = ?", 
        idListe);

    int ordre;
    if (maxOrdre == null || maxOrdre == DBNull.Value)
    {
        // Liste vide → premier média
        ordre = 1;
    }
    else
    {
        // Liste contient déjà des médias
        ordre = Convert.ToInt32(maxOrdre) + 1;
    }
    
    // ========== ÉTAPE 4 : INSERT dans detail ==========
    var result = bd.Execute(
        "INSERT INTO detail (ref_liste, ref_media, ordre) VALUES (?, ?, ?)",
        idListe, idMedia, ordre);

    if (result.IsSuccess)
    {
        UI.DisplayPrompt($"\n✅ Le média '{titreMedia}' a été ajouté à la liste '{nomListe}' en position {ordre} !\n");
    }
    else
    {
        UI.DisplayError($"\n❌ Erreur : {result.ErrorMessage}\n");
    }

    UI.WaitKey(ConsoleKey.Enter, "continuer", true);
}

        private static void SupprimerMediaListe(Db bd)
{
    UI.Clear();
    UI.DisplayStrong("=== SUPPRIMER UN MÉDIA D'UNE LISTE ===\n\n");
    
    // ========== ÉTAPE 1 : Sélectionner une LISTE ==========
    UI.DisplayStrong("Sélectionnez une liste :\n\n");
    var listes = new List<Tuple<long, string>>();
    int numero = 0;
    
    foreach (var row in bd.GetRows("SELECT id, nom FROM liste ORDER BY nom"))
    {
        numero++;
        long id = Convert.ToInt64(row["id"]);
        string nom = row["nom"].ToString();
        
        listes.Add(Tuple.Create(id, nom));
        UI.DisplayPrompt($"{numero}. {nom}\n");
    }
    
    if (listes.Count == 0)
    {
        UI.DisplayError("Aucune liste trouvée.\n");
        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        return;
    }
    
    int choixListe = UI.InputInt("\nNuméro de la liste : ", 1, listes.Count);
    long idListe = listes[choixListe - 1].Item1;
    string nomListe = listes[choixListe - 1].Item2;
    
    // ========== ÉTAPE 2 : Afficher les médias DE CETTE LISTE ==========
    UI.DisplayStrong($"\nMédias dans la liste '{nomListe}' :\n\n");
    
    // ATTENTION : On récupère les médias DE CETTE LISTE avec leur ORDRE !
    string sql = @"
        SELECT d.id, d.ref_media, d.ordre, m.titre
        FROM detail d
        INNER JOIN media m ON d.ref_media = m.id
        WHERE d.ref_liste = ?
        ORDER BY d.ordre ASC";
    
    var medias = new List<Tuple<long, int, string>>();  
    int numeroMedia = 0;
    
    foreach (var row in bd.GetRows(sql, idListe))
    {
        numeroMedia++;
        long idDetails = Convert.ToInt64(row["id"]);
        int ordre = Convert.ToInt32(row["ordre"]);
        string titre = row["titre"].ToString();
        
        medias.Add(Tuple.Create(idDetails, ordre, titre));
        UI.DisplayPrompt($"{numeroMedia}. [Position {ordre}] {titre}\n");
    }
    
    if (medias.Count == 0)
    {
        UI.DisplayError("Cette liste est vide.\n");
        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        return;
    }
    
    // ========== ÉTAPE 3 : Sélectionner le média à retirer ==========
    int choixMedia = UI.InputInt("\nNuméro du média à retirer : ", 1, medias.Count);
    long idDetail = medias[choixMedia - 1].Item1;
    int ordreSuppr = medias[choixMedia - 1].Item2;
    string titreMedia = medias[choixMedia - 1].Item3;
    
    // ========== ÉTAPE 4 : Confirmation ==========
    UI.DisplayPrompt($"\n⚠️  Vous allez retirer '{titreMedia}' de la liste '{nomListe}'.\n");
    
    if (!UI.AnswerYes("Confirmer la suppression"))
    {
        UI.DisplayPrompt("\nSuppression annulée.\n");
        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        return;
    }
    
    // ========== ÉTAPE 5 : Supprimer le detail ==========
    var resultDelete = bd.Execute("DELETE FROM detail WHERE id = ?", idDetail);
    
    if (!resultDelete.IsSuccess)
    {
        UI.DisplayError($"\n❌ Erreur lors de la suppression : {resultDelete.ErrorMessage}\n");
        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        return;
    }
    
    // ========== ÉTAPE 6 : RÉORGANISER LES ORDRES ==========
    // Tous les médias avec un ordre > ordreSuppr doivent être décalés de -1
    
    var resultUpdate = bd.Execute(
        "UPDATE detail SET ordre = ordre - 1 WHERE ref_liste = ? AND ordre > ? ORDER BY ordre ASC",
        idListe, ordreSuppr);
    
    if (resultUpdate.IsSuccess)
    {
        UI.DisplayPrompt($"\n✅ Le média '{titreMedia}' a été retiré de la liste '{nomListe}' !\n");
        UI.DisplayPrompt($"Les positions ont été réorganisées automatiquement.\n");
    }
    else
    {
        UI.DisplayError($"\n⚠️  Média supprimé mais erreur lors de la réorganisation : {resultUpdate.ErrorMessage}\n");
    }
    
    UI.WaitKey(ConsoleKey.Enter, "continuer", true);
}

        // ========================================
        // CONSULTATION 
        // ========================================
        private static void ConsulterPlaylists(Db bd)
{
    UI.Clear();
    UI.DisplayStrong("=== CONSULTER UNE PLAYLIST ===\n\n");
    
    // ========== ÉTAPE 1 : Sélectionner une LISTE ==========
    UI.DisplayStrong("Sélectionnez une playlist à consulter :\n\n");
    var listes = new List<Tuple<long, string>>();
    int numero = 0;
    
    foreach (var row in bd.GetRows("SELECT id, nom FROM liste ORDER BY nom"))
    {
        numero++;
        long id = Convert.ToInt64(row["id"]);
        string nom = row["nom"].ToString();
        
        listes.Add(Tuple.Create(id, nom));
        UI.DisplayPrompt($"{numero}. {nom}\n");
    }
    
    if (listes.Count == 0)
    {
        UI.DisplayError("Aucune playlist trouvée.\n");
        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        return;
    }
    
    int choix = UI.InputInt("\nNuméro de la playlist : ", 1, listes.Count);
    long idListe = listes[choix - 1].Item1;
    string nomListe = listes[choix - 1].Item2;
    
    // ========== ÉTAPE 2 : Afficher le contenu de la playlist ==========
    UI.Clear();
    UI.DisplayStrong($"=== PLAYLIST : {nomListe} ===\n\n");
    
    // TRIPLE JOIN pour récupérer : detail + media + categorie
    string sql = @"
        SELECT 
            d.ordre,
            m.titre,
            m.duree,
            c.nom AS categorie
        FROM detail d
        INNER JOIN media m ON d.ref_media = m.id
        INNER JOIN categorie c ON m.ref_categorie = c.id
        WHERE d.ref_liste = ?
        ORDER BY d.ordre ASC";
    
    int compteur = 0;
    
    foreach (var row in bd.GetRows(sql, idListe))
    {
        if (compteur == 0)
        {
            UI.DisplayPrompt("Pos | Titre                                    | Durée    | Catégorie\n");
            UI.DisplayPrompt("----|------------------------------------------|----------|--------------------\n");
        }
        compteur++;
        
        int ordre = Convert.ToInt32(row["ordre"]);
        string titre = row["titre"].ToString();
        string duree = row["duree"].ToString();
        string categorie = row["categorie"].ToString();
        
        UI.DisplayPrompt($"{ordre,-3} | {titre,-40} | {duree,-8} | {categorie}\n");
    }
    
    if (compteur == 0)
    {
        UI.DisplayError("Cette playlist est vide.\n");
        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        return;
    }
    
    // ========== ÉTAPE 3 : Calculer la DURÉE TOTALE ==========
    
    // Méthode 1 : Calculer en SQL avec SUM et TIME_TO_SEC
    string sqlDuree = @"
        SELECT SUM(TIME_TO_SEC(m.duree)) AS total_secondes
        FROM detail d
        INNER JOIN media m ON d.ref_media = m.id
        WHERE d.ref_liste = ?";
    
    object totalSecondes = bd.GetValue(sqlDuree, idListe);
    
    if (totalSecondes != null && totalSecondes != DBNull.Value)
    {
        long secondes = Convert.ToInt64(totalSecondes);
        
        // Convertir les secondes en HH:MM:SS
        int heures = (int)(secondes / 3600);
        int minutes = (int)((secondes % 3600) / 60);
        int secs = (int)(secondes % 60);
        
        string dureeFormatee = $"{heures:D2}:{minutes:D2}:{secs:D2}";
        
        UI.DisplayPrompt("\n");
        UI.DisplayPrompt("─────────────────────────────────────────────────────────────────────────────\n");
        UI.DisplayStrong($"Durée totale : {dureeFormatee}\n");
        UI.DisplayStrong($"Nombre de médias : {compteur}\n");
    }
    else
    {
        UI.DisplayPrompt("\n");
        UI.DisplayStrong("Durée totale : 00:00:00\n");
    }
    
    UI.WaitKey(ConsoleKey.Enter, "revenir au menu", true);
}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GEBD.MySql;
using GEBD.UI;
using static GEBD.MySql.Db;

namespace JournalDeQuetes
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var parametres = DbSettings.Create("localhost", "u_quetes", "MDP", "quetes");
            Db bd = new Db(parametres);

            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION JOURNAL DES QUETES ===\n\n");
                UI.DisplayPrompt("1. Gestion des régions\n");
                UI.DisplayPrompt("2. Gestion des entités\n");
                UI.DisplayPrompt("3. Gestion des caractéristiques\n");
                UI.DisplayPrompt("4. Gestion des quetes\n");
                UI.DisplayPrompt("5. Gestion des détails de quete\n");
                UI.DisplayPrompt("6. Rechercher une quete\n");
                UI.DisplayPrompt("0. Quitter\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 6);

                switch (choix)
                {
                    case 1:
                        MenuRegions(bd);
                        break;
                    case 2:
                        MenuEntites(bd);
                        break;
                    case 3:
                        MenuCaracteristiques(bd);
                        break;
                    case 4:
                        MenuQuetes(bd);
                        break;
                    case 5:
                        GererDetailsQuete(bd);
                        break;
                    case 6:
                        RechercherQuetes(bd);
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
        // MENU DES REGIONS
        // ========================================
        private static void MenuRegions(Db bd)
        {
            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION DES REGIONS ===\n\n");
                UI.DisplayPrompt("1. Lister les régions\n");
                UI.DisplayPrompt("2. Ajouter une région\n");
                UI.DisplayPrompt("3. Renommer une région\n");
                UI.DisplayPrompt("4. Supprimer une région\n");
                UI.DisplayPrompt("0. Retour au menu principal\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 4);

                switch (choix)
                {
                    case 1:
                        ListerRegions(bd);
                        break;
                    case 2:
                        AjouterRegion(bd);
                        break;
                    case 3:
                        RenommerRegion(bd);
                        break;
                    case 4:
                        SupprimerRegion(bd);
                        break;
                    case 0:
                        continuer = false;
                        break;
                }
            }
        }

        private static void ListerRegions(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== LISTE DES REGIONS ===\n\n");

            string sql = @"
                SELECT r.id, r.nom, COUNT(q.id) AS nb_quetes
                FROM region r
                LEFT JOIN quete q ON r.id = q.ref_region
                GROUP BY r.id, r.nom
                ORDER BY r.nom";

            int compteur = 0;
            foreach (var row in bd.GetRows(sql))
            {
                if (compteur == 0)
                {
                    UI.DisplayPrompt("ID    | Nom de la région                  | Nb quetes\n");
                    UI.DisplayPrompt("------|--------------------------------------|----------\n");
                }
                compteur++;

                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                long nbQuetes = Convert.ToInt64(row["nb_quetes"]);

                UI.DisplayPrompt($"{id,-11} | {nom,-36} | {nbQuetes}\n");
            }

            if (compteur == 0)
            {
                UI.DisplayError("Aucune région trouvée.\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void AjouterRegion(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== AJOUTER UNE REGION ===\n\n");

            string nom = UI.InputText("Nom de la région : ", 4, 120);

            long existe = bd.GetLong("SELECT COUNT(*) FROM region WHERE nom = ?", nom);
            if (existe > 0)
            {
                UI.DisplayError($"\nLa région '{nom}' existe déjà !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("INSERT INTO region (nom) VALUES (?)", nom);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nRégion '{nom}' ajoutée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void RenommerRegion(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== RENOMMER UNE REGION ===\n\n");

            var regions = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM region ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();

                regions.Add(Tuple.Create(id, nom));
                UI.DisplayPrompt($"{numero}. {nom}\n");
            }

            if (regions.Count == 0)
            {
                UI.DisplayError("Aucune région disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la région à renommer : ", 1, regions.Count);
            long idRegion = regions[choix - 1].Item1;
            string ancienNom = regions[choix - 1].Item2;

            string nouveauNom = UI.InputText($"\nNouveau nom (actuel : '{ancienNom}') : ", 4, 120);

            long existe = bd.GetLong("SELECT COUNT(*) FROM region WHERE nom = ? AND id != ?", nouveauNom, idRegion);

            if (existe > 0)
            {
                UI.DisplayError($"\nLe nom '{nouveauNom}' est déjà utilisé !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("UPDATE region SET nom = ? WHERE id = ?", nouveauNom, idRegion);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nRégion renommée : '{ancienNom}' → '{nouveauNom}'\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void SupprimerRegion(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== SUPPRIMER UNE REGION ===\n\n");

            var regions = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM region ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();

                regions.Add(Tuple.Create(id, nom));
                UI.DisplayPrompt($"{numero}. {nom}\n");
            }

            if (regions.Count == 0)
            {
                UI.DisplayError("Aucune région disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la région à supprimer : ", 1, regions.Count);
            long idRegion = regions[choix - 1].Item1;
            string nomRegion = regions[choix - 1].Item2;

            long nbQuetes = bd.GetLong("SELECT COUNT(*) FROM quete WHERE ref_region = ?", idRegion);

            if (nbQuetes > 0)
            {
                UI.DisplayError($"\nImpossible de supprimer '{nomRegion}' : {nbQuetes} quête(s) lié(s) !\n");
                UI.DisplayError("Supprimez d'abord les quetes de cette région.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            UI.DisplayPrompt($"\nSupprimer définitivement la région '{nomRegion}' ?\n");
            if (!UI.AnswerYes("Confirmer la suppression"))
            {
                UI.DisplayPrompt("\nSuppression annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("DELETE FROM region WHERE id = ?", idRegion);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nRégion '{nomRegion}' supprimée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // ========================================
        // MENU DES ENTITES
        // ========================================
        private static void MenuEntites(Db bd)
        {
            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION DES ENTITES ===\n\n");
                UI.DisplayPrompt("1. Lister les entités\n");
                UI.DisplayPrompt("2. Ajouter une entité\n");
                UI.DisplayPrompt("3. Renommer une entité\n");
                UI.DisplayPrompt("4. Supprimer une entité\n");
                UI.DisplayPrompt("0. Retour au menu principal\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 4);

                switch (choix)
                {
                    case 1:
                        ListerEntites(bd);
                        break;
                    case 2:
                        AjouterEntite(bd);
                        break;
                    case 3:
                        RenommerEntite(bd);
                        break;
                    case 4:
                        SupprimerEntite(bd);
                        break;
                    case 0:
                        continuer = false;
                        break;
                }
            }
        }

        private static void ListerEntites(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== LISTE DES ENTITES ===\n\n");

            string sql = @"
                SELECT e.id, e.nom, COUNT(qd.id) AS nb_references
                FROM entite e
                LEFT JOIN quete_detail qd ON e.id = qd.ref_entite
                GROUP BY e.id, e.nom
                ORDER BY e.nom";

            int compteur = 0;
            foreach (var row in bd.GetRows(sql))
            {
                if (compteur == 0)
                {
                    UI.DisplayPrompt("ID    | Nom de l' entité                  | Nb réf\n");
                    UI.DisplayPrompt("------|--------------------------------------|----------\n");
                }
                compteur++;

                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                long nbRef = Convert.ToInt64(row["nb_references"]);

                UI.DisplayPrompt($"{id,-11} | {nom,-56} | {nbRef}\n");
            }

            if (compteur == 0)
            {
                UI.DisplayError("Aucune entité trouvée.\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void AjouterEntite(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== AJOUTER UNE ENTITE ===\n\n");

            string nom = UI.InputText("Nom de l' entité : ", 4, 240);

            long existe = bd.GetLong("SELECT COUNT(*) FROM entite WHERE nom = ?", nom);
            if (existe > 0)
            {
                UI.DisplayError($"\nL' entité '{nom}' existe déjà !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("INSERT INTO entite (nom) VALUES (?)", nom);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nEntité '{nom}' ajoutée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void RenommerEntite(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== RENOMMER UNE ENTITE ===\n\n");

            var entites = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM entite ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();

                entites.Add(Tuple.Create(id, nom));
                UI.DisplayPrompt($"{numero}. {nom}\n");
            }

            if (entites.Count == 0)
            {
                UI.DisplayError("Aucune entité disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de l'entité à renommer : ", 1, entites.Count);
            long idEntite = entites[choix - 1].Item1;
            string ancienNom = entites[choix - 1].Item2;

            string nouveauNom = UI.InputText($"\nNouveau nom (actuel : '{ancienNom}') : ", 4, 240);

            long existe = bd.GetLong("SELECT COUNT(*) FROM entite WHERE nom = ? AND id != ?", nouveauNom, idEntite);

            if (existe > 0)
            {
                UI.DisplayError($"\nLe nom '{nouveauNom}' est déjà utilisé !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("UPDATE entite SET nom = ? WHERE id = ?", nouveauNom, idEntite);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nEntité renommée : '{ancienNom}' → '{nouveauNom}'\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void SupprimerEntite(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== SUPPRIMER UNE ENTITE ===\n\n");

            var entites = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM entite ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();

                entites.Add(Tuple.Create(id, nom));
                UI.DisplayPrompt($"{numero}. {nom}\n");
            }

            if (entites.Count == 0)
            {
                UI.DisplayError("Aucune entité disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de l'entité à supprimer : ", 1, entites.Count);
            long idEntite = entites[choix - 1].Item1;
            string nomEntite = entites[choix - 1].Item2;

            long nbRef = bd.GetLong("SELECT COUNT(*) FROM quete_detail WHERE ref_entite = ?", idEntite);

            if (nbRef > 0)
            {
                UI.DisplayError($"\nImpossible de supprimer '{nomEntite}' : {nbRef} quête(s) lié(s) !\n");
                UI.DisplayError("Supprimez d'abord les quetes de cette entité.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            UI.DisplayPrompt($"\nSupprimer définitivement l'entité '{nomEntite}' ?\n");
            if (!UI.AnswerYes("Confirmer la suppression"))
            {
                UI.DisplayPrompt("\nSuppression annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("DELETE FROM entite WHERE id = ?", idEntite);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nEntité '{nomEntite}' supprimée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // ========================================
        // MENU DES CARACTERISTIQUES
        // ========================================
        private static void MenuCaracteristiques(Db bd)
        {
            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION DES CARACTERISTIQUES ===\n\n");
                UI.DisplayPrompt("1. Lister les caracteristiques\n");
                UI.DisplayPrompt("2. Ajouter une caractéristique\n");
                UI.DisplayPrompt("3. Modifier une caractéristique\n");
                UI.DisplayPrompt("4. Supprimer une caractéristique\n");
                UI.DisplayPrompt("0. Retour au menu principal\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 4);

                switch (choix)
                {
                    case 1:
                        ListerCaracteristiques(bd);
                        break;
                    case 2:
                        AjouterCaracteristique(bd);
                        break;
                    case 3:
                        ModifierCaracteristique(bd);
                        break;
                    case 4:
                        SupprimerCaracteristique(bd);
                        break;
                    case 0:
                        continuer = false;
                        break;
                }
            }
        }

        private static void ListerCaracteristiques(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== LISTE DES CARACTERISTIQUES ===\n\n");

            string sql = @"
                SELECT c.id, c.nom, c.type FROM caracteristique c
                ORDER BY c.nom";

            int compteur = 0;
            foreach (var row in bd.GetRows(sql))
            {
                if (compteur == 0)
                {
                    UI.DisplayPrompt("ID    | Nom de la caractéristique            | Type\n");
                    UI.DisplayPrompt("------|--------------------------------------|----------\n");
                }
                compteur++;

                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                string type = row["type"].ToString();
                string typeLibelle;

                switch (type)
                {
                    case "T":
                        typeLibelle = "T (Texte)";
                        break;
                    case "M":
                        typeLibelle = "M (Montant)";
                        break;
                    case "Q":
                        typeLibelle = "Q (Quantité)";
                        break;
                    default:
                        typeLibelle = type;
                        break;
                }

                UI.DisplayPrompt($"{id,-6} | {nom,-40} | {typeLibelle}\n");
            }

            if (compteur == 0)
            {
                UI.DisplayError("Aucune caractéristique trouvée.\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void AjouterCaracteristique(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== AJOUTER UNE CARACTERISTIQUE ===\n\n");

            string nom = UI.InputText("Nom de la caractéristique : ", 4, 80);

            long existe = bd.GetLong("SELECT COUNT(*) FROM caracteristique WHERE nom = ?", nom);
            if (existe > 0)
            {
                UI.DisplayError($"\nLa caractéristique '{nom}' existe déjà !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            UI.DisplayPrompt("\nType de caractéristique :\n");
            UI.DisplayPrompt("1. T (Texte descriptif)\n");
            UI.DisplayPrompt("2. M (Montant)\n");
            UI.DisplayPrompt("3. Q (Quantité avec entité)\n");

            int choixType = UI.InputInt("\nVotre choix : ", 1, 3);

            string type;
            switch (choixType)
            {
                case 1:
                    type = "T";
                    break;
                case 2:
                    type = "M";
                    break;
                case 3:
                    type = "Q";
                    break;
                default:
                    type = "T";
                    break;
            }

            var result = bd.Execute("INSERT INTO caracteristique (nom, type) VALUES (?, ?)", nom, type);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nCaractéristique '{nom}' de type '{type}' ajoutée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void ModifierCaracteristique(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== MODIFIER UNE CARACTERISTIQUE ===\n\n");

            var caracteristiques = new List<Tuple<long, string, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom, type FROM caracteristique ORDER BY id"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                string type = row["type"].ToString();

                caracteristiques.Add(Tuple.Create(id, nom, type));
                UI.DisplayPrompt($"{numero}. {nom} . {type}\n");
            }

            if (caracteristiques.Count == 0)
            {
                UI.DisplayError("Aucune caractéristique disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la caractéristique à modifier : ", 1, caracteristiques.Count);
            long idCaracteristique = caracteristiques[choix - 1].Item1;
            string ancienNom = caracteristiques[choix - 1].Item2;
            string ancienType = caracteristiques[choix - 1].Item3;

            string nouveauNom = UI.InputText($"\nNouveau nom (actuel : '{ancienNom}') : ", 4, 80);

            UI.DisplayPrompt("\nType de caractéristique :\n");
            UI.DisplayPrompt("1. T (Texte descriptif)\n");
            UI.DisplayPrompt("2. M (Montant)\n");
            UI.DisplayPrompt("3. Q (Quantité avec entité)\n");

            int choixType = UI.InputInt("\nVotre choix : ", 1, 3);

            string nouveauType;
            switch (choixType)
            {
                case 1:
                    nouveauType = "T";
                    break;
                case 2:
                    nouveauType = "M";
                    break;
                case 3:
                    nouveauType = "Q";
                    break;
                default:
                    nouveauType = "T";
                    break;
            }

            // Changer le type d'une caractéristique déjà utilisée rendrait ses détails incohérents
            // (ex. : un détail "Texte" deviendrait une "Quantité" sans entité)
            if (nouveauType != ancienType)
            {
                long nbUtilisations = bd.GetLong("SELECT COUNT(*) FROM quete_detail WHERE ref_caracteristique = ?", idCaracteristique);
                if (nbUtilisations > 0)
                {
                    UI.DisplayError($"\nImpossible de changer le type : caractéristique utilisée dans {nbUtilisations} détail(s) de quête !\n");
                    UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                    return;
                }
            }

            long existe = bd.GetLong("SELECT COUNT(*) FROM caracteristique WHERE nom = ? And id != ?", nouveauNom, idCaracteristique);

            if (existe > 0)
            {
                UI.DisplayError($"\nLe nom '{nouveauNom}' est déjà utilisé !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("UPDATE caracteristique SET nom = ? , type = ? WHERE id =?", nouveauNom, nouveauType, idCaracteristique);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nCaractéristique modifiée : '{ancienNom}' → '{nouveauNom}' et '{ancienType}' → '{nouveauType}' \n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void SupprimerCaracteristique(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== SUPPRIMER UNE CARACTERISTIQUE ===\n\n");

            var caracteristiques = new List<Tuple<long, string, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom, type FROM caracteristique ORDER BY id"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                string type = row["type"].ToString();

                caracteristiques.Add(Tuple.Create(id, nom, type));
                UI.DisplayPrompt($"{numero}. {nom} . {type}\n");
            }

            if (caracteristiques.Count == 0)
            {
                UI.DisplayError("Aucune caractéristique disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la caractéristique à supprimer : ", 1, caracteristiques.Count);
            long idCaracteristique = caracteristiques[choix - 1].Item1;
            string nomCaracteristique = caracteristiques[choix - 1].Item2;

            long nbUtilisations = bd.GetLong("SELECT COUNT(*) FROM quete_detail WHERE ref_caracteristique = ?", idCaracteristique);

            if (nbUtilisations > 0)
            {
                UI.DisplayError($"\nImpossible de supprimer '{nomCaracteristique}' : utilisée dans {nbUtilisations} détail(s) de quête !\n");
                UI.DisplayError("Supprimez d'abord les détails utilisant cette caractéristique.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            UI.DisplayPrompt($"\nSupprimer définitivement la caractéristique '{nomCaracteristique}' ?\n");
            if (!UI.AnswerYes("Confirmer la suppression"))
            {
                UI.DisplayPrompt("\nSuppression annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("DELETE FROM caracteristique WHERE id = ?", idCaracteristique);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nCaractéristique '{nomCaracteristique}' supprimée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // ========================================
        // MENU DES QUETES
        // ========================================
        private static void MenuQuetes(Db bd)
        {
            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION DES QUETES ===\n\n");
                UI.DisplayPrompt("1. Lister les quêtes\n");
                UI.DisplayPrompt("2. Ajouter une quête\n");
                UI.DisplayPrompt("3. Modifier une quête\n");
                UI.DisplayPrompt("4. Supprimer une quête\n");
                UI.DisplayPrompt("0. Retour au menu principal\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 4);

                switch (choix)
                {
                    case 1:
                        ListerQuetes(bd);
                        break;
                    case 2:
                        AjouterQuete(bd);
                        break;
                    case 3:
                        ModifierQuete(bd);
                        break;
                    case 4:
                        SupprimerQuete(bd);
                        break;
                    case 0:
                        continuer = false;
                        break;
                }
            }
        }

        private static void ListerQuetes(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== LISTE DES QUETES ===\n\n");

            string sql = @"
                SELECT q.id, q.nom, r.nom AS nom_region, COUNT(qd.id) AS nb_details
                FROM quete q
                INNER JOIN region r on q.ref_region = r.id
                LEFT JOIN quete_detail qd on qd.ref_quete = q.id
                GROUP BY q.id, q.nom, q.description, r.nom
                ORDER BY q.nom";

            int compteur = 0;
            foreach (var row in bd.GetRows(sql))
            {
                if (compteur == 0)
                {
                    UI.DisplayPrompt("ID    | Nom de la quête                      | Région   | Nb détails\n");
                    UI.DisplayPrompt("------|--------------------------------------|----------|--------\n");
                }
                compteur++;

                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                string nom_region = row["nom_region"].ToString();
                long nb_details = Convert.ToInt64(row["nb_details"]);

                UI.DisplayPrompt($"{id,-6} | {nom,-40} | {nom_region,-30} | {nb_details,-6}\n");
            }

            if (compteur == 0)
            {
                UI.DisplayError("Aucune quête trouvée.\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void AjouterQuete(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== AJOUTER UNE QUETE ===\n\n");

            string nom = UI.InputText("Nom de la quête : ", 4, 240);

            long existe = bd.GetLong("SELECT COUNT(*) FROM quete WHERE nom = ?", nom);
            if (existe > 0)
            {
                UI.DisplayError($"\nLa quête '{nom}' existe déjà !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            string description = UI.InputText("Description de la quête : ", 1, 5000);

            var ref_region = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT id, nom FROM region ORDER BY nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom_region = row["nom"].ToString();

                ref_region.Add(Tuple.Create(id, nom_region));
                UI.DisplayPrompt($"{numero}. {nom_region}\n");
            }

            if (ref_region.Count == 0)
            {
                UI.DisplayError("Aucune région disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la région : ", 1, ref_region.Count);
            long idRegion = ref_region[choix - 1].Item1;

            var result = bd.Execute("INSERT INTO quete (nom,description,ref_region) VALUES (?,?,?)", nom, description, idRegion);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nQuête '{nom}' ajoutée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void ModifierQuete(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== MODIFIER UNE QUETE ===\n\n");
            UI.DisplayStrong("Quêtes disponibles :\n\n");

            var quetes = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT q.id, q.nom, r.nom AS nom_region, COUNT(qd.id) AS nb_details FROM quete q INNER JOIN region r ON q.ref_region = r.id LEFT JOIN quete_detail qd ON q.id = qd.ref_quete GROUP BY q.id, q.nom, q.description, r.nom ORDER BY q.nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                string nom_region = row["nom_region"].ToString();
                long nb_details = Convert.ToInt64(row["nb_details"]);

                UI.DisplayPrompt($"{numero}. {nom} ({nom_region}) - {nb_details} détail{(nb_details > 1 ? "s" : "")}\n");
                quetes.Add(Tuple.Create(id, nom));
            }

            if (quetes.Count == 0)
            {
                UI.DisplayError("Aucune quête disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la quête à modifier : ", 1, quetes.Count);
            long idQuete = quetes[choix - 1].Item1;
            string ancienNom = quetes[choix - 1].Item2;

            UI.DisplayStrong($"\nQuête sélectionnée : '{ancienNom}'\n\n");
            UI.DisplayStrong("Valeurs actuelles : ");

            string ancien_Nom = "";
            string ancienDescription = "";
            string ancienneRegion = "";

            var rowQuete = bd.GetRow("SELECT q.nom, q.description, r.nom AS nom_region from quete q INNER JOIN region r on q.ref_region = r.id WHERE q.id= ?", idQuete);

            if (rowQuete != null)
            {
                ancien_Nom = rowQuete["nom"].ToString();
                ancienDescription = rowQuete["description"].ToString();
                ancienneRegion = rowQuete["nom_region"].ToString();
                UI.DisplayPrompt($"Nom : {ancien_Nom}\n");
                UI.DisplayPrompt($"Description : {ancienDescription}\n");
                UI.DisplayPrompt($"Région : {ancienneRegion}\n");
            }

            UI.DisplayStrong("Modification des informations");
            string nouveauNom = UI.InputText($"\nNouveau nom (actuel : '{ancien_Nom}') : ", 4, 240);
            string nouvelleDescription = UI.InputText($"\nNouvelle description (actuel : '{ancienDescription}') : ", 1, 5000);

            var regions = new List<Tuple<long, string>>();

            foreach (var rowRegion in bd.GetRows("SELECT r.id, r.nom, COUNT(q.id) AS nb_quetes FROM region r LEFT JOIN quete q ON r.id = q.ref_region GROUP BY r.id, r.nom ORDER BY r.nom"))
            {
                long id = Convert.ToInt64(rowRegion["id"]);
                string nom = rowRegion["nom"].ToString();
                long nb_quetes = Convert.ToInt64(rowRegion["nb_quetes"]);

                UI.DisplayPrompt($"{regions.Count + 1}. {nom} ({nb_quetes} quête{(nb_quetes > 1 ? "s" : "")})\n");

                regions.Add(Tuple.Create(id, nom));
            }

            int choixRegion = UI.InputInt("\nNuméro de la nouvelle région : ", 1, regions.Count);
            long idRegion = regions[choixRegion - 1].Item1;

            long existe = bd.GetLong("SELECT COUNT(*) FROM quete WHERE nom = ? AND id != ?", nouveauNom, idQuete);

            if (existe > 0)
            {
                UI.DisplayError($"\nLe nom '{nouveauNom}' est déjà utilisé !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("UPDATE quete SET nom = ?, description = ?, ref_region = ? WHERE id = ?", nouveauNom, nouvelleDescription, idRegion, idQuete);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nQuête renommée : '{ancienNom}' → '{nouveauNom}'\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void SupprimerQuete(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("=== SUPPRIMER UNE QUETE ===\n\n");

            var quetes = new List<Tuple<long, string>>();
            int numero = 0;

            foreach (var row in bd.GetRows("SELECT q.id, q.nom, r.nom AS nom_region, COUNT(qd.id) AS nb_details FROM quete q INNER JOIN region r ON q.ref_region = r.id LEFT JOIN quete_detail qd ON q.id = qd.ref_quete GROUP BY q.id, q.nom, q.description, r.nom ORDER BY q.nom"))
            {
                numero++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                string nom_region = row["nom_region"].ToString();
                long nb_details = Convert.ToInt64(row["nb_details"]);

                UI.DisplayPrompt($"{numero}. {nom} ({nom_region}) - {nb_details} détail{(nb_details > 1 ? "s" : "")}\n");
                quetes.Add(Tuple.Create(id, nom));
            }

            if (quetes.Count == 0)
            {
                UI.DisplayError("Aucune quête disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la quête à supprimer : ", 1, quetes.Count);
            long idQuete = quetes[choix - 1].Item1;
            string nomQuete = quetes[choix - 1].Item2;

            long nbDetails = bd.GetLong("SELECT COUNT(*) FROM quete_detail WHERE ref_quete = ?", idQuete);

            if (nbDetails > 0)
            {
                UI.DisplayError($"\nImpossible de supprimer '{nomQuete}' : possède {nbDetails} détail(s) !\n");
                UI.DisplayError("Supprimez d'abord les détails.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            UI.DisplayPrompt($"\nSupprimer définitivement la quête '{nomQuete}' ?\n");
            if (!UI.AnswerYes("Confirmer la suppression"))
            {
                UI.DisplayPrompt("\nSuppression annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("DELETE FROM quete WHERE id = ?", idQuete);

            if (result.IsSuccess)
            {
                UI.DisplayPrompt($"\nQuête '{nomQuete}' supprimée avec succès !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // ========================================
        // MENU DES DETAILS DE QUETE
        // ========================================
        private static void GererDetailsQuete(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("Liste des quêtes\n\n");

            string sql = @"
                SELECT q.id, q.nom, r.nom AS nom_region, COUNT(qd.id) AS nb_details
                FROM quete q
                INNER JOIN region r on q.ref_region = r.id
                LEFT JOIN quete_detail qd on qd.ref_quete = q.id
                GROUP BY q.id, q.nom, q.description, r.nom
                ORDER BY q.nom";

            var idQuetes = new List<Tuple<long, string>>();
            int compteur = 0;
            foreach (var row in bd.GetRows(sql))
            {
                if (compteur == 0)
                {
                    UI.DisplayPrompt("N°    | Nom de la quête                      | Région   | Nb détails\n");
                    UI.DisplayPrompt("------|--------------------------------------|----------|--------\n");
                }
                compteur++;

                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                string nom_region = row["nom_region"].ToString();
                long nb_details = Convert.ToInt64(row["nb_details"]);

                // On affiche le numéro dans la liste (et non l'id) car c'est ce numéro qui est demandé ensuite
                UI.DisplayPrompt($"{compteur,-6} | {nom,-40} | {nom_region,-30} | {nb_details,-6}\n");
                idQuetes.Add(Tuple.Create(id, nom));
            }

            if (idQuetes.Count == 0)
            {
                UI.DisplayError("Aucune quête trouvée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choixQuete = UI.InputInt("\nNuméro de la quête à selectionner (0 pour annuler) : ", 0, idQuetes.Count);

            if (choixQuete == 0)
            {
                return;
            }

            long idQuete = idQuetes[choixQuete - 1].Item1;
            string nomQuete = idQuetes[choixQuete - 1].Item2;

            bool continuer = true;
            while (continuer)
            {
                UI.Clear();
                UI.DisplayStrong("=== GESTION DES DETAILS DE QUETE ===\n\n");
                UI.DisplayPrompt($"Quête : {nomQuete}\n\n");
                UI.DisplayPrompt("1. Afficher les détails\n");
                UI.DisplayPrompt("2. Ajouter un détail\n");
                UI.DisplayPrompt("3. Modifier un détail\n");
                UI.DisplayPrompt("4. Supprimer un détail\n");
                UI.DisplayPrompt("0. Retour au menu principal\n\n");

                int choix = UI.InputInt("Votre choix : ", 0, 4);

                switch (choix)
                {
                    case 1:
                        AfficherDetailsQuete(bd, idQuete, nomQuete);
                        break;
                    case 2:
                        AjouterDetail(bd, idQuete, nomQuete);
                        break;
                    case 3:
                        ModifierDetail(bd, idQuete, nomQuete);
                        break;
                    case 4:
                        SupprimerDetail(bd, idQuete, nomQuete);
                        break;
                    case 0:
                        continuer = false;
                        break;
                }
            }
        }

        private static void AfficherDetailsQuete(Db bd, long idQuete, string nomQuete)
        {
            UI.Clear();
            UI.DisplayStrong($"Détails de la quête : {nomQuete}\n");

            string sql = @"
                SELECT 
                    qd.id,
                    c.nom AS nom_caracteristique,
                    c.type,
                    qd.valeur,
                    e.nom AS nom_entite
                FROM quete_detail qd
                INNER JOIN caracteristique c ON qd.ref_caracteristique = c.id
                LEFT JOIN entite e ON qd.ref_entite = e.id
                WHERE qd.ref_quete = ?
                ORDER BY qd.id";

            int compteur = 0;

            foreach (var row in bd.GetRows(sql, idQuete))
            {
                compteur++;
                string nom_caracteristique = row["nom_caracteristique"].ToString();
                string type = row["type"].ToString();
                string valeur = row["valeur"].ToString();
                string nom_entite = row["nom_entite"]?.ToString();

                string typeLibelle;
                switch (type)
                {
                    case "T":
                        typeLibelle = "Texte";
                        break;
                    case "M":
                        typeLibelle = "Montant";
                        break;
                    case "Q":
                        typeLibelle = "Quantité";
                        break;
                    default:
                        typeLibelle = type;
                        break;
                }

                UI.DisplayPrompt($"{compteur}. {nom_caracteristique} ({typeLibelle})\n");
                if (type == "T")
                {
                    UI.DisplayPrompt($"   → {valeur}\n\n");
                }
                else if (type == "M")
                {
                    UI.DisplayPrompt($"   → {valeur} pièces d'or\n\n");
                }
                else if (type == "Q")
                {
                    UI.DisplayPrompt($"   → {valeur} × {nom_entite}\n\n");
                }
            }

            if (compteur == 0)
            {
                UI.DisplayPrompt("Aucun détail pour cette quête.\n\n");
            }
            else
            {
                UI.DisplayPrompt($"Total : {compteur} détail{(compteur > 1 ? "s" : "")}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void AjouterDetail(Db bd, long idQuete, string nomQuete)
        {
            UI.Clear();
            UI.DisplayStrong($"Ajouter un détail - Quête : {nomQuete}\n");
            UI.DisplayPrompt("Caractéristiques disponibles :\n\n");

            var caracteristiques = new List<Tuple<long, string, string>>();
            int compteur = 0;

            foreach (var row in bd.GetRows("SELECT id, nom, type FROM caracteristique ORDER BY nom"))
            {
                compteur++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                string typeCarac = row["type"].ToString();

                string typeLibelle;
                switch (typeCarac)
                {
                    case "T":
                        typeLibelle = "Texte";
                        break;
                    case "M":
                        typeLibelle = "Montant";
                        break;
                    case "Q":
                        typeLibelle = "Quantité";
                        break;
                    default:
                        typeLibelle = typeCarac;
                        break;
                }

                UI.DisplayPrompt($"{compteur}. {nom} ({typeLibelle})\n");
                caracteristiques.Add(Tuple.Create(id, nom, typeCarac));
            }

            if (compteur == 0)
            {
                UI.DisplayError("Aucune caractéristique disponible.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro de la caractéristique (0 pour annuler) : ", 0, caracteristiques.Count);

            if (choix == 0)
            {
                UI.DisplayPrompt("\nOpération annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            long idCaracteristique = caracteristiques[choix - 1].Item1;
            string nomCaracteristique = caracteristiques[choix - 1].Item2;
            string type = caracteristiques[choix - 1].Item3;

            UI.DisplayPrompt($"Caractéristique sélectionnée : {nomCaracteristique} ");

            if (type == "T")
                UI.DisplayPrompt("(Texte)\n");
            else if (type == "M")
                UI.DisplayPrompt("(Montant)\n");
            else if (type == "Q")
                UI.DisplayPrompt("(Quantité)\n");

            string valeur;
            long? idEntite = null;

            if (type == "T")
            {
                valeur = UI.InputText("Valeur (texte) : ", 1, 240);
            }
            else if (type == "M")
            {
                int montant = UI.InputInt("Montant : ", 1, int.MaxValue);
                valeur = montant.ToString();
            }
            else if (type == "Q")
            {
                int quantite = UI.InputInt("Quantité : ", 1, int.MaxValue);
                valeur = quantite.ToString();

                UI.DisplayPrompt("Entités disponibles :\n");

                var entites = new List<Tuple<long, string>>();
                int compteurEntites = 0;

                foreach (var rowEntite in bd.GetRows("SELECT id, nom FROM entite ORDER BY nom"))
                {
                    compteurEntites++;
                    long idEnt = Convert.ToInt64(rowEntite["id"]);
                    string nomEnt = rowEntite["nom"].ToString();

                    UI.DisplayPrompt($"{compteurEntites}. {nomEnt}\n");
                    entites.Add(Tuple.Create(idEnt, nomEnt));
                }

                if (compteurEntites == 0)
                {
                    UI.DisplayError("\nAucune entité disponible.\n");
                    UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                    return;
                }

                int choixEntite = UI.InputInt("\nNuméro de l'entité : ", 1, entites.Count);
                idEntite = entites[choixEntite - 1].Item1;
            }
            else
            {
                UI.DisplayError("Type de caractéristique invalide.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            IExecuteResult result;

            if (idEntite.HasValue)
            {
                result = bd.Execute("INSERT INTO quete_detail (ref_quete, ref_caracteristique, valeur, ref_entite) VALUES (?, ?, ?, ?)", idQuete, idCaracteristique, valeur, idEntite.Value);
            }
            else
            {
                result = bd.Execute("INSERT INTO quete_detail (ref_quete, ref_caracteristique, valeur, ref_entite) VALUES (?, ?, ?, NULL)", idQuete, idCaracteristique, valeur);
            }

            if (result.IsSuccess)
            {
                UI.DisplayStrong("\nDétail ajouté avec succès !\n\n");
                UI.DisplayPrompt($"Caractéristique : {nomCaracteristique}\n");

                if (type == "T")
                {
                    UI.DisplayPrompt($"Valeur : {valeur}\n");
                }
                else if (type == "M")
                {
                    UI.DisplayPrompt($"Valeur : {valeur} pièces d'or\n");
                }
                else if (type == "Q")
                {
                    var rowEntite = bd.GetRow("SELECT nom FROM entite WHERE id = ?", idEntite.Value);
                    string nomEntite = rowEntite["nom"].ToString();
                    UI.DisplayPrompt($"Valeur : {valeur} × {nomEntite}\n");
                }
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void ModifierDetail(Db bd, long idQuete, string nomQuete)
        {
            UI.Clear();
            UI.DisplayStrong($"Modifier un détail - Quête : {nomQuete}\n");
            UI.DisplayPrompt("Détails disponibles :\n\n");

            string sql = @"
                SELECT 
                    qd.id,
                    c.nom AS nom_caracteristique,
                    c.type,
                    qd.valeur,
                    e.nom AS nom_entite,
                    c.id AS id_caracteristique
                FROM quete_detail qd
                INNER JOIN caracteristique c ON qd.ref_caracteristique = c.id
                LEFT JOIN entite e ON qd.ref_entite = e.id
                WHERE qd.ref_quete = ?
                ORDER BY qd.id";

            var details = new List<Tuple<long, string, string>>();
            int compteur = 0;

            foreach (var row in bd.GetRows(sql, idQuete))
            {
                compteur++;
                long id = Convert.ToInt64(row["id"]);
                string nom_caracteristique = row["nom_caracteristique"].ToString();
                string typeCarac = row["type"].ToString();
                string valeur = row["valeur"].ToString();
                string nom_entite = row["nom_entite"]?.ToString();

                string typeLibelle;
                switch (typeCarac)
                {
                    case "T":
                        typeLibelle = "Texte";
                        break;
                    case "M":
                        typeLibelle = "Montant";
                        break;
                    case "Q":
                        typeLibelle = "Quantité";
                        break;
                    default:
                        typeLibelle = typeCarac;
                        break;
                }

                UI.DisplayPrompt($"{compteur}. {nom_caracteristique} ({typeLibelle})\n");

                if (typeCarac == "T")
                {
                    UI.DisplayPrompt($"   → {valeur}\n\n");
                }
                else if (typeCarac == "M")
                {
                    UI.DisplayPrompt($"   → {valeur} pièces d'or\n\n");
                }
                else if (typeCarac == "Q")
                {
                    UI.DisplayPrompt($"   → {valeur} × {nom_entite}\n\n");
                }

                details.Add(Tuple.Create(id, nom_caracteristique, typeCarac));
            }

            if (compteur == 0)
            {
                UI.DisplayPrompt("Aucun détail à modifier pour cette quête.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro du détail à modifier (0 pour annuler) : ", 0, details.Count);

            if (choix == 0)
            {
                UI.DisplayPrompt("\nOpération annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            long idDetail = details[choix - 1].Item1;
            string nomCaracteristique = details[choix - 1].Item2;
            string type = details[choix - 1].Item3;

            UI.DisplayPrompt($"Modification du détail : {nomCaracteristique} ");

            if (type == "T")
                UI.DisplayPrompt("(Texte)\n");
            else if (type == "M")
                UI.DisplayPrompt("(Montant)\n");
            else if (type == "Q")
                UI.DisplayPrompt("(Quantité)\n");

            string nouvelleValeur;
            long? nouvelleIdEntite = null;

            if (type == "T")
            {
                nouvelleValeur = UI.InputText("Nouvelle valeur (texte) : ", 1, 240);
            }
            else if (type == "M")
            {
                int montant = UI.InputInt("Nouveau montant : ", 1, int.MaxValue);
                nouvelleValeur = montant.ToString();
            }
            else if (type == "Q")
            {
                int quantite = UI.InputInt("Nouvelle quantité : ", 1, int.MaxValue);
                nouvelleValeur = quantite.ToString();

                UI.DisplayPrompt("Entités disponibles :\n");

                var entites = new List<Tuple<long, string>>();
                int compteurEntites = 0;

                foreach (var rowEntite in bd.GetRows("SELECT id, nom FROM entite ORDER BY nom"))
                {
                    compteurEntites++;
                    long idEnt = Convert.ToInt64(rowEntite["id"]);
                    string nomEnt = rowEntite["nom"].ToString();

                    UI.DisplayPrompt($"{compteurEntites}. {nomEnt}\n");
                    entites.Add(Tuple.Create(idEnt, nomEnt));
                }

                if (compteurEntites == 0)
                {
                    UI.DisplayError("\nAucune entité disponible.\n");
                    UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                    return;
                }

                int choixEntite = UI.InputInt("\nNuméro de l'entité : ", 1, entites.Count);
                nouvelleIdEntite = entites[choixEntite - 1].Item1;
            }
            else
            {
                UI.DisplayError("Type de caractéristique invalide.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            IExecuteResult result;

            if (nouvelleIdEntite.HasValue)
            {
                result = bd.Execute("UPDATE quete_detail SET valeur = ?, ref_entite = ? WHERE id = ?", nouvelleValeur, nouvelleIdEntite.Value, idDetail);
            }
            else
            {
                result = bd.Execute("UPDATE quete_detail SET valeur = ?, ref_entite = NULL WHERE id = ?", nouvelleValeur, idDetail);
            }

            if (result.IsSuccess)
            {
                UI.DisplayStrong("\nDétail modifié avec succès !\n\n");
                UI.DisplayPrompt($"Caractéristique : {nomCaracteristique}\n");

                if (type == "T")
                {
                    UI.DisplayPrompt($"Nouvelle valeur : {nouvelleValeur}\n");
                }
                else if (type == "M")
                {
                    UI.DisplayPrompt($"Nouvelle valeur : {nouvelleValeur} pièces d'or\n");
                }
                else if (type == "Q")
                {
                    var rowEntite = bd.GetRow("SELECT nom FROM entite WHERE id = ?", nouvelleIdEntite.Value);
                    string nomEntite = rowEntite["nom"].ToString();
                    UI.DisplayPrompt($"Nouvelle valeur : {nouvelleValeur} × {nomEntite}\n");
                }
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        private static void SupprimerDetail(Db bd, long idQuete, string nomQuete)
        {
            UI.Clear();
            UI.DisplayStrong($"Supprimer un détail - Quête : {nomQuete}\n\n");

            string sql = @"
                SELECT 
                    qd.id,
                    c.nom AS nom_caracteristique,
                    c.type,
                    qd.valeur,
                    e.nom AS nom_entite
                FROM quete_detail qd
                INNER JOIN caracteristique c ON qd.ref_caracteristique = c.id
                LEFT JOIN entite e ON qd.ref_entite = e.id
                WHERE qd.ref_quete = ?
                ORDER BY qd.id";

            var details = new List<Tuple<long, string>>();
            int compteur = 0;

            UI.DisplayPrompt("Détails disponibles :\n\n");

            foreach (var row in bd.GetRows(sql, idQuete))
            {
                compteur++;

                long id = Convert.ToInt64(row["id"]);
                string nom_caracteristique = row["nom_caracteristique"].ToString();
                string type = row["type"].ToString();
                string valeur = row["valeur"].ToString();
                string nom_entite = row["nom_entite"]?.ToString();

                string typeLibelle;
                switch (type)
                {
                    case "T":
                        typeLibelle = "Texte";
                        break;
                    case "M":
                        typeLibelle = "Montant";
                        break;
                    case "Q":
                        typeLibelle = "Quantité";
                        break;
                    default:
                        typeLibelle = type;
                        break;
                }

                UI.DisplayPrompt($"{compteur}. {nom_caracteristique} ({typeLibelle})\n");

                if (type == "T")
                    UI.DisplayPrompt($"   → {valeur}\n\n");
                else if (type == "M")
                    UI.DisplayPrompt($"   → {valeur} pièces d'or\n\n");
                else if (type == "Q")
                    UI.DisplayPrompt($"   → {valeur} × {nom_entite}\n\n");

                details.Add(Tuple.Create(id, nom_caracteristique));
            }

            if (compteur == 0)
            {
                UI.DisplayPrompt("Aucun détail à supprimer pour cette quête.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            int choix = UI.InputInt("\nNuméro du détail à supprimer (0 pour annuler) : ", 0, details.Count);

            if (choix == 0)
            {
                UI.DisplayPrompt("Opération annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            long idDetail = details[choix - 1].Item1;
            string nomCaracteristique = details[choix - 1].Item2;

            UI.DisplayPrompt($"\nSupprimer le détail '{nomCaracteristique}' ?\n");

            if (!UI.AnswerYes("Confirmer la suppression ? "))
            {
                UI.DisplayPrompt("Suppression annulée.\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }

            var result = bd.Execute("DELETE FROM quete_detail WHERE id = ?", idDetail);

            if (result.IsSuccess)
            {
                UI.DisplayStrong($"\nLe détail '{nomCaracteristique}' a été supprimé !\n");
            }
            else
            {
                UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }

        // ========================================
        // RECHERCHE DE QUETE
        // ========================================
        private static void RechercherQuetes(Db bd)
        {
            UI.Clear();
            UI.DisplayStrong("Rechercher des quêtes\n");

            UI.DisplayPrompt("La recherche s'effectue dans :\n");
            UI.DisplayPrompt("  • Nom de la quête\n");
            UI.DisplayPrompt("  • Description de la quête\n");
            UI.DisplayPrompt("  • Valeurs des détails\n");
            UI.DisplayPrompt("  • Noms des entités\n\n");

            string motCle = UI.InputText("Mot-clé à rechercher : ", 1, 100);
            string pattern = $"%{motCle}%";

            UI.Clear();
            UI.DisplayStrong($"Résultats de recherche pour : \"{motCle}\"\n\n");

            string sql = @"
                SELECT DISTINCT
                    q.id,
                    q.nom,
                    r.nom AS nom_region,
                    COUNT(DISTINCT qd.id) AS nb_details
                FROM quete q
                INNER JOIN region r ON q.ref_region = r.id
                LEFT JOIN quete_detail qd ON q.id = qd.ref_quete
                LEFT JOIN entite e ON qd.ref_entite = e.id
                WHERE 
                    q.nom LIKE ? OR
                    q.description LIKE ? OR
                    qd.valeur LIKE ? OR
                    e.nom LIKE ?
                GROUP BY q.id, q.nom, q.description, r.nom
                ORDER BY q.nom";

            int compteur = 0;

            foreach (var row in bd.GetRows(sql, pattern, pattern, pattern, pattern))
            {
                compteur++;
                long id = Convert.ToInt64(row["id"]);
                string nom = row["nom"].ToString();
                string nom_region = row["nom_region"].ToString();
                long nb_details = Convert.ToInt64(row["nb_details"]);

                UI.DisplayPrompt($"{compteur}. {nom}\n");
                UI.DisplayPrompt($"   Région : {nom_region}\n");
                UI.DisplayPrompt($"   Détails : {nb_details}\n\n");
            }

            if (compteur == 0)
            {
                UI.DisplayError($"Aucune quête trouvée contenant \"{motCle}\".\n");
            }
            else
            {
                UI.DisplayPrompt($"Total : {compteur} quête{(compteur > 1 ? "s" : "")} trouvée{(compteur > 1 ? "s" : "")}\n");
            }

            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
        }
    }
}
using GEBD.MySql;
using GEBD.UI;
using System;
using System.Collections.Generic;
using static GEBD.MySql.Db;

class Program
{
    static void Main(string[] args)
    {
        var parametres = DbSettings.Create("localhost", "u_conso", "MDP", "consommation");
        Db bd = new Db(parametres);

        bool continuer = true;
        while (continuer)
        {
            UI.Clear();
            UI.DisplayStrong("╔══════════════════════════════════════╗\n");
            UI.DisplayStrong("║     Gestion Consommation Electrique  ║\n");
            UI.DisplayStrong("╚══════════════════════════════════════╝\n\n");
            UI.DisplayPrompt("  1. Zones\n");
            UI.DisplayPrompt("  2. Types d'appareil\n");
            UI.DisplayPrompt("  3. Appareils\n");
            UI.DisplayPrompt("  4. Consommation par zone / periode\n");
            UI.DisplayPrompt("  0. Quitter\n\n");

            int choix = UI.InputInt("Votre choix : ", 0, 4);
            switch (choix)
            {
                case 1: MenuZones(bd); break;
                case 2: MenuTypes(bd); break;
                case 3: MenuAppareils(bd); break;
                case 4: ConsommationParZone(bd); break;
                case 0: continuer = false; 
                    UI.DisplayPrompt("\nAu revoir !\n"); break;
            }
        }
    }


    // MENU ZONES

    private static void MenuZones(Db bd)
    {
        bool continuer = true;
        while (continuer)
        {
            UI.Clear();
            UI.DisplayStrong("-- Zones --\n\n");
            UI.DisplayPrompt("  1. Voir toutes les zones\n");
            UI.DisplayPrompt("  2. Ajouter une zone\n");
            UI.DisplayPrompt("  3. Modifier une zone\n");
            UI.DisplayPrompt("  4. Supprimer une zone\n");
            UI.DisplayPrompt("  0. Retour\n\n");

            int choix = UI.InputInt("Votre choix : ", 0, 4);
            switch (choix)
            {
                case 1: ListerZones(bd); break;
                case 2: AjouterZone(bd); break;
                case 3: ModifierZone(bd); break;
                case 4: SupprimerZone(bd); break;
                case 0: continuer = false; break;
            }
        }
    }

    private static void ListerZones(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Liste des zones --\n\n");
        UI.DisplayPrompt($"  {"Nom",-25} {"Commentaire"}\n");
        UI.DisplayPrompt($"  {new string('-', 25)} {new string('-', 40)}\n");

        int compteur = 0;
        foreach (var row in bd.GetRows("SELECT id, nom, commentaire FROM zone ORDER BY nom"))
        {
            compteur++;
            string nom = row["nom"].ToString();
            string commentaire = row["commentaire"].ToString();
            if (commentaire.Length > 40)
                commentaire = commentaire.Substring(0, 37) + "...";
            UI.DisplayPrompt($"  {nom,-25} {commentaire}\n");
        }

        if (compteur == 0)
            UI.DisplayError("\n  Aucune zone enregistree.\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    private static void AjouterZone(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Ajouter une zone --\n\n");

        string nom = UI.InputText("Nom (1-40) : ", 1, 40);

        long existe = bd.GetLong("SELECT COUNT(*) FROM zone WHERE nom = ?", nom);
        if (existe > 0)
        {
            UI.DisplayError($"\nCe nom existe deja !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        string commentaire = UI.InputText("Commentaire : ", 0, 65535);

        var result = bd.Execute("INSERT INTO zone (nom, commentaire) VALUES (?, ?)", nom, commentaire);

        if (result.IsSuccess)
            UI.DisplayPrompt($"\nZone '{nom}' ajoutee avec succes !\n");
        else
            UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    private static void ModifierZone(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Modifier une zone --\n\n");

        var zones = new List<Tuple<long, string>>();
        int numero = 0;
        foreach (var row in bd.GetRows("SELECT id, nom FROM zone ORDER BY nom"))
        {
            numero++;
            long id = Convert.ToInt64(row["id"]);
            string nom = row["nom"].ToString();
            zones.Add(Tuple.Create(id, nom));
            UI.DisplayPrompt($"  {numero}. {nom}\n");
        }

        if (zones.Count == 0)
        {
            UI.DisplayError("\n  Aucune zone disponible.\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        int choixListe = UI.InputInt("\nNumero : ", 1, zones.Count);
        long idZone = zones[choixListe - 1].Item1;
        string ancNom = zones[choixListe - 1].Item2;

        string nouveauNom = UI.InputText($"Nouveau nom [{ancNom}] : ", 1, 40);
        string nouveauComm = UI.InputText("Nouveau commentaire : ", 0, 65535);

        long doublon = bd.GetLong(
            "SELECT COUNT(*) FROM zone WHERE nom = ? AND id != ?",
            nouveauNom, idZone);
        if (doublon > 0)
        {
            UI.DisplayError($"\nCe nom est deja utilise !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        var result = bd.Execute(
            "UPDATE zone SET nom = ?, commentaire = ? WHERE id = ?",
            nouveauNom, nouveauComm, idZone);

        if (result.IsSuccess)
            UI.DisplayPrompt("\nZone mise a jour !\n");
        else
            UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    private static void SupprimerZone(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Supprimer une zone --\n\n");

        var zones = new List<Tuple<long, string>>();
        int numero = 0;
        foreach (var row in bd.GetRows("SELECT id, nom FROM zone ORDER BY nom"))
        {
            numero++;
            long id = Convert.ToInt64(row["id"]);
            string nom = row["nom"].ToString();
            zones.Add(Tuple.Create(id, nom));
            UI.DisplayPrompt($"  {numero}. {nom}\n");
        }

        if (zones.Count == 0)
        {
            UI.DisplayError("\n  Aucune zone disponible.\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        int choixListe = UI.InputInt("\nNumero : ", 1, zones.Count);
        long idZone = zones[choixListe - 1].Item1;
        string nomZone = zones[choixListe - 1].Item2;

        long nbAppareils = bd.GetLong("SELECT COUNT(*) FROM appareil WHERE ref_zone = ?", idZone);
        if (nbAppareils > 0)
        {
            UI.DisplayError($"\nImpossible : {nbAppareils} appareil(s) utilise(nt) cette zone !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        if (!UI.AnswerYes($"Confirmer la suppression de '{nomZone}' ? "))
        {
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        var result = bd.Execute("DELETE FROM zone WHERE id = ?", idZone);

        if (result.IsSuccess)
            UI.DisplayPrompt($"\n'{nomZone}' supprimee !\n");
        else
            UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    // MENU TYPES D'APPAREIL

    private static void MenuTypes(Db bd)
    {
        bool continuer = true;
        while (continuer)
        {
            UI.Clear();
            UI.DisplayStrong("-- Types d'appareil --\n\n");
            UI.DisplayPrompt("  1. Voir tous les types\n");
            UI.DisplayPrompt("  2. Ajouter un type\n");
            UI.DisplayPrompt("  3. Modifier un type\n");
            UI.DisplayPrompt("  4. Supprimer un type\n");
            UI.DisplayPrompt("  0. Retour\n\n");

            int choix = UI.InputInt("Votre choix : ", 0, 4);
            switch (choix)
            {
                case 1: ListerTypes(bd); break;
                case 2: AjouterType(bd); break;
                case 3: ModifierType(bd); break;
                case 4: SupprimerType(bd); break;
                case 0: continuer = false; break;
            }
        }
    }

    private static void ListerTypes(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Liste des types d'appareil --\n\n");
        UI.DisplayPrompt($"  {"Nom",-25} {"Facteur",-10} {"Consommation"}\n");
        UI.DisplayPrompt($"  {new string('-', 25)} {new string('-', 10)} {new string('-', 25)}\n");

        int compteur = 0;
        foreach (var row in bd.GetRows("SELECT id, nom, commentaire, facteur_de_marche, puissance, consommation FROM type_appareil ORDER BY nom"))
        {
            compteur++;
            string nom = row["nom"].ToString();
            string commentaire = row["commentaire"].ToString();
            double facteur = Convert.ToDouble(row["facteur_de_marche"]);

            object objPuissance = row["puissance"];
            object objConso = row["consommation"];

            string consoAffichage;
            if (objConso != null && objConso != DBNull.Value)
            {
                decimal conso = Convert.ToDecimal(objConso);
                consoAffichage = $"{conso:F6} kW/h";
            }
            else if (objPuissance != null && objPuissance != DBNull.Value)
            {
                decimal puiss = Convert.ToDecimal(objPuissance);
                decimal calcul = puiss * (decimal)facteur * 0.001m;
                consoAffichage = $"{calcul:F6} kW/h (calc.)";
            }
            else
            {
                consoAffichage = "N.A.";
            }

            UI.DisplayPrompt($"  {nom,-25} {facteur,-10:F2} {consoAffichage}\n");
            if (commentaire.Length > 0)
                UI.DisplayPrompt($"  {"",25}   {commentaire}\n");
        }

        if (compteur == 0)
            UI.DisplayError("\n  Aucun type enregistre.\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    private static void AjouterType(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Ajouter un type d'appareil --\n\n");

        string nom = UI.InputText("Nom (2-80) : ", 2, 80);

        long existe = bd.GetLong("SELECT COUNT(*) FROM type_appareil WHERE nom = ?", nom);
        if (existe > 0)
        {
            UI.DisplayError($"\nCe nom existe deja !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        string commentaire = UI.InputText("Commentaire : ", 0, 65535);
        decimal facteur = UI.InputDecimal("Facteur de marche (0.0 a 1.0) : ", 0.0m, 1.0m);

        UI.DisplayPrompt("\nQue definir ?\n");
        UI.DisplayPrompt("  1. Puissance (W)\n");
        UI.DisplayPrompt("  2. Consommation (kW/h)\n");
        UI.DisplayPrompt("  3. Rien (NULL)\n\n");
        int choixVal = UI.InputInt("Choix : ", 1, 3);

        IExecuteResult result;
        if (choixVal == 1)
        {
            decimal puissance = UI.InputDecimal("Puissance (1 a 999999.999) : ", 1m, 999999.999m);
            result = bd.Execute(
                "INSERT INTO type_appareil (nom, commentaire, facteur_de_marche, puissance, consommation) VALUES (?, ?, ?, ?, NULL)",
                nom, commentaire, facteur, puissance);
        }
        else if (choixVal == 2)
        {
            decimal consoDec = UI.InputDecimal("Consommation kW/h (0.000001 a 999.999999) : ", 0.000001m, 999.999999m);
            result = bd.Execute(
                "INSERT INTO type_appareil (nom, commentaire, facteur_de_marche, puissance, consommation) VALUES (?, ?, ?, NULL, ?)",
                nom, commentaire, facteur, consoDec);
        }
        else
        {
            result = bd.Execute(
                "INSERT INTO type_appareil (nom, commentaire, facteur_de_marche, puissance, consommation) VALUES (?, ?, ?, NULL, NULL)",
                nom, commentaire, facteur);
        }

        if (result.IsSuccess)
            UI.DisplayPrompt($"\nType '{nom}' ajoute !\n");
        else
            UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    private static void ModifierType(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Modifier un type d'appareil --\n\n");

        var types = new List<Tuple<long, string>>();
        int numero = 0;
        foreach (var row in bd.GetRows("SELECT id, nom FROM type_appareil ORDER BY nom"))
        {
            numero++;
            long id = Convert.ToInt64(row["id"]);
            string nom = row["nom"].ToString();
            types.Add(Tuple.Create(id, nom));
            UI.DisplayPrompt($"  {numero}. {nom}\n");
        }

        if (types.Count == 0)
        {
            UI.DisplayError("\n  Aucun type disponible.\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        int choixListe = UI.InputInt("\nNumero : ", 1, types.Count);
        long idType = types[choixListe - 1].Item1;
        string ancNom = types[choixListe - 1].Item2;

        string nouveauNom = UI.InputText($"Nouveau nom [{ancNom}] : ", 2, 80);
        string nouveauComm = UI.InputText("Nouveau commentaire : ", 0, 65535);
        decimal nouveauFacteur = UI.InputDecimal("Nouveau facteur de marche (0.0 a 1.0) : ", 0.0m, 1.0m);

        long doublon = bd.GetLong(
            "SELECT COUNT(*) FROM type_appareil WHERE nom = ? AND id != ?",
            nouveauNom, idType);
        if (doublon > 0)
        {
            UI.DisplayError($"\nCe nom est deja utilise !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        UI.DisplayPrompt("\nModifier puissance/consommation ?\n");
        UI.DisplayPrompt("  1. Puissance (W)\n");
        UI.DisplayPrompt("  2. Consommation (kW/h)\n");
        UI.DisplayPrompt("  3. Rien (NULL)\n\n");
        int choixVal = UI.InputInt("Choix : ", 1, 3);

        IExecuteResult result;
        if (choixVal == 1)
        {
            decimal puissance = UI.InputDecimal("Puissance (1 a 999999.999) : ", 1m, 999999.999m);
            result = bd.Execute(
                "UPDATE type_appareil SET nom = ?, commentaire = ?, facteur_de_marche = ?, puissance = ?, consommation = NULL WHERE id = ?",
                nouveauNom, nouveauComm, nouveauFacteur, puissance, idType);
        }
        else if (choixVal == 2)
        {
            decimal consoDec = UI.InputDecimal("Consommation kW/h (0.000001 a 999.999999) : ", 0.000001m, 999.999999m);
            result = bd.Execute(
                "UPDATE type_appareil SET nom = ?, commentaire = ?, facteur_de_marche = ?, puissance = NULL, consommation = ? WHERE id = ?",
                nouveauNom, nouveauComm, nouveauFacteur, consoDec, idType);
        }
        else
        {
            result = bd.Execute(
                "UPDATE type_appareil SET nom = ?, commentaire = ?, facteur_de_marche = ?, puissance = NULL, consommation = NULL WHERE id = ?",
                nouveauNom, nouveauComm, nouveauFacteur, idType);
        }

        if (result.IsSuccess)
            UI.DisplayPrompt("\nType mis a jour !\n");
        else
            UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    private static void SupprimerType(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Supprimer un type d'appareil --\n\n");

        var types = new List<Tuple<long, string>>();
        int numero = 0;
        foreach (var row in bd.GetRows("SELECT id, nom FROM type_appareil ORDER BY nom"))
        {
            numero++;
            long id = Convert.ToInt64(row["id"]);
            string nom = row["nom"].ToString();
            types.Add(Tuple.Create(id, nom));
            UI.DisplayPrompt($"  {numero}. {nom}\n");
        }

        if (types.Count == 0)
        {
            UI.DisplayError("\n  Aucun type disponible.\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        int choixListe = UI.InputInt("\nNumero : ", 1, types.Count);
        long idType = types[choixListe - 1].Item1;
        string nomType = types[choixListe - 1].Item2;

        long nbAppareils = bd.GetLong("SELECT COUNT(*) FROM appareil WHERE ref_type = ?", idType);
        if (nbAppareils > 0)
        {
            UI.DisplayError($"\nImpossible : {nbAppareils} appareil(s) utilise(nt) ce type !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        if (!UI.AnswerYes($"Confirmer la suppression de '{nomType}' ? "))
        {
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        var result = bd.Execute("DELETE FROM type_appareil WHERE id = ?", idType);

        if (result.IsSuccess)
            UI.DisplayPrompt($"\n'{nomType}' supprime !\n");
        else
            UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    // MENU APPAREILS
    private static void MenuAppareils(Db bd)
    {
        bool continuer = true;
        while (continuer)
        {
            UI.Clear();
            UI.DisplayStrong("-- Appareils --\n\n");
            UI.DisplayPrompt("  1. Voir tous les appareils\n");
            UI.DisplayPrompt("  2. Ajouter un appareil\n");
            UI.DisplayPrompt("  3. Supprimer un appareil\n");
            UI.DisplayPrompt("  4. Ajouter une utilisation\n");
            UI.DisplayPrompt("  0. Retour\n\n");

            int choix = UI.InputInt("Votre choix : ", 0, 4);
            switch (choix)
            {
                case 1: ListerAppareils(bd); break;
                case 2: AjouterAppareil(bd); break;
                case 3: SupprimerAppareil(bd); break;
                case 4: AjouterUtilisation(bd); break;
                case 0: continuer = false; break;
            }
        }
    }

    private static void ListerAppareils(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Liste des appareils --\n\n");
        UI.DisplayPrompt($"  {"Zone",-15} {"Denomination",-25} {"Qte",-5} {"Type",-20} {"Conso unit.",-18} {"Conso max."}\n");
        UI.DisplayPrompt($"  {new string('-', 15)} {new string('-', 25)} {new string('-', 5)} {new string('-', 20)} {new string('-', 18)} {new string('-', 18)}\n");

        string sql = @"
            SELECT a.id, a.denomination, a.quantite,
                   z.nom AS nom_zone,
                   t.nom AS nom_type, t.commentaire AS comm_type,
                   t.facteur_de_marche,
                   a.puissance AS ap_puissance, a.consommation AS ap_conso,
                   t.puissance AS tp_puissance, t.consommation AS tp_conso
            FROM appareil a
            INNER JOIN zone z ON z.id = a.ref_zone
            INNER JOIN type_appareil t ON t.id = a.ref_type
            ORDER BY z.nom, a.denomination";

        int compteur = 0;
        foreach (var row in bd.GetRows(sql))
        {
            compteur++;
            string denomination = row["denomination"].ToString();
            long quantite = Convert.ToInt64(row["quantite"]);
            string nomZone = row["nom_zone"].ToString();
            string nomType = row["nom_type"].ToString();
            string commType = row["comm_type"].ToString();
            double facteur = Convert.ToDouble(row["facteur_de_marche"]);

            object objApPuiss = row["ap_puissance"];
            object objApConso = row["ap_conso"];
            object objTpPuiss = row["tp_puissance"];
            object objTpConso = row["tp_conso"];

            decimal consoUnitaire = 0m;
            bool consoTrouvee = false;

            if (objApConso != null && objApConso != DBNull.Value)
            {
                consoUnitaire = Convert.ToDecimal(objApConso);
                consoTrouvee = true;
            }
            else if (objApPuiss != null && objApPuiss != DBNull.Value)
            {
                consoUnitaire = Convert.ToDecimal(objApPuiss) * (decimal)facteur * 0.001m;
                consoTrouvee = true;
            }
            else if (objTpConso != null && objTpConso != DBNull.Value)
            {
                consoUnitaire = Convert.ToDecimal(objTpConso);
                consoTrouvee = true;
            }
            else if (objTpPuiss != null && objTpPuiss != DBNull.Value)
            {
                consoUnitaire = Convert.ToDecimal(objTpPuiss) * (decimal)facteur * 0.001m;
                consoTrouvee = true;
            }

            string consoUnitStr = consoTrouvee ? $"{consoUnitaire:F6}" : "N.A.";
            string consoMaxStr = consoTrouvee ? $"{consoUnitaire * quantite:F6}" : "N.A.";

            string zoneCol = nomZone.Length > 14 ? nomZone.Substring(0, 13) + "." : nomZone;
            string denomCol = denomination.Length > 24 ? denomination.Substring(0, 23) + "." : denomination;
            string typeCol = nomType.Length > 19 ? nomType.Substring(0, 18) + "." : nomType;

            UI.DisplayPrompt($"  {zoneCol,-15} {denomCol,-25} {quantite,-5} {typeCol,-20} {consoUnitStr,-18} {consoMaxStr}\n");
        }

        if (compteur == 0)
            UI.DisplayError("\n  Aucun appareil enregistre.\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    private static void AjouterAppareil(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Ajouter un appareil --\n\n");

        // Choisir le type
        var types = new List<Tuple<long, string>>();
        int num = 0;
        UI.DisplayPrompt("Types disponibles :\n");
        foreach (var row in bd.GetRows("SELECT id, nom FROM type_appareil ORDER BY nom"))
        {
            num++;
            long id = Convert.ToInt64(row["id"]);
            string nom = row["nom"].ToString();
            types.Add(Tuple.Create(id, nom));
            UI.DisplayPrompt($"  {num}. {nom}\n");
        }
        if (types.Count == 0)
        {
            UI.DisplayError("Aucun type disponible. Créez d'abord un type.\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }
        int choixType = UI.InputInt("\nNumero du type : ", 1, types.Count);
        long idType = types[choixType - 1].Item1;

        // Choisir la zone
        var zones = new List<Tuple<long, string>>();
        num = 0;
        UI.DisplayPrompt("\nZones disponibles :\n");
        foreach (var row in bd.GetRows("SELECT id, nom FROM zone ORDER BY nom"))
        {
            num++;
            long id = Convert.ToInt64(row["id"]);
            string nom = row["nom"].ToString();
            zones.Add(Tuple.Create(id, nom));
            UI.DisplayPrompt($"  {num}. {nom}\n");
        }
        if (zones.Count == 0)
        {
            UI.DisplayError("Aucune zone disponible. Créez d'abord une zone.\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }
        int choixZone = UI.InputInt("\nNumero de la zone : ", 1, zones.Count);
        long idZone = zones[choixZone - 1].Item1;

        string denomination = UI.InputText("\nDenomination (2-80) : ", 2, 80);
        int quantite = UI.InputInt("Quantite (1-200) : ", 1, 200);

        UI.DisplayPrompt("\nQue definir ?\n");
        UI.DisplayPrompt("  1. Puissance (W)\n");
        UI.DisplayPrompt("  2. Consommation (kW/h)\n");
        UI.DisplayPrompt("  3. Rien\n\n");
        int choixVal = UI.InputInt("Choix : ", 1, 3);

        IExecuteResult result;
        if (choixVal == 1)
        {
            decimal puissance = UI.InputDecimal("Puissance (1 a 999999.999) : ", 1m, 999999.999m);
            result = bd.Execute(
                "INSERT INTO appareil (denomination, ref_type, ref_zone, quantite, puissance, consommation) VALUES (?, ?, ?, ?, ?, NULL)",
                denomination, idType, idZone, quantite, puissance);
        }
        else if (choixVal == 2)
        {
            decimal consoDec = UI.InputDecimal("Consommation kW/h (0.000001 a 999.999999) : ", 0.000001m, 999.999999m);
            result = bd.Execute(
                "INSERT INTO appareil (denomination, ref_type, ref_zone, quantite, puissance, consommation) VALUES (?, ?, ?, ?, NULL, ?)",
                denomination, idType, idZone, quantite, consoDec);
        }
        else
        {
            // Vérifier que le type a au moins une valeur non NULL
            var typeRow = bd.GetRow("SELECT puissance, consommation FROM type_appareil WHERE id = ?", idType);
            object tpPuiss = typeRow["puissance"];
            object tpConso = typeRow["consommation"];
            bool typeSansPuissance = tpPuiss == null || tpPuiss == DBNull.Value;
            bool typeSansConso = tpConso == null || tpConso == DBNull.Value;
            if (typeSansPuissance && typeSansConso)
            {
                UI.DisplayError("\nImpossible : le type d'appareil n'a ni puissance ni consommation definie !\n");
                UI.WaitKey(ConsoleKey.Enter, "continuer", true);
                return;
            }
            result = bd.Execute(
                "INSERT INTO appareil (denomination, ref_type, ref_zone, quantite, puissance, consommation) VALUES (?, ?, ?, ?, NULL, NULL)",
                denomination, idType, idZone, quantite);
        }

        if (result.IsSuccess)
            UI.DisplayPrompt($"\nAppareil '{denomination}' ajoute !\n");
        else
            UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    private static void SupprimerAppareil(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Supprimer un appareil --\n\n");

        var appareils = new List<Tuple<long, string>>();
        int numero = 0;
        foreach (var row in bd.GetRows(@"
            SELECT a.id, a.denomination, z.nom AS nom_zone
            FROM appareil a
            INNER JOIN zone z ON z.id = a.ref_zone
            ORDER BY z.nom, a.denomination"))
        {
            numero++;
            long id = Convert.ToInt64(row["id"]);
            string den = row["denomination"].ToString();
            string zon = row["nom_zone"].ToString();
            appareils.Add(Tuple.Create(id, den));
            UI.DisplayPrompt($"  {numero}. [{zon}] {den}\n");
        }

        if (appareils.Count == 0)
        {
            UI.DisplayError("\n  Aucun appareil disponible.\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        int choixListe = UI.InputInt("\nNumero : ", 1, appareils.Count);
        long idAppareil = appareils[choixListe - 1].Item1;
        string nomApp = appareils[choixListe - 1].Item2;

        long nbUtilis = bd.GetLong("SELECT COUNT(*) FROM utilisation WHERE ref_appareil = ?", idAppareil);

        if (nbUtilis > 0)
        {
            UI.DisplayPrompt($"\nAttention : {nbUtilis} utilisation(s) liee(s) a cet appareil.\n");
            UI.DisplayPrompt("Elles seront supprimees automatiquement.\n");
        }

        if (!UI.AnswerYes($"\nConfirmer la suppression de '{nomApp}' ? "))
        {
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        if (nbUtilis > 0)
            bd.Execute("DELETE FROM utilisation WHERE ref_appareil = ?", idAppareil);

        var result = bd.Execute("DELETE FROM appareil WHERE id = ?", idAppareil);

        if (result.IsSuccess)
            UI.DisplayPrompt($"\n'{nomApp}' supprime !\n");
        else
            UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    private static void AjouterUtilisation(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Ajouter une utilisation --\n\n");

        var appareils = new List<Tuple<long, string, int>>();
        int numero = 0;
        foreach (var row in bd.GetRows(@"
            SELECT a.id, a.denomination, a.quantite, z.nom AS nom_zone
            FROM appareil a
            INNER JOIN zone z ON z.id = a.ref_zone
            ORDER BY z.nom, a.denomination"))
        {
            numero++;
            long id = Convert.ToInt64(row["id"]);
            string den = row["denomination"].ToString();
            int qte = Convert.ToInt32(row["quantite"]);
            string zon = row["nom_zone"].ToString();
            appareils.Add(Tuple.Create(id, den, qte));
            UI.DisplayPrompt($"  {numero}. [{zon}] {den} (max: {qte})\n");
        }

        if (appareils.Count == 0)
        {
            UI.DisplayError("\n  Aucun appareil disponible.\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        int choixApp = UI.InputInt("\nNumero de l'appareil : ", 1, appareils.Count);
        long idAppareil = appareils[choixApp - 1].Item1;
        string nomApp = appareils[choixApp - 1].Item2;
        int qteMax = appareils[choixApp - 1].Item3;

        int quantiteUtilisee = UI.InputInt($"Quantite utilisee (1 a {qteMax}) : ", 1, qteMax);
        decimal pourcentage = UI.InputDecimal("Pourcentage d'utilisation (0.0 a 1.0) : ", 0.0m, 1.0m);

        string debutStr = UI.InputText("Debut de periode (AAAA-MM-JJ HH:MM:SS) : ", 19, 19);
        DateTime debut;
        if (!DateTime.TryParseExact(debutStr, "yyyy-MM-dd HH:mm:ss",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out debut))
        {
            UI.DisplayError("\nFormat invalide !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        string finStr = UI.InputText("Fin de periode (AAAA-MM-JJ HH:MM:SS) : ", 19, 19);
        DateTime fin;
        if (!DateTime.TryParseExact(finStr, "yyyy-MM-dd HH:mm:ss",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out fin))
        {
            UI.DisplayError("\nFormat invalide !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        if (fin <= debut)
        {
            UI.DisplayError("\nLa fin doit etre apres le debut !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        var result = bd.Execute(
            "INSERT INTO utilisation (ref_appareil, quantite, pourcentage, debut_periode, fin_periode) VALUES (?, ?, ?, ?, ?)",
            idAppareil, quantiteUtilisee, pourcentage,
            debut.ToString("yyyy-MM-dd HH:mm:ss"),
            fin.ToString("yyyy-MM-dd HH:mm:ss"));

        if (result.IsSuccess)
            UI.DisplayPrompt($"\nUtilisation ajoutee pour '{nomApp}' !\n");
        else
            UI.DisplayError($"\nErreur : {result.ErrorMessage}\n");

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }

    // CONSOMMATION PAR ZONE ET PÉRIODE
    private static void ConsommationParZone(Db bd)
    {
        UI.Clear();
        UI.DisplayStrong("-- Consommation par zone --\n\n");

        var zones = new List<Tuple<long, string>>();
        int numero = 0;
        foreach (var row in bd.GetRows("SELECT id, nom FROM zone ORDER BY nom"))
        {
            numero++;
            long id = Convert.ToInt64(row["id"]);
            string nom = row["nom"].ToString();
            zones.Add(Tuple.Create(id, nom));
            UI.DisplayPrompt($"  {numero}. {nom}\n");
        }

        if (zones.Count == 0)
        {
            UI.DisplayError("\n  Aucune zone disponible.\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        int choixZone = UI.InputInt("\nNumero de la zone : ", 1, zones.Count);
        long idZone = zones[choixZone - 1].Item1;
        string nomZone = zones[choixZone - 1].Item2;

        string debutStr = UI.InputText("Date de debut (AAAA-MM-JJ) : ", 10, 10);
        DateTime dateDebut;
        if (!DateTime.TryParseExact(debutStr, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out dateDebut))
        {
            UI.DisplayError("\nFormat invalide !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        string finStr = UI.InputText("Date de fin (AAAA-MM-JJ) : ", 10, 10);
        DateTime dateFin;
        if (!DateTime.TryParseExact(finStr, "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out dateFin))
        {
            UI.DisplayError("\nFormat invalide !\n");
            UI.WaitKey(ConsoleKey.Enter, "continuer", true);
            return;
        }

        string debutComplet = dateDebut.ToString("yyyy-MM-dd") + " 00:00:00";
        string finComplet = dateFin.ToString("yyyy-MM-dd") + " 23:59:59";

        UI.Clear();
        UI.DisplayStrong($"-- Consommation : {nomZone} | {debutStr} au {finStr} --\n\n");
        UI.DisplayPrompt($"  {"Appareil",-25} {"Qte",-5} {"Pct",-8} {"Duree",-10} {"Consommation"}\n");
        UI.DisplayPrompt($"  {new string('-', 25)} {new string('-', 5)} {new string('-', 8)} {new string('-', 10)} {new string('-', 18)}\n");

        string sql = @"
            SELECT a.denomination,
                   t.facteur_de_marche,
                   a.puissance AS ap_puissance, a.consommation AS ap_conso,
                   t.puissance AS tp_puissance, t.consommation AS tp_conso,
                   u.quantite AS qte_utilise, u.pourcentage,
                   u.debut_periode, u.fin_periode
            FROM utilisation u
            INNER JOIN appareil a ON a.id = u.ref_appareil
            INNER JOIN type_appareil t ON t.id = a.ref_type
            WHERE a.ref_zone = ?
              AND u.debut_periode >= ?
              AND u.fin_periode   <= ?
            ORDER BY a.denomination";

        decimal totalGlobal = 0m;
        int compteur = 0;

        foreach (var row in bd.GetRows(sql, idZone, debutComplet, finComplet))
        {
            compteur++;
            string denomination = row["denomination"].ToString();
            int qteUtilise = Convert.ToInt32(row["qte_utilise"]);
            double pourcentage = Convert.ToDouble(row["pourcentage"]);
            double facteur = Convert.ToDouble(row["facteur_de_marche"]);
            DateTime debut = Convert.ToDateTime(row["debut_periode"]);
            DateTime fin = Convert.ToDateTime(row["fin_periode"]);

            object objApPuiss = row["ap_puissance"];
            object objApConso = row["ap_conso"];
            object objTpPuiss = row["tp_puissance"];
            object objTpConso = row["tp_conso"];

            decimal consoUnit = 0m;
            bool ok = false;
            if (objApConso != null && objApConso != DBNull.Value)
            {
                consoUnit = Convert.ToDecimal(objApConso);
                ok = true;
            }
            else if (objApPuiss != null && objApPuiss != DBNull.Value)
            {
                consoUnit = Convert.ToDecimal(objApPuiss) * (decimal)facteur * 0.001m;
                ok = true;
            }
            else if (objTpConso != null && objTpConso != DBNull.Value)
            {
                consoUnit = Convert.ToDecimal(objTpConso);
                ok = true;
            }
            else if (objTpPuiss != null && objTpPuiss != DBNull.Value)
            {
                consoUnit = Convert.ToDecimal(objTpPuiss) * (decimal)facteur * 0.001m;
                ok = true;
            }

            if (!ok)
            {
                string denomNa = denomination.Length > 24 ? denomination.Substring(0, 23) + "." : denomination;
                UI.DisplayPrompt($"  {denomNa,-25} {"N.A."}\n");
                continue;
            }

            double dureeHeures = (fin - debut).TotalHours;
            decimal consoEffective = consoUnit * qteUtilise * (decimal)pourcentage * (decimal)dureeHeures;
            totalGlobal += consoEffective;

            string denomCol = denomination.Length > 24 ? denomination.Substring(0, 23) + "." : denomination;
            string pctStr = $"{pourcentage * 100:F1}%";
            string dureeStr = $"{dureeHeures:F2}h";

            UI.DisplayPrompt($"  {denomCol,-25} {qteUtilise,-5} {pctStr,-8} {dureeStr,-10} {consoEffective:F6} kW/h\n");
        }

        if (compteur == 0)
            UI.DisplayError("\n  Aucune utilisation trouvee pour cette periode.\n");
        else
        {
            UI.DisplayPrompt($"  {new string('-', 70)}\n");
            UI.DisplayStrong($"  Total global : {totalGlobal:F6} kW/h\n");
        }

        UI.WaitKey(ConsoleKey.Enter, "continuer", true);
    }
}

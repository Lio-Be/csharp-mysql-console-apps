using MySql.Data.MySqlClient;
using Mysqlx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debug = System.Diagnostics.Debug;

namespace GEBD.MySql
{
    /// <summary>
    /// Définit la connexion à un serveur MySql pour manipuler une base de données
    /// </summary>
    public partial class Db : IDisposable
    {
        /// <summary>
        /// Permet de récupérer le premier enregistrement résultant de l'exécution d'une requête de consultation
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Résultat d'exécution de la requête de consultation</returns>
        public IRowResult GetRow(string sqlQuery, params object[] parameters)
        {
            try
            {
                using (var command = PrepareCommand(out QueryType queryType, QueryCategory.Reader, sqlQuery, parameters))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var fieldNames = new string[reader.FieldCount];
                            for (int i = 0; i < fieldNames.Length; i++)
                            {
                                fieldNames[i] = reader.GetName(i);
                            }
                            var row = RowResult.Create(0, fieldNames);
                            if (row.SetValues(reader))
                            {
                                return row;
                            }
                            else
                            {
                                throw new Exception("Erreur interne lors de la méthode SetValues() !");
                            }
                        }
                        else
                        {
                            throw new Exception("Aucun enregistrement récupérable !");
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Debug.WriteLine($"\nErreur de récupération du premier enregistrement pour la requête {sqlQuery} :\n{error.Message}\n");
                return RowResult.Create(0, sqlQuery, parameters, error.Message);
            }
        }
    }
}

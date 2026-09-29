using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Debug = System.Diagnostics.Debug;

namespace GEBD.MySql
{
    /// <summary>
    /// Définit la connexion à un serveur MySql pour manipuler une base de données
    /// </summary>
    public partial class Db : IDisposable
    {
        /// <summary>
        /// Définit un résultat d'exécution de requête SQL d'action
        /// </summary>
        public interface IExecuteResult
        {
            /// <summary>
            /// Indique si l'exécution d'une requête SQL d'action a réussi
            /// </summary>
            bool IsSuccess { get; }

            /// <summary>
            /// Retourne le nombre d'enregistrements affectés par l'exécution d'une requête SQL d'action, sinon 0
            /// </summary>
            int AffectedRowCount { get; }

            /// <summary>
            /// Retourne l'identifiant produit par l'exécution d'une requête SQL d'action de type INSERT et ayant créé un et un seul enregistrement, sinon 0
            /// </summary>
            long NewId { get; }

            /// <summary>
            /// Type de requête
            /// </summary>
            QueryType QueryType { get; }

            /// <summary>
            /// Retourne la requête SQL n'ayant pas pu être exécutée, sinon une chaîne vide
            /// </summary>
            string SqlQuery { get; }

            /// <summary>
            /// Retourne les paramètres de la requête SQL qui n'a pas pu être exécutée, sinon un tableau vide est retourné
            /// </summary>
            object[] Parameters { get; }

            /// <summary>
            /// Retourne le message d'erreur lorsqu'une requête SQL n'a pas pu être exécutée, sinon une chaîne vide est retournée
            /// </summary>
            string ErrorMessage { get; }
        }

        /// <summary>
        /// Implémente un résultat d'exécution de requête SQL d'action
        /// </summary>
        private class ExecuteResult : IExecuteResult
        {
            /// <summary>
            /// Tableau vide de paramètre
            /// </summary>
            private static readonly object[] c_NoneParameter = new object[0];

            /// <summary>
            /// Indique si l'exécution d'une requête SQL d'action a réussi
            /// </summary>
            public bool IsSuccess { get; }

            /// <summary>
            /// Retourne le nombre d'enregistrements affectés par l'exécution d'une requête SQL d'action, sinon 0
            /// </summary>
            public int AffectedRowCount { get; }

            /// <summary>
            /// Retourne l'identifiant produit par l'exécution d'une requête SQL d'action de type INSERT et ayant créé un et un seul enregistrement, sinon 0
            /// </summary>
            public long NewId { get; }

            /// <summary>
            /// Type de requête
            /// </summary>
            public QueryType QueryType { get; }

            /// <summary>
            /// Retourne la requête SQL n'ayant pas pu être exécutée, sinon une chaîne vide
            /// </summary>
            public string SqlQuery { get; }

            /// <summary>
            /// Retourne les paramètres de la requête SQL qui n'a pas pu être exécutée, sinon un tableau vide est retourné
            /// </summary>
            public object[] Parameters { get; }

            /// <summary>
            /// Retourne le message d'erreur lorsqu'une requête SQL n'a pas pu être exécutée, sinon une chaîne vide est retournée
            /// </summary>
            public string ErrorMessage { get; }

            /// <summary>
            /// Constructeur en cas de réussite d'exécution
            /// </summary>
            /// <param name="affectedRowCount">Nombre d'enregistrements affectés par l'exécution</param>
            /// <param name="newId">Nouvel identifiant généré en cas de création d'un et un seul enregistrement par une requête INSERT</param>
            public ExecuteResult(int affectedRowCount, long newId)
            {
                IsSuccess = true;
                AffectedRowCount = affectedRowCount;
                NewId = newId;
                SqlQuery = string.Empty;
                Parameters = c_NoneParameter;
                ErrorMessage = string.Empty;
            }

            /// <summary>
            /// Constructeur en cas d'échec d'exécution
            /// </summary>
            /// <param name="sqlQuery">Code SQL de la requête d'action à exécuter</param>
            /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête</param>
            /// <param name="errorMessage">Message d'erreur décrivant l'erreur qui a été détectée</param>
            public ExecuteResult(string sqlQuery, object[] parameters, string errorMessage)
            {
                IsSuccess = false;
                AffectedRowCount = 0;
                NewId = 0;
                SqlQuery = (sqlQuery == null) ? string.Empty : sqlQuery;
                Parameters = (parameters == null) ? c_NoneParameter : parameters;
                ErrorMessage = (errorMessage == null) ? string.Empty : errorMessage;
            }
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL d'action (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête d'action à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Résultat d'exécution de la requête d'action</returns>
        public IExecuteResult Execute(string sqlQuery, params object[] parameters)
        {
            try
            {
                using (var command = PrepareCommand(out QueryType queryType, QueryCategory.Writer, sqlQuery, parameters))
                {
                    var affectedRowCount = command.ExecuteNonQuery();
                    var newId = (queryType == QueryType.Insert) && (affectedRowCount == 1) ? command.LastInsertedId : 0;
                    return new ExecuteResult(affectedRowCount, newId);
                }
            }
            catch (Exception error)
            {
                Debug.WriteLine($"\nErreur d'exécution de la requête {sqlQuery} :\n{error.Message}\n");
                return new ExecuteResult(sqlQuery, parameters, error.Message);
            }
        }
    }
}

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
        /// Permet de récupérer l'un après l'autre, les enregistrement(s) résultant de l'exécution d'une requête de consultation
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Les enregistrements, les uns après les autres, qui résultent de l'exécution de la requête de consultation</returns>
        public IEnumerable<IRow> GetRows(string sqlQuery, params object[] parameters)
        {
            return GetRows(false, sqlQuery, parameters);
        }

        /// <summary>
        /// Permet de récupérer l'un après l'autre, les enregistrement(s) résultant de l'exécution d'une requête de consultation
        /// </summary>
        /// <param name="errorMustGenerateResult">Indique si une erreur d'exécution doit générer un IRowResult</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Soit les enregistrements, les uns après les autres, et résultant de l'exécution de la requête de consultation, soit éventuellement, un objet IRowResult décrivant une erreur</returns>
        public IEnumerable<IRowResult> GetRows(bool errorMustGenerateResult, string sqlQuery, params object[] parameters)
        {
            GetRowEnumerator rowEnumerator = new GetRowEnumerator(this);
            return rowEnumerator.Execute(errorMustGenerateResult, sqlQuery, parameters);
        }

        /// <summary>
        /// Classe à usage interne permettant de créer un énumérateur d'enregistrements résultant de l'exécution d'une requête de consultation
        /// </summary>
        private class GetRowEnumerator : IDisposable
        {
            /// <summary>
            /// Objet gérant une nouvelle connexion à la base de données MySql, spéciquement pour l'exécution d'une requête de consultation de plusieurs enregistrements
            /// </summary>
            private Db m_Db;

            /// <summary>
            /// Objet de commande d'exécution de la requête de consultation de plusieurs enregistrements
            /// </summary>
            private MySqlCommand m_Command;

            /// <summary>
            /// Objet de lecture des enregistrements résultant de l'exécution de cette requête de consultation
            /// </summary>
            private MySqlDataReader m_Reader;

            /// <summary>
            /// Constructeur spécifique
            /// </summary>
            /// <param name="dbSource">Objet source fournissant les paramètres de connexion</param>
            public GetRowEnumerator(Db dbSource)
            {
                m_Db = new Db(dbSource);
                m_Command = null;
                m_Reader = null;
            }

            /// <summary>
            /// Méthode appelée pour libérer les ressources de cet objet, à savoir, l'objet de connexion à la base de données, la commande d'exécution et le lecteur d'enregistrements
            /// </summary>
            public void Dispose()
            {
                if (m_Reader != null)
                {
                    m_Reader.DisposeAsync();
                    m_Reader = null;
                }
                if (m_Command != null)
                {
                    m_Command.Dispose();
                    m_Command = null;
                }
                if (m_Db != null)
                {
                    m_Db.Dispose();
                    m_Db = null;
                }
            }

            /// <summary>
            /// Lance l'exécution de la requête de consultation d'enregistrements
            /// </summary>
            /// <param name="errorMustGenerateResult">Indique si on doit retourner un objet décrivant l'erreur d'exécution, ou pas</param>
            /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
            /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
            /// <returns>Chaque enregistrement trouvé, et en cas d'erreur, éventuellement un objet décrivant l'erreur d'exécution rencontré</returns>
            public IEnumerable<IRowResult> Execute(bool errorMustGenerateResult, string sqlQuery, params object[] parameters)
            {
                try
                {
                    IRowResult errorDetected = null;
                    RowResult row = null;
                    int recordsetIndex = 0;
                    while (true)
                    {
                        try
                        {
                            if (m_Command == null)
                            {
                                m_Command = m_Db.PrepareCommand(out QueryType queryType, QueryCategory.Reader, sqlQuery, parameters);
                                m_Reader = m_Command.ExecuteReader();
                            }
                            bool done = false;
                            while (!m_Reader.Read())
                            {
                                recordsetIndex++;
                                if (!m_Reader.NextResult())
                                {
                                    done = true;
                                    break;
                                }
                                row = null;
                            }
                            if (done) break;
                            if (row == null)
                            {
                                var fieldNames = new string[m_Reader.FieldCount];
                                for (int i = 0; i < fieldNames.Length; i++)
                                {
                                    fieldNames[i] = m_Reader.GetName(i);
                                }
                                row = RowResult.Create(recordsetIndex, fieldNames);
                            }
                            if (!row.SetValues(m_Reader))
                            {
                                throw new Exception("Erreur interne lors de la méthode SetValues() !");
                            }
                        }
                        catch (Exception error)
                        {
                            Debug.WriteLine($"\nErreur de récupération du premier enregistrement pour la requête {sqlQuery} :\n{error.Message}\n");
                            errorDetected = RowResult.Create(recordsetIndex, sqlQuery, parameters, error.Message);
                            break;
                        }
                        yield return row;
                    }
                    if ((errorDetected != null) && errorMustGenerateResult)
                    {
                        yield return errorDetected;
                    }
                }
                finally
                {
                    Dispose();
                }
            }
        }
    }
}

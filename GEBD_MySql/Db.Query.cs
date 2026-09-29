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
        /// Type de requête SQL
        /// </summary>
        public enum QueryType
        {
            /// <summary>
            /// Requête inconnue (considéré comme une erreur)
            /// </summary>
            Unknown,
            /// <summary>
            /// Requête d'insertion (INSERT INTO)
            /// </summary>
            Insert,
            /// <summary>
            /// Requête de mise à jour (UPDATE)
            /// </summary>
            Update,
            /// <summary>
            /// Requête de suppression (DELETE FROM)
            /// </summary>
            Delete,
            /// <summary>
            /// Requête de consultation (SELECT)
            /// </summary>
            Select,
            /// <summary>
            /// Requête d'appel de procédure (CALL)
            /// </summary>
            Call,
            /// <summary>
            /// Requête d'interrogation structurelle (SHOW)
            /// </summary>
            Show
        }

        /// <summary>
        /// Catégorie de requête SQL
        /// </summary>
        public enum QueryCategory
        {
            /// <summary>
            /// Requête inconnue (considéré comme une erreur)
            /// </summary>
            Unknown,
            /// <summary>
            /// Requête d'action (qui modifie les enregistrements de la base de données)
            /// </summary>
            Writer,
            /// <summary>
            /// Requête de consultation (qui lit des enregistrements ou des éléments structurels de la base de données, ou exécute une fonction enregistrée)
            /// </summary>
            Reader
        }

        /// <summary>
        /// Retourne la catégorie de requête SQL en fonction du type de requête spécifié
        /// </summary>
        /// <param name="queryType">Type de requête SQL</param>
        /// <returns>Catégorie de cette requête SQL</returns>
        public static QueryCategory CategoryOf(QueryType queryType)
        {
            switch (queryType)
            {
                case QueryType.Select:
                    return QueryCategory.Reader;
                case QueryType.Insert:
                case QueryType.Update:
                case QueryType.Delete:
                    return QueryCategory.Writer;
                case QueryType.Call:
                case QueryType.Show:
                    return QueryCategory.Reader;
                //case QueryType.Unknown:
                default:
                    return QueryCategory.Unknown;
            }
        }

        /// <summary>
        /// Tableau des caractères assimilables à des espaces
        /// </summary>
        private static readonly char[] c_SpaceCharacters = new char[] { ' ', '\t', '\r', '\n' };

        /// <summary>
        /// Tableau des mots clés de début de requête SQL
        /// </summary>
        private static readonly string[] c_SqlKeywords = Enum.GetNames(typeof(QueryType)).Select(name => name.ToUpper()).ToArray();

        /// <summary>
        /// Tableau des types de données supportés comme valeur
        /// </summary>
        private static readonly Type[] c_ValueTypes = new Type[]
        {
            typeof(char),
            typeof(string),
            typeof(StringBuilder),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(float),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime)
        };

        /// <summary>
        /// Crée un objet d'exécution de requête MySql sur la connexion courante
        /// </summary>
        /// <param name="queryType">Type de cette requête SQL si reconnue, sinon QueryType.Unknown</param>
        /// <param name="expectedQueryCategory">Catégorie de requête attendue</param>
        /// <param name="sqlQuery">Requête SQL à "préparer"</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Objet d'exécution de requeête MySql si possible, sinon null</returns>
        /// <exception cref="Exception">Toute exception relative à une erreur "flagrante" de syntaxe SQL, ou d'erreur de la paramétrisation de la requête, ou d'un problème de connexion</exception>
        private MySqlCommand PrepareCommand(out QueryType queryType, QueryCategory expectedQueryCategory, string sqlQuery, params object[] parameters)
        {
            queryType = QueryType.Unknown;
            MySqlCommand command = null;
            if (!TryConnect()) throw new Exception("Échec de connexion !");
            if (string.IsNullOrWhiteSpace(sqlQuery)) throw new Exception("Absence de requête SQL !");
            sqlQuery = sqlQuery.TrimStart(c_SpaceCharacters);
            string firstKeyword = sqlQuery.Split(c_SpaceCharacters, 2)[0].ToUpper();
            int indexKeyword = c_SqlKeywords.IndexOf(firstKeyword);
            if (indexKeyword <= (int)QueryType.Unknown) throw new Exception("Type de requête SQL inconnu !");
            queryType = (QueryType)indexKeyword;
            if (expectedQueryCategory == QueryCategory.Unknown) throw new Exception("Catégorie attendue de requête SQL incorrecte !");
            QueryCategory queryCategory = CategoryOf(queryType);
            if (queryCategory != expectedQueryCategory) throw new Exception($"La requête SQL spécifiée est de type {queryType}, donc de catégorie {queryCategory}, alors que l'on attend la catégorie {expectedQueryCategory} !");
            command = new MySqlCommand(sqlQuery, m_Connection);
            if ((parameters != null) && (parameters.Length >= 1))
            {
                if ((parameters.Length == 1) && (parameters[0] is object[]))
                {
                    parameters = parameters[0] as object[];
                }
                DetectInjectionCharacters(sqlQuery, out bool at, out bool questionMark);
                if (!at && !questionMark)
                {
                    throw new Exception($"Erreur de paramétrisation de requête SQL : aucun marqueur d'injection détecté dans la requête, alors qu'il existe un ou plusieurs paramètres !");
                }
                else if (at && questionMark)
                {
                    throw new Exception($"Erreur de paramétrisation de requête SQL : les deux types de marqueur d'injection ont été détectés dans la requête, il y a donc mélange de paramètre nommé et indicé : ce qui est interdit !");
                }
                else if (at)
                {
                    SetNamedParameters(command, parameters);
                }
                else
                {
                    SetOrderedParameters(command, parameters);
                }
            }
            return command;
        }

        /// <summary>
        /// Définit tous les paramètres nommés de la commande à exécuter, à partir des paires de nom/valeur de paramètres à appliquer sur cette requête
        /// </summary>
        /// <param name="command">Commande</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête</param>
        /// <exception cref="Exception">Toute exception relative à une erreur de la paramétrisation de la requête</exception>
        private static void SetNamedParameters(MySqlCommand command, object[] parameters)
        {
            for (int nameIndex = 0; nameIndex < parameters.Length; nameIndex += 2)
            {
                int valueIndex = nameIndex + 1;
                if (valueIndex == parameters.Length) throw new Exception($"Erreur de paramétrisation de requête SQL : parité nom/valeur non respectée sur l'élément n°{nameIndex / 2 + 1} !");
                if (!(parameters[nameIndex] is string)) throw new Exception($"Erreur de paramétrisation de requête SQL : l'élément n°{nameIndex / 2 + 1} a un \"nom\" qui n'est pas une chaîne de caractères !");
                string name = ((string)parameters[nameIndex]).Trim(c_SpaceCharacters);
                if (name.Length == 0) throw new Exception($"Erreur de paramétrisation de requête SQL : l'élément n°{nameIndex / 2 + 1} a un nom vide !");
                if (command.Parameters.OfType<MySqlParameter>().Any(parameter => parameter.ParameterName.Equals(name))) throw new Exception($"Erreur de paramétrisation de requête SQL : le nom \"{name}\" de l'élément n°{nameIndex / 2 + 1} existe déjà !");
                object value = parameters[valueIndex];
                if (value != null)
                {
                    Type valueType = value.GetType();
                    if (!c_ValueTypes.Contains(valueType)) throw new Exception($"Erreur de paramétrisation de requête SQL : la valeur associée à \"{name}\" (élément n°{nameIndex / 2 + 1}) est d'un type non acceptable : {valueType.FullName} !");
                    if ((valueType == typeof(StringBuilder)) || (valueType == typeof(char)))
                    {
                        value = value.ToString();
                    }
                    else if (valueType == typeof(bool))
                    {
                        value = ((bool)value) ? (sbyte)1 : (sbyte)0;
                    }
                }
                command.Parameters.Add(new MySqlParameter(name, value));
            }
        }

        /// <summary>
        /// Définit tous les paramètres ordonnancés de la commande à exécuter, à partir des valeurs de paramètres à appliquer sur cette requête
        /// </summary>
        /// <param name="command">Commande</param>
        /// <param name="parameters">Série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <exception cref="Exception">Toute exception relative à une erreur de la paramétrisation de la requête</exception>
        private static void SetOrderedParameters(MySqlCommand command, object[] parameters)
        {
            for (int valueIndex = 0; valueIndex < parameters.Length; valueIndex++)
            {
                object value = parameters[valueIndex];
                if (value != null)
                {
                    Type valueType = value.GetType();
                    if (!c_ValueTypes.Contains(valueType)) throw new Exception($"Erreur de paramétrisation de requête SQL : la valeur (élément n°{valueIndex + 1}) est d'un type non acceptable : {valueType.FullName} !");
                    if ((valueType == typeof(StringBuilder)) || (valueType == typeof(char)))
                    {
                        value = value.ToString();
                    }
                    else if (valueType == typeof(bool))
                    {
                        value = ((bool)value) ? (sbyte)1 : (sbyte)0;
                    }
                }
                command.Parameters.Add(new MySqlParameter() { Value = value });
            }
        }

        /// <summary>
        /// Vérifie si on détecte la présence de marqueurs d'injection au sein d'une requête SQL
        /// </summary>
        /// <param name="sqlQuery">Requête SQL à analyser</param>
        /// <param name="at">Indique la présence de marqueur(s) d'injection pour paramètre(s) nommé(s)</param>
        /// <param name="questionMark">Indique la présence de marqueur(s) d'injection pour paramètre(s) indicé(s)</param>
        private static void DetectInjectionCharacters(string sqlQuery, out bool at, out bool questionMark)
        {
            at = false;
            questionMark = false;
            bool apostrophe = false;
            bool quotationMark = false;
            for (int i = 0; i < sqlQuery.Length; i++)
            {
                if ((sqlQuery[i] == '\'') && ((i == 0) || (sqlQuery[i - 1] != '\\')) && !quotationMark)
                {
                    apostrophe = !apostrophe;
                }
                else if ((sqlQuery[i] == '"') && ((i == 0) || (sqlQuery[i - 1] != '\\')) && !apostrophe)
                {
                    quotationMark = !quotationMark;
                }
                else if ((sqlQuery[i] == '?') && !apostrophe && !quotationMark)
                {
                    questionMark = true;
                }
                else if ((sqlQuery[i] == '@') && !apostrophe && !quotationMark)
                {
                    at = true;
                }
            }
        }
    }
}

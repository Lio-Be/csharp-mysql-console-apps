using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Debug = System.Diagnostics.Debug;

namespace GEBD.MySql
{
    /// <summary>
    /// Contient des méthodes d'extension relatives à la classe Db et ce qu'elle contient comme sous-type
    /// </summary>
    public static partial class DbEx
    {
        /// <summary>
        /// Catégorie de requête
        /// </summary>
        /// <param name="executeResult">Résultat d'une requête d'action</param>
        /// <returns>Catégorie de requête</returns>
        public static Db.QueryCategory Category(this Db.IExecuteResult executeResult)
        {
            return Db.CategoryOf(executeResult.QueryType);
        }
    }
}

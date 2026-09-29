using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEBD.MySql
{
    /// <summary>
    /// Définit les paramètres de connexion à un serveur MySql pour y manipuler une base de données
    /// </summary>
    public class DbSettings : IEquatable<DbSettings>
    {
        /// <summary>
        /// Adresse du serveur
        /// </summary>
        public string ServerAddress { get; private set; }

        /// <summary>
        /// Port de communication du serveur
        /// </summary>
        public int ServerPort { get; private set; }

        /// <summary>
        /// Nom d'utilisateur pour s'authentifier sur le serveur et y gagner ses droits d'accès
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Mot de passe de l'utilisateur utilisé lors de l'authentification
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Nom de la base de données à manipuler
        /// </summary>
        public string DbName { get; private set; }

        /// <summary>
        /// Instancie, si possible, un objet DbSettings avec les paramètres spécifiés
        /// <para>Port de communication du serveur = 3306</para>
        /// </summary>
        /// <param name="serverAddress">Adresse du serveur</param>
        /// <param name="userName">Nom d'utilisateur MySql pour authentification</param>
        /// <param name="password">Mot de passe de cet utilisateur</param>
        /// <param name="dbName">Nom de la base de données à manipuler</param>
        /// <returns>Objet des paramètres de connexion si possible, sinon null</returns>
        public static DbSettings Create(string serverAddress, string userName, string password, string dbName)
        {
            return Create(serverAddress, 3306, userName, password, dbName);
        }

        /// <summary>
        /// Instancie, si possible, un objet DbSettings avec les paramètres spécifiés
        /// </summary>
        /// <param name="serverAddress">Adresse du serveur</param>
        /// <param name="serverPort">Port de communication du serveur</param>
        /// <param name="userName">Nom d'utilisateur MySql pour authentification</param>
        /// <param name="password">Mot de passe de cet utilisateur</param>
        /// <param name="dbName">Nom de la base de données à manipuler</param>
        /// <returns>Objet des paramètres de connexion si possible, sinon null</returns>
        public static DbSettings Create(string serverAddress, int serverPort, string userName, string password, string dbName)
        {
            if (string.IsNullOrWhiteSpace(serverAddress)) return null;
            if ((serverPort <= 0) || (serverPort > 65535)) return null;
            if (string.IsNullOrWhiteSpace(userName)) return null;
            if (password == null) password = string.Empty;
            if (string.IsNullOrWhiteSpace(dbName)) return null;
            return new DbSettings(serverAddress.Trim(), serverPort, userName.Trim(), password, dbName.Trim());
        }

        /// <summary>
        /// Constructeur spécifique privé
        /// </summary>
        /// <param name="serverAddress">Adresse du serveur</param>
        /// <param name="serverPort">Port de communication du serveur</param>
        /// <param name="userName">Nom d'utilisateur MySql pour authentification</param>
        /// <param name="password">Mot de passe de cet utilisateur</param>
        /// <param name="dbName">Nom de la base de données à manipuler</param>
        private DbSettings(string serverAddress, int serverPort, string userName, string password, string dbName)
        {
            ServerAddress = serverAddress;
            ServerPort = serverPort;
            UserName = userName;
            Password = password;
            DbName = dbName;
        }

        #region Implémentation de l'interface IEquatable<DbSettings>
        public bool Equals(DbSettings other)
        {
            return (other != null)
                && ServerAddress.Equals(other.ServerAddress, StringComparison.OrdinalIgnoreCase)
                && ServerPort.Equals(other.ServerPort)
                && UserName.Equals(other.UserName)
                && Password.Equals(other.Password)
                && DbName.Equals(other.DbName, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}

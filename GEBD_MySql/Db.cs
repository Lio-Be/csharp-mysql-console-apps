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
        /// Compteur d'occurrences "actives"
        /// </summary>
        private static int s_Count = 0;

        /// <summary>
        /// Retourne le nombre d'occurrences "actives" d'objet de type Db
        /// </summary>
        public static int Count => s_Count;

        /// <summary>
        /// Indique si cet objet a déjà subi l'appel de sa méthode Dispose
        /// </summary>
        private bool m_Disposed;

        /// <summary>
        /// Paramètres actuels de connexion
        /// </summary>
        private DbSettings m_Settings;

        /// <summary>
        /// Objet de connexion au serveur MySql
        /// </summary>
        private MySqlConnection m_Connection;

        /// <summary>
        /// Paramètres de connexion
        /// </summary>
        public DbSettings Settings
        {
            get
            {
                return m_Settings;
            }
            set
            {
                if (ReferenceEquals(value, m_Settings) || ((value != null) && value.Equals(m_Settings))) return;
                Disconnect();
                m_Settings = value;
            }
        }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Db()
        {
            s_Count++;
            m_Disposed = false;
            m_Settings = null;
            m_Connection = null;
        }

        /// <summary>
        /// Constructeur spécifique
        /// </summary>
        /// <param name="settings">Paramètres de connexion</param>
        public Db(DbSettings settings)
            : this()
        {
            m_Settings = settings;
        }

        /// <summary>
        /// Constructeur par copie
        /// </summary>
        /// <param name="source">Objet de type Db qui est la source de cette copie</param>
        private Db(Db source)
            : this()
        {
            m_Settings = source.m_Settings;
        }

        /// <summary>
        /// Méthode permettant de libérer les ressources de cet objet
        /// <para>Implémentation de l'interface IDisposable</para>
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            if (!m_Disposed)
            {
                m_Disposed = true;
                s_Count--;
            }
        }

        /// <summary>
        /// Tente de se connecter au serveur MySql si besoin est
        /// </summary>
        /// <returns>Vrai si la connexion est déjà établie, ou a pu être établie, sinon faux</returns>
        private bool TryConnect()
        {
            if (m_Connection != null) return true;
            try
            {
                if (m_Settings == null) throw new Exception("Aucun paramètre de connexion défini !");
                m_Connection = new MySqlConnection();
                m_Connection.ConnectionString = $"user={m_Settings.UserName};pwd={m_Settings.Password};server={m_Settings.ServerAddress};port={m_Settings.ServerPort}";
                m_Connection.Open();
                using (var command = new MySqlCommand($"USE `{m_Settings.DbName}`;", m_Connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception error)
            {
                Debug.WriteLine($"\nErreur de tentative de connexion au serveur {m_Settings.ServerAddress}, sur le port {m_Settings.ServerPort}, avec l'utilisateur {m_Settings.UserName} et son mot de passe {new string('*', m_Settings.Password.Length)} :\n{error.Message}\n");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Ferme la connexion avec le serveur MySql
        /// </summary>
        /// <returns>Vrai en cas de réussite de déconnexion du serveur, sinon faux</returns>
        private bool Disconnect()
        {
            if (m_Connection == null) return true;
            try
            {
                m_Connection.Dispose();
                return true;
            }
            catch (Exception error)
            {
                Debug.WriteLine($"\nErreur de déconnexion du serveur {m_Settings.ServerAddress}, sur le port {m_Settings.ServerPort}, avec l'utilisateur {m_Settings.UserName} et son mot de passe {new string('*', m_Settings.Password.Length)} :\n{error.Message}\n");
                return false;
            }
            finally
            {
                m_Connection = null;
            }
        }
    }
}

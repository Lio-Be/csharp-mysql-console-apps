using MySql.Data.MySqlClient;
using Mysqlx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// Définit un enregistrement résultant de l'exécution réussie d'une requête SQL de consultation
        /// </summary>
        public interface IRow
        {
            /// <summary>
            /// Indice de jeu d'enregistrements auquel appartient celui-ci
            /// </summary>
            int RecordsetIndex { get; }

            /// <summary>
            /// Indice d'enregistrements dans le jeu d'enregistrements auquel il appartient
            /// </summary>
            int RecordIndex { get; }

            /// <summary>
            /// Nombre de champs d'enregistrement
            /// </summary>
            int FieldCount { get; }

            /// <summary>
            /// Retourne le nom du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer le nom</param>
            /// <returns>Nom du champ spécifié par son indice, sinon null</returns>
            string GetName(int index);

            #region Récupération de la valeur du champ spécifié par son indice
            /// <summary>
            /// Retourne la valeur non typée du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur non typée du champ spécifié par son indice, sinon null</returns>
            object this[int index] { get; }

            /// <summary>
            /// Retourne la valeur typée string du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée string du champ spécifié par son indice, sinon null</returns>
            string GetString(int index);

            /// <summary>
            /// Retourne la valeur typée string du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée string du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type string, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetString(int index, out string value, string defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée sbyte du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée sbyte du champ spécifié par son indice, sinon 0</returns>
            sbyte GetSByte(int index);

            /// <summary>
            /// Retourne la valeur typée sbyte du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée sbyte du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type sbyte, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetSByte(int index, out sbyte value, sbyte defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée byte du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée byte du champ spécifié par son indice, sinon 0</returns>
            byte GetByte(int index);

            /// <summary>
            /// Retourne la valeur typée byte du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée byte du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type byte, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetByte(int index, out byte value, byte defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée short du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée short du champ spécifié par son indice, sinon 0</returns>
            short GetShort(int index);

            /// <summary>
            /// Retourne la valeur typée short du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée short du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type short, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetShort(int index, out short value, short defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée ushort du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée ushort du champ spécifié par son indice, sinon 0</returns>
            ushort GetUShort(int index);

            /// <summary>
            /// Retourne la valeur typée ushort du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée ushort du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type ushort, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetUShort(int index, out ushort value, ushort defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée int du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée int du champ spécifié par son indice, sinon 0</returns>
            int GetInt(int index);

            /// <summary>
            /// Retourne la valeur typée int du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée int du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type int, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetInt(int index, out int value, int defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée uint du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée uint du champ spécifié par son indice, sinon 0</returns>
            uint GetUInt(int index);

            /// <summary>
            /// Retourne la valeur typée uint du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée uint du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type uint, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetUInt(int index, out uint value, uint defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée long du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée long du champ spécifié par son indice, sinon 0</returns>
            long GetLong(int index);

            /// <summary>
            /// Retourne la valeur typée long du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée long du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type long, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetLong(int index, out long value, long defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée ulong du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée ulong du champ spécifié par son indice, sinon 0</returns>
            ulong GetULong(int index);

            /// <summary>
            /// Retourne la valeur typée ulong du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée ulong du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type ulong, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetULong(int index, out ulong value, ulong defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée float du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée float du champ spécifié par son indice, sinon 0</returns>
            float GetFloat(int index);

            /// <summary>
            /// Retourne la valeur typée float du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée float du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type float, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetFloat(int index, out float value, float defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée double du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée double du champ spécifié par son indice, sinon 0</returns>
            double GetDouble(int index);

            /// <summary>
            /// Retourne la valeur typée double du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée double du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type double, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetDouble(int index, out double value, double defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée decimal du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée decimal du champ spécifié par son indice, sinon 0</returns>
            decimal GetDecimal(int index);

            /// <summary>
            /// Retourne la valeur typée decimal du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée decimal du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type decimal, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetDecimal(int index, out decimal value, decimal defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée bool du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée bool du champ spécifié par son indice, sinon false</returns>
            bool GetBool(int index);

            /// <summary>
            /// Retourne la valeur typée bool du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée bool du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type bool, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetBool(int index, out bool value, bool defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée DateTime du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée DateTime du champ spécifié par son indice, sinon DateTime.MinValue</returns>
            DateTime GetDateTime(int index);

            /// <summary>
            /// Retourne la valeur typée DateTime du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée DateTime du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type DateTime, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetDateTime(int index, out DateTime value, DateTime defaultValue = default);
            #endregion

            #region Récupération de la valeur du champ spécifié par son nom
            /// <summary>
            /// Retourne la valeur non typée du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur non typée du champ spécifié par son nom, sinon null</returns>
            object this[string fieldName] { get; }

            /// <summary>
            /// Retourne la valeur typée string du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée string du champ spécifié par son nom, sinon null</returns>
            string GetString(string fieldName);

            /// <summary>
            /// Retourne la valeur typée string du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée string du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type string, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetString(string fieldName, out string value, string defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée sbyte du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée sbyte du champ spécifié par son nom, sinon 0</returns>
            sbyte GetSByte(string fieldName);

            /// <summary>
            /// Retourne la valeur typée sbyte du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée sbyte du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type sbyte, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetSByte(string fieldName, out sbyte value, sbyte defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée byte du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée byte du champ spécifié par son nom, sinon 0</returns>
            byte GetByte(string fieldName);

            /// <summary>
            /// Retourne la valeur typée byte du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée byte du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type byte, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetByte(string fieldName, out byte value, byte defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée short du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée short du champ spécifié par son nom, sinon 0</returns>
            short GetShort(string fieldName);

            /// <summary>
            /// Retourne la valeur typée short du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée short du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type short, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetShort(string fieldName, out short value, short defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée ushort du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée ushort du champ spécifié par son nom, sinon 0</returns>
            ushort GetUShort(string fieldName);

            /// <summary>
            /// Retourne la valeur typée ushort du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée ushort du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type ushort, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetUShort(string fieldName, out ushort value, ushort defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée int du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée int du champ spécifié par son nom, sinon 0</returns>
            int GetInt(string fieldName);

            /// <summary>
            /// Retourne la valeur typée int du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée int du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type int, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetInt(string fieldName, out int value, int defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée uint du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée uint du champ spécifié par son nom, sinon 0</returns>
            uint GetUInt(string fieldName);

            /// <summary>
            /// Retourne la valeur typée uint du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée uint du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type uint, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetUInt(string fieldName, out uint value, uint defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée long du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée long du champ spécifié par son nom, sinon 0</returns>
            long GetLong(string fieldName);

            /// <summary>
            /// Retourne la valeur typée long du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée long du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type long, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetLong(string fieldName, out long value, long defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée ulong du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée ulong du champ spécifié par son nom, sinon 0</returns>
            ulong GetULong(string fieldName);

            /// <summary>
            /// Retourne la valeur typée ulong du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée ulong du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type ulong, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetULong(string fieldName, out ulong value, ulong defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée float du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée float du champ spécifié par son nom, sinon 0</returns>
            float GetFloat(string fieldName);

            /// <summary>
            /// Retourne la valeur typée float du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée float du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type float, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetFloat(string fieldName, out float value, float defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée double du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée double du champ spécifié par son nom, sinon 0</returns>
            double GetDouble(string fieldName);

            /// <summary>
            /// Retourne la valeur typée double du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée double du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type double, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetDouble(string fieldName, out double value, double defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée decimal du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée decimal du champ spécifié par son nom, sinon 0</returns>
            decimal GetDecimal(string fieldName);

            /// <summary>
            /// Retourne la valeur typée decimal du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée decimal du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type decimal, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetDecimal(string fieldName, out decimal value, decimal defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée bool du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée bool du champ spécifié par son nom, sinon 0</returns>
            bool GetBool(string fieldName);

            /// <summary>
            /// Retourne la valeur typée bool du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée bool du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type bool, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetBool(string fieldName, out bool value, bool defaultValue = default);

            /// <summary>
            /// Retourne la valeur typée DateTime du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée DateTime du champ spécifié par son nom, sinon 0</returns>
            DateTime GetDateTime(string fieldName);

            /// <summary>
            /// Retourne la valeur typée DateTime du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée DateTime du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type DateTime, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            bool GetDateTime(string fieldName, out DateTime value, DateTime defaultValue = default);
            #endregion
        }

        /// <summary>
        /// Définit un résultat d'exécution de requête SQL de consultation
        /// </summary>
        public interface IRowResult : IRow
        {
            /// <summary>
            /// Indique si l'exécution d'une requête SQL de consultation a réussi
            /// </summary>
            bool IsSuccess { get; }

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
        /// Implémente un enregistrement résultant de l'exécution réussie d'une requête SQL de consultation
        /// </summary>
        private class Row : IRow
        {
            /// <summary>
            /// Tableau contenant les noms des champs
            /// </summary>
            private string[] m_FieldNames;

            /// <summary>
            /// Tableau contenant les valeurs des champs pour l'enregistrement courant
            /// </summary>
            private object[] m_FieldValues;

            /// <summary>
            /// Dictionnaire d'association nom/valeur des champs pour l'enregistrement courant
            /// </summary>
            private Dictionary<string, object> m_NamedValues;

            /// <summary>
            /// Indice de jeu d'enregistrements auquel appartient celui-ci
            /// </summary>
            public int RecordsetIndex { get; }

            /// <summary>
            /// Indice d'enregistrements dans le jeu d'enregistrements auquel il appartient
            /// </summary>
            public int RecordIndex { get; private set; }

            /// <summary>
            /// Nombre de champs d'enregistrement
            /// </summary>
            public int FieldCount => m_FieldNames.Length;

            /// <summary>
            /// Retourne le nom du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer le nom</param>
            /// <returns>Nom du champ spécifié par son indice, sinon null</returns>
            public string GetName(int index) => ((index >= 0) && (index < m_FieldNames.Length)) ? m_FieldNames[index] : null;

            #region Récupération de la valeur du champ spécifié par son indice
            /// <summary>
            /// Retourne la valeur non typée du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur non typée du champ spécifié par son indice, sinon null</returns>
            public object this[int index] => ((index >= 0) && (index < m_FieldValues.Length)) ? m_FieldValues[index] : null;

            /// <summary>
            /// Retourne la valeur typée string du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée string du champ spécifié par son indice, sinon null</returns>
            public string GetString(int index) => ((index >= 0) && (index < m_FieldValues.Length) && ((m_FieldValues[index] is string) || (m_FieldValues[index] == null))) ? (string)m_FieldValues[index] : null;

            /// <summary>
            /// Retourne la valeur typée string du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée string du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type string, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetString(int index, out string value, string defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && ((m_FieldValues[index] is string) || (m_FieldValues[index] == null)))) return false;
                value = (string)m_FieldValues[index];
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée sbyte du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée sbyte du champ spécifié par son indice, sinon 0</returns>
            public sbyte GetSByte(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is sbyte typedValue)) ? typedValue : (sbyte)0;

            /// <summary>
            /// Retourne la valeur typée sbyte du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée sbyte du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type sbyte, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetSByte(int index, out sbyte value, sbyte defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is sbyte typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée byte du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée byte du champ spécifié par son indice, sinon 0</returns>
            public byte GetByte(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is byte typedValue)) ? typedValue : (byte)0;

            /// <summary>
            /// Retourne la valeur typée byte du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée byte du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type byte, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetByte(int index, out byte value, byte defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is byte typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée short du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée short du champ spécifié par son indice, sinon 0</returns>
            public short GetShort(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is short typedValue)) ? typedValue : (short)0;

            /// <summary>
            /// Retourne la valeur typée short du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée short du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type short, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetShort(int index, out short value, short defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is short typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée ushort du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée ushort du champ spécifié par son indice, sinon 0</returns>
            public ushort GetUShort(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is ushort typedValue)) ? typedValue : (ushort)0;

            /// <summary>
            /// Retourne la valeur typée ushort du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée ushort du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type ushort, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetUShort(int index, out ushort value, ushort defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is ushort typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée int du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée int du champ spécifié par son indice, sinon 0</returns>
            public int GetInt(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is int typedValue)) ? typedValue : (int)0;

            /// <summary>
            /// Retourne la valeur typée int du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée int du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type int, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetInt(int index, out int value, int defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is int typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée uint du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée uint du champ spécifié par son indice, sinon 0</returns>
            public uint GetUInt(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is uint typedValue)) ? typedValue : (uint)0;

            /// <summary>
            /// Retourne la valeur typée uint du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée uint du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type uint, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetUInt(int index, out uint value, uint defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is uint typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée long du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée long du champ spécifié par son indice, sinon 0</returns>
            public long GetLong(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is long typedValue)) ? typedValue : (long)0;

            /// <summary>
            /// Retourne la valeur typée long du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée long du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type long, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetLong(int index, out long value, long defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is long typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée ulong du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée ulong du champ spécifié par son indice, sinon 0</returns>
            public ulong GetULong(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is ulong typedValue)) ? typedValue : (ulong)0;

            /// <summary>
            /// Retourne la valeur typée ulong du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée ulong du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type ulong, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetULong(int index, out ulong value, ulong defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is ulong typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée float du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée float du champ spécifié par son indice, sinon 0</returns>
            public float GetFloat(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is float typedValue)) ? typedValue : (float)0;

            /// <summary>
            /// Retourne la valeur typée float du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée float du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type float, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetFloat(int index, out float value, float defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is float typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée double du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée double du champ spécifié par son indice, sinon 0</returns>
            public double GetDouble(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is double typedValue)) ? typedValue : (double)0;

            /// <summary>
            /// Retourne la valeur typée double du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée double du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type double, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetDouble(int index, out double value, double defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is double typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée decimal du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée decimal du champ spécifié par son indice, sinon 0</returns>
            public decimal GetDecimal(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is decimal typedValue)) ? typedValue : (decimal)0;

            /// <summary>
            /// Retourne la valeur typée decimal du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée decimal du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type decimal, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetDecimal(int index, out decimal value, decimal defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is decimal typedValue))) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée bool du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée bool du champ spécifié par son indice, sinon false</returns>
            public bool GetBool(int index) => GetBool(index, out var result) ? result : false;

            /// <summary>
            /// Retourne la valeur typée bool du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée bool du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type bool, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetBool(int index, out bool value, bool defaultValue = default)
            {
                value = defaultValue;
                if ((index < 0) || (index >= m_FieldValues.Length)) return false;
                object fieldValue = m_FieldValues[index];
                if (fieldValue is sbyte typeValueSB) { value = (typeValueSB != 0); return true; }
                if (fieldValue is byte typeValueB) { value = (typeValueB != 0); return true; }
                if (fieldValue is short typeValueS) { value = (typeValueS != 0); return true; }
                if (fieldValue is ushort typeValueUS) { value = (typeValueUS != 0); return true; }
                if (fieldValue is int typeValueI) { value = (typeValueI != 0); return true; }
                if (fieldValue is uint typeValueUI) { value = (typeValueUI != 0); return true; }
                if (fieldValue is long typeValueL) { value = (typeValueL != 0); return true; }
                if (fieldValue is ulong typeValueUL) { value = (typeValueUL != 0); return true; }
                return false;
            }

            /// <summary>
            /// Retourne la valeur typée DateTime du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée DateTime du champ spécifié par son indice, sinon DateTime.MinValue</returns>
            public DateTime GetDateTime(int index) => ((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is DateTime typedValue)) ? typedValue : DateTime.MinValue;

            /// <summary>
            /// Retourne la valeur typée DateTime du champ spécifié par son indice
            /// </summary>
            /// <param name="index">Indice du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée DateTime du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type DateTime, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetDateTime(int index, out DateTime value, DateTime defaultValue = default)
            {
                value = defaultValue;
                if (!((index >= 0) && (index < m_FieldValues.Length) && (m_FieldValues[index] is DateTime typedValue))) return false;
                value = typedValue;
                return true;
            }
            #endregion

            #region Récupération de la valeur du champ spécifié par son nom
            /// <summary>
            /// Retourne la valeur non typée du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur non typée du champ spécifié par son nom, sinon null</returns>
            public object this[string fieldName] => !string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) ? value : null;

            /// <summary>
            /// Retourne la valeur typée string du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée string du champ spécifié par son nom, sinon null</returns>
            public string GetString(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && ((value is string) || (value == null))) ? (string)value : (string)null;

            /// <summary>
            /// Retourne la valeur typée string du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée string du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type string, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetString(string fieldName, out string value, string defaultValue = default)
            {
                value = defaultValue;
                if (string.IsNullOrEmpty(fieldName) || !m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || (!(fieldValue is string) && (fieldValue != null))) return false;
                value = (string)fieldValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée sbyte du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée sbyte du champ spécifié par son nom, sinon 0</returns>
            public sbyte GetSByte(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is sbyte typedValue)) ? typedValue : (sbyte)0;

            /// <summary>
            /// Retourne la valeur typée sbyte du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée sbyte du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type sbyte, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetSByte(string fieldName, out sbyte value, sbyte defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is sbyte typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée byte du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée byte du champ spécifié par son nom, sinon 0</returns>
            public byte GetByte(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is byte typedValue)) ? typedValue : (byte)0;

            /// <summary>
            /// Retourne la valeur typée byte du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée byte du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type byte, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetByte(string fieldName, out byte value, byte defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is byte typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée short du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée short du champ spécifié par son nom, sinon 0</returns>
            public short GetShort(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is short typedValue)) ? typedValue : (short)0;

            /// <summary>
            /// Retourne la valeur typée short du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée short du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type short, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetShort(string fieldName, out short value, short defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is short typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée ushort du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée ushort du champ spécifié par son nom, sinon 0</returns>
            public ushort GetUShort(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is ushort typedValue)) ? typedValue : (ushort)0;

            /// <summary>
            /// Retourne la valeur typée ushort du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée ushort du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type ushort, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetUShort(string fieldName, out ushort value, ushort defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is ushort typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée int du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée int du champ spécifié par son nom, sinon 0</returns>
            public int GetInt(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is int typedValue)) ? typedValue : (int)0;

            /// <summary>
            /// Retourne la valeur typée int du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée int du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type int, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetInt(string fieldName, out int value, int defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is int typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée uint du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée uint du champ spécifié par son nom, sinon 0</returns>
            public uint GetUInt(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is uint typedValue)) ? typedValue : (uint)0;

            /// <summary>
            /// Retourne la valeur typée uint du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée uint du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type uint, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetUInt(string fieldName, out uint value, uint defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is uint typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée long du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée long du champ spécifié par son nom, sinon 0</returns>
            public long GetLong(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is long typedValue)) ? typedValue : (long)0;

            /// <summary>
            /// Retourne la valeur typée long du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée long du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type long, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetLong(string fieldName, out long value, long defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is long typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée ulong du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée ulong du champ spécifié par son nom, sinon 0</returns>
            public ulong GetULong(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is ulong typedValue)) ? typedValue : (ulong)0;

            /// <summary>
            /// Retourne la valeur typée ulong du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée ulong du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type ulong, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetULong(string fieldName, out ulong value, ulong defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is ulong typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée float du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée float du champ spécifié par son nom, sinon 0</returns>
            public float GetFloat(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is float typedValue)) ? typedValue : (float)0;

            /// <summary>
            /// Retourne la valeur typée float du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée float du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type float, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetFloat(string fieldName, out float value, float defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is float typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée double du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée double du champ spécifié par son nom, sinon 0</returns>
            public double GetDouble(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is double typedValue)) ? typedValue : (double)0;

            /// <summary>
            /// Retourne la valeur typée double du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée double du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type double, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetDouble(string fieldName, out double value, double defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is double typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée decimal du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée decimal du champ spécifié par son nom, sinon 0</returns>
            public decimal GetDecimal(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is decimal typedValue)) ? typedValue : (decimal)0;

            /// <summary>
            /// Retourne la valeur typée decimal du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée decimal du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type decimal, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetDecimal(string fieldName, out decimal value, decimal defaultValue = default)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) || !(fieldValue is decimal typedValue)) return false;
                value = typedValue;
                return true;
            }

            /// <summary>
            /// Retourne la valeur typée bool du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée bool du champ spécifié par son nom, sinon 0</returns>
            public bool GetBool(string fieldName) => GetBool(fieldName, out var result) ? result : false;

            /// <summary>
            /// Retourne la valeur typée bool du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée bool du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type bool, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetBool(string fieldName, out bool value, bool defaultValue = default)
            {
                value = defaultValue;
                if (string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue)) return false;
                if (fieldValue is sbyte typeValueSB) { value = (typeValueSB != 0); return true; }
                if (fieldValue is byte typeValueB) { value = (typeValueB != 0); return true; }
                if (fieldValue is short typeValueS) { value = (typeValueS != 0); return true; }
                if (fieldValue is ushort typeValueUS) { value = (typeValueUS != 0); return true; }
                if (fieldValue is int typeValueI) { value = (typeValueI != 0); return true; }
                if (fieldValue is uint typeValueUI) { value = (typeValueUI != 0); return true; }
                if (fieldValue is long typeValueL) { value = (typeValueL != 0); return true; }
                if (fieldValue is ulong typeValueUL) { value = (typeValueUL != 0); return true; }
                return false;
            }

            /// <summary>
            /// Retourne la valeur typée DateTime du champ spécifié par son nom
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <returns>Valeur typée DateTime du champ spécifié par son nom, sinon 0</returns>
            public DateTime GetDateTime(string fieldName) => (!string.IsNullOrEmpty(fieldName) && m_NamedValues.TryGetValue(fieldName.ToLower(), out object value) && (value is DateTime)) ? (DateTime)value : DateTime.MinValue;

            /// <summary>
            /// Retourne la valeur typée DateTime du champ spécifié par son indice
            /// </summary>
            /// <param name="fieldName">Nom du champ dont on veut récupérer la valeur</param>
            /// <param name="value">Valeur typée DateTime du champ spécifié par son indice, sinon la valeur par défaut</param>
            /// <param name="defaultValue">Valeur par défaut, de type DateTime, à retourner quand on arrive pas à trouver le champ attendu par son nom ou par son type</param>
            /// <returns>Vrai si la récupération a pu se faire, sinon faux</returns>
            public bool GetDateTime(string fieldName, out DateTime value, DateTime defaultValue)
            {
                value = defaultValue;
                if (!string.IsNullOrEmpty(fieldName) || m_NamedValues.TryGetValue(fieldName.ToLower(), out object fieldValue) && !(fieldValue is DateTime)) return false;
                value = (DateTime)fieldValue;
                return true;
            }
            #endregion

            /// <summary>
            /// Constructeur d'enregistrement
            /// </summary>
            /// <param name="recordsetIndex">Indice de jeu d'enregistrements auquel appartient celui-ci</param>
            /// <param name="fieldNames">Tableau des noms des champs</param>
            protected Row(int recordsetIndex, string[] fieldNames)
            {
                RecordsetIndex = recordsetIndex;
                RecordIndex = -1;
                m_FieldNames = fieldNames;
                m_FieldValues = new object[m_FieldNames.Length];
                m_NamedValues = new Dictionary<string, object>();
                foreach (var fieldName in m_FieldNames) m_NamedValues.Add(fieldName.ToLower(), null);
            }

            /// <summary>
            /// Instanciateur d'enregistrement
            /// </summary>
            /// <param name="recordsetIndex">Indice de jeu d'enregistrements auquel appartient celui-ci</param>
            /// <param name="fieldNames">Tableau des noms des champs</param>
            /// <returns>Objet enregistrement créé, sinon null</returns>
            public static Row Create(int recordsetIndex, string[] fieldNames)
            {
                if ((fieldNames == null) || (fieldNames.Length == 0)) return null;
                return new Row(recordsetIndex, fieldNames);
            }

            /// <summary>
            /// Permet de mettre à jour les valeurs de champ pour l'enregistrement courant
            /// </summary>
            /// <param name="reader">Objet de lecture servant à accéder aux données de l'enregistrement courant</param>
            /// <returns>Vrai si l'opération est réussie, sinon faux</returns>
            public bool SetValues(MySqlDataReader reader)
            {
                if (reader == null) return false;
                RecordIndex++;
                reader.GetValues(m_FieldValues);
                for (int i = 0; i < m_FieldNames.Length; i++)
                {
                    m_NamedValues[m_FieldNames[i].ToLower()] = m_FieldValues[i];
                }
                return true;
            }
        }

        /// <summary>
        /// Implémente un résultat d'exécution de requête SQL de consultation
        /// </summary>
        private class RowResult : Row, IRowResult
        {
            /// <summary>
            /// Tableau vide de paramètre
            /// </summary>
            private static readonly object[] c_NoneParameter = new object[0];

            /// <summary>
            /// Indique si l'exécution d'une requête SQL de consultation a réussi
            /// </summary>
            public bool IsSuccess { get; }

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
            /// Constructeur en cas de réussite de consultation d'enregistrement
            /// </summary>
            /// <param name="recordsetIndex">Indice de jeu d'enregistrements auquel appartient celui-ci</param>
            /// <param name="fieldNames">Tableau des noms des champs</param>
            private RowResult(int recordsetIndex, string[] fieldNames)
                : base(recordsetIndex, fieldNames)
            {
                IsSuccess = true;
                SqlQuery = string.Empty;
                Parameters = c_NoneParameter;
                ErrorMessage = string.Empty;
            }

            /// <summary>
            /// Constructeur en cas d'échec de consultation d'enregistrement
            /// </summary>
            /// <param name="recordsetIndex">Indice de jeu d'enregistrements auquel "appartient" cet échec</param>
            /// <param name="sqlQuery">Code SQL de la requête d'action à exécuter</param>
            /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête</param>
            /// <param name="errorMessage">Message d'erreur décrivant l'erreur qui a été détectée</param>
            private RowResult(int recordsetIndex, string sqlQuery, object[] parameters, string errorMessage)
                : base(recordsetIndex, new string[0])
            {
                IsSuccess = false;
                SqlQuery = (sqlQuery == null) ? string.Empty : sqlQuery;
                Parameters = (parameters == null) ? c_NoneParameter : parameters;
                ErrorMessage = (errorMessage == null) ? string.Empty : errorMessage;
            }

            /// <summary>
            /// Constructeur en cas de réussite de consultation d'enregistrement
            /// </summary>
            /// <param name="recordsetIndex">Indice de jeu d'enregistrements auquel appartient celui-ci</param>
            /// <param name="fieldNames">Tableau des noms des champs</param>
            /// <returns>Résultat en cas de réussite de consultation d'enregistrement, si possible, sinon null</returns>
            public static new RowResult Create(int recordsetIndex, string[] fieldNames)
            {
                if ((fieldNames == null) || (fieldNames.Length == 0)) return null;
                return new RowResult(recordsetIndex, fieldNames);
            }

            /// <summary>
            /// Constructeur en cas d'échec de consultation d'enregistrement
            /// </summary>
            /// <param name="recordsetIndex">Indice de jeu d'enregistrements auquel "appartient" cet échec</param>
            /// <param name="sqlQuery">Code SQL de la requête d'action à exécuter</param>
            /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête</param>
            /// <param name="errorMessage">Message d'erreur décrivant l'erreur qui a été détectée</param>
            /// <returns>Résultat en cas d'échec de consultation d'enregistrement, si possible, sinon null</returns>
            public static RowResult Create(int recordsetIndex, string sqlQuery, object[] parameters, string errorMessage)
            {
                return new RowResult(recordsetIndex, sqlQuery, parameters, errorMessage);
            }
        }
    }
}

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
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement, si possible, sinon null</returns>
        public object GetValue(string sqlQuery, params object[] parameters)
        {
            return GetValueOrDefault(null, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public object GetValueOrDefault(object defaultValue, string sqlQuery, params object[] parameters)
        {
            try
            {
                using (var command = PrepareCommand(out QueryType queryType, QueryCategory.Reader, sqlQuery, parameters))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader[0];
                        }
                        else
                        {
                            return defaultValue;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Debug.WriteLine($"\nErreur de récupération de la valeur du premier champ du premier enregistrement pour la requête {sqlQuery} :\n{error.Message}\n");
                return defaultValue;
            }
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ textuel
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée string, si possible, sinon null</returns>
        public string GetString(string sqlQuery, params object[] parameters)
        {
            return GetStringOrDefault(null, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ textuel
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type string, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée string, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public string GetStringOrDefault(string defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return ((value is string) || (value == null)) ? (string)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type sbyte
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée sbyte, si possible, sinon null</returns>
        public sbyte GetSByte(string sqlQuery, params object[] parameters)
        {
            return GetSByteOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type sbyte
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type sbyte, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée sbyte, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public sbyte GetSByteOrDefault(sbyte defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is sbyte) ? (sbyte)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type byte
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée byte, si possible, sinon null</returns>
        public byte GetByte(string sqlQuery, params object[] parameters)
        {
            return GetByteOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type byte
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type byte, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée byte, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public byte GetByteOrDefault(byte defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is byte) ? (byte)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type short
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée short, si possible, sinon null</returns>
        public short GetShort(string sqlQuery, params object[] parameters)
        {
            return GetShortOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type short
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type short, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée short, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public short GetShortOrDefault(short defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is short) ? (short)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type ushort
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée ushort, si possible, sinon null</returns>
        public ushort GetUShort(string sqlQuery, params object[] parameters)
        {
            return GetUShortOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type ushort
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type ushort, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée ushort, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public ushort GetUShortOrDefault(ushort defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is ushort) ? (ushort)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type int
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée int, si possible, sinon null</returns>
        public int GetInt(string sqlQuery, params object[] parameters)
        {
            return GetIntOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type int
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type int, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée int, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public int GetIntOrDefault(int defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is int) ? (int)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type uint
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée uint, si possible, sinon null</returns>
        public uint GetUInt(string sqlQuery, params object[] parameters)
        {
            return GetUIntOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type uint
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type uint, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée uint, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public uint GetUIntOrDefault(uint defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is uint) ? (uint)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type long
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée long, si possible, sinon null</returns>
        public long GetLong(string sqlQuery, params object[] parameters)
        {
            return GetLongOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type long
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type long, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée long, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public long GetLongOrDefault(long defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is long) ? (long)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type ulong
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée ulong, si possible, sinon null</returns>
        public ulong GetULong(string sqlQuery, params object[] parameters)
        {
            return GetULongOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type ulong
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type ulong, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée ulong, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public ulong GetULongOrDefault(ulong defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is ulong) ? (ulong)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type float
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée float, si possible, sinon null</returns>
        public float GetFloat(string sqlQuery, params object[] parameters)
        {
            return GetFloatOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type float
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type float, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée float, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public float GetFloatOrDefault(float defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is float) ? (float)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type double
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée double, si possible, sinon null</returns>
        public double GetDouble(string sqlQuery, params object[] parameters)
        {
            return GetDoubleOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type double
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type double, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée double, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public double GetDoubleOrDefault(double defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is double) ? (double)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type decimal
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée decimal, si possible, sinon null</returns>
        public decimal GetDecimal(string sqlQuery, params object[] parameters)
        {
            return GetDecimalOrDefault(0, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique de type decimal
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type decimal, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée decimal, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public decimal GetDecimalOrDefault(decimal defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is decimal) ? (decimal)value : defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique considéré comme représentatif d'une valeur booléenne
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée bool, si possible, sinon null</returns>
        public bool GetBool(string sqlQuery, params object[] parameters)
        {
            return GetBoolOrDefault(false, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ numérique considéré comme représentatif d'une valeur booléenne
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type bool, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée bool, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public bool GetBoolOrDefault(bool defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            if (value is sbyte) return ((sbyte)value != 0);
            if (value is byte) return ((byte)value != 0);
            if (value is short) return ((short)value != 0);
            if (value is ushort) return ((ushort)value != 0);
            if (value is int) return ((int)value != 0);
            if (value is uint) return ((uint)value != 0);
            if (value is long) return ((long)value != 0);
            if (value is ulong) return ((ulong)value != 0);
            return defaultValue;
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ de type DateTime (DATETIME, DATE ou TIME)
        /// </summary>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée DateTime, si possible, sinon null</returns>
        public DateTime GetDateTime(string sqlQuery, params object[] parameters)
        {
            return GetDateTimeOrDefault(DateTime.MinValue, sqlQuery, parameters);
        }

        /// <summary>
        /// Tente d'exécuter une requête SQL de consultation (SELECT, CALL, SHOW) afin de récupérer la valeur du premier champ du premier enregistrement, pour peu qu'il s'agisse d'un champ de type DateTime (DATETIME, DATE ou TIME)
        /// </summary>
        /// <param name="defaultValue">Valeur par défaut, de type DateTime, à retourner si la requête ne peut pas être exécutée, ou si son exécution ne produit aucun enregistrement</param>
        /// <param name="sqlQuery">Code SQL de la requête de consultation à exécuter</param>
        /// <param name="parameters">Série de paires de nom/valeur de paramètres à appliquer sur cette requête OU série de valeurs de paramètres à appliquer sur cette requête</param>
        /// <returns>Valeur du premier champ du premier enregistrement typée DateTime, si possible, sinon la valeur par défaut qui a été spécifiée</returns>
        public DateTime GetDateTimeOrDefault(DateTime defaultValue, string sqlQuery, params object[] parameters)
        {
            object value = GetValueOrDefault(defaultValue, sqlQuery, parameters);
            return (value is DateTime) ? (DateTime)value : defaultValue;
        }
    }
}

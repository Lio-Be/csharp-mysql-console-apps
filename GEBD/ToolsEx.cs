using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace GEBD
{
    /// <summary>
    /// Méthodes d'extensions proposant des outils complémentaires aux "types de base" de C#
    /// </summary>
    public static class ToolsEx
    {
        /// <summary>
        /// Informations culturelles anglophones
        /// </summary>
        private static readonly CultureInfo c_EnglishCulture = CultureInfo.GetCultureInfo("en-US");

        /// <summary>
        /// Style numérique pour les nombres réels
        /// </summary>
        private const NumberStyles c_RealStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

        #region Méthodes de conversion de chaîne en valeur entière
        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type sbyte)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static sbyte ToSByte(this string text, sbyte defaultValue = 0)
        {
            return (text != null) && sbyte.TryParse(text.Trim(), out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type sbyte)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToSByte(this string text, out sbyte value, sbyte defaultValue = 0)
        {
            if ((text == null) || !sbyte.TryParse(text.Trim(), out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type byte)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static byte ToByte(this string text, byte defaultValue = 0)
        {
            return (text != null) && byte.TryParse(text.Trim(), out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type byte)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToByte(this string text, out byte value, byte defaultValue = 0)
        {
            if ((text == null) || !byte.TryParse(text.Trim(), out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type short)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static short ToShort(this string text, short defaultValue = 0)
        {
            return (text != null) && short.TryParse(text.Trim(), out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type short)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToShort(this string text, out short value, short defaultValue = 0)
        {
            if ((text == null) || !short.TryParse(text.Trim(), out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type ushort)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static ushort ToUShort(this string text, ushort defaultValue = 0)
        {
            return (text != null) && ushort.TryParse(text.Trim(), out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type ushort)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToUShort(this string text, out ushort value, ushort defaultValue = 0)
        {
            if ((text == null) || !ushort.TryParse(text.Trim(), out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type int)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static int ToInt(this string text, int defaultValue = 0)
        {
            return (text != null) && int.TryParse(text.Trim(), out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type int)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToInt(this string text, out int value, int defaultValue = 0)
        {
            if ((text == null) || !int.TryParse(text.Trim(), out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type uint)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static uint ToUInt(this string text, uint defaultValue = 0)
        {
            return (text != null) && uint.TryParse(text.Trim(), out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type uint)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToUInt(this string text, out uint value, uint defaultValue = 0)
        {
            if ((text == null) || !uint.TryParse(text.Trim(), out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type long)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static long ToLong(this string text, long defaultValue = 0)
        {
            return (text != null) && long.TryParse(text.Trim(), out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type long)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToLong(this string text, out long value, long defaultValue = 0)
        {
            if ((text == null) || !long.TryParse(text.Trim(), out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type ulong)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static ulong ToULong(this string text, ulong defaultValue = 0)
        {
            return (text != null) && ulong.TryParse(text.Trim(), out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur entière (type ulong)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToULong(this string text, out ulong value, ulong defaultValue = 0)
        {
            if ((text == null) || !ulong.TryParse(text.Trim(), out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }
        #endregion

        #region Méthodes de conversion de chaîne en valeur réelle
        /// <summary>
        /// Tente de convertir ce texte en une valeur réelle (type float)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static float ToFloat(this string text, float defaultValue = 0)
        {
            return (text != null) && float.TryParse(text.Trim().Replace(',', '.'), c_RealStyle, c_EnglishCulture, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur réelle (type float)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToFloat(this string text, out float value, float defaultValue = 0)
        {
            if ((text == null) || !float.TryParse(text.Trim().Replace(',', '.'), c_RealStyle, c_EnglishCulture, out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur réelle (type double)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static double ToDouble(this string text, double defaultValue = 0)
        {
            return (text != null) && double.TryParse(text.Trim().Replace(',', '.'), c_RealStyle, c_EnglishCulture, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur réelle (type double)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToDouble(this string text, out double value, double defaultValue = 0)
        {
            if ((text == null) || !double.TryParse(text.Trim().Replace(',', '.'), c_RealStyle, c_EnglishCulture, out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur réelle (type decimal)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Valeur convertie, sinon valeur par défaut</returns>
        public static decimal ToDecimal(this string text, decimal defaultValue = 0)
        {
            return (text != null) && decimal.TryParse(text.Trim().Replace(',', '.'), c_RealStyle, c_EnglishCulture, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Tente de convertir ce texte en une valeur réelle (type decimal)
        /// </summary>
        /// <param name="text">Texte à convertir</param>
        /// <param name="value">La valeur résultant de la conversion réussie, sinon la valeur par défaut en cas d'échec de conversion</param>
        /// <param name="defaultValue">Valeur par défaut qui sera retournée si la conversion échoue</param>
        /// <returns>Vrai si la conversion est réussie, sinon faux</returns>
        public static bool ToDecimal(this string text, out decimal value, decimal defaultValue = 0)
        {
            if ((text == null) || !decimal.TryParse(text.Trim().Replace(',', '.'), c_RealStyle, c_EnglishCulture, out value))
            {
                value = defaultValue;
                return false;
            }
            return true;
        }
        #endregion

        #region Méthodes d'extensions de IEnumerable<T>
        /// <summary>
        /// Retourne l'indice de la première occurrence d'élément correspondant à ce qui est recherché
        /// </summary>
        /// <typeparam name="T">Type des éléments et de ce qui est recherché</typeparam>
        /// <param name="items">Éléments pour lesquels faire la recherche</param>
        /// <param name="searchItem">Élément recherché</param>
        /// <returns>Indice de la première occurrence d'élément correspondant à ce qui est recherché et qui a pu être trouvé, sinon -1</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T searchItem)
        {
            if (items == null) return -1;
            int index = 0;
            foreach (var item in items)
            {
                if (((item == null) && (searchItem == null)) || ((item != null) && item.Equals(searchItem)))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Retourne l'indice de la première occurrence d'élément correspondant à ce qui est recherché
        /// </summary>
        /// <typeparam name="T1">Type des éléments</typeparam>
        /// <typeparam name="T2">Type de ce qui est recherché</typeparam>
        /// <param name="items">Éléments pour lesquels faire la recherche</param>
        /// <param name="searchItem">Élément recherché</param>
        /// <returns>Indice de la première occurrence d'élément correspondant à ce qui est recherché et qui a pu être trouvé, sinon -1</returns>
        public static int IndexOfAny<T1, T2>(this IEnumerable<T1> items, T2 searchItem)
        {
            if (items == null) return -1;
            int index = 0;
            foreach (var item in items)
            {
                if (((item == null) && (searchItem == null)) || ((item != null) && item.Equals(searchItem)))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
        #endregion
    }
}

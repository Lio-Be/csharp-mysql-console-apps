using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Debug = System.Diagnostics.Debug;

namespace GEBD.UI
{
    /// <summary>
    /// Contient des fonctionnalités de gestion de l'interface utilisateur en mode console
    /// </summary>
    public static partial class UI
    {
        #region Méthodes simplifiées pour l'encodage d'une chaîne de caractères
        /// <summary>
        /// Permet à l'utilisateur d'encoder une chaîne de caractères
        /// <para>Méthode "brute" (aucune vérification de contenu, et aucun post-traitement)</para>
        /// <para>La chaîne encodée ne peut pas faire plus de 254 caractères !</para>
        /// </summary>
        /// <returns>Chaîne encodée par l'utilisateur</returns>
        public static string InputString()
        {
            string result = null;
            With(InputColor, () =>
            {
                result = Console.ReadLine();
            });
            return result;
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une chaîne de caractère, après l'affichage en début de ligne du caractère d'invite d'encodage
        /// <para>Méthode "brute" (aucune vérification de contenu, et aucun post-traitement)</para>
        /// <para>La chaîne encodée ne peut pas faire plus de 254 caractères !</para>
        /// </summary>
        /// <returns>Chaîne encodée par l'utilisateur</returns>
        public static string InputStringLine()
        {
            string result = null;
            With(InputPromptColor, () =>
            {
                if (Console.CursorLeft > 0) Console.WriteLine();
                Console.Write("> ");
            });
            With(InputColor, () =>
            {
                result = Console.ReadLine();
            });
            return result;
        }
        #endregion

        #region Méthodes d'encodage de valeur entière
        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur entière (type sbyte)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static sbyte InputSByte(string prompt, sbyte minimum = sbyte.MinValue, sbyte maximum = sbyte.MaxValue, params sbyte[] excludedValues)
        {
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (excludedValues == null) excludedValues = new sbyte[0];
            if (excludedValues.Length > 0)
            {
                var excludedValuesInBound = excludedValues.Where(v => (v >= minimum) && (v <= maximum)).Distinct().Count();
                if (excludedValuesInBound == (maximum - minimum + 1))
                {
                    Debug.WriteLine($"\nErreur de paramétrage dans InputSByte(..., {minimum}, {maximum}, {string.Join(", ", excludedValues)}) !\n");
                    return 0;
                }
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToSByte(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur entière !\n");
                }
                else if (value < minimum)
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (value > maximum)
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur entière (type byte)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static byte InputByte(string prompt, byte minimum = byte.MinValue, byte maximum = byte.MaxValue, params byte[] excludedValues)
        {
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (excludedValues == null) excludedValues = new byte[0];
            if (excludedValues.Length > 0)
            {
                var excludedValuesInBound = excludedValues.Where(v => (v >= minimum) && (v <= maximum)).Distinct().Count();
                if (excludedValuesInBound == (maximum - minimum + 1))
                {
                    Debug.WriteLine($"\nErreur de paramétrage dans InputByte(..., {minimum}, {maximum}, {string.Join(", ", excludedValues)}) !\n");
                    return 0;
                }
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToByte(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur entière !\n");
                }
                else if (value < minimum)
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (value > maximum)
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur entière (type short)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static short InputShort(string prompt, short minimum = short.MinValue, short maximum = short.MaxValue, params short[] excludedValues)
        {
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (excludedValues == null) excludedValues = new short[0];
            if (excludedValues.Length > 0)
            {
                var excludedValuesInBound = excludedValues.Where(v => (v >= minimum) && (v <= maximum)).Distinct().Count();
                if (excludedValuesInBound == (maximum - minimum + 1))
                {
                    Debug.WriteLine($"\nErreur de paramétrage dans InputShort(..., {minimum}, {maximum}, {string.Join(", ", excludedValues)}) !\n");
                    return 0;
                }
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToShort(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur entière !\n");
                }
                else if (value < minimum)
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (value > maximum)
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur entière (type ushort)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static ushort InputUShort(string prompt, ushort minimum = ushort.MinValue, ushort maximum = ushort.MaxValue, params ushort[] excludedValues)
        {
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (excludedValues == null) excludedValues = new ushort[0];
            if (excludedValues.Length > 0)
            {
                var excludedValuesInBound = excludedValues.Where(v => (v >= minimum) && (v <= maximum)).Distinct().Count();
                if (excludedValuesInBound == (maximum - minimum + 1))
                {
                    Debug.WriteLine($"\nErreur de paramétrage dans InputUShort(..., {minimum}, {maximum}, {string.Join(", ", excludedValues)}) !\n");
                    return 0;
                }
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToUShort(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur entière !\n");
                }
                else if (value < minimum)
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (value > maximum)
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur entière (type int)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static int InputInt(string prompt, int minimum = int.MinValue, int maximum = int.MaxValue, params int[] excludedValues)
        {
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (excludedValues == null) excludedValues = new int[0];
            if (excludedValues.Length > 0)
            {
                var excludedValuesInBound = excludedValues.Where(v => (v >= minimum) && (v <= maximum)).Distinct().Count();
                if (excludedValuesInBound == (maximum - minimum + 1))
                {
                    Debug.WriteLine($"\nErreur de paramétrage dans InputInt(..., {minimum}, {maximum}, {string.Join(", ", excludedValues)}) !\n");
                    return 0;
                }
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToInt(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur entière !\n");
                }
                else if (value < minimum)
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (value > maximum)
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur entière (type uint)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static uint InputUInt(string prompt, uint minimum = uint.MinValue, uint maximum = uint.MaxValue, params uint[] excludedValues)
        {
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (excludedValues == null) excludedValues = new uint[0];
            if (excludedValues.Length > 0)
            {
                var excludedValuesInBound = excludedValues.Where(v => (v >= minimum) && (v <= maximum)).Distinct().Count();
                if (excludedValuesInBound == (maximum - minimum + 1))
                {
                    Debug.WriteLine($"\nErreur de paramétrage dans InputUInt(..., {minimum}, {maximum}, {string.Join(", ", excludedValues)}) !\n");
                    return 0;
                }
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToUInt(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur entière !\n");
                }
                else if (value < minimum)
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (value > maximum)
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur entière (type long)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static long InputLong(string prompt, long minimum = long.MinValue, long maximum = long.MaxValue, params long[] excludedValues)
        {
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (excludedValues == null) excludedValues = new long[0];
            if (excludedValues.Length > 0)
            {
                var excludedValuesInBound = excludedValues.Where(v => (v >= minimum) && (v <= maximum)).Distinct().Count();
                if (excludedValuesInBound == (maximum - minimum + 1))
                {
                    Debug.WriteLine($"\nErreur de paramétrage dans InputLong(..., {minimum}, {maximum}, {string.Join(", ", excludedValues)}) !\n");
                    return 0;
                }
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToLong(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur entière !\n");
                }
                else if (value < minimum)
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (value > maximum)
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur entière (type ulong)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static ulong InputULong(string prompt, ulong minimum = ulong.MinValue, ulong maximum = ulong.MaxValue, params ulong[] excludedValues)
        {
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (excludedValues == null) excludedValues = new ulong[0];
            if (excludedValues.Length > 0)
            {
                var excludedValuesInBound = excludedValues.Where(v => (v >= minimum) && (v <= maximum)).Distinct().Count();
                if (excludedValuesInBound == ((int)(maximum - minimum) + 1))
                {
                    Debug.WriteLine($"\nErreur de paramétrage dans InputULong(..., {minimum}, {maximum}, {string.Join(", ", excludedValues)}) !\n");
                    return 0;
                }
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToULong(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur entière !\n");
                }
                else if (value < minimum)
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (value > maximum)
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }
        #endregion

        #region Méthodes d'encodage de valeur réelle
        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur réelle (type double)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale comprise pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale comprise pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static double InputDouble(string prompt, double minimum = int.MinValue, double maximum = int.MaxValue, params double[] excludedValues)
        {
            return InputDouble(prompt, minimum, true, maximum, true, excludedValues);
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur réelle (type double)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="minimumAllowed">Indique si la borne minimale est autorisée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="maximumAllowed">Indique si la borne maximale est autorisée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static double InputDouble(string prompt, double minimum, bool minimumAllowed, double maximum = int.MaxValue, bool maximumAllowed = true, params double[] excludedValues)
        {
            if (double.IsNaN(minimum))
            {
                minimum = double.NegativeInfinity;
            }
            if (double.IsNaN(maximum))
            {
                maximum = double.PositiveInfinity;
            }
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (double.IsNegativeInfinity(minimum))
            {
                minimum = double.MinValue;
                minimumAllowed = true;
                if (double.IsNegativeInfinity(maximum))
                {
                    maximum = double.MinValue;
                    maximumAllowed = true;
                }
            }
            if (double.IsPositiveInfinity(maximum))
            {
                maximum = double.MaxValue;
                maximumAllowed = true;
                if (double.IsPositiveInfinity(minimum))
                {
                    minimum = double.MaxValue;
                    minimumAllowed = true;
                }
            }
            if (excludedValues == null) excludedValues = new double[0];
            if ((minimum == maximum) && (!minimumAllowed || !maximumAllowed || excludedValues.Contains(minimum)))
            {
                Debug.WriteLine($"\nErreur de paramétrage dans InputDouble(..., {minimum}, {minimumAllowed}, {maximum}, {maximumAllowed}, {string.Join(", ", excludedValues)}) !\n");
                return 0;
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToDouble(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur réelle !\n");
                }
                else if (minimumAllowed && (value < minimum))
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (!minimumAllowed && (value <= minimum))
                {
                    DisplayError($"La valeur encodée doit être strictement plus grande que {minimum} !\n");
                }
                else if (maximumAllowed && (value > maximum))
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (!maximumAllowed && (value >= maximum))
                {
                    DisplayError($"La valeur encodée doit être strictement plus petite que {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur réelle (type float)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale comprise pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale comprise pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static float InputFloat(string prompt, float minimum = int.MinValue, float maximum = int.MaxValue, params float[] excludedValues)
        {
            return InputFloat(prompt, minimum, true, maximum, true, excludedValues);
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur réelle (type float)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="minimumAllowed">Indique si la borne minimale est autorisée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="maximumAllowed">Indique si la borne maximale est autorisée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static float InputFloat(string prompt, float minimum, bool minimumAllowed, float maximum = int.MaxValue, bool maximumAllowed = true, params float[] excludedValues)
        {
            if (float.IsNaN(minimum))
            {
                minimum = float.NegativeInfinity;
            }
            if (float.IsNaN(maximum))
            {
                maximum = float.PositiveInfinity;
            }
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (float.IsNegativeInfinity(minimum))
            {
                minimum = float.MinValue;
                minimumAllowed = true;
                if (float.IsNegativeInfinity(maximum))
                {
                    maximum = float.MinValue;
                    maximumAllowed = true;
                }
            }
            if (float.IsPositiveInfinity(maximum))
            {
                maximum = float.MaxValue;
                maximumAllowed = true;
                if (float.IsPositiveInfinity(minimum))
                {
                    minimum = float.MaxValue;
                    minimumAllowed = true;
                }
            }
            if (excludedValues == null) excludedValues = new float[0];
            if ((minimum == maximum) && (!minimumAllowed || !maximumAllowed || excludedValues.Contains(minimum)))
            {
                Debug.WriteLine($"\nErreur de paramétrage dans InputFloat(..., {minimum}, {minimumAllowed}, {maximum}, {maximumAllowed}, {string.Join(", ", excludedValues)}) !\n");
                return 0;
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToFloat(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur réelle !\n");
                }
                else if (minimumAllowed && (value < minimum))
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (!minimumAllowed && (value <= minimum))
                {
                    DisplayError($"La valeur encodée doit être strictement plus grande que {minimum} !\n");
                }
                else if (maximumAllowed && (value > maximum))
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (!maximumAllowed && (value >= maximum))
                {
                    DisplayError($"La valeur encodée doit être strictement plus petite que {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur réelle (type decimal)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale comprise pour la valeur encodée</param>
        /// <param name="maximum">Borne maximale comprise pour la valeur encodée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static decimal InputDecimal(string prompt, decimal minimum = int.MinValue, decimal maximum = int.MaxValue, params decimal[] excludedValues)
        {
            return InputDecimal(prompt, minimum, true, maximum, true, excludedValues);
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder une valeur réelle (type decimal)
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minimum">Borne minimale pour la valeur encodée</param>
        /// <param name="minimumAllowed">Indique si la borne minimale est autorisée</param>
        /// <param name="maximum">Borne maximale pour la valeur encodée</param>
        /// <param name="maximumAllowed">Indique si la borne maximale est autorisée</param>
        /// <param name="excludedValues">Valeurs à exclure pour la valeur encodée</param>
        /// <returns>Valeur correctement encodée<para>Ou 0, si les contraintes d'encodage ne permettent plus aucune valeur légitime !</para></returns>
        public static decimal InputDecimal(string prompt, decimal minimum, bool minimumAllowed, decimal maximum = int.MaxValue, bool maximumAllowed = true, params decimal[] excludedValues)
        {
            Tools.EnsureMinMax(ref minimum, ref maximum);
            if (excludedValues == null) excludedValues = new decimal[0];
            if ((minimum == maximum) && (!minimumAllowed || !maximumAllowed || excludedValues.Contains(minimum)))
            {
                Debug.WriteLine($"\nErreur de paramétrage dans InputDecimal(..., {minimum}, {minimumAllowed}, {maximum}, {maximumAllowed}, {string.Join(", ", excludedValues)}) !\n");
                return 0;
            }
            DisplayPrompt(prompt);
            while (true)
            {
                if (!InputStringLine().ToDecimal(out var value))
                {
                    DisplayError("Impossible de convertir cela en une valeur réelle !\n");
                }
                else if (minimumAllowed && (value < minimum))
                {
                    DisplayError($"La valeur encodée doit être plus grande ou égale à {minimum} !\n");
                }
                else if (!minimumAllowed && (value <= minimum))
                {
                    DisplayError($"La valeur encodée doit être strictement plus grande que {minimum} !\n");
                }
                else if (maximumAllowed && (value > maximum))
                {
                    DisplayError($"La valeur encodée doit être plus petite ou égale à {maximum} !\n");
                }
                else if (!maximumAllowed && (value >= maximum))
                {
                    DisplayError($"La valeur encodée doit être strictement plus petite que {maximum} !\n");
                }
                else if (excludedValues.Contains(value))
                {
                    DisplayError($"Exceptionnellement, la valeur encodée ne peut pas être {value} !\n");
                }
                else
                {
                    return value;
                }
            }
        }
        #endregion

        #region Méthodes d'encodage d'un texte
        /// <summary>
        /// Permet à l'utilisateur d'encoder un texte
        /// <para>Le texte encodé devra avoir une longueur comprise entre 0 et 65535</para>
        /// <para>L'encodage se terminera par la suppression des espaces de début et de fin de chaîne</para>
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="regEx">Expression régulière à respecter, optionnelle sinon null</param>
        /// <returns>Texte correctement encodé</returns>
        public static string InputText(string prompt, string regEx = null)
        {
            return InputText(prompt, 0, 65535, true, regEx);
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder un texte
        /// <para>Le texte encodé devra avoir une longueur comprise entre 0 et 65535</para>
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="applyTrim">Indique si l'encodage doit se terminer par la suppression des espaces de début et de fin de chaîne</param>
        /// <param name="regEx">Expression régulière à respecter, optionnelle sinon null</param>
        /// <returns>Texte correctement encodé</returns>
        public static string InputText(string prompt, bool applyTrim, string regEx = null)
        {
            return InputText(prompt, 0, 65535, applyTrim, regEx);
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder un texte
        /// <para>L'encodage se terminera par la suppression des espaces de début et de fin de chaîne</para>
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minLength">Borne minimale pour la valeur encodée</param>
        /// <param name="maxLength">Borne maximale pour la valeur encodée</param>
        /// <param name="regEx">Expression régulière à respecter, optionnelle sinon null</param>
        /// <returns>Texte correctement encodé</returns>
        public static string InputText(string prompt, int minLength, int maxLength, string regEx)
        {
            return InputText(prompt, minLength, maxLength, true, regEx);
        }

        /// <summary>
        /// Permet à l'utilisateur d'encoder un texte
        /// </summary>
        /// <param name="prompt">Message d'invite d'encodage</param>
        /// <param name="minLength">Borne minimale pour la valeur encodée</param>
        /// <param name="maxLength">Borne maximale pour la valeur encodée</param>
        /// <param name="applyTrim">Indique si l'encodage doit se terminer par la suppression des espaces de début et de fin de chaîne</param>
        /// <param name="regEx">Expression régulière à respecter, optionnelle sinon null</param>
        /// <returns>Texte correctement encodé</returns>
        public static string InputText(string prompt, int minLength, int maxLength, bool applyTrim = true, string regEx = null)
        {
            Tools.EnsureMinMax(ref minLength, ref maxLength);
            DisplayPrompt(prompt);
            var inputText = new StringBuilder();
            var trimmedText = new StringBuilder();
            while (true)
            {
                inputText.Clear();
                do
                {
                    var inputString = InputStringLine();
                    if (applyTrim)
                    {
                        if (inputText.Length == 0) inputString = inputString.TrimStart();
                        trimmedText.Clear();
                        trimmedText.Append(inputText);
                        trimmedText.Append(inputString.TrimEnd());
                        inputText.Append(inputString);
                        if (trimmedText.Length >= maxLength) break;
                    }
                    else
                    {
                        inputText.Append(inputString);
                        if (inputText.Length >= maxLength) break;
                    }
                } while (AnswerYes("Désirez-vous compléter ce texte ?"));
                var text = inputText.ToString();
                if (applyTrim) text = text.TrimEnd();
                if (text.Length < minLength)
                {
                    DisplayError($"Le texte encodé doit comporter au moins {minLength} caractère{(minLength >= 2 ? "s" : "")}{(applyTrim ? $" significatif{(minLength >= 2 ? "s" : "")}" : "")} !\n");
                }
                else if (text.Length > maxLength)
                {
                    DisplayError($"Le texte encodé ne peut pas contenir plus de {maxLength} caractère{(maxLength >= 2 ? "s" : "")}{(applyTrim ? $" significatif{(maxLength >= 2 ? "s" : "")}" : "")} !\n");
                }
                else if (!string.IsNullOrEmpty(regEx) && !Regex.IsMatch(text, regEx))
                {
                    DisplayError($"Le texte encodé ne respecte pas les règles de formatage prévu !\n");
                }
                else
                {
                    return text;
                }
            }
        }
        #endregion

        #region Méthodes d'attente d'une touche
        /// <summary>
        /// Attend que l'utilisateur appuie sur la touche spécifiée
        /// <para>Le message d'invite d'appui est : Veuillez appuyer sur la touche {key}</para>
        /// </summary>
        /// <param name="key">Touche attendue</param>
        /// <param name="insertEmptyLine">Indique si on doit insérer une ligne vide avant l'affichage du message d'invite</param>
        public static void WaitKey(ConsoleKey key, bool insertEmptyLine = false)
        {
            WaitKey(key, null, insertEmptyLine);
        }

        /// <summary>
        /// Attend que l'utilisateur appuie sur la touche spécifiée
        /// <para>Le message d'invite d'appui est soit :</para>
        /// <para>* Veuillez appuyer sur la touche {key}</para>
        /// <para>* Veuillez appuyer sur la touche {key} pour {reason}</para>
        /// </summary>
        /// <param name="key">Touche attendue</param>
        /// <param name="reason">Explicatif du pour quoi il faut appuyer sur cette touche, ou null si pas d'explication à donner</param>
        /// <param name="insertEmptyLine">Indique si on doit insérer une ligne vide avant l'affichage du message d'invite</param>
        public static void WaitKey(ConsoleKey key, string reason, bool insertEmptyLine = false)
        {
            With(PromptColor, () =>
            {
                while (Console.KeyAvailable) Console.ReadKey(true);
                if (Console.CursorLeft > 0) Console.WriteLine();
                if (insertEmptyLine) Console.WriteLine();
                Console.Write("Veuillez appuyer sur la touche ");
                With(InputColor, Console.BackgroundColor, () =>
                {
                    Console.Write(key);
                });
                if (!string.IsNullOrWhiteSpace(reason))
                {
                    Console.Write($" pour {reason.TrimStart()}");
                }
                while (Console.ReadKey(true).Key != key) ;
                Console.WriteLine();
            });
        }

        /// <summary>
        /// Retourne vrai si l'utilisateur répond OUI à une question qui lui est posée, et à laquelle il répond par l'appui sur la touche O ou sur la touche N
        /// </summary>
        /// <param name="question">Question posée à l'utilisateur</param>
        /// <returns>Vrai si la réponse est OUI, sinon faux</returns>
        public static bool AnswerYes(string question)
        {
            return Answer(question) == Answer_.Yes;
        }

        /// <summary>
        /// Retourne vrai si l'utilisateur répond NON à une question qui lui est posée, et à laquelle il répond par l'appui sur la touche O ou sur la touche N
        /// </summary>
        /// <param name="question">Question posée à l'utilisateur</param>
        /// <returns>Vrai si la réponse est NON, sinon faux</returns>
        public static bool AnswerNo(string question)
        {
            return Answer(question) == Answer_.No;
        }

        /// <summary>
        /// Réponse de type Oui/Non
        /// </summary>
        public enum Answer_
        {
            /// <summary>
            /// Réponse NON
            /// </summary>
            No,
            /// <summary>
            /// Réponse OUI
            /// </summary>
            Yes
        }

        /// <summary>
        /// Pose une question à l'utilisateur, à laquelle il doit répondre par [O]ui ou par [N]on (par l'appui sur la touche O ou sur la touche N)
        /// </summary>
        /// <param name="question">Question posée à l'utilisateur</param>
        /// <returns>Réponse de type Oui/Non</returns>
        public static Answer_ Answer(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                Debug.WriteLine($"\nErreur de paramétrage dans Answer(...)) : aucune question posée !\n");
                return Answer_.No;
            }
            question = question.TrimEnd(' ', '\r', '\n', '\t');
            while (Console.KeyAvailable) Console.ReadKey(true);
            DisplayPrompt(question);
            var posX = Console.CursorLeft;
            if ((posX + 10) >= Console.BufferWidth) Console.WriteLine();
            Display($" £{Console.BackgroundColor}/{PositiveColor}£O£{PositiveColor}£ui£{PromptColor}£/£{Console.BackgroundColor} / {NegativeColor}£N£{NegativeColor}£on£{PromptColor}");
            posX = Console.CursorLeft - 10;
            Answer_ answer;
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.O)
                {
                    answer = Answer_.Yes;
                    break;
                }
                else if (key == ConsoleKey.N)
                {
                    answer = Answer_.No;
                    break;
                }
            }
            Console.CursorLeft = posX;
            if (answer == Answer_.Yes)
            {
                Display($" £{PositiveColor}£Oui      \n");
            }
            else
            {
                Display($" £{NegativeColor}£Non      \n");
            }
            return answer;
        }
        #endregion
    }
}

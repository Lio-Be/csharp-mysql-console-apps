using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GEBD;

namespace GEBD.UI
{
    /// <summary>
    /// Contient des fonctionnalités de gestion de l'interface utilisateur en mode console
    /// </summary>
    public static partial class UI
    {
        #region Couleurs par défaut
        /// <summary>
        /// Couleur par défaut de fond
        /// </summary>
        public const ConsoleColor DefaultBackColor = ConsoleColor.Black;

        /// <summary>
        /// Couleur par défaut d'invite
        /// </summary>
        public const ConsoleColor DefaultPromptColor = ConsoleColor.White;

        /// <summary>
        /// Couleur par défaut d'erreur
        /// </summary>
        public const ConsoleColor DefaultErrorColor = ConsoleColor.Red;

        /// <summary>
        /// Couleur par défaut pour ce qui est important
        /// </summary>
        public const ConsoleColor DefaultStrongColor = ConsoleColor.Yellow;

        /// <summary>
        /// Couleur par défaut du caractère d'invite d'encodage
        /// </summary>
        public const ConsoleColor DefaultInputPromptColor = ConsoleColor.Gray;

        /// <summary>
        /// Couleur par défaut d'encodage
        /// </summary>
        public const ConsoleColor DefaultInputColor = ConsoleColor.Cyan;

        /// <summary>
        /// Couleur par défaut pour quelque chose de "positif"
        /// </summary>
        public const ConsoleColor DefaultPositiveColor = ConsoleColor.Green;

        /// <summary>
        /// Couleur par défaut pour quelque chose de "négatif"
        /// </summary>
        public const ConsoleColor DefaultNegativeColor = ConsoleColor.Red;
        #endregion

        #region Couleurs paramétrables
        /// <summary>
        /// Couleur de fond
        /// </summary>
        public static ConsoleColor BackColor { get; set; } = DefaultBackColor;

        /// <summary>
        /// Couleur d'invite
        /// </summary>
        public static ConsoleColor PromptColor { get; set; } = DefaultPromptColor;

        /// <summary>
        /// Couleur d'erreur
        /// </summary>
        public static ConsoleColor ErrorColor { get; set; } = DefaultErrorColor;

        /// <summary>
        /// Couleur pour ce qui est important
        /// </summary>
        public static ConsoleColor StrongColor { get; set; } = DefaultStrongColor;

        /// <summary>
        /// Couleur du caractère d'invite d'encodage
        /// </summary>
        public static ConsoleColor InputPromptColor { get; set; } = DefaultInputPromptColor;

        /// <summary>
        /// Couleur d'encodage
        /// </summary>
        public static ConsoleColor InputColor { get; set; } = DefaultInputColor;

        /// <summary>
        /// Couleur de quelque chose de "positif"
        /// </summary>
        public static ConsoleColor PositiveColor { get; set; } = DefaultPositiveColor;

        /// <summary>
        /// Couleur de quelque chose de "négatif"
        /// </summary>
        public static ConsoleColor NegativeColor { get; set; } = DefaultNegativeColor;
        #endregion

        #region Méthodes d'effactement de la console
        /// <summary>
        /// Permet d'effacer l'écran
        /// </summary>
        public static void Clear()
        {
            Clear(BackColor);
        }

        /// <summary>
        /// Permet d'effacer l'écran avec la couleur spécifiée
        /// </summary>
        /// <param name="backColor">Couleur de fond à appliquer pour cet effacement</param>
        public static void Clear(ConsoleColor backColor)
        {
            Clear(backColor, backColor);
        }

        /// <summary>
        /// Permet d'effacer l'écran avec les couleurs spécifiées
        /// </summary>
        /// <param name="backColor">Couleur de fond à appliquer pour cet effacement</param>
        /// <param name="textColor">Couleur de texte à appliquer lors de cet effacement</param>
        public static void Clear(ConsoleColor backColor, ConsoleColor textColor)
        {
            Console.BackgroundColor = backColor;
            Console.ForegroundColor = textColor;
            Console.Clear();
        }
        #endregion

        #region Méthodes avec mécanisme de callback pour pouvoir afficher avec changement temporaire de couleur(s)
        /// <summary>
        /// Permet d'exécuter un code avec un changement "temporaire" de couleur de texte
        /// </summary>
        /// <param name="textColor">Couleur de texte à appliquer le temps d'exécuter le code spécifié</param>
        /// <param name="action">Méthode contenant le code à exécuter</param>
        public static void With(ConsoleColor textColor, ActionToDo action)
        {
            With(Console.BackgroundColor, textColor, action);
        }

        /// <summary>
        /// Permet d'exécuter un code avec un changement "temporaire" des couleurs de fond et de texte
        /// </summary>
        /// <param name="backColor">Couleur de fond à appliquer le temps d'exécuter le code spécifié</param>
        /// <param name="textColor">Couleur de texte à appliquer le temps d'exécuter le code spécifié</param>
        /// <param name="action">Méthode contenant le code à exécuter</param>
        public static void With(ConsoleColor backColor, ConsoleColor textColor, ActionToDo action)
        {
            if (action == null) return;
            var defaultBackColor = Console.BackgroundColor;
            var defaultTextColor = Console.ForegroundColor;
            Console.BackgroundColor = backColor;
            Console.ForegroundColor = textColor;
            action();
            Console.BackgroundColor = defaultBackColor;
            Console.ForegroundColor = defaultTextColor;
        }
        #endregion

        #region Méthodes d'affichage avec colorisation
        /// <summary>
        /// Affiche un message d'invite
        /// <para>Il n'y aucun passage à la ligne, pas plus que de suppression d'espaces superflus</para>
        /// </summary>
        /// <param name="text">Message d'invite</param>
        public static void DisplayPrompt(string text)
        {
            With(PromptColor, () =>
            {
                Console.Write(text);
            });
        }

        /// <summary>
        /// Affiche un message d'erreur
        /// <para>Il n'y aucun passage à la ligne, pas plus que de suppression d'espaces superflus</para>
        /// </summary>
        /// <param name="text">Message d'erreur</param>
        public static void DisplayError(string text)
        {
            With(ErrorColor, () =>
            {
                Console.Write(text);
            });
        }

        /// <summary>
        /// Affiche un texte important
        /// <para>Il n'y aucun passage à la ligne, pas plus que de suppression d'espaces superflus</para>
        /// </summary>
        /// <param name="text">Texte important</param>
        public static void DisplayStrong(string text)
        {
            With(StrongColor, () =>
            {
                Console.Write(text);
            });
        }

        /// <summary>
        /// Affiche un texte pouvant contenir des parties d'informations définissant couleurs de texte et éventuellement de fond
        /// <para>Le texte est considéré comme constitué d'une alternance de partie textuelle et de partie de colorisation ; on considère par définition que la première est partie est de type textuelle</para>
        /// <para>Chaque partie est séparée de la suivante par un caractère séparateur (partSeparator) qui, par défaut, est le caractère £</para>
        /// <para>Une partie de colorisation doit contenir un nom de couleur de texte, et peut être éventuellement suivi du caractère / et d'un nom de couleur de fond</para>
        /// </summary>
        /// <param name="colorizedText">Texte avec informations de colorisation</param>
        /// <param name="partSeparator">Caractère séparateur de partie</param>
        public static void Display(string colorizedText, char partSeparator = '£')
        {
            if (string.IsNullOrEmpty(colorizedText)) return;
            With(Console.BackgroundColor, Console.ForegroundColor, () =>
            {
                var backColor = Console.BackgroundColor;
                var isColorPart = false;
                var parts = colorizedText.Split(partSeparator);
                for (int iPart = 0; iPart < parts.Length; iPart++, isColorPart = !isColorPart)
                {
                    if (isColorPart)
                    {
                        var colors = parts[iPart].Split('/');
                        for (int iColor = 0, nColor = Math.Min(colors.Length, 2); iColor < nColor; iColor++)
                        {
                            if (Enum.TryParse<ConsoleColor>(colors[iColor], true, out var color))
                            {
                                if (iColor == 0)
                                {
                                    Console.BackgroundColor = backColor;
                                    Console.ForegroundColor = color;
                                }
                                else
                                {
                                    Console.BackgroundColor = color;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.Write(parts[iPart]);
                    }
                }
            });
        }
        #endregion
    }
}

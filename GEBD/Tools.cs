using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace GEBD
{
    /// <summary>
    /// Méthodes diverses
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Permet de permuter les deux éléments spécifiés
        /// </summary>
        /// <typeparam name="T">Type des éléments à permuter</typeparam>
        /// <param name="item1">Un des éléments à permuter</param>
        /// <param name="item2">L'autre élément à permuter</param>
        public static void Swap<T>(ref T item1, ref T item2)
        {
            var temp = item1;
            item1 = item2;
            item2 = temp;
        }

        /// <summary>
        /// Garantit que le minimum spécifié sera au final bien inférieur ou égal au maximum spécifié
        /// </summary>
        /// <typeparam name="T">Type des éléments traités</typeparam>
        /// <param name="minimum">Élément devant être le minimum des deux</param>
        /// <param name="maximum">Élément devant être le maximum des deux</param>
        public static void EnsureMinMax<T>(ref T minimum, ref T maximum)
            where T : IComparable<T>
        {
            if (minimum == null) return;
            if (minimum.CompareTo(maximum) > 0)
            {
                var temp = minimum;
                minimum = maximum;
                maximum = temp;
            }
        }
    }
}

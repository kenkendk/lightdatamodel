using System;
using System.Collections.Generic;
using System.Text;

namespace LimeTime.Search
{
    /// <summary>
    /// This class represents a simplified score based search.
    /// The idea is similar to a fitness function in neural networks,
    /// but there is no training involved.
    /// </summary>
    public static class IntelligentSearch
    {
        /// <summary>
        /// The best score (excluding the perfect score)
        /// </summary>
        public const int MAX_VALUE = 1000;
        /// <summary>
        /// The perfect score for an exact match
        /// </summary>
        public const int PERFECT_VALUE = 1001;
        /// <summary>
        /// The worst possible score
        /// </summary>
        public const int MIN_VALUE = -1000;
        /// <summary>
        /// The maximum offset to award points
        /// </summary>
        public const int SEARCH_DISTANCE = 100;
        /// <summary>
        /// The number of points awarded for a single hit
        /// </summary>
        public const int SEARCH_HIT_VALUE = 1;
        /// <summary>
        /// The number of points awarded for a single miss
        /// </summary>
        public const int SEARCH_MISS_VALUE = -1;

        /// <summary>
        /// Evaluates a value against the optimal
        /// </summary>
        /// <param name="optimal">The optimal value</param>
        /// <param name="value">The value to evaluate</param>
        /// <returns>Returns a score describing how well the value matches the optimal</returns>
        public static int Evaluate(string optimal, string value)
        {
            if (string.Compare(optimal, value, StringComparison.CurrentCultureIgnoreCase) == 0)
                return PERFECT_VALUE;

            if (string.IsNullOrEmpty(value))
                return MIN_VALUE;

            int score = 0;

            optimal = optimal.Trim();
            value = value.Trim();

            //For each character in the string, find matches in optimal
            for (int i = 0; i < value.Length; i++)
                if (optimal.IndexOf(value[i].ToString(), StringComparison.CurrentCultureIgnoreCase) >= 0)
                    score += SEARCH_HIT_VALUE;
                else
                    score += SEARCH_MISS_VALUE;

            int longestSubString = 0;
            int firstIndex = SEARCH_DISTANCE;
            for (int i = 1; i <= value.Length; i++)
                if (optimal.IndexOf(value.Substring(0, i), StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    longestSubString = i;
                    firstIndex = optimal.IndexOf(value.Substring(0, i), StringComparison.CurrentCultureIgnoreCase);
                }

            score += (SEARCH_DISTANCE - firstIndex) * longestSubString * SEARCH_HIT_VALUE;
            return Math.Max(MIN_VALUE, Math.Min(MAX_VALUE, score));
        }
    }

    /// <summary>
    /// This class can be used to sort a list of strings, in relevance to an optimal
    /// </summary>
    public class IntelligentSorter : System.Collections.IComparer
    {
        private string m_optimal;

        public IntelligentSorter(string optimal)
        {
            m_optimal = optimal;
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (x == null || y == null)
                return 0;

            if (x is string && y is string)
            {
                int a = IntelligentSearch.Evaluate(m_optimal, x as string);
                int b = IntelligentSearch.Evaluate(m_optimal, y as string);
                if (a == b)
                    return 0;
                else
                    return a > b ? 1 : -1;
            }
            else 
                throw new Exception("Bad class types, must be strings");
        }

        #endregion
    }
}

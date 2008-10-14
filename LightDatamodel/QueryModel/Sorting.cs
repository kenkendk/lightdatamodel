using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System.Data.LightDatamodel.QueryModel
{
    /// <summary>
    /// Represents a sortable column and its sorting direction
    /// </summary>
    public class SortableParameter : OperationOrParameter
    {
        private OperationOrParameter m_column;
        private bool m_ascending;

        public SortableParameter(OperationOrParameter column, bool ascending)
        {
            m_column = column;
            m_ascending = ascending;
        }

        public OperationOrParameter Column { get { return m_column; } }
        public bool Ascending { get { return m_ascending; } }
        public override bool IsOperation { get { return m_column.IsOperation; } }
        public override object Evaluate(object item, object[] parameters)
        {
            return m_column.Evaluate(item, parameters);
        }
    }

    /// <summary>
    /// Represents a sortable extension to an operation
    /// </summary>
    public class SortableOperation : Operation
    {
        private SortableParameter[] m_sortparameters;
        public SortableOperation(SortableParameter[] sortparameters, Operators @operator, params OperationOrParameter[] parameters)
            : base(@operator, parameters)
        {
            m_sortparameters = sortparameters;
        }

        public SortableParameter[] SortParameters { get { return m_sortparameters; } }

        /// <summary>
        /// Evaluates a list of objects against the query
        /// </summary>
        /// <param name="items">The items to filter</param>
        /// <returns>A filtered list with only the matching items</returns>
        public override object[] EvaluateList(IEnumerable items, params object[] parameters)
        {
            return Sorter.Sort<object>(this, base.EvaluateList(items, parameters)).ToArray();
        }

        /// <summary>
        /// Evaluates a list of objects against the query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override T[] EvaluateList<T>(IEnumerable items, params object[] parameters)
        {
            return Sorter.Sort<T>(this, base.EvaluateList<T>(items, parameters)).ToArray();
        }

    }

    /// <summary>
    /// Class that holds code to sort a list of arbitrary objects
    /// </summary>
    public class Sorter
    {
        public static System.Collections.Generic.List<T> Sort<T>(SortableOperation opr, System.Collections.Generic.ICollection<T> items)
        {
            System.Collections.Generic.List<T> res;
            if (items as System.Collections.Generic.List<T> != null)
                res = items as System.Collections.Generic.List<T>;
            else
                res = new System.Collections.Generic.List<T>(items);

            res.Sort(new ListComparer<T>(opr.SortParameters));
            return res;
        }

        public static ArrayList Sort(SortableOperation opr, ICollection items)
        {
            ArrayList res;
            if (items as ArrayList != null)
                res = items as ArrayList;
            else
                res = new ArrayList(items);

            res.Sort(new ListComparer(opr.SortParameters));
            return res;
        }

        private class ListComparer<T> : System.Collections.Generic.IComparer<T>
        {
            SortableParameter[] m_sortorder;
            public ListComparer(SortableParameter[] sortorder)
            {
                m_sortorder = sortorder;
            }

            #region IComparer<T> Members

            public int Compare(T x, T y)
            {
                foreach (SortableParameter sp in m_sortorder)
                {
                    object v1 = sp.Evaluate(x, null);
                    object v2 = sp.Evaluate(y, null);

                    int res = Comparer.CompareTo(v1, v2);
                    if (res != 0)
                        return res * (sp.Ascending ? 1 : -1);
                }

                return 0;
            }

            #endregion
        }

        private class ListComparer : IComparer
        {
            SortableParameter[] m_sortorder;
            public ListComparer(SortableParameter[] sortorder)
            {
                m_sortorder = sortorder;
            }

            #region IComparer Members

            public int Compare(object x, object y)
            {
                foreach (SortableParameter sp in m_sortorder)
                {
                    object v1 = sp.Evaluate(x, null);
                    object v2 = sp.Evaluate(y, null);

                    int res = Comparer.CompareTo(v1, v2);
                    if (res != 0)
                        return res * (sp.Ascending ? 1 : -1);
                }

                return 0;
            }

            #endregion

        }

    }
}

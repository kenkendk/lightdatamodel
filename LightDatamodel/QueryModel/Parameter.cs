using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.LightDatamodel.QueryModel
{
    /// <summary>
    /// Represents a parameter in a query
    /// </summary>
    public class Parameter : OperationOrParameter
    {
        private bool m_isColumn;
        private object m_value;
        private int m_boundIndex = -1;
        public override bool IsOperation { get { return false; } }

        /// <summary>
        /// Constructs a new ubound parameter that may be bound at construction time
        /// </summary>
        /// <param name="values">The (optional) values to bind with at constrution time</param>
        /// <param name="bindIndex">The index into the parameter list</param>
        public Parameter(object[] values, int bindIndex)
        {
            m_isColumn = false;

            if (values == null || bindIndex >= values.Length)
                m_boundIndex = bindIndex - (values == null ? 0 : values.Length);
            else
                m_value = values[bindIndex];
        }

        /// <summary>
        /// Constructs a new parameter, that is either a constant value or a column
        /// </summary>
        /// <param name="value">The value or column name</param>
        /// <param name="isColumn">True if the parameter is a column name</param>
        public Parameter(object value, bool isColumn)
        {
            if (isColumn)
                if (value == null)
                    throw new Exception("Column name not specified");
                else if (value.GetType() != typeof(string))
                    throw new Exception("Column name must be a string");
                else if (value.ToString().Trim().Length <= 0)
                    throw new Exception("Column name not specified");

            m_isColumn = isColumn;
            m_value = value;
        }

        /// <summary>
        /// Returns true if the parameter represents a column
        /// </summary>
        public virtual bool IsColumn { get { return m_isColumn; } }
        /// <summary>
        /// Returns the value of the parameter. The value is the column name if the property is a column. Call evaluate to get the column value.
        /// </summary>
        public virtual object Value { get { return m_value; } }

        /// <summary>
        /// Returns the current value of this parameter
        /// </summary>
        /// <param name="item">The object being evaluated</param>
        /// <param name="parameters">A list of unbound parameters</param>
        /// <returns>The value of the parameter, given the object and parameters</returns>
        public override object Evaluate(object item, object[] parameters)
        {
            if (m_boundIndex >= 0)
                if (parameters == null || m_boundIndex >= parameters.Length)
                    throw new Exception("Failed to evaluate expression, because there were not enough parameters given");
                else
                    return parameters[m_boundIndex];

            if (!m_isColumn)
                return m_value;

            if (item == null)
                return null;

            object retval = item;
            string[] parts = ((string)m_value).Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                System.Reflection.PropertyInfo pi = retval.GetType().GetProperty(parts[i]);
                if (pi == null)
                    pi = retval.GetType().GetProperty(parts[i], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.FlattenHierarchy);
                if (pi == null)
                {
                    System.Reflection.MethodInfo mi = retval.GetType().GetMethod(parts[i]);
                    if (mi == null)
                        mi = retval.GetType().GetMethod(parts[i], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.FlattenHierarchy);
                    if (mi == null)
                        throw new Exception("Invalid parameter: " + parts[i] + " no such public property or method found\nWas looking for method with path '" + (string)m_value + "' on type: " + retval.GetType().FullName);

                    retval = mi.Invoke(retval, null);
                }
                else
                    retval = pi.GetValue(retval, null);

                if (retval == null)
                    return null;
            }

            return retval;
        }
    }
}

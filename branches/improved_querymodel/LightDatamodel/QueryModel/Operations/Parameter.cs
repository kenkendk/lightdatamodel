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
        private OperationOrParameter m_bindContext;
        private OperationOrParameter[] m_functionArgs;
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
            m_functionArgs = null;
            m_bindContext = null;

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
            m_bindContext = null;
            m_functionArgs = null;
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
        /// Constructs a new parameter, that is a function call
        /// </summary>
        /// <param name="value">The name of the function to call</param>
        public Parameter(object value, OperationOrParameter[] parameters)
            : this(value, true)
        {
            m_functionArgs = parameters == null ? m_functionArgs = new OperationOrParameter[0] : parameters;
        }

        /// <summary>
        /// Returns true if the parameter is a funktion call
        /// </summary>
        public virtual bool IsFunction { get { return m_functionArgs != null; } }

        /// <summary>
        /// Returns true if the parameter represents a column
        /// </summary>
        public virtual bool IsColumn { get { return m_isColumn; } }
        /// <summary>
        /// Returns the value of the parameter. The value is the column name if the property is a column. Call evaluate to get the column value.
        /// </summary>
        public virtual object Value { get { return m_value; } }

        /// <summary>
        /// Gets or sets the operation used as the basis for the call, null means the queried object
        /// </summary>
        public virtual OperationOrParameter BindContext
        {
            get { return m_bindContext; }
            set { m_bindContext = value; }
        }


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

            object retval;

            string v = m_value as String;

            if (m_bindContext != null)
            {
                retval = m_bindContext.Evaluate(item, parameters);
                if (v.StartsWith("."))
                    v = v.Substring(1);
            }
            else
            {
                if (v.StartsWith("::"))
                {
                    v = v.Substring(2);
                    string[] ns = v.Split('.');

                    Type bestMatch = null;
                    foreach (System.Reflection.Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                        foreach (System.Type t in asm.GetExportedTypes())
                            if (v.StartsWith(t.FullName) && (bestMatch == null || t.FullName.Length > bestMatch.FullName.Length))
                                bestMatch = t;

                    if (bestMatch == null)
                        throw new Exception("Unable to find static match for " + v);
                    else
                        retval = bestMatch;
                    v = v.Substring(bestMatch.FullName.Length + 1); // 1 == '.'.Length
                    //TODO: retval is the type, but should be the object
                    //We can't get the object because it is static
                }
                else
                    retval = item;
            }

            string[] parts = v.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                System.Reflection.PropertyInfo pi = retval.GetType().GetProperty(parts[i]);
                if (pi == null)
                    pi = retval.GetType().GetProperty(parts[i], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.FlattenHierarchy);
                if (pi == null && i == parts.Length - 1)
                {
                    System.Reflection.MemberInfo[] mis = retval.GetType().GetMethods();
                    System.Reflection.MethodInfo mi = null;
                    foreach(System.Reflection.MethodInfo mix in mis)
                        if (mix.Name == parts[i] && mix.GetParameters().Length == m_functionArgs.Length)
                        {
                            mi = mix;
                            break;
                        }

                    if (mi == null)
                        throw new Exception("Failed to find method named " + parts[i] + " which takes " + m_functionArgs.Length.ToString() + " arguments, on type " + retval.GetType().FullName);


                    if (mi == null)
                        mi = retval.GetType().GetMethod(parts[i], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.FlattenHierarchy);
                    if (mi == null)
                        throw new Exception("Invalid parameter: " + parts[i] + " no such public property or method found\nWas looking for method with path '" + (string)m_value + "' on type: " + retval.GetType().FullName);

                    retval = mi.Invoke(retval, UnwrapFunctionArguments(item, parameters));
                }
                else if (pi == null)
                {
                    throw new Exception("Failed to find property named " + parts[i] + " on type " + retval.GetType().FullName);
                }
                else
                {
                    if (i == parts.Length - 1 && m_functionArgs != null)
                        throw new Exception("Tried to execute property as function with arguments");
                    retval = pi.GetValue(retval, null);
                }

                if (retval == null)
                    return null;
            }

            return retval;
        }

        private object[] UnwrapFunctionArguments(object item, object[] parameters)
        {
            if (m_functionArgs == null || m_functionArgs.Length == 0)
                return null;

            object[] args = new object[m_functionArgs.Length];
            for (int i = 0; i < args.Length; i++)
                args[i] = m_functionArgs[i].Evaluate(item, parameters);
            return args;
        }
    }
}

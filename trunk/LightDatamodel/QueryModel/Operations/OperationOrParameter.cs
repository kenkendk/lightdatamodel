#region Disclaimer / License
// Copyright (C) 2008, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System.Data.LightDatamodel.QueryModel
{
    /// <summary>
    /// This is a base representation for the two items in the query model
    /// </summary>
    public abstract class OperationOrParameter
    {
        /// <summary>
        /// Gets a value indicating the column is an operation
        /// </summary>
        public abstract bool IsOperation { get; }

        /// <summary>
        /// Evaluates a parameter or operation
        /// </summary>
        /// <param name="item">The item to evaluate</param>
        /// <param name="parameters">The (optional) unbound parameters</param>
        /// <returns>The resulting value</returns>
        public abstract object Evaluate(object item, object[] parameters);

        /// <summary>
        /// Returns a string representation of the parsed query.
        /// </summary>
        /// <returns>A string representation of the parsed query</returns>
        public override string ToString()
        {
            return this.ToString(true);
        }

        /// <summary>
        /// Returns a string representation of the parsed query.
        /// </summary>
        /// <param name="allowNonprimitives">True to allow existence of non-simple types like lists, false otherwise</param>
        /// <returns>A string representation of the parsed query</returns>
        public virtual string ToString(bool allowNonprimitives)
        {
            StringBuilder sb = new StringBuilder();
            if (AsStringInternal(this, allowNonprimitives, sb))
                return sb.ToString();
            else
                return null;
        }

        /// <summary>
        /// Returns a string representation of the element, or null if the item contains invalid items
        /// </summary>
        /// <param name="allowNonprimitives">True if the result may contain non primitive types</param>
        /// <returns>The string representation</returns>
        protected virtual bool AsStringInternal(OperationOrParameter opm, bool allowNonprimitives, StringBuilder sb)
        {
            if (opm.IsOperation)
            {
                QueryModel.Operation operation = opm as QueryModel.Operation;
                switch (operation.Operator)
                {
                    case Operators.NOP:
                        return true;
                    case QueryModel.Operators.Not:
                        sb.Append("Not ");
                        if (!WrapIfNeeded(operation.Parameters[0], allowNonprimitives, sb))
                            return false;
                        break;
                    case QueryModel.Operators.IIF:
                        sb.Append("IIF(");
                        if (!WrapIfNeeded(operation.Parameters[0], allowNonprimitives, sb))
                            return false;
                        sb.Append(",");

                        if (!WrapIfNeeded(operation.Parameters[1], allowNonprimitives, sb))
                            return false;
                        sb.Append(",");

                        if (!WrapIfNeeded(operation.Parameters[2], allowNonprimitives, sb))
                            return false;

                        sb.Append(")");
                        break;
                    case QueryModel.Operators.In:
                        if (operation.Parameters.Length == 2 && operation.Parameters[1] as QueryModel.Parameter != null && ((QueryModel.Parameter)operation.Parameters[1]).Value as IEnumerable != null && ((QueryModel.Parameter)operation.Parameters[1]).Value as string == null)
                        {
                            if (!WrapIfNeeded(operation.Parameters[0], allowNonprimitives, sb))
                                return false;
                            sb.Append(" In (");
                            int i = 0;
                            foreach (object o in (IEnumerable)(((QueryModel.Parameter)operation.Parameters[1]).Value))
                            {
                                i++;
                                if (!AddParameter(o, allowNonprimitives, sb))
                                    return false;
                                sb.Append(",");
                            }

                            if (i == 0)
                                AddParameter(null, allowNonprimitives, sb);
                            else
                                sb.Length--;	//remove last ,
                        }
                        else
                        {
                            for (int i = 1; i < operation.Parameters.Length; i++)
                                if (!WrapIfNeeded(operation.Parameters[i], allowNonprimitives, sb))
                                    return false;
                        }
                        sb.Append(")");
                        break;
                    case System.Data.LightDatamodel.QueryModel.Operators.Between:
                        {
                            if (!WrapIfNeeded(operation.Parameters[0], allowNonprimitives, sb))
                                return false;
                            sb.Append(" ");
                            sb.Append(TranslateOperator(Operators.Between));
                            sb.Append(" ");
                            if (!WrapIfNeeded(operation.Parameters[1], allowNonprimitives, sb))
                                return false;
                            sb.Append(" AND ");
                            if (!WrapIfNeeded(operation.Parameters[2], allowNonprimitives, sb))
                                return false;
                        }
                        break;
                    default:
                        if (!WrapIfNeeded(operation.Parameters[0], allowNonprimitives, sb))
                            return false;
                        sb.Append(" ");
                        sb.Append(TranslateOperator(operation.Operator));
                        sb.Append(" ");
                        if (!WrapIfNeeded(operation.Parameters[1], allowNonprimitives, sb))
                            return false;

                        break;
                }
            }
            else
            {
                QueryModel.Parameter parameter = opm as QueryModel.Parameter;
                if (parameter.IsColumn)
                    QuoteColumnName((string)parameter.Value, sb);
                else
                    return AddParameter(parameter.Value, allowNonprimitives, sb);
            }

            return true;
        }

        /// <summary>
        /// Puts quotes around a column name. Should be overriden by a provider.
        /// </summary>
        /// <param name="columnname">The column name to quote</param>
        /// <param name="sb">The stringbuilder to append the quoted column name to</param>
        protected virtual void QuoteColumnName(string columnname, StringBuilder sb)
        {
            sb.Append("[");
            sb.Append(columnname);
            sb.Append("]");
        }

        /// <summary>
        /// Converts a parameter to a string representation.
        /// </summary>
        /// <param name="o">The parameter to convert</param>
        /// <param name="allowNonprimitives">True if non-primitives, like lists, are allowed</param>
        /// <param name="sb">The stringbuilder to append the convert value to</param>
        /// <returns>True if the conversion succeded, false otherwise</returns>
        protected virtual bool AddParameter(object o, bool allowNonprimitives, StringBuilder sb)
        {
            if (o == null)
                sb.Append("NULL");
            else if (o.GetType().IsPrimitive)
                sb.Append(o.ToString());
            else if (o as string != null)
                sb.Append("'" + (string)o + "'");
            else if (o.GetType() == typeof(DateTime))
                sb.Append("'" + ((DateTime)o).ToString("s") + "'");
            else if (!allowNonprimitives)
                return false;
            else
                sb.Append(o.GetType() + ":" + o.GetHashCode().ToString());

            return true;
        }



        /// <summary>
        /// Private helper to avoid too many nested ( )
        /// </summary>
        /// <param name="opm">The operand to translate</param>
        /// <param name="cmds">The list of parameters</param>
        /// <returns></returns>
        protected virtual bool WrapIfNeeded(QueryModel.OperationOrParameter opm, bool allowNonprimitives, StringBuilder sb)
        {
            if (opm.IsOperation)
            {
                sb.Append("(");
                if (!AsStringInternal(opm, allowNonprimitives, sb))
                    return false;
                sb.Append(")");
                return true;
            }
            else
                return AsStringInternal(opm, allowNonprimitives, sb);
        }

        /// <summary>
        /// Translates the operation enum into a string representation. 
        /// This function should be overriden by a provider.
        /// </summary>
        /// <param name="opr">The operator to translate</param>
        /// <returns>The translated value</returns>
        protected virtual string TranslateOperator(QueryModel.Operators opr)
        {
            switch (opr)
            {
                case QueryModel.Operators.And:
                    return "AND";
                case QueryModel.Operators.Equal:
                    return "=";
                case QueryModel.Operators.GreaterThan:
                    return ">";
                case QueryModel.Operators.GreaterThanOrEqual:
                    return ">=";
                case QueryModel.Operators.IIF:
                    return "IIF";
                case QueryModel.Operators.In:
                    return "IN";
                case QueryModel.Operators.LessThan:
                    return "<";
                case QueryModel.Operators.LessThanOrEqual:
                    return "<=";
                case QueryModel.Operators.Like:
                    return "LIKE";
                case QueryModel.Operators.Not:
                    return "NOT";
                case QueryModel.Operators.NotEqual:
                    return "<>";
                case QueryModel.Operators.Or:
                    return "OR";
                case QueryModel.Operators.Xor:
                    return "XOR";
                case QueryModel.Operators.Between:
                    return "BETWEEN";
                case Operators.Is:
                    return "IS";
                default:
                    throw new Exception("Bad operator: " + opr.ToString());

            }
        }

		/// <summary>
		/// Evaluates a list of objects against the query
		/// </summary>
		/// <param name="items">The items to filter</param>
		/// <returns>A filtered list with only the matching items</returns>
		public virtual ArrayList EvaluateList(IEnumerable items, params object[] parameters)
		{
			ArrayList lst = new ArrayList();
			foreach (object o in items)
				if (ResAsBool(this.Evaluate(o, parameters)))
					lst.Add(o);
			return lst;
		}

		/// <summary>
		/// Evaluates a list of objects against the query
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public virtual List<T> EvaluateList<T>(IEnumerable items, params object[] parameters)
		{
			System.Collections.Generic.List<T> lst = new System.Collections.Generic.List<T>();
			foreach (object o in items)
				if (ResAsBool(this.Evaluate(o, parameters)))
					lst.Add((T)o);
			return lst;
		}


		/// <summary>
		/// Returns a boolean value for any object
		/// </summary>
		/// <param name="item">The item to convert to a boolean</param>
		/// <returns>The most appropriate boolean return value</returns>
		public static bool ResAsBool(object item)
		{
			if (item == null)
				return false;
			else if (item.GetType() == typeof(bool))
				return (bool)item;
			else
				return false;
		}
    }
}

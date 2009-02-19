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
    /// Represents a single operation
    /// </summary>
    public class Operation : OperationOrParameter
    {
        protected OperationOrParameter[] m_parameters;
        protected Operators m_operator;
        public override bool IsOperation { get { return true; } }

        /// <summary>
        /// Constructs a new operation
        /// </summary>
        /// <param name="operator">The operation to represent</param>
        /// <param name="parameters">The parameters the operation handles</param>
        public Operation(Operators @operator, params OperationOrParameter[] parameters)
        {
            if (parameters == null)
                parameters = new OperationOrParameter[] { };

            switch (@operator)
            {
                case Operators.NOP:
                    if (parameters.Length != 0)
                        throw new Exception("The NOP operator must not have any parameters");
                    break;
                case Operators.Not:
                    if (parameters.Length != 1)
                        throw new Exception("The NOT operator must have exactly one parameter");
                    break;
                case Operators.Between:
                    if (parameters.Length != 2 || parameters[1] as Operation == null || (parameters[1] as Operation).Operator != Operators.And)
                        throw new Exception("The BETWEEN operator must have two parameters, and the second must be the AND operator");
                    break;
                case Operators.IIF:
                    if (parameters.Length != 3)
                        throw new Exception("The IIF operator must have exactly three parameters");
                    break;
                case Operators.In:
                    if (parameters.Length < 2)
                        throw new Exception("The In operator must have two or more parameters");
                    break;
                default:
                    if (parameters.Length != 2)
                        throw new Exception("The " + @operator.ToString() + " operator must have exactly two parameters");
                    break;
            }

            m_operator = @operator;
            m_parameters = parameters;
        }

        /// <summary>
        /// Implements lazy cached evaluations for operations, a simple performance speedup 
        /// that avoids evaluating deep trees when evaluation is unnecesary
        /// </summary>
        private struct LazyEvaluator
        {
            private object m_result;
            private bool m_evaluated;
            private object[] m_parameters;
            public OperationOrParameter Op;
            public object Item;

            /// <summary>
            /// Constructs a lazy evaluator for the given operation or parameter
            /// </summary>
            /// <param name="op">The operation or parameter to lazy evaluate</param>
            /// <param name="item">The object the evaluation is bound to</param>
            public LazyEvaluator(OperationOrParameter op, object item, object[] parameters)
            {
                m_result = null;
                m_evaluated = false;
                m_parameters = parameters;
                Op = op;
                Item = item;
            }

            /// <summary>
            /// Gets the result of the evaluation
            /// </summary>
            public object Result
            {
                get
                {
                    if (!m_evaluated)
                    {
                        m_result = Op.Evaluate(Item, m_parameters);
                        m_evaluated = true;
                    }
                    return m_result;
                }
            }
        }

        public OperationOrParameter[] Parameters { get { return m_parameters; } }
        public Operators Operator { get { return m_operator; } }

        /// <summary>
        /// Evaluates an object with the current query
        /// </summary>
        /// <param name="item">The item to evaluate</param>
        /// <param name="parameters">The (optional) parameters to evaluate with</param>
        /// <returns>True if the object satisfies the query, false otherwise</returns>
        public override object Evaluate(object item, object[] parameters)
        {
            LazyEvaluator[] res = new LazyEvaluator[m_parameters.Length];

            for (int i = 0; i < res.Length; i++)
                res[i] = new LazyEvaluator(m_parameters[i], item, parameters);

            switch (m_operator)
            {
                case Operators.NOP:
                    return true;
                case Operators.Not:
                    return !ResAsBool(res[0].Result);
                case Operators.Equal:
                    return Comparer.CompareTo(res[0].Result, res[1].Result) == 0;
                case Operators.Is:
                    if (res[1].Result == null)
                    {
                        return res[0].Result == null || (res[0].Result.GetType() == typeof(DateTime) && (DateTime)res[0].Result == new DateTime(1, 1, 1)) || (res[0].Result.GetType() == typeof(int) && (int)res[0].Result == int.MinValue) || (res[0].Result.GetType() == typeof(float) && (float)res[0].Result == float.MinValue) || (res[0].Result.GetType() == typeof(double) && (double)res[0].Result == double.MinValue);
                    }
                    else
                        return res[0].Result == res[1].Result;
                case Operators.NotEqual:
                    return Comparer.CompareTo(res[0].Result, res[1].Result) != 0;
                case Operators.GreaterThan:
                    return Comparer.CompareTo(res[0].Result, res[1].Result) > 0;
                case Operators.LessThan:
                    return Comparer.CompareTo(res[0].Result, res[1].Result) < 0;
                case Operators.LessThanOrEqual:
                    return Comparer.CompareTo(res[0].Result, res[1].Result) <= 0;
                case Operators.GreaterThanOrEqual:
                    return Comparer.CompareTo(res[0].Result, res[1].Result) >= 0;
                case Operators.Like:
                    if (res[0].Result == null && res[1].Result == null)
                        return true;
                    else if (res[0].Result == null || res[1].Result == null)
                        return false;
                    else
                        return res[0].Result.ToString().ToLower().Trim().Equals(res[1].Result.ToString().ToLower());
                case Operators.Or:
                    return ResAsBool(res[0].Result) || ResAsBool(res[1].Result);
                case Operators.And:
                    return ResAsBool(res[0].Result) && ResAsBool(res[1].Result);
                case Operators.Xor:
                    return ResAsBool(res[0].Result) ^ ResAsBool(res[1].Result);
                case Operators.IIF:
                    return ResAsBool(res[0].Result) ? res[1].Result : res[2].Result;
                case Operators.Between:
                    {
                        Operation andOp = m_parameters[1] as Operation;
                        if (andOp == null || andOp.Operator != Operators.And || andOp.Parameters == null || andOp.Parameters.Length != 2)
                            throw new Exception("Bad parameter for the between operator!");

                        object min = andOp.Parameters[0].Evaluate(item, parameters);
                        object max = andOp.Parameters[1].Evaluate(item, parameters);

                        //Swap if needed
                        if (Comparer.CompareTo(min, max) < 0)
                            return Comparer.CompareTo(res[0].Result, min) >= 0 && Comparer.CompareTo(res[0].Result, max) <= 0;
                        else
                            return Comparer.CompareTo(res[0].Result, max) >= 0 && Comparer.CompareTo(res[0].Result, min) <= 0;
                    }
                case Operators.In:
                    for (int i = 1; i < res.Length; i++)
                        if (res[i].Result as ICollection != null)
                        {
                            foreach (object ox in res[i].Result as ICollection)
                                if (Comparer.CompareTo(res[0].Result, ox) == 0)
                                    return true;
                        }
                        else if (Comparer.CompareTo(res[0].Result, res[i].Result) == 0)
                            return true;
                    return false;
                default:
                    throw new Exception("Bad operator: " + m_operator.ToString());

            }
        }


  
    }
}

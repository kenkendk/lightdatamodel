using System;
using System.Collections.Generic;
using System.Text;
using System.Data.LightDatamodel.QueryModel;

namespace System.Data.LightDatamodel
{
    /// <summary>
    /// This class is an interface to the query model
    /// </summary>
    public static class Query
    {
        /// <summary>
        /// Parses an SQL like string into an object model
        /// </summary>
        /// <param name="query">The string to parse</param>
        /// <param name="values">Any values to insert into the query</param>
        /// <returns>A parsed object, representing the query</returns>
        public static Operation Parse(string query, params object[] values)
        {
            return Parser.ParseQuery(query, values);
        }

        /// <summary>
        /// Creates a literary bound value item.
        /// </summary>
        /// <param name="value">The value to use</param>
        /// <returns>A literary value</returns>
        public static Parameter Value(object value)
        {
            return new Parameter(value, false);
        }

        /// <summary>
        /// Creates a list of values.
        /// </summary>
        /// <param name="values">The values to form a list for</param>
        /// <returns>A list of values</returns>
        public static OperationOrParameter[] ListValue(params object[] values)
        {
            if (values == null)
                return new OperationOrParameter[0];
            else
            {
                OperationOrParameter[] p = new OperationOrParameter[values.Length];
                for (int i = 0; i < p.Length; i++)
                    p[i] = Value(values[i]);
                return p;
            }
        }

        /// <summary>
        /// Creates a property value
        /// </summary>
        /// <param name="name">The property name</param>
        /// <returns>A property parameter</returns>
        public static Parameter Property(string name)
        {
            return new Parameter(name, true);
        }

        /// <summary>
        /// Returns an operation that is an equality test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>An equality operation</returns>
        public static Operation Equal(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.Equal, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is a not equal test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>A not equal operation</returns>
        public static Operation NotEqual(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.NotEqual, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is a less than test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>A less than operation</returns>
        public static Operation LessThan(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.LessThan, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is a greater than test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>A greater than operation</returns>
        public static Operation GreaterThan(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.GreaterThan, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is a less than or equal test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>A less than or equal operation</returns>
        public static Operation LessThanOrEqual(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.LessThan, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is a greater than or equal test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>A greater than or equal operation</returns>
        public static Operation GreaterThanOrEqual(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.GreaterThan, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is a like test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>A like operation</returns>
        public static Operation Like(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.Like, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is an and test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>An and operation</returns>
        public static Operation And(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.Like, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is an or test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>An or operation</returns>
        public static Operation Or(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.Like, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is a xor test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>A xor operation</returns>
        public static Operation Xor(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.Like, op1, op2);
        }

        /// <summary>
        /// Returns an operation that is a negation.
        /// </summary>
        /// <param name="op">The value or operation to negate</param>
        /// <returns>A negation operation</returns>
        public static Operation Not(OperationOrParameter op)
        {
            return new Operation(Operators.Not, op);
        }

        /// <summary>
        /// Returns an operation that is an inline if operation.
        /// </summary>
        /// <param name="testvalue">The boolean test value or operation</param>
        /// <param name="truepart">The action to perform if the test value is true</param>
        /// <param name="falsepart">The action to perform if the test value is false</param>
        /// <returns>An inline if operation</returns>
        public static Operation IIF(OperationOrParameter testvalue, OperationOrParameter truepart, OperationOrParameter falsepart)
        {
            return new Operation(Operators.IIF, testvalue, truepart, falsepart);
        }

        /// <summary>
        /// Returns an operation that is an in test.
        /// </summary>
        /// <param name="testvalue">The value or operation to evaluate against the list</param>
        /// <param name="list">Values to test against, can contain multiple values or a list or both</param>
        /// <returns>An in operation</returns>
        public static Operation In(OperationOrParameter testvalue, params OperationOrParameter[] list)
        {
            List<OperationOrParameter> parameters = new List<OperationOrParameter>();
            parameters.Add(testvalue);
            if (list != null)
                parameters.AddRange(list);

            return new Operation(Operators.In, parameters.ToArray());
        }

        /// <summary>
        /// Returns an operation that is a between operation.
        /// </summary>
        /// <param name="testvalue">The test value or operation</param>
        /// <param name="lowerbound">The lower inclusive bound</param>
        /// <param name="upperbound">The upper exclusive bound</param>
        /// <returns>A between operation</returns>
        public static Operation Between(OperationOrParameter testvalue, OperationOrParameter lowerbound, OperationOrParameter upperbound)
        {
            return new Operation(Operators.Between, testvalue, lowerbound, upperbound);
        }

        /// <summary>
        /// Returns an operation that is a object reference equality test.
        /// </summary>
        /// <param name="op1">The left hand side of the comparision</param>
        /// <param name="op2">The right hand side of the comparision</param>
        /// <returns>A object reference equality operation</returns>
        public static Operation Is(OperationOrParameter op1, OperationOrParameter op2)
        {
            return new Operation(Operators.Is, op1, op2);
        }

        /// <summary>
        /// Returns a no-operation operation.
        /// </summary>
        /// <returns>A no-operation operation</returns>
        public static Operation NOP()
        {
            return new Operation(Operators.NOP);
        }
    }
}
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
    /// Class that parses SQL like statements, and builds a object tree with the operators and parameters.
    /// </summary>
    public class Parser
    {
        private static System.Globalization.CultureInfo CI = System.Globalization.CultureInfo.InvariantCulture;

        /// <summary>
        /// All characters that are considered whitespace
        /// </summary>
        private static Dictionary<string, string> WhiteSpace = null;
        /// <summary>
        /// All strings that represent an operator
        /// </summary>
        private static Dictionary<string, Operators> OperatorList = null;
        /// <summary>
        /// All strings that must be present in pairs
        /// </summary>
        private static Dictionary<string, string> Pairwise = null;
        /// <summary>
        /// Maps all operators to a priority index
        /// </summary>
        private static Dictionary<Operators, int> OperatorPrecedence = null;
        /// <summary>
        /// Special values that seperate two operands
        /// </summary>
        private static Dictionary<string, string> OperatorSeperators = null;

        /// <summary>
        /// Initialize the global filter list
        /// </summary>
        private static object InitializeFilters()
        {
            WhiteSpace = new Dictionary<string, string>();
            OperatorList = new Dictionary<string, Operators>();
            Pairwise = new Dictionary<string, string>();
            OperatorPrecedence = new Dictionary<Operators, int>();
            OperatorSeperators = new Dictionary<string, string>();

            WhiteSpace.Add(" ", null);
            WhiteSpace.Add(",", null);

            OperatorSeperators.Add("=", null);
            OperatorSeperators.Add("<", null);
            OperatorSeperators.Add(">", null);
            OperatorSeperators.Add("<=", null);
            OperatorSeperators.Add(">=", null);
            OperatorSeperators.Add("<>", null);
            OperatorSeperators.Add("!=", null);
            OperatorSeperators.Add("!", null);

            OperatorList.Add("=", Operators.Equal);
            OperatorList.Add("<", Operators.LessThan);
            OperatorList.Add(">", Operators.GreaterThan);
            OperatorList.Add("<=", Operators.LessThanOrEqual);
            OperatorList.Add(">=", Operators.GreaterThanOrEqual);
            OperatorList.Add("<>", Operators.NotEqual);
            OperatorList.Add("!=", Operators.NotEqual);
            OperatorList.Add("AND", Operators.And);
            OperatorList.Add("OR", Operators.Or);
            OperatorList.Add("XOR", Operators.Xor);
            OperatorList.Add("IN", Operators.In);
            OperatorList.Add("LIKE", Operators.Like);
            OperatorList.Add("IIF", Operators.IIF);
            OperatorList.Add("NOT", Operators.Not);
            OperatorList.Add("!", Operators.Not);
            OperatorList.Add("BETWEEN", Operators.Between);
            OperatorList.Add("IS", Operators.Is);

            Pairwise.Add("[", "]");
            Pairwise.Add("(", ")");
            Pairwise.Add("{", "}");
            Pairwise.Add("'", "'");
            Pairwise.Add("\"", "\"");

            OperatorPrecedence.Add(Operators.IIF, 1);

            OperatorPrecedence.Add(Operators.Equal, 4);
            OperatorPrecedence.Add(Operators.LessThan, 4);
            OperatorPrecedence.Add(Operators.GreaterThan, 4);
            OperatorPrecedence.Add(Operators.LessThanOrEqual, 4);
            OperatorPrecedence.Add(Operators.GreaterThanOrEqual, 4);
            OperatorPrecedence.Add(Operators.NotEqual, 4);
            OperatorPrecedence.Add(Operators.Is, 5);
            OperatorPrecedence.Add(Operators.Like, 5);

            OperatorPrecedence.Add(Operators.Xor, 5);
            OperatorPrecedence.Add(Operators.Not, 6);
            OperatorPrecedence.Add(Operators.And, 7);
            OperatorPrecedence.Add(Operators.Or, 8);
            OperatorPrecedence.Add(Operators.In, 8);
            OperatorPrecedence.Add(Operators.Between, 8);

            return null;
        }

        /// <summary>
        /// Static constructor for initializing common entries.
        /// </summary>
        static Parser()
        {
            InitializeFilters();
        }

        /// <summary>
        /// Parses an SQL string into an Operation statement
        /// </summary>
        /// <param name="query">The SQL Query to parse</param>
        /// <returns>The equvalent query structure</returns>
        public static Operation ParseQuery(string query, params object[] values)
        {
            if (query == null || query.Trim().Length == 0)
                return new Operation(Operators.NOP);

            int bindIndex = 0;

            OperationOrParameter[] op = InternalParseQuery(query, values, ref bindIndex);
            if (op.Length != 1)
                throw new Exception("Failed to parse the query into a meaningfull representation");
            else if (!op[0].IsOperation)
                throw new Exception("Query does not produce a valid statement");

            return (Operation)op[0];
        }

        /// <summary>
        /// Parses an SQL string into an OperationOrParameter statement
        /// </summary>
        /// <param name="query">The SQL Query to parse</param>
        /// <returns>The equvalent query structure</returns>
        private static OperationOrParameter[] InternalParseQuery(string query, object[] parameters, ref int bindIndex)
        {
            //Stage 1, tokenize
            Queue<string> tokens = new Queue<string>();
            string curpair = null;
            bool isEscaped = false;
            int paircount = 0;

            int tokenstart = 0;

            for (int i = 0; i < query.Length; i++)
            {
                if (isEscaped)
                    isEscaped = false;
                else
                {
                    if (query[i] == '\\')
                        isEscaped = true;
                    else
                    {
                        if (curpair != null)
                        {
                            //Do we have a matching group character?
                            if (((string)Pairwise[curpair])[0] == query[i])
                            {
                                paircount--;
                                if (paircount == 0)
                                {
                                    curpair = null;
                                    tokens.Enqueue(query.Substring(tokenstart, (i - tokenstart) + 1).Trim());
                                    tokenstart = i + 1;
                                }
                            }
                            else if (curpair[0] == query[i])
                                paircount++;
                        }
                        else if (Pairwise.ContainsKey(query[i].ToString()))
                        {
                            //Do not add the seperator as a token, but rather save it for addition when the match occurs
                            string f = query.Substring(tokenstart, i - tokenstart).Trim();
                            if (f.Length > 0)
                                tokens.Enqueue(f);

                            tokenstart = i;
                            curpair = query[i].ToString();
                            paircount = 1;
                        }
                        else
                        {
                            //Operators (=, <, >, <=, >=, !, <>, !=) are both seperators and tokens
                            if (OperatorList.ContainsKey(query[i].ToString()))
                            {
                                int operatorLength = HasExtraSeperators(query, i) ? 2 : 1;
                                i += operatorLength - 1;

                                string f = query.Substring(tokenstart, (i - tokenstart) + 1).Trim();
                                string right = f.Substring(0, f.Length - operatorLength).Trim();
                                if (right.Length > 0)
                                    tokens.Enqueue(right);
                                tokens.Enqueue(f.Substring(f.Length - operatorLength).Trim());

                                tokenstart = i + 1;
                            }
                            else if (WhiteSpace.ContainsKey(query[i].ToString()))
                            {
                                string f = query.Substring(tokenstart, i - tokenstart).Trim();
                                if (f.Length > 0)
                                    tokens.Enqueue(f);
                                tokenstart = i + 1;
                            }
                        }
                    }
                }
            }

            string remainder = query.Substring(tokenstart).Trim();
            if (remainder.Length > 0)
                tokens.Enqueue(remainder);

            if (isEscaped)
                throw new Exception("Statement cannot end with \"\\\"");

            if (paircount != 0)
                throw new Exception("Failed to find closing match for " + curpair);


            //Covert all string tokens to operands
            ArrayList parsed = new ArrayList();
            List<SortableParameter> sortorders = null;

            while (tokens.Count > 0)
            {
                string opr = tokens.Dequeue();
                if (OperatorList.ContainsKey(opr.ToUpper()))
                {
                    Operators op = OperatorList[opr.ToUpper()];
                    parsed.Add(op);
                }
                else if (opr.ToUpper().Equals("ORDER"))
                {
                    if (!((string)tokens.Dequeue()).ToUpper().Equals("BY"))
                        throw new Exception("The keyword ORDER must be followed by the keyword BY");

                    sortorders = new List<SortableParameter>();
                    //Rest of the items are sort expressions
                    while (tokens.Count > 0)
                    {
                        string col = (string)tokens.Dequeue();
                        OperationOrParameter[] p = TokenAsParameter(col, parameters, ref bindIndex);
                        if (p == null || p.Length != 1)
                            throw new Exception("Bad sort operand: " + col);
                        bool asc = true;
                        if (tokens.Count > 0)
                        {
                            string next = (string)tokens.Peek();
                            if (next != null || (next.ToUpper().Equals("ASC") || next.ToUpper().Equals("DESC")))
                            {
                                tokens.Dequeue();
                                if (next.ToUpper().Equals("DESC"))
                                    asc = false;
                            }
                        }

                        sortorders.Add(new SortableParameter(p[0], asc));
                    }
                }
                else
                {
                    OperationOrParameter[] opm = TokenAsParameter(opr, parameters, ref bindIndex);
                    parsed.Add(opm);
                }
            }


            //Combine function calls into a single operand
            for (int i = 0; i < parsed.Count - 1; i++)
                if (
                    parsed[i] as OperationOrParameter[] != null && 
                    (parsed[i] as OperationOrParameter[]).Length == 1 &&
                    (parsed[i] as OperationOrParameter[])[0] as Parameter != null &&
                    ((parsed[i] as OperationOrParameter[])[0] as Parameter).IsColumn && 
                    parsed[i + 1] as OperationOrParameter[] != null)
                {
                    Parameter func = (parsed[i] as OperationOrParameter[])[0] as Parameter;
                    parsed[i] = new Parameter(func.Value, parsed[i + 1] as OperationOrParameter[]);
                    parsed.RemoveAt(i + 1);
                    i--;
                }

            for (int i = 0; i < parsed.Count; i++)
                if (parsed[i] as OperationOrParameter[] != null && (parsed[i] as OperationOrParameter[]).Length == 1)
                    parsed[i] = (parsed[i] as OperationOrParameter[])[0];

            //Check for sequences like "GetType().FullName"
            for (int i = 1; i < parsed.Count; i++)
                if (
                    parsed[i-1] as Parameter != null && 
                    (parsed[i-1] as Parameter).IsFunction &&
                    parsed[i] as Parameter != null &&
                    (parsed[i] as Parameter).IsColumn && 
                    (parsed[i] as Parameter).Value as String != null &&
                    ((parsed[i] as Parameter).Value as String).StartsWith(".")
                )
                {
                    (parsed[i] as Parameter).BindContext = parsed[i-1] as Parameter;
                    parsed[i - 1] = parsed[i];
                    parsed.RemoveAt(i);
                    i--;
                }





            //Sort out operator precedence
            SortedList<int, List<int>> operators = new SortedList<int, List<int>>();
            for(int pix = 0; pix < parsed.Count; pix++)
            {
                if (parsed[pix] is Operators)
                {
                    Operators op = (Operators)parsed[pix];
                    int precedence = 10;
                    if (OperatorPrecedence.ContainsKey(op))
                        precedence = OperatorPrecedence[op];
                    if (!operators.ContainsKey(precedence))
                        operators.Add(precedence, new List<int>());
                    operators[precedence].Add(pix);
                }
            }

            //Build tree, bind the top binding operators first
            foreach (List<int> de in operators.Values)
                //Running the list backwards, ensures that items to the left are closer to the root
                //than items on the right, aka. left-to-right association
                for (int i = de.Count - 1; i >= 0; i--)
                {
                    int pos = de[i];
                    Operators op = (Operators)parsed[pos];
                    OperationOrParameter opm;
                    ArrayList lm;


                    int rm = pos - 1;
                    int rc = 3;

                    switch (op)
                    {
                        case Operators.Not:
                            if (pos >= parsed.Count)
                                throw new Exception("No parameters for the not operator");
                            opm = new Operation(op, (OperationOrParameter)parsed[pos + 1]);
                            //parsed.RemoveRange(pos, 2);
                            //parsed.Insert(pos, opm);
                            rc = 2;
                            rm = pos;
                            break;
                        case Operators.IIF:
                            if (pos + 1 >= parsed.Count)
                                throw new Exception("Not enough parameters for the IIF operator");
                            if (parsed[pos + 1] as OperationOrParameter[] == null)
                                throw new Exception("The IIF operator must have a single list operand");
                            opm = new Operation(op, (OperationOrParameter[])parsed[pos + 1]);
                            rc = 2;
                            rm = pos;
                            break;
                        case Operators.In:
                            if (pos == 0 || pos + 1 >= parsed.Count)
                                throw new Exception("Not enough parameters for the IN operator");
                            if (parsed[pos - 1] as OperationOrParameter == null)
                                throw new Exception("The IN operator must have a single regular operand and a list or regular operand");
                            if (parsed[pos + 1] as OperationOrParameter[] == null && parsed[pos + 1] as OperationOrParameter == null)
                                throw new Exception("The IN operator must have a single regular operand and a list or regular operand");

                            if (parsed[pos + 1] as OperationOrParameter[] != null)
                            {
                                lm = new ArrayList();
                                lm.Add(parsed[pos - 1]);
                                lm.AddRange((OperationOrParameter[])parsed[pos + 1]);
                                opm = new Operation(op, (OperationOrParameter[])lm.ToArray(typeof(OperationOrParameter)));
                            }
                            else
                                opm = new Operation(op, (OperationOrParameter)parsed[pos - 1], (OperationOrParameter)parsed[pos + 1]);

                            rc = 3;
                            break;
                        default:
                            if (pos == 0 || pos + 1 >= parsed.Count)
                                throw new Exception("Must have preceeding and succeding token for the " + op.ToString() + " operator");
                            if (parsed[pos - 1] as OperationOrParameter == null || parsed[pos + 1] as OperationOrParameter == null)
                                throw new Exception("List values can only be used with the IN or IIF operator");
                            opm = new Operation(op, (OperationOrParameter)parsed[pos - 1], (OperationOrParameter)parsed[pos + 1]);
                            break;
                    }

                    parsed.RemoveRange(rm, rc);
                    parsed.Insert(rm, opm);

                    //TODO: This adjustment REALLY sucks
                    foreach (List<int> dex in operators.Values)
                        for (int j = 0; j < dex.Count; j++)
                        {
                            if (dex[j] > rm)
                                dex[j] = dex[j] - (rc - 1);
                        }

                }

            SortableParameter[] sorted = sortorders != null ? sortorders.ToArray() : null;

            if (parsed.Count == 0)
            {
                if (sorted == null)
                    return new OperationOrParameter[] { };
                else
                    return new OperationOrParameter[] { new SortableOperation(sorted, Operators.NOP) };
            }
            else if (parsed.Count == 1 && parsed[0] as Operation != null)
            {
                if (sorted == null)
                    return new OperationOrParameter[] { (OperationOrParameter)parsed[0] };

                Operation op = (Operation)parsed[0];
                return new OperationOrParameter[] { new SortableOperation(sorted, op.Operator, op.Parameters) };
            }
            else
            {
                if (sorted != null)
                    throw new Exception("Found an ORDER BY directive inside a substatement");
                else
                    return (OperationOrParameter[])parsed.ToArray(typeof(OperationOrParameter));
            }
        }

        /// <summary>
        /// Tries to end recursion by detecting if an operand is a parameter.
        /// Otherwise passes the remaining string for further parsing.
        /// This is usefull for dealing with nested operations
        /// </summary>
        /// <param name="query">The query string fragment to evaluate</param>
        /// <param name="parameters">Unbound parameters</param>
        /// <param name="bindIndex">The next unbound parameter to use</param>
        /// <returns>A parsed list representing the query string fragment</returns>
        private static OperationOrParameter[] TokenAsParameter(string query, object[] parameters, ref int bindIndex)
        {
            //TODO: This will not recognize lists, thus IIF and IN does not work
            double v;

            if (query.Trim() == "?")
                return new OperationOrParameter[] { new Parameter(parameters, bindIndex++) };
            else if (query.Trim().ToUpper() == "NULL")
                return new OperationOrParameter[] { new Parameter(null, false) };
            else if (double.TryParse(query.Trim(), System.Globalization.NumberStyles.Integer, CI, out v))
                return new OperationOrParameter[] { new Parameter((long)v, false) };
            else if (double.TryParse(query.Trim(), System.Globalization.NumberStyles.Float, CI, out v))
                return new OperationOrParameter[] { new Parameter(v, false) };
            else if (query.Trim().ToLower().Equals("true") || query.Trim().ToLower().Equals("false"))
                return new OperationOrParameter[] { new Parameter(bool.Parse(query.Trim()), false) };
            else if (query.Trim().StartsWith("\"") || query.Trim().StartsWith("'"))
            {
                string vs = query.Trim();
                vs = vs.Substring(1, vs.Length - 2);
                int ix = vs.IndexOf("\\");
                while (ix >= 0)
                {
                    vs = vs.Substring(0, ix) + vs.Substring(ix, vs.Length - ix - 1);
                    ix = vs.IndexOf("\\", ix);
                }
                return new OperationOrParameter[] { new Parameter(vs, false) };
            }
            else if (Pairwise.ContainsKey(query.Trim().Substring(0, 1)))
            {
                query = query.Trim();
                return InternalParseQuery(query.Substring(1, query.Length - 2), parameters, ref bindIndex);
            }
            else if (!HasSeperators(query))
                return new OperationOrParameter[] { new Parameter(query.Trim(), true) };
            else
                return InternalParseQuery(query, parameters, ref bindIndex);
        }

        /// <summary>
        /// Determines if a query string fragment can be parsed as a single value
        /// </summary>
        /// <param name="query">The query string fragment to evaluate</param>
        /// <returns>True if the fragment contains multiple items, false otherwise</returns>
        private static bool HasSeperators(string query)
        {
            query = query.Trim();
            foreach (string s in WhiteSpace.Keys)
                if (query.IndexOf(s) >= 0)
                    return true;

            foreach (string s in OperatorSeperators.Keys)
                if (query.IndexOf(s) >= 0)
                    return true;

            return false;
        }

        /// <summary>
        /// Tests for existence of string seperators that are also operators
        /// </summary>
        /// <param name="query">The query string fragment to examine</param>
        /// <param name="index">The string index to begin examination at</param>
        /// <returns>True if the query string fragment contains an operator seperator</returns>
        private static bool HasExtraSeperators(string query, int index)
        {
            foreach (string s in OperatorSeperators.Keys)
                if (s.Length > 1 && query.Substring(index, Math.Min(query.Length - index, s.Length)) == s)
                    return true;
            return false;

        }
    }
}

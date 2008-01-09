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
using System.Collections;


//This file implements all aspects of the query model
namespace System.Data.LightDatamodel.QueryModel
{
	/// <summary>
	/// Represents the avalible operators in the query model
	/// </summary>
	public enum Operators
	{
		Equal,
		NotEqual,
		GreaterThan,
		LessThan,
		LessThanOrEqual,
		GreaterThanOrEqual,
		Like,
		And,
		Or,
		Xor,
		Not,
		IIF,
		In
	}

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
	}

	/// <summary>
	/// Represents a single operation
	/// </summary>
	public class Operation : OperationOrParameter
	{
		private OperationOrParameter[] m_parameters;
		private Operators m_operator;
		public override bool IsOperation { get { return true; } }

		/// <summary>
		/// Constructs a new operation
		/// </summary>
		/// <param name="operator">The operation to represent</param>
		/// <param name="parameters">The parameters the operation handles</param>
		public Operation(Operators @operator, params OperationOrParameter[] parameters)
		{
			if (parameters == null)
				parameters = new OperationOrParameter[] {};

			switch(@operator)
			{
				case Operators.Not:
					if (parameters.Length != 1)
						throw new Exception("The NOT operator must have exactly one parameter");
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
						throw new Exception("The " + @operator.ToString()  + " operator must have exactly two parameters");
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

			for(int i = 0; i < res.Length; i++)
				res[i] = new LazyEvaluator(m_parameters[i], item, parameters);

			switch(m_operator)
			{
				case Operators.Not:
					return !ResAsBool(res[0].Result);
				case Operators.Equal:
					return CompareTo(res[0].Result, res[1].Result) == 0;
				case Operators.NotEqual:
					return CompareTo(res[0].Result, res[1].Result) != 0;
				case Operators.GreaterThan:
					return CompareTo(res[0].Result, res[1].Result) > 0;
				case Operators.LessThan:
					return CompareTo(res[0].Result, res[1].Result) < 0;
				case Operators.LessThanOrEqual:
					return CompareTo(res[0].Result, res[1].Result) <= 0;
				case Operators.GreaterThanOrEqual:
					return CompareTo(res[0].Result, res[1].Result) >= 0;
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
				case Operators.In:
					for(int i = 1; i < res.Length; i++)
						if (res[1].Result as ICollection != null)
						{
							foreach(object ox in res[1].Result as ICollection)
								if (CompareTo(res[0].Result, ox) == 0)
									return true;
						}
						else if (CompareTo(res[0].Result, res[1].Result) == 0)
							return true;
					return false;
				default:
					throw new Exception("Bad operator: " + m_operator.ToString());

			}
		}

		/// <summary>
		/// Compares one operand to another. Deals with te various odd conversions that .Net imposes for boxed variables
		/// </summary>
		/// <param name="op1">Operand 1 (usually left hand argument)</param>
		/// <param name="op2">Operand 2 (usually right hand argument)</param>
		/// <returns>0 if the operands are considered equal, negative if the op1 is less than op2 and positive otherwise. May throw an exception if the two operands cannot be compared.</returns>
		private int CompareTo(object op1, object op2)
		{
			if (op1 == null && op2 == null)
				return 0;
			else if (op1 == null)
				return -1;
			else if (op2 == null)
				return 1;
			else if (op1 as IComparable == null || op2 as IComparable == null)
				throw new Exception("Unable to compare: " + op1.GetType() + " with " + op2.GetType());
			else if (op1.GetType().IsPrimitive && op2.GetType().IsPrimitive && op1.GetType() != op2.GetType())
			{
				if (op1.GetType() == typeof(double) || op1.GetType() == typeof(float) || op1.GetType() == typeof(decimal) &&
					(op2.GetType() == typeof(double) || op2.GetType() == typeof(float) || op2.GetType() == typeof(decimal)))
					return CompareTo(Convert.ChangeType(op1, typeof(double)), Convert.ChangeType(op2, typeof(double)));
				else if (op1.GetType() == typeof(long) || op1.GetType() == typeof(int) || op1.GetType() == typeof(byte) || op1.GetType() == typeof(short) || op1.GetType() == typeof(byte) &&
					(op2.GetType() == typeof(long) || op2.GetType() == typeof(int) || op1.GetType() == typeof(byte) || op2.GetType() == typeof(short) || op2.GetType() == typeof(byte)))
					return CompareTo(Convert.ChangeType(op1, typeof(long)), Convert.ChangeType(op2, typeof(long)));
				else if (op1.GetType() == typeof(ulong) || op1.GetType() == typeof(uint) || op1.GetType() == typeof(ushort) &&
					(op2.GetType() == typeof(ulong) || op2.GetType() == typeof(uint) || op2.GetType() == typeof(ushort)))
					return CompareTo(Convert.ChangeType(op1, typeof(ulong)), Convert.ChangeType(op2, typeof(ulong)));
				else 
					throw new Exception("Could not find suitable comparision for type " + op1.GetType().FullName + " and " + op2.GetType().FullName);
			}
			else if (op1.GetType() == typeof(string) || op2.GetType() == typeof(string))
			{
				return op1.ToString().CompareTo(op2.ToString());
			}
			else
			{
				return ((IComparable)op1).CompareTo((IComparable)op2);
			}

		}

		/// <summary>
		/// Evaluates a list of objects against the query
		/// </summary>
		/// <param name="items">The items to filter</param>
		/// <returns>A filtered list with only the matching items</returns>
		public object[] EvaluateList(IEnumerable items, params object[] parameters)
		{
			ArrayList lst = new ArrayList();
			foreach(object o in items)
				if (ResAsBool(this.Evaluate(o, parameters)))
					lst.Add(o);
			return (object[])lst.ToArray(typeof(object));
		}


		/// <summary>
		/// Returns a boolean value for any object
		/// </summary>
		/// <param name="item">The item to convert to a boolean</param>
		/// <returns>The most appropriate boolean return value</returns>
		private bool ResAsBool(object item)
		{
			if (item == null)
				return false;
			else if (item.GetType() == typeof(bool))
				return (bool)item;
			else
				return false;
		}

	}

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
				if(value == null)
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
			for(int i = 0; i < parts.Length; i++)
			{
				System.Reflection.PropertyInfo pi = retval.GetType().GetProperty(parts[i]);
				if (pi == null)
				{
					System.Reflection.MethodInfo mi = retval.GetType().GetMethod(parts[i]);
					if (mi == null)
						throw new Exception("Invalid parameter: " + parts + " (" + (string)m_value + ")" + " on type: " + retval.GetType());
				
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

	/// <summary>
	/// Class that holds all parser related code
	/// </summary>
	public class Parser
	{
		private static System.Globalization.CultureInfo CI = System.Globalization.CultureInfo.InvariantCulture;

		private static Hashtable WhiteSpace = null;
		private static Hashtable OperatorList = null;
		private static Hashtable Pairwise = null;
		private static Hashtable OperatorPrecedence = null;
		private static Hashtable OperatorSeperators = null;

		private static void InitializeFilters()
		{
			WhiteSpace = new Hashtable();
			OperatorList = new Hashtable();
			Pairwise = new Hashtable();
			OperatorPrecedence = new Hashtable();
			OperatorSeperators = new Hashtable();

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

			OperatorPrecedence.Add(Operators.Xor, 5);
			OperatorPrecedence.Add(Operators.Not, 6);
			OperatorPrecedence.Add(Operators.And, 7);
			OperatorPrecedence.Add(Operators.Or, 8);
			OperatorPrecedence.Add(Operators.In, 8);
			OperatorPrecedence.Add(Operators.Like, 8);
		}

		/// <summary>
		/// Parses an SQL string into an Operation statement
		/// </summary>
		/// <param name="query">The SQL Query to parse</param>
		/// <returns>The equvalent query structure</returns>
		public static Operation ParseQuery(string query, params object[] values)
		{
			//TODO: Add a NOP operation
			if (query == null || query.Trim().Length == 0)
				return new Operation(Operators.Not, new Parameter(false, false));

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
			if (WhiteSpace == null)
				InitializeFilters();
			
			//Stage 1, tokenize
			Queue tokens = new Queue();
			string curpair = null;
			bool isEscaped = false;
			int paircount = 0;

			int tokenstart = 0;

			for(int i = 0; i < query.Length; i++)
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
							if (OperatorList.ContainsKey(query[i].ToString()) && (i == query.Length - 1 || !HasExtraSeperators(query , i)))
							{
								int operatorLength = HasExtraSeperators(query, i) ? 2 : 1;

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

			//Stage 2, build tree

			//Sort out operator precedence
			SortedList operators = new SortedList();
			ArrayList parsed = new ArrayList();
			int pix = 0;
			while(tokens.Count > 0)
			{
				string opr = (string)tokens.Dequeue();
				if (OperatorList.ContainsKey(opr.ToUpper()))
				{
					Operators op = (Operators)OperatorList[opr.ToUpper()];
					int precedence = 10;
					if (OperatorPrecedence.ContainsKey(op))
						precedence = (int)OperatorPrecedence[op];
					if (!operators.ContainsKey(precedence))
						operators.Add(precedence, new ArrayList());
					((ArrayList)operators[precedence]).Add(pix);
					parsed.Add(op);
				}
				else
				{
					OperationOrParameter[] opm = TokenAsParameter(opr, parameters, ref bindIndex);
					if (opm.Length == 1)
						parsed.Add(opm[0]);
					else
						parsed.Add(opm);
				}
				pix++;
			}

			//Build tree, bind the top binding operators first
			foreach(DictionaryEntry de in operators)
				//Running the list backwards, ensures that items to the left are closer to the root
				//than items on the right, aka. left-to-right association
				for(int i = ((ArrayList)de.Value).Count - 1; i >= 0; i--)
				{
					int pos = (int)((ArrayList)de.Value)[i];
					Operators op = (Operators)parsed[pos];
					OperationOrParameter opm;
					ArrayList lm;


					int rm = pos - 1;
					int rc = 3; 

					switch(op)
					{
						case Operators.Not:
							if (pos >= parsed.Count)
								throw new Exception("No parameters for the not operator");
							opm = new Operation(op, (OperationOrParameter)parsed[pos + 1]);
							parsed.RemoveRange(pos, 2);
							parsed.Insert(pos, opm);
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
					foreach(DictionaryEntry dex in operators)
						for(int j = 0; j < ((ArrayList)dex.Value).Count; j++)
						{
							if (((int)((ArrayList)dex.Value)[j]) > rm)
								((ArrayList)dex.Value)[j] =  ((int)((ArrayList)dex.Value)[j]) - (rc - 1);
						}

				}

			if (parsed.Count == 0)
				return new OperationOrParameter[] {};
			else if (parsed.Count == 1 && parsed[0] as Operation != null)
				return new OperationOrParameter[] { (OperationOrParameter)parsed[0] };
			else 
				return (OperationOrParameter[])parsed.ToArray(typeof(OperationOrParameter));
		}

		private static OperationOrParameter[] TokenAsParameter(string query, object[] parameters, ref int bindIndex)
		{
			//TODO: This will not recognize lists, thus IIF and IN does not work
			double v;

			if (query.Trim() == "?")
				return new OperationOrParameter[] { new Parameter(parameters, bindIndex++) };
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
				while(ix >= 0)
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

		private static bool HasSeperators(string query)
		{
			query = query.Trim();
			foreach(string s in WhiteSpace.Keys)
				if (query.IndexOf(s) >= 0)
					return true;

			foreach(string s in OperatorSeperators.Keys)
				if (query.IndexOf(s) >= 0)
					return true;

			return false;
		}

		private static bool HasExtraSeperators(string query, int index)
		{
			foreach(string s in OperatorSeperators.Keys)
				if (s.Length > 1 && query.Substring(index, Math.Min(query.Length - index, s.Length)) == s)
					return true;
			return false;

		}
	}
}

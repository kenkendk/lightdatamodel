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
		public abstract bool IsOperation { get; }
		public abstract object Evaluate(object item);
	}

	/// <summary>
	/// Represents a single operation
	/// </summary>
	public class Operation : OperationOrParameter
	{
		private OperationOrParameter[] m_parameters;
		private Operators m_operator;
		public override bool IsOperation { get { return true; } }

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
			public OperationOrParameter Op;
			public object Item;

			public LazyEvaluator(OperationOrParameter op, object item)
			{
				m_result = null;
				m_evaluated = false;
				Op = op;
				Item = item;
			}

			public object Result 
			{
				get 
				{
					if (!m_evaluated)
					{
						m_result = Op.Evaluate(Item);
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
		/// <returns>True if the object satisfies the query, false otherwise</returns>
		public override object Evaluate(object item)
		{
			LazyEvaluator[] res = new LazyEvaluator[m_parameters.Length];

			for(int i = 0; i < res.Length; i++)
				res[i] = new LazyEvaluator(m_parameters[i], item);

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
						if (CompareTo(res[0].Result, res[1].Result) == 0)
							return true;
					return false;
				default:
					throw new Exception("Bad operator: " + m_operator.ToString());

			}
		}

		private int CompareTo(object op1, object op2)
		{
			if (op1 == null && op2 == null)
				return 0;
			else if (op1 == null)
				return -1;
			else if (op2 == null)
				return -2;
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
		public object[] EvaluateList(IEnumerable items)
		{
			ArrayList lst = new ArrayList();
			foreach(object o in items)
				if (ResAsBool(this.Evaluate(o)))
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
		public override bool IsOperation { get { return false; } }

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

		public bool IsColumn { get { return m_isColumn; } }
		public object Value { get { return m_value; } }

		public override object Evaluate(object item)
		{
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
		public static Operation ParseQuery(string query)
		{
			//TODO: Add a NOP operation
			if (query == null || query.Trim().Length == 0)
				return new Operation(Operators.Not, new Parameter(false, false));

			OperationOrParameter[] op = InternalParseQuery(query);
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
		private static OperationOrParameter[] InternalParseQuery(string query)
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

			//Sort out operator precedence, the arraylist makes sure we have right-to-left associativity for all operators
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
						operators.Add(precedence, new ArrayList()); //Adjust with queue or stack, if we want switching between left and right associative
					((ArrayList)operators[precedence]).Add(pix);
					parsed.Add(op);
				}
				else
				{
					OperationOrParameter[] opm = TokenAsParameter(opr);
					if (opm.Length == 1)
						parsed.Add(opm[0]);
					else
						parsed.Add(opm);
				}
				pix++;
			}

			//Build tree, bind the top binding operators first
			foreach(DictionaryEntry de in operators)
				for(int i = 0; i < ((ArrayList)de.Value).Count; i++)
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
							if (parsed[pos - 1] as OperationOrParameter == null || parsed[pos + 1] as OperationOrParameter[] == null)
								throw new Exception("The IN operator must have a single regular operand and a list operand");
							lm = new ArrayList();
							lm.Add(parsed[pos - 1]);
							lm.AddRange((OperationOrParameter[])parsed[pos + 1]);
							opm = new Operation(op, (OperationOrParameter[])lm.ToArray(typeof(OperationOrParameter)));
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

		private static OperationOrParameter[] TokenAsParameter(string query)
		{
			//TODO: This will not recognize lists, thus IIF and IN does not work
			double v;

			if (double.TryParse(query.Trim(), System.Globalization.NumberStyles.Integer, CI, out v))
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
			else if (!HasSeperators(query))
				return new OperationOrParameter[] { new Parameter(query.Trim(), true) };
			else if (Pairwise.ContainsKey(query.Trim().Substring(0, 1)))
			{
				query = query.Trim();
				return InternalParseQuery(query.Substring(1, query.Length - 2));
			}
			else
				return InternalParseQuery(query);
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

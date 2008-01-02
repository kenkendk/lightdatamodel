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

			//TODO: Validate that only booleans are allowed for and, or, not, xor

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
					if (res[0].Result == null && res[1].Result == null)
						return true;
					else if (res[0].Result == null || res[1].Result == null)
						return false;
					else
						return res[0].Result.Equals(res[1].Result);
				case Operators.NotEqual:
					if (res[0].Result == null && res[1].Result == null)
						return false;
					else if (res[0].Result == null || res[1].Result == null)
						return true;
					else
						return !res[0].Result.Equals(res[1].Result);
				case Operators.GreaterThan:
					if (res[0].Result == null || res[1].Result == null)
						return false;
					else if (res[0].Result as IComparable == null || res[1].Result as IComparable == null)
						throw new Exception("Unable to compare: " + res[0].Result.GetType() + " with " + res[1].Result.GetType());
					else 
						return ((IComparable)res[0].Result).CompareTo((IComparable)res[1].Result) > 0;
				case Operators.LessThan:
					if (res[0].Result == null || res[1].Result == null)
						return false;
					else if (res[0].Result as IComparable == null || res[1].Result as IComparable == null)
						throw new Exception("Unable to compare: " + res[0].Result.GetType() + " with " + res[1].Result.GetType());
					else 
						return ((IComparable)res[0].Result).CompareTo((IComparable)res[1].Result) < 0;
				case Operators.LessThanOrEqual:
					if (res[0].Result == null && res[1].Result == null)
						return true;
					else if (res[0].Result == null || res[1].Result == null)
						return false;
					else if (res[0].Result as IComparable == null || res[1].Result as IComparable == null)
						throw new Exception("Unable to compare: " + res[0].Result.GetType() + " with " + res[1].Result.GetType());
					else 
						return ((IComparable)res[0].Result).CompareTo((IComparable)res[1].Result) <= 0;
				case Operators.GreaterThanOrEqual:
					if (res[0].Result == null && res[1].Result == null)
						return true;
					else if (res[0].Result == null || res[1].Result == null)
						return false;
					else if (res[0].Result as IComparable == null || res[1].Result as IComparable == null)
						throw new Exception("Unable to compare: " + res[0].Result.GetType() + " with " + res[1].Result.GetType());
					else 
						return ((IComparable)res[0].Result).CompareTo((IComparable)res[1].Result) >= 0;
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
						if (res[0].Result == null && res[i].Result == null)
							return true;
						else if (res[0].Result != null && res[i].Result != null && (res[0].Result.Equals(res[i].Result)))
							return true;
						
					return false;
				default:
					throw new Exception("Bad operator: " + m_operator.ToString());

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
					throw new Exception("Invalid parameter: " + parts + " (" + (string)m_value + ")" + " on type: " + retval.GetType());
				
                retval = pi.GetValue(retval, null);
				if (retval == null)
					return null;
			}

			return retval;
		}

		private static Hashtable WhiteSpace = null;
		private static Hashtable OperatorList = null;
		private static Hashtable Pairwise = null;

		private static void InitializeFilters()
		{
			WhiteSpace = new Hashtable();
			OperatorList = new Hashtable();
			Pairwise = new Hashtable();

			WhiteSpace.Add(" ", null);
			WhiteSpace.Add(",", null);

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
		}

		public static OperationOrParameter ParseQuery(string query)
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
							if (curpair[0] == query[i])
								paircount++;
							else if (((string)Pairwise[curpair])[0] == query[i])
							{
								paircount--;
								if (paircount == 0)
								{
									curpair = null;
									tokens.Enqueue(query.Substring(tokenstart, i - tokenstart).Trim());
									tokenstart = i + 1;
								}
							}
						}

						if (Pairwise.ContainsKey(query[i].ToString()))
						{
							tokenstart = i + 1;
							curpair = query[i].ToString();
							paircount = 1;
						}
						else
						{
							if (WhiteSpace.ContainsKey(query[i].ToString()))
							{
								string f = query.Substring(tokenstart, i - tokenstart).Trim();
								if (f.Length > 0)
								{
									tokens.Enqueue(f);
									tokenstart = i + 1;
								}
							}
						}
						
					}
				}
			}

			query = query.Substring(tokenstart).Trim();
			if (query.Length > 0)
				tokens.Enqueue(query);

			if (isEscaped)
				throw new Exception("Statement cannot end with \"\\\"");

			if (paircount != 0)
				throw new Exception("Failed to find closing match for " + curpair);

			ArrayList ops = new ArrayList();
			OperationOrParameter prevToken = null;
			//Misses operator precedence...

			while(tokens.Count > 0)
			{
				string opr = (string)tokens.Dequeue();
				if (OperatorList.ContainsKey(opr.ToUpper()))
				{
					Operators op = (Operators)OperatorList[opr.ToUpper()];
					switch(op)
					{
						case Operators.Not:
							if (prevToken != null)
								throw new Exception("Cannot have preceeding token for the Not operator");
							prevToken = new Operation(op, TokenAsParameter(tokens.Dequeue()));
							break;
						case Operators.IIF:
							if (prevToken != null)
								throw new Exception("Cannot have preceeding token for the IIF operator");
							prevToken = new Operation(op, new OperationOrParameter[] { TokenAsParameter(tokens.Dequeue()), TokenAsParameter(tokens.Dequeue()), TokenAsParameter(tokens.Dequeue())} );
							break;
						case Operators.In:
							if (prevToken == null)
								throw new Exception("Must have preceeding token for the In operator");
							prevToken = new Operation(op, prevToken, TokenAsParameter(tokens.Dequeue()));
							break;
						default:
							if (prevToken == null)
								throw new Exception("Must have preceeding token for the " + op.ToString() + " operator");
							prevToken = new Operation(op, prevToken ,TokenAsParameter(tokens.Dequeue()));
							break;
					}
				}
				else if (prevToken == null)
				{
					prevToken = TokenAsParameter(opr);
				}
				else 
					throw new Exception("Parse error");
			}

			return prevToken;
		}

		/// <summary>
		/// Helper to remove annoying typecasting
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		private static OperationOrParameter TokenAsParameter(object query)
		{
			return TokenAsParameter((string)query);
		}

		private static OperationOrParameter TokenAsParameter(string query)
		{
			double v;
			if (double.TryParse(query.Trim(), System.Globalization.NumberStyles.Any, null, out v))
				return new Parameter(v, false);
			else if (query.Trim().IndexOf(" ") < 0 && query.Trim().IndexOf(",") < 0)
				return new Parameter(query.Trim(), true);
			else
				return ParseQuery(query);
		}


	}
}

using System;
using System.Collections.Generic;
using System.Text;

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
        In,
        Between,
        Is,
        NOP
    }
}

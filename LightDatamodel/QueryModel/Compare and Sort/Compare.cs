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
    /// Simple comparer for .Net types
    /// </summary>
    public class Comparer : IComparer
    {
        /// <summary>
        /// Compares one operand to another. Deals with the various odd conversions that .Net imposes for boxed variables
        /// </summary>
        /// <param name="op1">Operand 1 (usually left hand argument)</param>
        /// <param name="op2">Operand 2 (usually right hand argument)</param>
        /// <returns>0 if the operands are considered equal, negative if the op1 is less than op2 and positive otherwise. May throw an exception if the two operands cannot be compared.</returns>
        public static int CompareTo(object op1, object op2)
        {
            if ((op1 == null && op2 == null) || (op1 == DBNull.Value && op2 == DBNull.Value))
                return 0;
            else if (op1 == null || op1 == DBNull.Value)
                return -1;
            else if (op2 == null || op2 == DBNull.Value)
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
                //do case insentinsive
                return string.Compare(op1.ToString(), op2.ToString(), true);
            }
            else
            {
                return ((IComparable)op1).CompareTo((IComparable)op2);
            }
        }

        #region IComparer Members

        /// <summary>
        /// Compares one operand to another. Deals with te various odd conversions that .Net imposes for boxed variables
        /// </summary>
        /// <param name="op1">Operand 1 (usually left hand argument)</param>
        /// <param name="op2">Operand 2 (usually right hand argument)</param>
        /// <returns>0 if the operands are considered equal, negative if the op1 is less than op2 and positive otherwise. May throw an exception if the two operands cannot be compared.</returns>
        public int Compare(object op1, object op2)
        {
            return CompareTo(op1, op2);
        }

        #endregion

    }
}

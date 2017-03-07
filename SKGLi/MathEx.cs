// 
//    Copyright (c) 2011-2015, Artem Los (original author)
//    Copyright (c) 2017, Fahmi Noor Fiqri (SKGLi modified version)
//    All rights reserved.
// 
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
// 
//   * Redistributions of source code must retain the above copyright notice, this
//     list of conditions and the following disclaimer.
//   * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//   * Neither the name of SKGL nor the names of its
//     contributors may be used to endorse or promote products derived from
//     this software without specific prior written permission.
// 
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//   AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//   DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
//   FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//   DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//   CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//   OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//   OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace SKGLi
{
    /// <summary>
    /// Provides 
    /// </summary>
    public static class MathEx
    {
        /// <summary>
        /// Converts array of <see cref="bool"/> to <see cref="int"/>.
        /// </summary>
        /// <param name="boolArray">Input <see cref="bool"/> array.</param>
        /// <returns><see cref="int"/> value representing specified <see cref="bool"/> array.</returns>
        /// <remarks>
        /// In this function we are converting a binary value array to a int
        /// A binary array can max contain 4 values.
        /// </remarks>
        public static int BoolArrayToInt32(bool[] boolArray)
        {
            var aVector = 0;

            for (var i = 0; i < boolArray.Length; i++)
            {
                if (!boolArray[i]) continue;
                aVector += Convert.ToInt32(Math.Pow(2, boolArray.Length - i - 1));
                // times 1 has been removed
            }
            return aVector;
        }

        /// <summary>
        /// Converts array of <see cref="int"/> to <see cref="bool"/>.
        /// </summary>
        /// <param name="num">Input <see cref="int"/>.</param>
        /// <returns><see cref="bool"/> array representing specified <see cref="int"/>.</returns>
        /// <remarks>
        /// In this function we are converting an int
        /// (created with privious function) to a binary array.
        /// </remarks>
        public static bool[] Int32ToBoolArray(int num)
        {
            var bReturn = Convert.ToInt32(Convert.ToString(num, 2));
            var aReturn = FixLength(bReturn.ToString(), 8);
            var cReturn = new bool[8];
            
            for (var i = 0; i <= 7; i++)
            {
                cReturn[i] = aReturn.Substring(i, 1) == "1";
            }
            return cReturn;
        }

        /// <summary>
        /// Calculates modulo from specified number based on base number.
        /// </summary>
        /// <param name="num">Input number.</param>
        /// <param name="baseNum">Modulo base.</param>
        /// <returns>Modulo from specified number.</returns>
        /// <remarks>
        /// This function simply calculates the "right Modulo". By using this function,
        /// there won't, hopefully be a negative number in the result!
        /// </remarks>
        public static int Modulo(int num, int baseNum)
        {
            return num - baseNum * Convert.ToInt32(Math.Floor(num / (double)baseNum));
        }

        /// <summary>
        /// Calculate a specified number raised to the specified power.
        /// </summary>
        /// <param name="x">Number to be raised to a power.</param>
        /// <param name="y">Number that specifies a power.</param>
        /// <returns>The number <paramref name="x"/> raised to the power <paramref name="y"/>.</returns>
        /// <remarks>
        /// Because of the uncertain answer using <see cref="Math.Pow"/> and <c>^</c>, 
        /// this function is here to solve that issue.
        /// </remarks>
        public static BigInteger PowerOf(int x, int y)
        {
            BigInteger newNum = 1;

            switch (y)
            {
                case 0:
                    // if 0, return 1, e.g. x^0 = 1 (mathematicaly proven!) 
                    newNum = 1;
                    break;
               
                case 1:
                    // if 1, return x, which is the base, e.g. x^1 = x
                    newNum = x;
                    break;
                
                default:
                    // if both conditions are not satisfied, this loop
                    // will continue to y, which is the exponent.
                    for (var i = 0; i <= y - 1; i++)
                    {
                        newNum = newNum * x;
                    }
                    break;
            }
            return newNum;
        }

        /// <summary>
        /// Checks specified <see cref="string"/> for containing non-digit character(s).
        /// </summary>
        /// <param name="text">Input <see cref="string"/>.</param>
        /// <returns><c>true</c> if the specified input contains non-digit character(s), otherwise <c>false</c>.</returns>
        public static bool ContainsNonDigitChar(string text)
        {
            var reg = new Regex("^\\d$", RegexOptions.Compiled);
            return reg.IsMatch(text);
        }

        /// <summary>
        /// Shifts decimal number.
        /// </summary>
        /// <param name="num">Input number.</param>
        /// <param name="shift">Shift value.</param>
        public static void ShiftNumber(ref decimal num, int shift)
        {
            num *= shift;
        }


        // ----- PRIVATE METHODS ---------------------------------------------
        private static string FixLength(string number, int lenght)
        {
            // This function create 3 lenght char ex: 39 to 039
            if (number.Length == lenght) return number;
            while (number.Length != lenght)
            {
                number = "0" + number;
            }
            return number;
        }
    }
}

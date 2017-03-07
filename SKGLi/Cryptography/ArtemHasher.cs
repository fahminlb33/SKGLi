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
using System.Text;

namespace SKGLi.Cryptography
{
    /// <summary>
    /// Provides Artem's hashing algorithms.
    /// </summary>
    /// <remarks>This library currently implements SKA-2 and ISF-2 (SKGL API 2.0.5.2).</remarks>
    public static class ArtemHasher
    {
        /// <summary>
        /// Base character used to hash.
        /// </summary>
        private const string BaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Compute a 25-byte hash of <paramref name="text"/>.
        /// </summary>
        /// <param name="text">Input <see cref="string"/> to hash.</param>
        /// <returns>25-byte hash of specified <paramref name="text"/>.</returns>
        public static string Hash25Byte(string text)
        {
            var amountOfBlocks = text.Length / 5;
            var preHash = new string[amountOfBlocks + 1];

            if (text.Length <= 5)
            {
                //if the input string is shorter than 5, no need of blocks! 
                preHash[0] = Hash8Byte(text).ToString();
            }
            else if (text.Length > 5)
            {
                //if the input is more than 5, there is a need of dividing it into blocks.
                for (var i = 0; i <= amountOfBlocks - 2; i++)
                {
                    preHash[i] = Hash8Byte(text.Substring(i * 5, 5)).ToString();
                }

                var data = text.Substring((preHash.Length - 2)*5);
                var length = text.Length - (preHash.Length - 2) * 5;
                preHash[preHash.Length - 2] = Hash8Byte(data, length).ToString();
            }
            return string.Join("", preHash);
        }

        /// <summary>
        /// Compute an 8-bytes hash of specified <paramref name="text"/>.
        /// </summary>
        /// <param name="text">Input <see cref="string"/> to hash.</param>
        /// <param name="mustBeLessThan">Maximum result length.</param>
        /// <returns>8-byte hash of specified <paramref name="text"/>.</returns>
        /// <remarks>
        /// <para>This function generates a eight byte hash.</para>
        /// <para>
        /// The length of the result might be changed to any length.
        /// Just set the amount of zeroes in <paramref name="mustBeLessThan"/>
        /// to any length you want.
        /// </para>
        /// </remarks>
        public static int Hash8Byte(string text, int mustBeLessThan = 1000000000)
        {
            uint hash = 0;

            foreach (var b in Encoding.Unicode.GetBytes(text))
            {
                hash += b;
                hash += hash << 10;
                hash ^= hash >> 6;
            }

            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;

            //we want the result to not be zero, as this would thrown an exception in check.
            var result = (int)(hash % mustBeLessThan);
            if (result == 0) result = 1;
            
            var check = mustBeLessThan / result;
            if (check > 1) result *= check;

            //when result is less than mustBeLessThan, multiplication of result with check will be in that boundary.
            //otherwise, we have to divide by 10.
            if (mustBeLessThan == result) result /= 10;
            
            return result;
        }

        /// <summary>
        /// Converts value from Base10 to Base25.
        /// </summary>
        /// <param name="text">Input <see cref="string"/> to hash.</param>
        /// <returns>Base25 value from specified <paramref name="text"/>.</returns>
        /// <remarks>
        /// This method is converting a base 10 number to base 26 number.
        /// Note that this method will still work, even though you only 
        /// it is limited at 232 characters with a <see cref="BigInteger"/>.
        /// </remarks>
        public static string Base10ToBase26(string text)
        {
            var allowedLetters = BaseCharacters.ToCharArray();

            var num = Convert.ToDecimal(text);
            var result = new char[text.Length + 1];
            var j = 0;

            while (num >= 26)
            {
                var reminder = Convert.ToInt32(num % 26);
                result[j] = allowedLetters[reminder];
                num = (num - reminder) / 26;
                j += 1;
            }

            result[j] = allowedLetters[Convert.ToInt32(num)];

            // final calculation
            var returnNum = "";

            for (int k = j; k >= 0; k -= 1)  // not sure
            {
                returnNum += result[k];
            }
            return returnNum;
        }

        /// <summary>
        /// Converts value from Base25 to Base10.
        /// </summary>
        /// <param name="text">Input <see cref="string"/> to hash.</param>
        /// <returns>Base10 value from specified <paramref name="text"/>.</returns>
        /// <remarks>
        /// This function will convert a number that has been generated
        /// with <see cref="Base10ToBase26"/>, and get the actual number in decimal
        /// </remarks>
        public static string Base26ToBase10(string text)
        {
            var result = new BigInteger();
            for (var i = 0; i <= text.Length - 1; i += 1)
            {
                var pow = MathEx.PowerOf(26, text.Length - i - 1);
                result += BaseCharacters.IndexOf(text.Substring(i, 1), StringComparison.Ordinal) * pow;
            }

            return result.ToString(); //not sure
        }
    }
}

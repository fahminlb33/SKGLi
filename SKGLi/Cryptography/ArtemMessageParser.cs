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

namespace SKGLi.Cryptography
{
    /// <summary>
    /// Provides Artem's message parsing capability.
    /// </summary>
    /// <remarks>This library currently implements SKA-2 and ISF-2 (SKGL API 2.0.5.2).</remarks>
    public sealed class ArtemMessageParser
    {
        private readonly string _codedData;
        internal ArtemMessageParser(string codedData)
        {
            _codedData = codedData;
        }

        /// <summary>
        /// Get serial key creation date.
        /// </summary>
        /// <returns>Serial key creation date.</returns>
        public DateTime GetCreationDate()
        {
            var yy = Convert.ToInt32(_codedData.Substring(9, 4));
            var mm = Convert.ToInt32(_codedData.Substring(13, 2));
            var dd = Convert.ToInt32(_codedData.Substring(15, 2));
            return new DateTime(yy, mm, dd);
        }

        /// <summary>
        /// Get serial key's expire days.
        /// </summary>
        /// <returns>Serial key set time.</returns>
        public int GetSetTime()
        {
            return Convert.ToInt32(_codedData.Substring(17, 3));
        }

        /// <summary>
        /// Get serial key expire day(s) left relative to current date.
        /// </summary>
        /// <returns>Number of days left before serial key expired.</returns>
        public int GetDaysLeft()
        {
            return Convert.ToInt32((GetExpireDate() - DateTime.Today).TotalDays);
        }

        /// <summary>
        /// Get serial key expire date.
        /// </summary>
        /// <returns>Serial key expire date.</returns>
        public DateTime GetExpireDate()
        {
            var setTime = GetSetTime();
            return setTime > 0 ? GetCreationDate().AddDays(setTime) : DateTime.MaxValue;
        }

        /// <summary>
        /// Get serial key feature array (of <see cref="bool"/> array).
        /// </summary>
        /// <returns>Serial key feature array (of <see cref="bool"/> array).</returns>
        public bool[] GetFeatures()
        {
            return MathEx.Int32ToBoolArray(Convert.ToInt32(_codedData.Substring(20, 3)));
        }

        /// <summary>
        /// Get serial key machine code.
        /// </summary>
        /// <returns>Serial key machine code.</returns>
        public int GetMachineCode()
        {
            return Convert.ToInt32(_codedData.Substring(23, 5));
        }
    }
}

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

namespace SKGLi
{
    /// <summary>
    /// Serial key informations.
    /// </summary>
    public class KeyData : ICloneable
    {
        #region Fields
        /// <summary>
        /// Specifies no feature enabled.
        /// </summary>
        public static readonly bool[] EmptyFeatures = { false, false, false, false, false, false, false, false };
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyData"/> class.
        /// </summary>
        public KeyData()
        {
            TimeLeft = 30;
            CreationDate = DateTime.Today;
            Features = EmptyFeatures;
            MachineCode = -1;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Expiration days after this serial is generated.
        /// </summary>
        public int TimeLeft { get; set; }

        /// <summary>
        /// Date on when this serial is created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Features to be added to license.
        /// </summary>
        public bool[] Features { get; set; }

        /// <summary>
        /// Unique machine code for machine-locking feature.
        /// </summary>
        public int MachineCode { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}

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
    /// Detail information from validated serial number.
    /// </summary>
    public class KeyDetails : KeyData
    {
        /// <summary>
        /// Gets serial key expiration date.
        /// </summary>
        public DateTime ExpireDate { get; internal set; }

        /// <summary>
        /// Get serial key expiration day.
        /// </summary>
        public int SetTime { get; internal set; }

        /// <summary>
        /// Gets a value indicating if the serial key is expired or not.
        /// </summary>
        public bool IsExpired { get; internal set; }

        /// <summary>
        /// Gets a value indicating if the serial is entered on right machine or not.
        /// </summary>
        public bool IsOnRightMachine { get; internal set; }

        /// <summary>
        /// Gets a value indicating this key is valid or not.
        /// </summary>
        /// <remarks>
        /// This property only means that this serial key is not modified. You still
        /// need to check other properties such as <see cref="IsExpired"/> and 
        /// <see cref="IsOnRightMachine"/> to make sure the serial key is "really" valid.
        /// </remarks>
        public bool IsValid { get; internal set; }
    }
}

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
    /// Abstract implementation of Artem's Serial Key Algorithm.
    /// </summary>
    /// <remarks>This library currently implements SKA-2 and ISF-2 (SKGL API 2.0.5.2).</remarks>
    public abstract class Artem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Artem"/> implementation class.
        /// </summary>
        /// <returns><see cref="Artem"/> derived class, <see cref="ArtemManaged"/>.</returns>
        public static Artem Create()
        {
            return new ArtemManaged();
        }
        
        #region Properties
        /// <summary>
        /// Secret phase used to encrypt/decrypt the serial key.
        /// </summary>
        public abstract string SecretPhase { get; set; }
        #endregion

        #region Methods
        // ----- Artem's Serial Key Algorithm ------------------------------------
        /// <summary>
        /// Checks for secret phase validity.
        /// </summary>
        /// <remarks>
        /// This will checks the secret phase for empty or null value and
        /// hashed secret phase for non-digit character(s). The hashed 
        /// SecretPhase shall only consist of digits!
        /// </remarks>
        public abstract void CheckSecretPhase();
        /// <summary>
        /// Encrypts specified string using SKA-2 algorithm.
        /// </summary>
        /// <param name="inputData">Input data to encrypt.</param>
        /// <returns>Encrypted data from supplied data.</returns>
        public abstract string Encrypt(string inputData);
        /// <summary>
        /// Decrypts specified string using SKA-2 algorithm.
        /// </summary>
        /// <param name="inputData">Input data to decrypt.</param>
        /// <returns>Decrypted data from supplied data.</returns>
        public abstract string Decrypt(string inputData);

        // ----- Artem's Information Storage Format ------------------------------
        /// <summary>
        /// Compute data using SKA-2 encrypted number.
        /// </summary>
        /// <param name="days">Number of days left.</param>
        /// <param name="features">Features to be included on serial (8 features).</param>
        /// <param name="id">Random ID, usually machine code (see <see cref="MachineIdentificator"/>).</param>
        /// <param name="creationDate"><see cref="DateTime"/> value when this serial is generated.</param>
        /// <returns>Number representation of encrypted informations.</returns>
        public abstract decimal ComputeData(int days, bool[] features, int id, DateTime creationDate);
        /// <summary>
        /// Encrypts specified computed information.
        /// </summary>
        /// <returns>Encrypted serial number using SKA-2 and formatted using ISF-2.</returns>
        public abstract string Store(decimal data);
        /// <summary>
        /// Decrypts specified information using SKA-2 algorithm.
        /// </summary>
        /// <param name="serial">Formatted serial key using ISF-2.</param>
        /// <returns>Decoded SKA-2 message.</returns>
        public abstract string Retrieve(string serial);
        /// <summary>
        /// Calculate hash from decoded key.
        /// </summary>
        /// <param name="decoded">Decoded key.</param>
        /// <returns>Calculated hash value from decoded key.</returns>
        /// <remarks>
        /// Don't be confused with <see cref="GetStoredHashFromKey"/>. This method
        /// calculate hash value based on encrypted information on serial key, not
        /// by decoding stored hash.</remarks>
        public abstract string CalculateHashFromKey(string decoded);
        /// <summary>
        /// Get hash information from decoded string.
        /// </summary>
        /// <param name="decoded">Decoded key.</param>
        /// <returns>Stored hash value from decoded key.</returns>
        /// <remarks>
        /// Don't be confused with <see cref="CalculateHashFromKey"/>. This method
        /// retrieve stored hash value in the beginning of serial key, not by 
        /// calculating encrypted information on serial key.
        /// </remarks>
        public abstract string GetStoredHashFromKey(string decoded);
        #endregion
    }
}

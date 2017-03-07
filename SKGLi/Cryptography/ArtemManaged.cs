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
using System.Globalization;
using System.Text;

namespace SKGLi.Cryptography
{
    /// <summary>
    /// Concrete implementation of Artem's Serial Key Algorithm (see <see cref="Artem"/>).
    /// </summary>
    /// <remarks>This library currently implements SKA-2 and ISF-2 (SKGL API 2.0.5.2).</remarks>
    public class ArtemManaged : Artem
    {
        private string _secretPhase;
        private string _hashedSecretPhase;

        #region Properties
        /// <summary>
        /// Secret phase used to encrypt/decrypt the serial key.
        /// </summary>
        public override string SecretPhase
        {
            get { return _secretPhase; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new InvalidOperationException("SecretPhase cannot be empty or null.");

                _secretPhase = value;
                _hashedSecretPhase = ArtemHasher.Hash25Byte(value);
            }
        }
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
        public override void CheckSecretPhase()
        {
            if (string.IsNullOrEmpty(_secretPhase))
                throw new InvalidOperationException("SecretPhase cannot be empty or null.");

            if (MathEx.ContainsNonDigitChar(_hashedSecretPhase))
                throw new InvalidOperationException("The hashed secret phase consist of non-numerical letters.");
        }
        
        /// <summary>
        /// Encrypts specified string using SKA-2 algorithm.
        /// </summary>
        /// <param name="inputData">Input data to encrypt.</param>
        /// <returns>Encrypted data from supplied data.</returns>
        public override string Encrypt(string inputData)
        {
            //in this class we are encrypting the integer array.
            var res = new StringBuilder();

            for (var i = 0; i <= inputData.Length - 1; i++)
            {
                var pos = MathEx.Modulo(i, _hashedSecretPhase.Length);
                var num = Convert.ToInt32(inputData.Substring(i, 1));
                num += Convert.ToInt32(_hashedSecretPhase.Substring(pos, 1));
                res.Append(MathEx.Modulo(num, 10));
            }

            return res.ToString();
        }
        
        /// <summary>
        /// Decrypts specified string using SKA-2 algorithm.
        /// </summary>
        /// <param name="inputData">Input data to decrypt.</param>
        /// <returns>Decrypted data from supplied data.</returns>
        public override string Decrypt(string inputData)
        {
            //in this class we are decrypting the text encrypted with the function above.
            var res = new StringBuilder();

            for (var i = 0; i <= inputData.Length - 1; i++)
            {
                var pos = MathEx.Modulo(i, _hashedSecretPhase.Length);
                var num = Convert.ToInt32(inputData.Substring(i, 1));
                num -= Convert.ToInt32(_hashedSecretPhase.Substring(pos, 1));
                res.Append(MathEx.Modulo(num, 10));
            }

            return res.ToString();
        }

        // ----- Artem's Information Storage Format ------------------------------
        /// <summary>
        /// Compute data using SKA-2 encrypted number.
        /// </summary>
        /// <param name="days">Number of days left.</param>
        /// <param name="features">Features to be included on serial (8 features).</param>
        /// <param name="id">Random ID, usually machine code (see <see cref="MachineIdentificator"/>).</param>
        /// <param name="creationDate"><see cref="DateTime"/> value when this serial is generated.</param>
        /// <returns>Number representation of encrypted informations.</returns>
        public override decimal ComputeData(int days, bool[] features, int id, DateTime creationDate)
        {
            decimal result = 0;

            // adding the current date; the generation date; today.
            result += Convert.ToInt32(creationDate.ToString("yyyyMMdd"));

            // shifting three times at left
            MathEx.ShiftNumber(ref result, 1000);

            // adding time left
            result += days;

            // shifting three times at left
            MathEx.ShiftNumber(ref result, 1000);

            // adding features
            result += MathEx.BoolArrayToInt32(features);

            //shifting three times at left
            MathEx.ShiftNumber(ref result, 100000);

            // adding random id
            result += id;

            return result;
        }

        /// <summary>
        /// Encrypts specified computed information.
        /// </summary>
        /// <returns>Encrypted serial number using SKA-2 and formatted using ISF-2.</returns>
        public override string Store(decimal data)
        {
            // This part of the function uses Artem's SKA-2
            var resultString = data.ToString(CultureInfo.InvariantCulture);
            return ArtemHasher.Base10ToBase26(ArtemHasher.Hash8Byte(resultString) + Encrypt(resultString));
        }

        /// <summary>
        /// Decrypts specified information using SKA-2 algorithm.
        /// </summary>
        /// <param name="serial">Formatted serial key using ISF-2.</param>
        /// <returns>Decoded SKA-2 message.</returns>
        public override string Retrieve(string serial)
        {
            var usefulInformation = ArtemHasher.Base26ToBase10(serial);
            return usefulInformation.Substring(0, 9) + Decrypt(usefulInformation.Substring(9));
        }

        /// <summary>
        /// Calculate hash from decoded key.
        /// </summary>
        /// <param name="decoded">Decoded key.</param>
        /// <returns>Calculated hash value from decoded key.</returns>
        /// <remarks>
        /// Don't be confused with <see cref="GetStoredHashFromKey"/>. This method
        /// calculate hash value based on encrypted information on serial key, not
        /// by decoding stored hash.</remarks>
        public override string CalculateHashFromKey(string decoded)
        {
            return ArtemHasher.Hash8Byte(decoded.Substring(9, 19)).ToString().Substring(0, 9);
        }

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
        public override string GetStoredHashFromKey(string decoded)
        {
            return decoded.Substring(0, 9);
        }
        #endregion

    }
}

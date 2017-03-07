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
using System.Security.Cryptography;
using System.Text;
using SKGLi.Cryptography;

namespace SKGLi
{
    /// <summary>
    /// Main SKGL API.
    /// </summary>
    public class SerialFactory : IDisposable
    {
        private readonly RNGCryptoServiceProvider _rng;
        private readonly Artem _ska;
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SerialFactory"/> class.
        /// </summary>
        public SerialFactory()
        {
            _rng = new RNGCryptoServiceProvider();
            _ska = Artem.Create();
            _ska.SecretPhase = GenerateSecretPhase();

            AddSplitChar = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Add split character to generated serial key ("-"/dash character).
        /// </summary>
        public bool AddSplitChar { get; set; }
        /// <summary>
        /// Secret phase to encrypt/decrypt serial key.
        /// </summary>
        public string SecretPhase
        {
            get { return _ska.SecretPhase; }
            set { _ska.SecretPhase = value; }
        }
        #endregion

        #region Methods
        // ----- GenerateKey Overloads ----------------------------------
        /// <summary>
        /// Generate new serial key.
        /// </summary>
        /// <param name="timeLeft">Number of days left.</param>
        /// <returns>Unique serial key based on specified information(s).</returns>
        public string GenerateSerial(int timeLeft)
        {
            return GenerateSerial(timeLeft, DateTime.Today);
        }
        /// <summary>
        /// Generate new serial key.
        /// </summary>
        /// <param name="timeLeft">Number of days left.</param>
        /// <param name="creationDate">Serial key creation date.</param>
        /// <returns>Unique serial key based on specified information(s).</returns>
        public string GenerateSerial(int timeLeft, DateTime creationDate)
        {
            return GenerateSerial(timeLeft, creationDate, KeyData.EmptyFeatures);
        }
        /// <summary>
        /// Generate new serial key.
        /// </summary>
        /// <param name="timeLeft">Number of days left.</param>
        /// <param name="machineCode">Serial key creation date.</param>
        /// <returns>Unique serial key based on specified information(s).</returns>
        public string GenerateSerial(int timeLeft, int machineCode)
        {
            return GenerateSerial(timeLeft, DateTime.Today, machineCode);
        }
        /// <summary>
        /// Generate new serial key.
        /// </summary>
        /// <param name="timeLeft">Number of days left.</param>
        /// <param name="creationDate">Serial key creation date.</param>
        /// <param name="machineCode">Unique ID (usually machine code; get using <see cref="MachineIdentificator.GetMachineCode"/>.</param>
        /// <returns>Unique serial key based on specified information(s).</returns>
        public string GenerateSerial(int timeLeft, DateTime creationDate, int machineCode)
        {
            return GenerateSerial(timeLeft, creationDate, KeyData.EmptyFeatures, machineCode);
        }
        /// <summary>
        /// Generate new serial key.
        /// </summary>
        /// <param name="timeLeft">Number of days left.</param>
        /// <param name="features">Features available for license.</param>
        /// <returns>Unique serial key based on specified information(s).</returns>
        public string GenerateSerial(int timeLeft, bool[] features)
        {
            return GenerateSerial(timeLeft, DateTime.Today, features);
        }
        /// <summary>
        /// Generate new serial key.
        /// </summary>
        /// <param name="timeLeft">Number of days left.</param>
        /// <param name="creationDate">Serial key creation date.</param>
        /// <param name="features">Features available for license.</param>
        /// <param name="machineCode">Unique ID (usually machine code; get using <see cref="MachineIdentificator.GetMachineCode"/>.</param>
        /// <returns>Unique serial key based on specified information(s).</returns>
        public string GenerateSerial(int timeLeft, DateTime creationDate, bool[] features, int machineCode = 0)
        {
            var data = new KeyData
            {
                TimeLeft = timeLeft,
                CreationDate = creationDate,
                Features = features,
                MachineCode = machineCode,
            };
            return GenerateSerial(data);
        }
        /// <summary>
        /// Generate new serial key.
        /// </summary>
        /// <param name="data">Serial key information(s).</param>
        /// <returns>Unique serial key based on specified information(s).</returns>
        public string GenerateSerial(KeyData data)
        {
            if (data.TimeLeft > 999)
                throw new ArgumentException("The timeLeft is larger than 999. It can only consist of three digits.");
            if (data.MachineCode > 99999)
                throw new ArgumentException("Machine ID is larger than 99,999. It can only consist of five digits.");
         
            //if no exception is thown, do following
            if (!(data.MachineCode > 0 & data.MachineCode <= 99999))
                data.MachineCode = GenerateRandomNumber();

            //compute
            var computed = _ska.ComputeData(data.TimeLeft, data.Features, data.MachineCode, data.CreationDate);
            var stageThree = _ska.Store(computed);

            return AddSplitChar ? InsertSplitChar(stageThree) : stageThree;
        }


        // ----- ValidateKey Overloads ----------------------------------
        /// <summary>
        /// Validate serial key.
        /// </summary>
        /// <param name="serial">Input serial key to validate.</param>
        /// <returns><see cref="KeyDetails"/> instance containing serial key informations.</returns>
        public KeyDetails ValidateSerial(string serial)
        {
            // check if serial length is correct
            if (serial.Length != 20 && serial.Length != 23)
            {
                return new KeyDetails {IsValid = false};
            }
            
            // check for checksum
            var decoded = DecodeKeyToString(serial);
            if (_ska.GetStoredHashFromKey(decoded) != _ska.CalculateHashFromKey(decoded))
            {
                return new KeyDetails {IsValid = false};
            }

            // parse data
            var parser = new ArtemMessageParser(decoded);
            var info = new KeyDetails
            {
                IsValid = true,
                IsExpired = parser.GetDaysLeft() < 0,
                IsOnRightMachine = parser.GetMachineCode() == MachineIdentificator.GetMachineCode(),

                CreationDate = parser.GetCreationDate(),
                ExpireDate = parser.GetExpireDate(),
                Features = parser.GetFeatures(),
                MachineCode = parser.GetMachineCode(),
                SetTime = parser.GetSetTime(),
                TimeLeft = parser.GetDaysLeft(),
            };
            
            return info;
        }


        // ----- Methods ------------------------------------------------
        /// <summary>
        /// Generate cryptographically strong random text.
        /// </summary>
        /// <returns>Random text.</returns>
        public string GenerateSecretPhase()
        {
            var buffer = new byte[32];
            _rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer).Substring(0, 20);
        }


        // ----- PRIVATE METHODS ----------------------------------------
        private string DecodeKeyToString(string serial)
        {
            var cleanSerial = serial.Replace("-", "");
            return _ska.Retrieve(cleanSerial);
        }
        private int GenerateRandomNumber()
        {
            var buffer = new byte[4];
            _rng.GetBytes(buffer);
            var currentValue = Math.Abs(BitConverter.ToInt32(buffer, 0)).ToString();
            var fixedFive = currentValue.Substring(0, 5);
            return Convert.ToInt32(fixedFive);
        }
        private string InsertSplitChar(string key)
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (var c in key)
            {
                if (i % 5 == 0 && i != 0) sb.Append("-");
                sb.Append(c);
                i++;
            }
            return sb.ToString();
        }
        #endregion

        #region IDisposable Support
        private bool _disposedValue;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                if (_rng != null) _rng.Dispose();
            }

            _disposedValue = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

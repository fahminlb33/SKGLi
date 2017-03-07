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
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security;
using System.Text;
using SKGLi.Cryptography;

namespace SKGLi
{
    /// <summary>
    /// Provides machine unique identification.
    /// </summary>
    [SecuritySafeCritical]
    public static class MachineIdentificator
    {
        /// <summary>
        /// Calculate local machine unique ID.
        /// </summary>
        /// <returns>Local machine unique ID.</returns>
        /// <remarks>
        /// <para>Copyright (C) 2012 Artem Los, All rights reserved.</para>
        /// <para>
        /// This code will generate a 5 digits long key, finger print, of the system
        /// where this method is being executed. However, that might be changed in the
        /// hash function <see cref="ArtemHasher.Hash8Byte"/>, by changing the amount 
        /// of zeroes in <see cref="ArtemHasher.Hash8Byte"/> to the one you want to have. 
        /// e.g. 1000 will return 3 digits long hash.
        /// </para>
        /// </remarks>
        [SecuritySafeCritical]
        public static int GetMachineCode()
        {
            var sb = new StringBuilder();

            // stage 1 - collect machine name
            sb.Append(Environment.MachineName);

            // stage 2 - get network mac address
            sb.Append(GetLastPhysicalAddress());

            // stage 3 - get physical media info
            sb.Append(GetPhysicalSerialNumber());
            
            // hash the output
            return ArtemHasher.Hash8Byte(sb.ToString(), 100000);
        }

        /// <summary>
        /// Get last network interface physical address.
        /// </summary>
        /// <returns>Last network interface physical address.</returns>
        [SecuritySafeCritical]
        public static string GetLastPhysicalAddress()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Select(ni => ni.GetPhysicalAddress().ToString())
                .OrderBy(mac => mac).Last();
        }

        /// <summary>
        /// Get all physical media serial number.
        /// </summary>
        /// <returns>Concated physical media serial number.</returns>
        [SecuritySafeCritical]
        public static string GetPhysicalSerialNumber()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia"))
            {
                var x = searcher.Get()
                    .Cast<ManagementObject>()
                    .Where(wmiHd => wmiHd["SerialNumber"] != null || wmiHd["Removable"] != null || wmiHd["Replaceable"] != null)
                    .Select(wmiHd => wmiHd["SerialNumber"])
                    .OrderBy(hdSerial => hdSerial);
                return string.Join("", x);
            }
        }
    }
}

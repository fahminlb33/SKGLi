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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace SKGLi.Demo.Keygen
{
    public partial class Form1 : Form
    {
        private delegate object[] GenereateSerialDelegate(KeyData data, int amount);

        private const string SecretPhase = "djgyaus4&*gigds*^&*W%EWds";
        private readonly SerialFactory _manager;

        public Form1()
        {
            InitializeComponent();
            _manager = new SerialFactory();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblMachineCode.Text = MachineIdentificator.GetMachineCode().ToString();
            txtSecretPhase.Text = _manager.SecretPhase;
        }

        #region Main Tab
        // ----- Button Click Subscribers -----------------------------------
        private void cmdGeneratePhase_Click(object sender, EventArgs e)
        {
            txtSecretPhase.Text = _manager.GenerateSecretPhase();
            _manager.SecretPhase = txtSecretPhase.Text;
        }
        private void cmdGenerate_Click(object sender, EventArgs e)
        {
            // --- Validations
            if (txtSecretPhase.TextLength == 0)
            {
                MessageBox.Show("Please fullfill required informations!");
                return;
            }
            if (txtSecretPhase.TextLength < 20 || txtSecretPhase.TextLength > 20)
            {
                MessageBox.Show("Your secret phase is too short/long. Use 20 character secret phase.");
            }

            // --- Gather informations
            // add values
            var data = new KeyData
            {
                CreationDate = dtCreationDate.Value,
                TimeLeft = (int) numTimeLeft.Value,
            };
            // add features
            var dd = new bool[8];
            for (var i = 0; i < chlFeatures.Items.Count; i++)
            {
                dd[i] = chlFeatures.GetItemChecked(i);
            }
            data.Features = dd;
            // add machine code
            if (chkMachineLocking.Checked)
                data.MachineCode = Convert.ToInt32(lblMachineCode.Text);

            // --- Set secret phase
            _manager.SecretPhase = txtSecretPhase.Text;

            // --- Generate serial
            var caller = new GenereateSerialDelegate(GenerateSerials);
            caller.BeginInvoke(data, (int) numNumberOfKeys.Value, GenerateSerialCallback, null);
        }
        private void cmdCopyToClipboard_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lstSerial.Items.Count == 0) return;

            var data = string.Join(Environment.NewLine, lstSerial.Items.Cast<string>());
            Clipboard.SetText(data);
        }

        // ----- Methods ----------------------------------------------------
        private object[] GenerateSerials(KeyData data, int amount)
        {
            var lst = new List<object>();
            for (var i = 0; i < amount; i++)
            {
                var serial = _manager.GenerateSerial((KeyData)data.Clone());
                lst.Add(serial);
                Debug.Print(serial);
            }
            return lst.ToArray();
        }
        private void GenerateSerialCallback(IAsyncResult ar)
        {
            var asyncResult = (AsyncResult)ar;
            var caller = (GenereateSerialDelegate)asyncResult.AsyncDelegate;

            var objs = caller.EndInvoke(ar);
            this.InvokeOnUiThreadIfRequired(() =>
            {
                lstSerial.Items.Clear();
                lstSerial.Items.AddRange(objs);
            });
        }
        #endregion

        #region Validate Tab
        private void cmdValidate_Click(object sender, EventArgs e)
        {
            if (txtSecretPhaseValidation.TextLength == 0 || txtSerialKeyValidation.TextLength == 0)
            {
                MessageBox.Show("Please fullfill required informations!");
                return;
            }

            // validate
            _manager.SecretPhase = txtSecretPhaseValidation.Text;
            var result = _manager.ValidateSerial(txtSerialKeyValidation.Text);
            propInfo.SelectedObject = result;
        }
        #endregion

        private void AboutLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var url = (LinkLabel) sender;
            Process.Start(url.Text);
        }
    }
}

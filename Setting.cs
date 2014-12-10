using System;
using System.Windows.Forms;
using ComponentPro.Net;
using ComponentPro;

namespace SftpClient
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();
        }

        public Setting(SettingInfo s)
        {
            InitializeComponent();

            _info = s;

            numTransfers.Value = s.Transfers;
            txtThrottle.Text = s.Throttle.ToString();
            txtTimeout.Text = s.Timeout.ToString();
            txtKeepAliveInterval.Text = s.KeepAlive.ToString();
            rbtAscii.Checked = s.AsciiTransfer;
            rbtBinary.Checked = !s.AsciiTransfer;
            tbProgress.Value = s.ProgressUpdateInterval / 50;
            chkAdvancedTransfer.Checked = s.AdvancedTransfer;
            txtReconnectDelay.Text = (s.ReconnectionFailureDelay / 1000).ToString();
            txtConnectionRetries.Value = s.ReconnectionMaxRetries;
            txtOffsetDay.Text = s.TimeZoneOffset.Days.ToString();
            txtOffsetHour.Text = s.TimeZoneOffset.Hours.ToString();
            txtOffsetMin.Text = s.TimeZoneOffset.Minutes.ToString();
            txtOffsetSec.Text = s.TimeZoneOffset.Seconds.ToString();
            chkRestoreFileLastWriteTime.Checked = s.RestoreFileProperties;
            
            #region FTP
            txtUploadBuffer.Text = s.UploadBufferSize.ToString();
            chkSendABOR.CheckState = GetState(s.SendAbor);
            chkSendSignals.Checked = s.SendSignals;
            chkChDirBeforeListing.Checked = s.ChangeDirBeforeListing;
            chkChDirBeforeTransfer.Checked = s.ChangeDirBeforeTransfer;
            chkCompress.Checked = s.Compress;
            chkSmartPath.Checked = s.SmartPath;
            #endregion

            #region SFTP
            cbxServerOs.SelectedIndex = s.ServerOs;
            #endregion
        }



        CheckState GetState(OptionValue value)
        {
            switch (value)
            {
                case OptionValue.Auto:
                    return CheckState.Indeterminate;
                case OptionValue.Yes:
                    return CheckState.Checked;
            }

            return CheckState.Unchecked;
        }

        OptionValue GetState(CheckState value)
        {
            switch (value)
            {
                case CheckState.Indeterminate:
                    return OptionValue.Auto;
                case CheckState.Checked:
                    return OptionValue.Yes;
            }

            return OptionValue.No;
        }

        private readonly SettingInfo _info;

        bool ValidateInt(string valueText, int min, int max, string valueName, out int value)
        {
            if (!int.TryParse(valueText, out value) || value < min || value > max)
            {
                MessageBox.Show(string.Format("Invalid {0}. It must be in {1} and {2} range.", valueName, min, max), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int limit;
            try
            {
                limit = int.Parse(txtThrottle.Text);
            }
            catch (Exception exc)
            {
                Util.ShowError(exc, "Invalid Throttle");
                return;
            }
            if (limit < 0)
            {
                MessageBox.Show("Invalid throttle", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int timeout;
            try
            {
                timeout = int.Parse(txtTimeout.Text);
            }
            catch (Exception exc)
            {
                Util.ShowError(exc, "Invalid Timeout");
                return;
            }
            if (timeout < 1)
            {
                MessageBox.Show("Invalid timeout, it must be greater than or equal to 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int keepaliveint;
            try
            {
                keepaliveint = int.Parse(txtKeepAliveInterval.Text);
            }
            catch (Exception exc)
            {
                Util.ShowError(exc, "Invalid Interval");
                return;
            }
            if (keepaliveint < 10)
            {
                MessageBox.Show("Invalid keep alive interval, it must be greater than or equal to 10", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int reconnectDelay;
            if (!int.TryParse(txtReconnectDelay.Text, out reconnectDelay) || reconnectDelay < 0 || reconnectDelay > 999)
            {
                MessageBox.Show("Invalid delay between failed connection attempts. It must be between 0 and 999 seconds.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            int uploadBufferSize;
            if (!int.TryParse(txtUploadBuffer.Text, out uploadBufferSize) || uploadBufferSize < 1 || uploadBufferSize > 16 * 1024)
            {
                MessageBox.Show("Invalid upload buffer size. It must be between 1 Kb and 16384 Kb.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            int offsetDay, offsetHour, offsetMin, offsetSec;

            if (!ValidateInt(txtOffsetDay.Text, int.MinValue, int.MaxValue, "Server Time Zone Offset's Days", out offsetDay) ||
                !ValidateInt(txtOffsetHour.Text, -23, 23, "Server Time Zone Offset's Hours", out offsetHour) ||
                !ValidateInt(txtOffsetMin.Text, -59, 59, "Server Time Zone Offset's Minutes", out offsetMin) ||
                !ValidateInt(txtOffsetSec.Text, -59, 59, "Server Time Zone Offset's Seconds", out offsetSec))
                return;

            _info.AsciiTransfer = rbtAscii.Checked;
            _info.KeepAlive = keepaliveint;

            _info.Transfers = (int)numTransfers.Value;
            _info.Throttle = limit;
            _info.Timeout = timeout;
            _info.ProgressUpdateInterval = tbProgress.Value * 50;
            _info.AdvancedTransfer = chkAdvancedTransfer.Checked;
            _info.ReconnectionFailureDelay = reconnectDelay * 1000;
            _info.ReconnectionMaxRetries = (int)txtConnectionRetries.Value;
            _info.TimeZoneOffset = new TimeSpan(offsetDay, offsetHour, offsetMin, offsetSec);
            _info.RestoreFileProperties = chkRestoreFileLastWriteTime.Checked;
            
            _info.SendAbor = GetState(chkSendABOR.CheckState);
            _info.SendSignals = chkSendSignals.Checked;
            _info.ChangeDirBeforeListing = chkChDirBeforeListing.Checked;
            _info.ChangeDirBeforeTransfer = chkChDirBeforeTransfer.Checked;
            _info.Compress = chkCompress.Checked;
            _info.SmartPath = chkSmartPath.Checked;

            _info.UploadBufferSize = uploadBufferSize;
            _info.ServerOs = cbxServerOs.SelectedIndex;

            this.DialogResult = DialogResult.OK;
        }

        private void chkRestoreFileLastWriteTime_CheckedChanged(object sender, EventArgs e)
        {
            txtOffsetDay.Enabled = txtOffsetHour.Enabled = txtOffsetMin.Enabled = txtOffsetSec.Enabled = chkRestoreFileLastWriteTime.Checked;
        }

        private void chkAdvancedTransfer_CheckedChanged(object sender, EventArgs e)
        {
            numTransfers.Enabled = chkAdvancedTransfer.Checked;
        }
    }
}
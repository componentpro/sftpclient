using System;
using System.Windows.Forms;
using ComponentPro.Diagnostics;
using ComponentPro.Net;

namespace SftpClient
{
    /// <summary>
    /// Represents the Login form.
    /// </summary>
    public partial class Login : Form
    {
        private readonly LoginInfo _info;
        private readonly SiteInfo[] _sites;

        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the form base on the provided LoginInfo that is loaded from the Registry.
        /// </summary>
        /// <param name="s">The LoginInfo object.</param>
        public Login(LoginInfo s)
        {
            InitializeComponent();

            _info = s;

            txtServer.Text = s.ServerName;
            txtPort.Text = s.ServerPort.ToString();
            txtUserName.Text = s.UserName;
            txtPassword.Text = s.Password;
            txtRemoteDir.Text = s.RemoteDir;
            txtLocalDir.Text = s.LocalDir;
            
            chkUtf8Encoding.Checked = s.Utf8Encoding;

            txtProxyHost.Text = s.ProxyServer;
            txtProxyPort.Text = s.ProxyPort.ToString();
            txtProxyUser.Text = s.ProxyUser;
            txtProxyPassword.Text = s.ProxyPassword;
            txtProxyDomain.Text = s.ProxyDomain;
            cbxProxyType.SelectedIndex = (int) s.ProxyType;
            cbxProxyMethod.SelectedIndex = (int) s.ProxyMethod;



            chkCompress.Checked = s.EnableCompression;            

            #region SFTP
            txtPrivateKey.Text = s.PrivateKey;
            #endregion

            switch (_info.LogLevel)
            {
                case TraceEventType.Error:
                    cbxLogLevel.SelectedIndex = 1;
                    break;

                case TraceEventType.Information:
                    cbxLogLevel.SelectedIndex = 2;
                    break;

                case TraceEventType.Verbose:
                    cbxLogLevel.SelectedIndex = 3;
                    break;

                case TraceEventType.Transfer:
                    cbxLogLevel.SelectedIndex = 4;
                    break;

                default: // None
                    cbxLogLevel.SelectedIndex = 0;
                    break;
            }

            _sites = SiteLoader.GetSites();

            if (_sites == null)
            {
                MessageBox.Show("Sites.xml not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            foreach (SiteInfo info in _sites)
            {
                cbxSites.Items.Add(info.Description);
            }
        }

        /// <summary>
        /// Handles the Login button's Click event.
        /// </summary>
        /// <param name="sender">The button object.</param>
        /// <param name="e">The event arguments.</param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            int port;
            // Check port number.
            try
            {
                port = int.Parse(txtPort.Text);
            }
            catch (Exception exc)
            {
                Util.ShowError(exc, "Invalid Port");
                return;
            }
            if (port < 0 || port > 65535)
            {
                MessageBox.Show("Invalid port number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check server name.
            if (txtServer.Text.Length == 0)
            {
                MessageBox.Show("Host name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check proxy port.
            int proxyport;
            try
            {
                proxyport = int.Parse(txtProxyPort.Text);
            }
            catch (Exception exc)
            {
                Util.ShowError(exc, "Invalid Proxy Port");
                return;
            }
            if (proxyport < 0 || proxyport > 65535)
            {
                MessageBox.Show("Invalid port number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

#if !FTP // FTP
            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                MessageBox.Show("SFTP User Name cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtPassword.Text) && string.IsNullOrEmpty(txtPrivateKey.Text))
            {
                MessageBox.Show("You must specify either password or private key file or both of them", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
#endif // FTP

            _info.ServerName = txtServer.Text;
            _info.ServerPort = int.Parse(txtPort.Text);
            _info.UserName = txtUserName.Text;
            _info.Password = txtPassword.Text;
            _info.RemoteDir = txtRemoteDir.Text;
            _info.LocalDir = txtLocalDir.Text;
            
            _info.Utf8Encoding = chkUtf8Encoding.Checked;

            _info.ProxyServer = txtProxyHost.Text;
            _info.ProxyPort = int.Parse(txtProxyPort.Text);
            _info.ProxyUser = txtProxyUser.Text;
            _info.ProxyPassword = txtProxyPassword.Text;
            _info.ProxyDomain = txtProxyDomain.Text;
            _info.ProxyType = (ProxyType) cbxProxyType.SelectedIndex;
            _info.ProxyMethod = (ProxyHttpConnectAuthMethod) cbxProxyMethod.SelectedIndex;



            #region SFTP
            _info.PrivateKey = txtPrivateKey.Text;
            #endregion

            _info.EnableCompression = chkCompress.Checked;            

            switch (cbxLogLevel.SelectedIndex)
            {
                case 0:
                    _info.LogLevel = (TraceEventType)0;
                    break;

                case 1:
                    _info.LogLevel = TraceEventType.Error;
                    break;

                case 2:
                    _info.LogLevel = TraceEventType.Information;
                    break;

                case 3:
                    _info.LogLevel = TraceEventType.Verbose;
                    break;

                case 4:
                    _info.LogLevel = TraceEventType.Transfer;
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles the LocalDirBrowse button's Click event.
        /// </summary>
        /// <param name="sender">The button object.</param>
        /// <param name="e">The event arguments.</param>
        private void btnLocalDirBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.Description = "Select local folder";
                dlg.SelectedPath = txtLocalDir.Text;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtLocalDir.Text = dlg.SelectedPath;
                }
            }
            catch (Exception exc)
            {
                Util.ShowError(exc);
            }
        }

        /// <summary>
        /// Handles the proxy type combobox's SelectedIndexChanged event.
        /// </summary>
        /// <param name="sender">The combobox</param>
        /// <param name="e">The event arguments.</param>
        private void cbxProxy_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enable = cbxProxyType.SelectedIndex > 0;
            bool enableAuth = true;

            cbxProxyMethod.Enabled = cbxProxyType.SelectedIndex == (int)ProxyType.HttpConnect; // Authentication method is available for HTTP Connect only.
            txtProxyDomain.Enabled = cbxProxyMethod.Enabled && cbxProxyMethod.SelectedIndex == (int)ProxyHttpConnectAuthMethod.Ntlm; // Domain is available for NTLM authentication method only.
            txtProxyUser.Enabled = enable && enableAuth;
            txtProxyPassword.Enabled = enable && enableAuth;
            txtProxyHost.Enabled = enable; // Proxy host and port are not available in NoProxy type.
            txtProxyPort.Enabled = enable;
        }

        /// <summary>
        /// Handles the CertBrowse button's Click event.
        /// </summary>
        /// <param name="sender">The button object.</param>
        /// <param name="e">The event arguments.</param>
        private void btnCertBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "Select a certificate file";
                dlg.FileName = txtCertificate.Text;
                dlg.Filter = "All files|*.*";
                dlg.FilterIndex = 1;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtCertificate.Text = dlg.FileName;
                }
            }
            catch (Exception exc)
            {
                Util.ShowError(exc);
            }
        }

        private void cbxSec_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enable = cbxSec.SelectedIndex > 0;
            chkClearCommandChannel.Enabled = enable;
            txtCertificate.Enabled = enable;
            btnCertBrowse.Enabled = enable;
        }

        private void cbxSites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_sites != null)
            {
                SiteInfo info = _sites[cbxSites.SelectedIndex];

                txtServer.Text = info.Address;
                txtPort.Text = info.Port > 0 ? info.Port.ToString() : string.Empty;
                txtUserName.Text = info.UserName;
                txtPassword.Text = info.Password;
                cbxSec.SelectedIndex = info.Security;
            }
        }

        private void btnKeyBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "Select a private key file";
                dlg.FileName = txtPrivateKey.Text;
                dlg.Filter = "All files|*.*";
                dlg.FilterIndex = 1;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtPrivateKey.Text = dlg.FileName;
                }
            }
            catch (Exception exc)
            {
                Util.ShowError(exc);
            }
        }
    }
}
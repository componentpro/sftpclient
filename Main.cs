#define SHOWSPEED

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ComponentPro;
using ComponentPro.IO;
using ComponentPro.Net;
using System.Drawing;
using ComponentPro.Diagnostics;
using ComponentPro.Security.Certificates;

namespace SftpClient
{
    public partial class Main : Form
    {
        #region ConnectionState

        enum ConnectionState
        {
            NotConnected,
            Connecting,
            Ready,
            Disconnecting,
        }

        #endregion

        #region Fields

                        bool _sessionAccepted;

        private readonly bool _exception;
        private LoginInfo _loginSettings; // Login settings.
        private SettingInfo _ftpSettings; // General settings.
        private string _lastRemoteDirectory; // Last remote directory. If changing to another directory fails, we need to change back to the last remote directory.


        private ConnectionState _state; // The connection state.

        private int _folderIconIndex; // Image index of the folder icon.
        private FileIconManager _iconManager; // Icon manager.
        private const int UpFolderImageIndex = 0;
        private const int FolderLinkImageIndex = 1;
        private const int SymlinkImageIndex = 2;

        private FileOperation _fileOpForm;
        private string _mask = "*.*";

        private int _multiThreadTransfer;

        #endregion

        public Main()
        {
            // This try catch block is not needed if you have a production license.
            try
            {
                InitializeComponent();
            }

            catch (ComponentPro.Licensing.Sftp.UltimateLicenseException exc)
            {
                MessageBox.Show(exc.Message, "Error");
                _exception = true;
            }
        }

        

        #region Load and Save Settings

        /// <summary>
        /// Handles the form's Load event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_exception)
            {
                Close();
                return;
            }

            // Load settings from the Registry.
            _loginSettings = LoginInfo.LoadConfig();
            _ftpSettings = SettingInfo.LoadConfig();

            txtLocalDir.Text = _loginSettings.LocalDir;

            // Create a new icon manager object.
            _iconManager = new FileIconManager(imglist, IconSize.Small);
            // Get folder image index.
            _folderIconIndex = _iconManager.AddFolderIcon(FolderType.Closed);

            // Show local files.
            RefreshLocalList();

            _fileOpForm = new FileOperation();
        }

        /// <summary>
        /// Handles the form's Close event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnClosed(EventArgs e)
        {
            // Logged in?
            if (loginToolStripMenuItem.Enabled && loginToolStripMenuItem.Text == "&Disconnect")
            {
                // Disconnect.
                Disconnect();                

                // and wait for the completion.
                while (_state != ConnectionState.NotConnected)
                {
                    System.Threading.Thread.Sleep(50);
                    Application.DoEvents();
                }
            }

            try
            {
                // Try to delete temporary folder that was used to download files for viewing.
                DeleteTempFolder();
            }
            catch { }

            if (_loginSettings != null)
            {
                // Save settings to the Registry.
                _loginSettings.SaveConfig();
                _ftpSettings.SaveConfig();
            }

            base.OnClosed(e);
        }

        #endregion

        #region Connection



        void SftpConnect(Sftp sftp)
        {
            sftp.CompressionEnabled = _loginSettings.EnableCompression;

            WebProxyEx proxy = new WebProxyEx();
            sftp.Proxy = proxy;
            if (_loginSettings.ProxyServer.Length > 0 && _loginSettings.ProxyPort > 0)
            {
                proxy.Server = _loginSettings.ProxyServer;
                proxy.Port = _loginSettings.ProxyPort;
                proxy.UserName = _loginSettings.ProxyUser;
                proxy.Password = _loginSettings.ProxyPassword;
                proxy.Domain = _loginSettings.ProxyDomain;
                proxy.ProxyType = (ProxyType)_loginSettings.ProxyType;
                proxy.AuthenticationMethod = (ProxyHttpConnectAuthMethod)_loginSettings.ProxyMethod;
            }

            SftpApplySettings(sftp);

            // Asynchronously connect to the server. ConnectCompleted event will be fired when it's completed.
            client.ConnectAsync(_loginSettings.ServerName, _loginSettings.ServerPort); // SFTP connection
        }

        void SftpAuthenticate(Sftp sftp)
        {
            SecureShellPrivateKey key = null;

            if (!string.IsNullOrEmpty(_loginSettings.PrivateKey))
            {
                PasswordPrompt dlg = new PasswordPrompt();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        key = new SecureShellPrivateKey(_loginSettings.PrivateKey, dlg.Password);
                    }
                    catch (Exception exc)
                    {
                        Util.ShowError(exc);
                        Disconnect();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(Messages.NoPassword, "Login");
                    Disconnect();
                    return;
                }
            }

            // Asynchronously login. AuthenticateCompleted event will be raised when the operation completes.
            sftp.AuthenticateAsync(_loginSettings.UserName, _loginSettings.Password, key);
        }

        void SftpApplySettings(Sftp sftp)
        {
            if (_ftpSettings.ServerOs > 0)
                sftp.ServerOs = (RemoteServerOs)(_ftpSettings.ServerOs - 1);
        }

        void SftpAddItemPermissions(ListViewItem item, AbstractFileInfo f)
        {
            item.SubItems.Add(ToPermissions(((SftpFileInfo)f).Permissions));
        }

        void SftpShowItemProperties(ListViewItem item)
        {
            SftpPropertiesForm dlg = new SftpPropertiesForm();
            dlg.FileName = GetItemFullName(item);
            dlg.Directory = item.ImageIndex == _folderIconIndex;
            dlg.Permissions = (SftpFilePermissions)Convert.ToUInt32(item.SubItems[3].Text.Substring(0, 3), 16);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            SftpFileAttributes attrs = new SftpFileAttributes();
            attrs.Permissions = dlg.Permissions;

            if (item.ImageIndex == _folderIconIndex && dlg.Recursive)
            {
                EnableProgress(true);

                client.SetMultipleFilesAttributesAsync(dlg.FileName, attrs, dlg.Recursive, null);
            }
            else
            {
                try
                {
                    client.SetFileAttributes(dlg.FileName, attrs);
                    RefreshRemoteList();
                }
                catch (Exception exc)
                {
                    if (!HandleException(exc))
                        return;
                }
            }
        }

        readonly Hashtable _acceptedKeys = new Hashtable();

        void client_CheckingFingerprint(object sender, ComponentPro.Net.HostKeyVerifyingEventArgs e)
        {
            string key = e.HostKey;
            if (_acceptedKeys.ContainsKey(key) || _sessionAccepted)
            {
                e.Accept = true;
                return;
            }

            UnknownHostKey dlg = new UnknownHostKey(_loginSettings.ServerName, _loginSettings.ServerPort, "ssh-" + e.HostKeyAlgorithm + " " + key);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.AlwaysAccept)
                {
                    // Add to the cache.
                    _acceptedKeys.Add(key, true);
                }

                _sessionAccepted = true;
                e.Accept = true;
            }
        }

        void client_SetMultipleFilesAttributesCompleted(object sender, ExtendedAsyncCompletedEventArgs<FileSystemTransferStatistics> e)
        {
            if (e.Error != null)
            {
				// If we encounter a fatal error we need to return immediately.
                if (!HandleException(e.Error))
                    return;
            }

            RefreshRemoteList();
            EnableProgress(false);
        }


        #endregion

        #region Main Menu

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show general settings dialog.
            Setting dlg = new Setting(_ftpSettings);
            if (dlg.ShowDialog() == DialogResult.OK && client != null)
            {
                // Apply changes immmediately.
                client.Timeout = _ftpSettings.Timeout * 1000;
                client.MaxDownloadSpeed = _ftpSettings.Throttle;
                client.MaxUploadSpeed = _ftpSettings.Throttle;
                ((FileSystem)client).ProgressInterval = _ftpSettings.ProgressUpdateInterval;
                client.ReconnectionFailureDelay = _ftpSettings.ReconnectionFailureDelay;
                client.ReconnectionMaxRetries = _ftpSettings.ReconnectionMaxRetries;
                client.ServerTimeZoneOffset = _ftpSettings.TimeZoneOffset;
                client.RestoreFileProperties = _ftpSettings.RestoreFileProperties;
                
                keepAliveTimer.Interval = _ftpSettings.KeepAlive * 1000;
                if (keepAliveTimer.Enabled)
                {
                    keepAliveTimer.Stop();
                    keepAliveTimer.Start();
                }
                client.TransferType = _ftpSettings.AsciiTransfer ? FileTransferType.Ascii : FileTransferType.Binary;

                
                    SftpApplySettings(client);
            }
        }

        /// <summary>
        /// Connects to the server.
        /// </summary>
        private void Connect()
        {
            

            _sessionAccepted = false;

            client.Timeout = _ftpSettings.Timeout*1000;
            client.MaxDownloadSpeed = _ftpSettings.Throttle;
            client.MaxUploadSpeed = _ftpSettings.Throttle;
            client.TransferType = _ftpSettings.AsciiTransfer ? FileTransferType.Ascii : FileTransferType.Binary;
            ((FileSystem) client).ProgressInterval = _ftpSettings.ProgressUpdateInterval;
            client.ReconnectionFailureDelay = _ftpSettings.ReconnectionFailureDelay;
            client.ReconnectionMaxRetries = _ftpSettings.ReconnectionMaxRetries;
            client.ServerTimeZoneOffset = _ftpSettings.TimeZoneOffset;
            client.RestoreFileProperties = _ftpSettings.RestoreFileProperties;

            if (_loginSettings.Utf8Encoding)
                client.Encoding = Encoding.UTF8;
            else
                client.Encoding = Encoding.Default;

            loginToolStripMenuItem.Enabled = false;
            tsbLogin.Enabled = false;
            settingsToolStripMenuItem.Enabled = false;
            tsbSettings.Enabled = false;
            exitToolStripMenuItem.Enabled = false;

            keepAliveTimer.Interval = _ftpSettings.KeepAlive > 0 ? _ftpSettings.KeepAlive * 1000 : 60000;
            keepAliveTimer.Enabled = true;
            keepAliveTimer.Start();

            Util.EnableCloseButton(this, false);

            _state = ConnectionState.Connecting;

            if (XTrace.Listeners.Count == 0)
            {
                XTrace.Listeners.Add(new RichTextBoxTraceListener(txtLog));

#if LOGFILE
                // Add the UltimateConsoleTraceListener listener to write to Console.
                XTrace.Listeners.Add(new UltimateConsoleTraceListener());
                // Add the UltimateTextWriterTraceListener listener to write to a file.
                XTrace.Listeners.Add(new UltimateTextWriterTraceListener("log.log"));
#endif
            }

            XTrace.Level = _loginSettings.LogLevel;

            
            SftpConnect(client);
        }



        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Not logged in?
            if (loginToolStripMenuItem.Text == "&Connect...")
            {
                // Show the Login form.
                Login form = new Login(_loginSettings);
                if (form.ShowDialog() == DialogResult.Cancel)
                    return;

                // Clear log text box.
                txtLog.Clear();

                // Connect to the server.
                Connect();
            }
            else
            {
                // Log out.
                Disconnect();
            }
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwRemote.Focused)
            {
                DoRemoteMove();
            }
            else
            {
                DoLocalMove();
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwRemote.Focused)
            {
                lvwRemote.SelectedItems[0].BeginEdit();
            }
            else
            {
                lvwLocal.SelectedItems[0].BeginEdit();
            }
        }

        private void makeDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwRemote.Focused)
            {
                DoRemoteMakeDir();
            }
            else
            {
                DoLocalMakeDir();
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!progressBar.Enabled)
                // When the File menu item is clicked, enable/disable its child menu items.
                DoRemoteDownload();
        }

        private void uploadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoLocalUpload();
        }



        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwRemote.Focused)
            {
                if (lvwRemote.SelectedItems.Count > 0)
                    DoRemoteProperties(lvwRemote.SelectedItems[0]);
            }
            else
            {
                if (lvwLocal.SelectedItems.Count > 0)
                    DoLocalProperties(lvwLocal.SelectedItems[0]);
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwRemote.Focused)
            {
                DoRemoteView(lvwRemote.SelectedItems[0]);
            }
            else
            {
                DoLocalView(lvwLocal.SelectedItems[0]);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwRemote.Focused)
                RefreshRemoteList();
            else
                RefreshLocalList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwRemote.Focused && lvwRemote.SelectedItems.Count > 0)
            {
                DoRemoteDelete(null, null);
            }
            else if (lvwLocal.Focused && lvwLocal.SelectedItems.Count > 0)
            {
                DoLocalDelete();
            }
        }

        private void synchronizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoSynchronize();
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }



        private void getTimeDifferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnuPopGetTimeDiff_Click(null, null);
        }

        private void resumeDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnuPopResumeDownload_Click(null, null);
        }

        private void resumeUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lcmResumeUpload_Click(null, null);
        }

        private void calculateTotalSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnuPopCalcTotalSize_Click(null, null);
        }

        private void deleteMultipleFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnuPopDeleteMultipleFiles_Click(null, null);
        }

        #endregion

        #region Common

        /// <summary>
        /// Gets a boolean value indicating whether the client object is ready to send a command.
        /// </summary>
        bool Ready
        {
            get { return _enabled == 0; }
        }

        static string GetItemFullName(ListViewItem item)
        {
            return ((AbstractFileInfo)item.Tag).FullName;
        }

        static void SetItemFullName(ListViewItem item, string newFullName)
        {
            ((AbstractFileInfo)item.Tag).UpdateFullName(newFullName);
        }        

        /// <summary>
        /// Loads the icon system resource for the given extension.
        /// </summary>
        /// <param name="name">The file name.</param>
        /// <returns>The image index for the given file name.</returns>
        private int SetFileIcon(string name)
        {
            return _iconManager.AddFileIcon(name);
        }



        /// <summary>
        /// Parses permissions from the given permissions string.
        /// </summary>
        /// <param name="p">The permissions string.</param>
        private static string ToPermissions(SftpFilePermissions p)
        {
            string permissions = Convert.ToString((int)p, 16).PadLeft(3, '0') + " ";

            permissions += GetPermissionChar(p, SftpFilePermissions.GroupExecute, 'x');
            permissions += GetPermissionChar(p, SftpFilePermissions.GroupRead, 'r');
            permissions += GetPermissionChar(p, SftpFilePermissions.GroupWrite, 'w');
            permissions += GetPermissionChar(p, SftpFilePermissions.OthersExecute, 'x');
            permissions += GetPermissionChar(p, SftpFilePermissions.OthersRead, 'r');
            permissions += GetPermissionChar(p, SftpFilePermissions.OthersWrite, 'w');
            permissions += GetPermissionChar(p, SftpFilePermissions.OwnerExecute, 'x');
            permissions += GetPermissionChar(p, SftpFilePermissions.OwnerRead, 'r');
            permissions += GetPermissionChar(p, SftpFilePermissions.OwnerWrite, 'w');

            return permissions;
        }

        private static char GetPermissionChar(SftpFilePermissions p, SftpFilePermissions mask, char ch)
        {
            return (p & mask) == mask ? ch : '-';
        }

        /// <summary>
        /// Returns fully qualified remote path.
        /// </summary>
        /// <param name="fileName">The file path.</param>
        /// <returns>A fully qualified remote path.</returns>
        private string GetFullPath(string fileName)
        {
            return FileSystemPath.Combine(_currentDirectory, fileName);
        }

        #endregion

        #region Log

        /// <summary>
        /// Writes a string with the specified color to the log text box.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// <param name="color">The text color.</param>
        private void WriteLine(string str, Color color)
        {
            string log = str + "\r\n";
            txtLog.SelectionColor = color;
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.SelectedText = log;
            txtLog.ScrollToCaret();
        }

        #endregion

        #region Enable and Disable Progress Bars

        int _enabled;
        /// <summary>
        /// Enables/Disables the dialog. In disabled state, cursor is WaitCursor (an hourglass shape), menus and toolbar are disabled.
        /// </summary>
        /// <param name="enable"></param>
        private void EnableDialog(bool enable)
        {
            if (enable)
            {
                if (_enabled > 0)
                    _enabled--;
            }
            else
                _enabled++;

            if (_enabled == 1)
            {
                Cursor = Cursors.WaitCursor;
                toolbarMain.Enabled = false;
                menuStrip.Enabled = false;
            }
            else if (_enabled == 0)
            {
                Cursor = Cursors.Default;
                toolbarMain.Enabled = true;
                menuStrip.Enabled = true;
            }
        }

        Control _lastFocus;

        /// <summary>
        /// Enables/disables progress bar and abort button.
        /// </summary>
        /// <param name="enable"></param>
        private void EnableProgress(bool enable)
        {
            if (enable)
            {
                // Save the last focused control.
                if (lvwLocal.Focused)
                    _lastFocus = lvwLocal;
                else _lastFocus = lvwRemote.Focused ? lvwRemote : null;
            }
            else
                status.Text = "Ready";

            _aborting = false;
            menuStrip.Enabled = !enable;

            txtLocalDir.Enabled = !enable;
            btnLocalDirBrowse.Enabled = !enable;
            txtRemoteDir.Enabled = !enable;

            lvwRemote.Enabled = !enable;
            lvwLocal.Enabled = !enable;

            progressBar.Enabled = enable;
            progressBar.Value = 0;
            progressBarTotal.Enabled = enable;
            progressBarTotal.Value = 0;

            #region Toolbar

            tsbLogout.Enabled = !enable;
            tsbRefresh.Enabled = !enable;
            tsbSettings.Enabled = !enable;
            tsbUpload.Enabled = !enable;
            tsbDownload.Enabled = !enable;
            tsbDelete.Enabled = !enable;
            tsbMove.Enabled = !enable;
            tsbCreateDir.Enabled = !enable;
            tsbView.Enabled = !enable;
            tsbSynchronize.Enabled = !enable;

            btnAbort.Enabled = enable;

            #endregion

            keepAliveTimer.Enabled = !enable;

            // Disable/Enable the Close button as well.
            Util.EnableCloseButton(this, !enable);

            if (!enable)
            {
                if (_lastFocus != null)
                {
                    // Enable the last focused control.
                    _lastFocus.Focus();
                    EnableButtons();
                }
            }

            if (enable)
                _fileOpForm.Init();
        }

        /// <summary>
        /// Enables/disables control buttons (of menu and toolbar).
        /// </summary>
        private void EnableButtons()
        {
            bool connected = _state == ConnectionState.Ready && Ready;
            bool selected;
            ListViewItem selectedItem;
            bool isFile;

            if (lvwRemote.Focused)
            {
                if (connected && lvwRemote.SelectedItems.Count > 0 &&
                                            lvwRemote.SelectedItems[0].ImageIndex != UpFolderImageIndex)
                    selectedItem = lvwRemote.SelectedItems[0];
                else
                    selectedItem = null;

                selected = connected && (lvwRemote.SelectedItems.Count > 1 || selectedItem != null);
                isFile = selectedItem != null && selectedItem.ImageIndex != _folderIconIndex && selectedItem.ImageIndex != FolderLinkImageIndex;

                renameToolStripMenuItem.Enabled = selectedItem != null;
                deleteToolStripMenuItem.Enabled = selected;
                deleteMultipleFilesToolStripMenuItem.Enabled = selected;
                moveToolStripMenuItem.Enabled = selected;
                makeDirectoryToolStripMenuItem.Enabled = connected;
                uploadFileToolStripMenuItem.Enabled = false;
                uploadUniqueFileToolStripMenuItem.Enabled = false;
                downloadToolStripMenuItem.Enabled = selected;
                viewToolStripMenuItem.Enabled = isFile;
                refreshToolStripMenuItem.Enabled = connected;
                synchronizeToolStripMenuItem.Enabled = connected;
                propertiesToolStripMenuItem.Enabled = selected;

                resumeUploadToolStripMenuItem.Enabled = false;
                resumeDownloadToolStripMenuItem.Enabled = isFile || lvwRemote.SelectedItems.Count > 1;
                executeCommandToolStripMenuItem.Enabled = connected;
                calculateTotalSizeToolStripMenuItem.Enabled = selected;
                getTimeDifferenceToolStripMenuItem.Enabled = connected;
            }
            else if (lvwLocal.Focused)
            {
                selectedItem = (lvwLocal.SelectedItems.Count > 0 &&
                                        lvwLocal.SelectedItems[0].ImageIndex != UpFolderImageIndex) ? lvwLocal.SelectedItems[0] : null;
                selected = lvwLocal.SelectedItems.Count > 1 || selectedItem != null;
                isFile = selectedItem != null && selectedItem.ImageIndex != _folderIconIndex; 

                renameToolStripMenuItem.Enabled = selectedItem != null;
                deleteToolStripMenuItem.Enabled = selected;
                deleteMultipleFilesToolStripMenuItem.Enabled = false;
                moveToolStripMenuItem.Enabled = selected;
                makeDirectoryToolStripMenuItem.Enabled = true;
                uploadFileToolStripMenuItem.Enabled = connected && selected;
                uploadUniqueFileToolStripMenuItem.Enabled = connected && isFile;
                downloadToolStripMenuItem.Enabled = false;
                viewToolStripMenuItem.Enabled = isFile;
                refreshToolStripMenuItem.Enabled = true;
                synchronizeToolStripMenuItem.Enabled = connected;
                propertiesToolStripMenuItem.Enabled = selected;

                resumeUploadToolStripMenuItem.Enabled = connected && (isFile || lvwLocal.SelectedItems.Count > 1);
                resumeDownloadToolStripMenuItem.Enabled = false;
                executeCommandToolStripMenuItem.Enabled = connected;
                calculateTotalSizeToolStripMenuItem.Enabled = false;
                getTimeDifferenceToolStripMenuItem.Enabled = connected;
            }
            else
            {
                renameToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
                deleteMultipleFilesToolStripMenuItem.Enabled = false;
                moveToolStripMenuItem.Enabled = false;
                makeDirectoryToolStripMenuItem.Enabled = false;
                uploadFileToolStripMenuItem.Enabled = false;
                uploadUniqueFileToolStripMenuItem.Enabled = false;
                downloadToolStripMenuItem.Enabled = false;
                viewToolStripMenuItem.Enabled = false;
                refreshToolStripMenuItem.Enabled = false;
                synchronizeToolStripMenuItem.Enabled = connected;
                propertiesToolStripMenuItem.Enabled = false;

                resumeUploadToolStripMenuItem.Enabled = false;
                resumeDownloadToolStripMenuItem.Enabled = false;
                executeCommandToolStripMenuItem.Enabled = false;
                calculateTotalSizeToolStripMenuItem.Enabled = false;
                getTimeDifferenceToolStripMenuItem.Enabled = connected;
            }

            tsbDelete.Enabled = deleteToolStripMenuItem.Enabled;
            tsbMove.Enabled = moveToolStripMenuItem.Enabled;
            tsbCreateDir.Enabled = makeDirectoryToolStripMenuItem.Enabled;
            tsbUpload.Enabled = uploadFileToolStripMenuItem.Enabled;
            tsbDownload.Enabled = downloadToolStripMenuItem.Enabled;
            tsbView.Enabled = viewToolStripMenuItem.Enabled;
            tsbRefresh.Enabled = refreshToolStripMenuItem.Enabled;
            tsbSynchronize.Enabled = synchronizeToolStripMenuItem.Enabled;
        }

        #endregion

        #region Context Menus

        private void lvwLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable/disable control buttons when the selected item on the local view control is changed.
            EnableButtons();
        }

        private void lvwRemote_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable/disable control buttons when the selected item on the remote view control is changed.
            EnableButtons();
        }

        #endregion

        #region Toolbar Buttons

        private void tsbLogin_Click(object sender, EventArgs e)
        {
            loginToolStripMenuItem_Click(sender, null);
        }

        private void tsbLogout_Click(object sender, EventArgs e)
        {
            loginToolStripMenuItem_Click(sender, null);
        }

        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            refreshToolStripMenuItem_Click(sender, null);
        }

        private void tsbSettings_Click(object sender, EventArgs e)
        {
            settingsToolStripMenuItem_Click(sender, null);
        }

        private void tsbCreateDir_Click(object sender, EventArgs e)
        {
            makeDirectoryToolStripMenuItem_Click(sender, null);
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            deleteToolStripMenuItem_Click(sender, null);
        }

        private void tsbMove_Click(object sender, EventArgs e)
        {
            moveToolStripMenuItem_Click(sender, null);
        }

        private void tsbDownload_Click(object sender, EventArgs e)
        {
            downloadToolStripMenuItem_Click(sender, null);
        }

        private void tsbUpload_Click(object sender, EventArgs e)
        {
            uploadFileToolStripMenuItem_Click(sender, null);
        }

        private void tsbView_Click(object sender, EventArgs e)
        {
            viewToolStripMenuItem_Click(sender, null);
        }

        private void tsbSynchronize_Click(object sender, EventArgs e)
        {
            synchronizeToolStripMenuItem_Click(sender, null);
        }

        #endregion

        #region Client

        private string _currentDirectory;
        private bool _aborting;

        private void btnAbort_Click(object sender, EventArgs e)
        {
            // Abort transferring.
            client.Cancel();
            // Set aborting state = true.
            _aborting = true;
            if (_multiThreadTransferStatistics != null)
                _multiThreadTransferStatistics.Threads.StopAll();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        void Disconnect()
        {
            _state = ConnectionState.Disconnecting;

            // Asynchronously disconnect. DisconnectCompleted event will be fired when the operation completes.
            client.DisconnectAsync();
        }

        void SetDisconnectedState()
        {
            keepAliveTimer.Enabled = false;
            keepAliveTimer.Stop();

            EnableProgress(false);
            
            loginToolStripMenuItem.Enabled = true; tsbLogin.Enabled = true; tsbLogin.Visible = true; tsbLogout.Visible = false;
            settingsToolStripMenuItem.Enabled = true; tsbSettings.Enabled = true;
            exitToolStripMenuItem.Enabled = true;
            loginToolStripMenuItem.Text = "&Connect...";
            txtRemoteDir.Enabled = false;
            lvwRemote.Items.Clear();
            Util.EnableCloseButton(this, true);

            Cursor = Cursors.Default;
            toolbarMain.Enabled = true;
            menuStrip.Enabled = true;
            _enabled = 0;

            _state = ConnectionState.NotConnected;

            EnableButtons();
        }

        /// <summary>
        /// Handles the client's DisconnectCompleted event.
        /// </summary>
        /// <param name="sender">The Ftp object.</param>
        /// <param name="e">The event arguments.</param>
        private void client_DisconnectCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Util.ShowError(e.Error);
            }

            SetDisconnectedState();
        }

        /// <summary>
        /// Handles the client's ConnectCompleted event.
        /// </summary>
        /// <param name="sender">The Ftp object.</param>
        /// <param name="e">The event arguments.</param>
        private void client_ConnectCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                string msg = e.Error.InnerException != null ? e.Error.InnerException.Message : e.Error.Message;
                int serverKind = 0;


                Util.ShowError(e.Error);
                Disconnect();
                return;
            }

            Login();
        }

        /// <summary>
        /// Handles the client's AuthenticateCompleted event.
        /// </summary>
        /// <param name="sender">The Ftp object.</param>
        /// <param name="e">The event arguments.</param>
        private void client_AuthenticateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Util.ShowError(e.Error);
                Disconnect();
                return;
            }

            ListDefaultDirectory();
        }

        /// <summary>
        /// Authenticates the user
        /// </summary>
        void Login()
        {
            if (_state == ConnectionState.Disconnecting)
            {
                Disconnect();
                return;
            }

            
            SftpAuthenticate(client);
        }

        #region Listing

        void ListDefaultDirectory()
        {
            if (_state == ConnectionState.Disconnecting)
            {
                Disconnect();
                return;
            }

            try
            {


                _state = ConnectionState.Ready;

                _currentDirectory = _loginSettings.RemoteDir;

                if (_currentDirectory.Length == 0) // Empty means default user's folder.
                {
                    // Get current directory.
                    _currentDirectory = client.GetCurrentDirectory();
                }
                else
                    client.SetCurrentDirectory(_currentDirectory);

                // Update the remote dir text box.
                txtRemoteDir.Text = _currentDirectory;

                loginToolStripMenuItem.Enabled = true; tsbLogin.Visible = false; tsbLogout.Visible = true;
                settingsToolStripMenuItem.Enabled = true; tsbSettings.Enabled = true;
                exitToolStripMenuItem.Enabled = true;
                loginToolStripMenuItem.Text = "&Disconnect";
            }
            catch (Exception exc)
            {
                Util.ShowError(exc);
                Disconnect();
                return;
            }

            // Enable the Close button.
            Util.EnableCloseButton(this, true);

            // Show remote files.
            RefreshRemoteList();

            if (_loginSettings.LocalDir.Trim().Length != 0)
            {
                txtLocalDir.Text = _loginSettings.LocalDir;
                RefreshLocalList();
            }
        }

        /// <summary>
        /// Refreshes the remote list view.
        /// </summary>
        void RefreshRemoteList()
        {
            // Disable the dialog.
            EnableDialog(false);

            // Asynchronously retrieve a list of remote files. ListDirectoryCompleted event will be fired when the operation completes.
            client.ListDirectoryAsync();
        }

        /// <summary>
        /// Checks whether the given path is a directory or a symlink.
        /// </summary>
        /// <param name="entry">The remote path.</param>
        /// <returns></returns>
        private int RemoteCheckLink(string entry)
        {
            // Assume it's a symlink.
            int iconIndex = SymlinkImageIndex;

            try
            {
                // Check to see whether the entry is linked to a directory.
                client.SetCurrentDirectory(entry);
            }
            catch
            {
                goto quit;
            }

            // No it's a directory.
            iconIndex = FolderLinkImageIndex;

            try
            {
                // Switch back to the working directory.
                client.SetCurrentDirectory(_currentDirectory);
            }
            catch
            {
                goto quit;
            }

        quit:

            return iconIndex;
        }

        private void ListFiles(FileInfoCollection files, bool rootDir)
        {
            ListViewItem item;

            // Clear the list view.
            lvwRemote.Items.Clear();

            // Add directories into the list first.
            foreach (AbstractFileInfo f in files)
            {
                if (f.IsDirectory)
                {
                    if (f.Name != "." && f.Name != "..")
                    {
                        item = lvwRemote.Items.Add(f.Name, _folderIconIndex);

                        item.SubItems.Add("");
                        item.SubItems.Add(f.LastWriteTime.ToString());

                        
                                    SftpAddItemPermissions(item, f);
                        
                        item.Tag = f;
                    }
                }
            }

            // And files after.
            foreach (AbstractFileInfo f2 in files)
            {
                if (f2.IsSymlink) // Add symlinks.
                {
                    item = lvwRemote.Items.Add(f2.Name, RemoteCheckLink(f2.SymlinkPath));
                    item.SubItems.Add("");
                    item.SubItems.Add(f2.LastWriteTime.ToString());

                    
                            SftpAddItemPermissions(item, f2);

                    item.Tag = f2;
                }
                else if (!f2.IsDirectory) //Add Files.
                {
                    item = lvwRemote.Items.Add(f2.Name, SetFileIcon(Path.Combine(txtLocalDir.Text, f2.Name)));
                    item.SubItems.Add(Util.FormatSize(f2.Length));
                    item.SubItems.Add(f2.LastWriteTime.ToString());

                    
                            SftpAddItemPermissions(item, f2);

                    item.Tag = f2;
                }
            }

            UpdateRemoteListViewSorter();

            if (!rootDir)
            {
                // Add Cdup list item.
                ListViewItem cdup = new ListViewItem("..", 0);
                lvwRemote.Items.Insert(0, cdup);
            }
        }

        void ShowRemoteFiles(FileInfoCollection files)
        {
            if (_state == ConnectionState.Disconnecting)
            {
                Disconnect();
                return;
            }

            EnableDialog(true);

            ListFiles(files, _currentDirectory.Length <= 1);
            if (lvwRemote.Items.Count > 0)
                lvwRemote.Items[0].Selected = true;
            txtRemoteDir.Enabled = true;

            if (_lastSelectedFileName != null)
            {
                ListViewItem fi = lvwRemote.FindItemWithText(_lastSelectedFileName);
                if (fi != null)
                {
                    lvwRemote.SelectedItems.Clear();
                    fi.Selected = true;
                }
                _lastSelectedFileName = null;
            }

            _lastRemoteDirectory = _currentDirectory;
        }

        string _lastSelectedFileName;

        /// <summary>
        /// Handles the client's GetFileListCompleted event.
        /// </summary>
        /// <param name="sender">The Ftp object.</param>
        /// <param name="e">The event arguments.</param>
        private void client_ListDirectoryCompleted(object sender, ExtendedAsyncCompletedEventArgs<FileInfoCollection> e)
        {
            if (e.Error != null)
            {
				// If we encounter a fatal error we need to return immediately.
                if (!HandleException(e.Error))
                    return;

                if (_lastRemoteDirectory != null)
                {
                    client.SetCurrentDirectory(_lastRemoteDirectory);
                    _currentDirectory = _lastRemoteDirectory;
                    _lastRemoteDirectory = null;
                }

                EnableDialog(true);            

                return;
            }

            ShowRemoteFiles(e.Result);
        }

        #endregion

        /// <summary>
        /// Handles the client's UploadUniqueCompleted event.
        /// </summary>
        /// <param name="sender">The Ftp object.</param>
        /// <param name="e">The event arguments.</param>
        private void client_UploadUniqueFileCompleted(object sender, ExtendedAsyncCompletedEventArgs<string> e)
        {
            if (e.Error != null)
            {
				// If we encounter a fatal error we need to return immediately.
                if (!HandleException(e.Error))
                    return;
            }

            EnableProgress(false);
            if (_state == ConnectionState.Disconnecting)
            {
                Disconnect();
                return;
            }

            RefreshRemoteList();
        }

        /// <summary>
        /// Renames the selected item.
        /// </summary>
        /// <param name="newname">The new remote path name.</param>
        /// <returns>true if successful; otherwise is false.</returns>
        private bool DoRemoteRename(string newname)
        {
            if (!Ready || lvwRemote.SelectedIndices.Count == 0)
                return false;

            // Attempts to rename the currently selected item to the new name.
            ListViewItem item = lvwRemote.SelectedItems[0];
            // Not a valid item?
            if (item == null || string.IsNullOrEmpty(newname))
                return false;

            try
            {
                // Disable the dialog.
                EnableDialog(false);
                client.Rename(FileSystemPath.Combine(_currentDirectory, item.Text), FileSystemPath.Combine(_currentDirectory, newname));
                // Change the image index of the selected item if the selected item not a folder or symlink.
                if (item.ImageIndex != _folderIconIndex && item.ImageIndex != FolderLinkImageIndex && item.ImageIndex != SymlinkImageIndex)
                    item.ImageIndex = _iconManager.AddFileIcon(newname);
            }
            catch (Exception exc)
            {
                HandleException(exc);
                return false;
            }
            finally
            {
                EnableDialog(true);
            }

            // Refresh the remote list.
            RefreshRemoteList();
            _lastSelectedFileName = newname;

            return true;
        }

        /// <summary>
        /// Changes current directory.
        /// </summary>
        /// <param name="dir">The path of the destination directory.</param>
        private void DoRemoteChangeDirectory(string dir)
        {
            // Busy?
            if (!Ready)
                return;

            // Disable dialog.
            EnableDialog(false);
            string oldDir = _currentDirectory;
            try
            {
                // Change directory.
                client.SetCurrentDirectory(dir);
                // Get the current directory.
                string newdir = client.GetCurrentDirectory();
                if (newdir != oldDir)
                {
                    // If the current directory has been changed, refresh the remote list view.
                    RefreshRemoteList();
                    // Update internal var and textbox's value as well.
                    _currentDirectory = newdir;
                    txtRemoteDir.Text = newdir;
                }
            }
            catch (Exception exc)
            {
                if (!HandleException(exc))
                    return;
                _currentDirectory = oldDir;
            }
            finally
            {
                // Enable dialog when finish.
                EnableDialog(true);
            }
        }

        #region Remove Files/Directories

        string _deleteFilesMask = "*.*";

        /// <summary>
        /// Deletes selected items.
        /// </summary>
        private void DoRemoteDeleteMultipleFiles()
        {
            if (!Ready)
                return;

            FileMask dlg = new FileMask(_deleteFilesMask, "Delete files that match");
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;
            _deleteFilesMask = dlg.Mask;

            List<AbstractFileInfo> files = new List<AbstractFileInfo>();
            SearchCondition condition = new NameSearchCondition(dlg.Mask);

            foreach (ListViewItem item in lvwRemote.Items)
            {
                AbstractFileInfo i = (AbstractFileInfo)item.Tag;

                if (i.Name != ".." && (i.IsDirectory || condition.Matches(i)))
                    files.Add(i);
            }

            DoRemoteDelete(files, condition);
        }

        /// <summary>
        /// Deletes selected items.
        /// </summary>
        void DoRemoteDelete(List<AbstractFileInfo> list, SearchCondition condition)
        {
            if (!Ready)
                return;
				
			if (list == null)
                list = BuildFileList(lvwRemote.SelectedItems);

            // Enable progress bar, Abort button, and disable other controls.
            EnableProgress(true);
            btnAbort.Enabled = true;            

            int totalProcessingItems = list.Count;

            // Delete one item?
			if (totalProcessingItems == 1)
			{
				AbstractFileInfo item = list[0];
				if (item.IsDirectory) // is a folder
				{
					// User really wants to do that?
                    if (
                        MessageBox.Show(
                            string.Format(MessageDeleteFolder, item.Name),
                            "Ftp Client Demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        System.Windows.Forms.DialogResult.No)
                    {
                        EnableProgress(false);
                        return;
                    }
				}
				else if (item.IsFile)
				{
					// User really wants to do that?
                    if (
                        MessageBox.Show(
                            string.Format(MessageDeleteFile, item.Name),
                            "Ftp Client Demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        System.Windows.Forms.DialogResult.No)
                    {
                        EnableProgress(false);
                        return;
                    }
				}
			}
			else
			{
				// Delete multiple files/folders.
				if (MessageBox.Show(
							string.Format(MessageDeleteItems, totalProcessingItems),
							"Ftp Client Demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
						System.Windows.Forms.DialogResult.No)
					return;
			}

            client.DeleteFilesAsync(list, RecursionMode.Recursive, condition == null, condition);
        }

        /// <summary>
        /// Handles the client's DeleteFilesCompleted event.
        /// </summary>
        /// <param name="sender">The client object.</param>
        /// <param name="e">The event arguments.</param>
        void client_DeleteFilesCompleted(object sender, ExtendedAsyncCompletedEventArgs<FileSystemTransferStatistics> e)
        {
            if (e.Error != null)
            {
				// If we encounter a fatal error we need to return immediately.                
                if (!HandleException(e.Error))
                    return;
            }

            EnableProgress(false);
            RefreshRemoteList();
        }

        #endregion

        #region View

        const string TempDirName = "\\SftpClientDemo";

        /// <summary>
        /// Returns a temporary file name.
        /// </summary>
        /// <param name="fileName">The file name that will be used for generating temp file name.</param>
        static string GetTempFileName(string fileName)
        {
            string tempFileName;
            string tempDirPath;

            FileSystemPath.SplitPath(Path.GetTempFileName(), FileSystemPath.DefaultDirectorySeparators, out tempDirPath, out tempFileName);
            tempDirPath += TempDirName;
            if (!Directory.Exists(tempDirPath))
                Directory.CreateDirectory(tempDirPath);

            return tempDirPath + "\\" + Path.GetFileName(fileName) + "_" + tempFileName;
        }

        /// <summary>
        /// Deletes the temporary folder.
        /// </summary>
        static void DeleteTempFolder()
        {
            Directory.Delete(Path.GetTempPath() + TempDirName, true);
        }

        /// <summary>
        /// Downloads the selected item and open notepad.exe for viewing the newly downloaded file.
        /// </summary>
        /// <param name="item">The selected item.</param>
        void DoRemoteView(ListViewItem item)
        {
            // Busy?
            if (!Ready)
                return;

            if (item.ImageIndex != UpFolderImageIndex && item.ImageIndex != FolderLinkImageIndex && item.ImageIndex != _folderIconIndex) // Not a folder or a symlink
            {
                string tempFile = GetTempFileName(item.Text);

                EnableProgress(true);

                client.DownloadFileCompleted += client_GetFileForViewingCompleted;

                // Asynchronously download the remote file. DownloadFileCompleted event will be fired when the operation completes.   
                client.DownloadFileAsync(item.Text, tempFile, tempFile);
            }
        }

        /// <summary>
        /// Handles the client's DownloadFileCompleted event.
        /// </summary>
        /// <param name="sender">The Ftp object.</param>
        /// <param name="e">The event arguments.</param>
        private void client_GetFileForViewingCompleted(object sender, ExtendedAsyncCompletedEventArgs<long> e)
        {
            bool aborted = false;

            if (e.Error != null)
            {
				// If we encounter a fatal error we need to return immediately.
                if (!HandleException(e.Error))
                    return;

                FileSystemException fe = e.Error as FileSystemException;
                if (fe != null && fe.Status == FileSystemExceptionStatus.OperationCancelled)
                    aborted = true;
            }

            EnableProgress(false);

            if (_state == ConnectionState.Disconnecting)
            {
                Disconnect();
                return;
            }

            // Detach the event handler.
            client.DownloadFileCompleted -= client_GetFileForViewingCompleted;

            if (!aborted)
            {
                // temporary file name is saved in the async state object.
                string tempFile = (string)e.UserState;

                try
                {
                    Process.Start("Notepad.exe", tempFile);
                }
                catch (Exception exc)
                {
                    WriteLine("Error:" + exc.Message, Color.Red);
                }
            }
        }

        #endregion

        List<AbstractFileInfo> BuildFileList(IList items)
        {
            List<AbstractFileInfo> files = new List<AbstractFileInfo>();
            foreach (ListViewItem item in items)
            {
                if (item.ImageIndex != UpFolderImageIndex)
                    files.Add((AbstractFileInfo)item.Tag);
            }
            if (files.Count == 0)
                return null;

            return files;
        }

        #region Move

        private string _moveToFolder;
        /// <summary>
        /// Moves one or more items to another remote folder.
        /// </summary>
        void DoRemoteMove()
        {
            if (!Ready)
                return;

            List<AbstractFileInfo> fileList = BuildFileList(lvwRemote.SelectedItems);
            if (fileList == null)
                return;

            if (_moveToFolder == null)
                _moveToFolder = _currentDirectory;

            MoveToRemoteFolder dlg = new MoveToRemoteFolder(_moveToFolder);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.RemoteDir == _currentDirectory)
                {
                    MessageBox.Show(Messages.InvalidSourceDestDir, Messages.MessageTitle,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                    return;
                }

                _moveToFolder = dlg.RemoteDir;


                // Enable progress bar, Abort button, and disable other controls.
                EnableProgress(true);

                TransferOptions opt = new TransferOptions(true, RecursionMode.Recursive, true,
                                                          string.IsNullOrEmpty(dlg.FileMasks)
                                                              ? (SearchCondition) null
                                                              : new NameSearchCondition(dlg.FileMasks),
                                                          FileExistsResolveAction.Confirm,
                                                          SymlinksResolveAction.Confirm);

                // Asynchronously Move files. MoveFilesCompleted event will be fired when the operation completes.
                client.MoveFilesAsync(_currentDirectory, fileList, _moveToFolder, opt);
            }
        }

        void client_MoveFilesCompleted(object sender, ExtendedAsyncCompletedEventArgs<FileSystemTransferStatistics> e)
        {
            if (e.Error != null)
            {
				// If we encounter a fatal error we need to return immediately.
                if (!HandleException(e.Error))
                    return;
            }

            RefreshRemoteList();

            EnableProgress(false);
        }

        #endregion

        #region Make Dir

        /// <summary>
        /// Makes a new remote directory.
        /// </summary>
        private void DoRemoteMakeDir()
        {
            if (!Ready)
                return;

            FolderNamePrompt dlg = new FolderNamePrompt();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                EnableDialog(false);

                try
                {
                    // Create a new directory.
                    client.CreateDirectory(dlg.FolderName);

                    lvwRemote.Items.Add(dlg.FolderName, _folderIconIndex);
                }
                catch (Exception exc)
                {
                    if (!HandleException(exc))
                        return;
                }
                finally
                {
                    // Refresh the remote list view.
                    RefreshRemoteList();
                    EnableDialog(true);
                }
            }
        }

        #endregion

        #region Remote Property

        /// <summary>
        /// Shows the properties dialog to change file's permissions.
        /// </summary>
        /// <param name="item">The selected list view item.</param>
        void DoRemoteProperties(ListViewItem item)
        {
            string permissions = item.SubItems[3].Text;
            if (permissions.Length < 3)
            {
                MessageBox.Show("Remote file system does not support this operation", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            
            SftpShowItemProperties(item);            
        }

        #endregion

        #region Synchronize

        private bool _restore;
        /// <summary>
        /// Synchronizes local and remote folders.
        /// </summary>
        void DoSynchronize()
        {
            SynchronizeFolders dlg = new SynchronizeFolders(lvwLocal.Focused, _loginSettings.SyncRecursive, _loginSettings.SyncDateTime, _loginSettings.SyncMethod, _loginSettings.SyncResumability, _loginSettings.SyncSearchPattern);

            if (dlg.ShowDialog() != DialogResult.OK) return;
            if (MessageBox.Show(
                    string.Format(
                            Messages.SyncConfirm,
                            dlg.RemoteIsMaster ? txtLocalDir.Text : txtRemoteDir.Text,
                            dlg.RemoteIsMaster ? txtRemoteDir.Text : txtLocalDir.Text
                            ),
                        Messages.MessageTitle,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                EnableProgress(true);

                _loginSettings.SyncRecursive = dlg.Recursive;
                _loginSettings.SyncSearchPattern = dlg.SearchPattern;
                _loginSettings.SyncDateTime = dlg.SyncDateTime;
                _loginSettings.SyncMethod = dlg.ComparisonMethod;
                _loginSettings.SyncResumability = dlg.CheckForResumability;

                SyncOptions opt = new SyncOptions();
                opt.Recursive = _loginSettings.SyncRecursive;
                opt.SearchCondition = new NameSearchCondition(_loginSettings.SyncSearchPattern);

                _restore = client.RestoreFileProperties;
                client.RestoreFileProperties = _loginSettings.SyncDateTime;

                switch (_loginSettings.SyncMethod)
                {
                    case 0:
                        opt.Comparer = FileComparers.FileLastWriteTimeComparer;
                        break;

                    case 1:
                        opt.Comparer = _loginSettings.SyncResumability ? FileComparers.FileContentComparerWithResumabilityCheck : FileComparers.FileContentComparer;
                        break;

                    case 2:
                        opt.Comparer = FileComparers.FileNameComparer;
                        break;
                }

                // Asynchronously synchronize folders. SynchronizeCompleted event will be fired when the operation completes.
                client.SynchronizeAsync(txtRemoteDir.Text, txtLocalDir.Text, dlg.RemoteIsMaster, opt, dlg.RemoteIsMaster);
            }
        }

        /// <summary>
        /// Handles the client's SynchronizeCompleted event.
        /// </summary>
        /// <param name="sender">The Ftp object.</param>
        /// <param name="e">The event arguments.</param>
        private void client_SynchronizeCompleted(object sender, AsyncCompletedEventArgs e)
        {
            client.RestoreFileProperties = _restore;

            bool remoteIsMaster = (bool)e.UserState;
            if (e.Error != null)
            {
				// If we encounter a fatal error we need to return immediately.
                if (!HandleException(e.Error))
                    return;
            }

            if (remoteIsMaster)
                RefreshLocalList();
            else
                RefreshRemoteList();
            EnableProgress(false);
        }

        #endregion

        /// <summary>
        /// Determines whether the exception indicates that the connection should be reconnected.
        /// </summary>
        /// <param name="exc"></param>
        /// <returns>true if the caller can continue; otherwise is false.</returns>
        private bool HandleException(Exception exc)
        {
            return HandleException(exc, true);
        }

        // returns true if the caller can continue; otherwise is false.
        private bool HandleException(Exception exc, bool showError)
        {
            while (exc.InnerException != null)
                exc = exc.InnerException;

            if (showError)
                Util.ShowError(exc);

			// Check to see if the error is fatal.
			// Fatal error may be: 
			//  + Connection closed by the server
			//  + Timeout
			//  + Server goes offline
			//  + Socket error
			// When we encounter a fatal error, we need to reconnect to the server.
			// The client automatically reconnects to the server if the ReconnectionMaxRetries property is greater than 0.
            if (client.IsFatalError(exc))
            {
                SetDisconnectedState();
                return false;
            }

            return true;
        }

        private void client_TransferConfirm(object sender, TransferConfirmEventArgs e)
        {
            if (_multiThreadTransfer == 0)
                _fileOpForm.Show(this, e, (FileSystem)sender);
            else
            {
                lock (_transferConfirmFormSync)
                {
                    Invoke(new TransferConfirmEventHandler(TransferConfirm), sender, e);
                }
            }
        }

        object _transferConfirmFormSync = new object();
        void TransferConfirm(object sender, TransferConfirmEventArgs e)
        {
            _fileOpForm.Show(this, e, (FileSystem)sender);
        }

        private void client_Progress(object sender, FileSystemProgressEventArgs e)
        {
            if (_multiThreadTransfer == 0)
                Progress(sender, e);
            else
                Invoke(new FileSystemProgressEventHandler(Progress), sender, e);
        }

        void Progress(object sender, FileSystemProgressEventArgs e)
        {
            switch (e.State)
            {
                case TransferState.DeletingDirectory:
                    // It's about to delete a directory. To skip deleting this directory, simply set e.Skip = true.
                    status.Text = string.Format("Deleting directory {0}...", e.SourceFileSystem.GetFileName(e.SourcePath));
                    Application.DoEvents();
                    return;

                case TransferState.DeletingFile:
                    // It's about to delete a file. To skip deleting this file, simply set e.Skip = true.
                    status.Text = string.Format("Deleting file {0}...", e.SourceFileSystem.GetFileName(e.SourcePath));
                    Application.DoEvents();
                    return;

                case TransferState.BuildingDirectoryStructure:
                    // It informs us that the directory structure has been prepared for the multiple file transfer.
                    status.Text = "Building directory structure...";
                    Application.DoEvents();
                    break;

                #region Comparing File Events

                case TransferState.StartComparingFile:
                    // Source file and destination file are about to be compared.
                    // To skip comparing these files, simply set e.Skip = true.
                    // To override the comparison result, set the e.ComparionResult property.
                    status.Text = string.Format("Comparing file {0}...", System.IO.Path.GetFileName(e.SourcePath));
                    progressBar.Value = (int)e.Percentage;
                    progressBarTotal.Value = (int)e.TotalPercentage;
                    break;

                case TransferState.Comparing:
                    // Source file and destination file are being compared.
#if SHOWSPEED
                    status.Text =
                            string.Format("Comparing file {0}... {1} {2} remaining",
                            System.IO.Path.GetFileName(e.SourcePath), e.BytesPerSecond > 0 ? (Util.FormatSize(e.BytesPerSecond) + "/sec"): null,
                                          e.RemainingTime);
                    progressBar.Value = (int)e.Percentage;
                    progressBarTotal.Value = (int)e.TotalPercentage;
#endif
                    break;

                case TransferState.FileCompared:
                    // Source file and destination file have been compared.
                    // Comparison result is saved in the e.ComparisonResult property.
                    progressBar.Value = (int)e.Percentage;
                    progressBarTotal.Value = (int)e.TotalPercentage;
                    break;

                #endregion

                #region Uploading File Events

                case TransferState.StartUploadingFile:
                    // Source file (local file) is about to be uploaded. Destination file is the remote file.
                    // To skip uploading this file, simply set e.Skip = true.
                    status.Text = string.Format("Uploading file {0}...", System.IO.Path.GetFileName(e.SourcePath));
                    progressBar.Value = (int)e.Percentage;
                    progressBarTotal.Value = (int)e.TotalPercentage;
                    break;

                case TransferState.Uploading:
                    // Source file is being uploaded to the remote server.
#if SHOWSPEED

                    status.Text =
                            string.Format("Uploading file {0}... {1} {2} remaining",
                            System.IO.Path.GetFileName(e.SourcePath), e.BytesPerSecond > 0 ? (Util.FormatSize(e.BytesPerSecond) + "/sec") : null,
                                          e.RemainingTime);
                    progressBar.Value = (int)e.Percentage;
                    progressBarTotal.Value = (int)e.TotalPercentage;

#endif
                    break;

                case TransferState.FileUploaded:
                    // Source file has been uploaded.
                    progressBar.Value = (int)e.Percentage;
                    progressBarTotal.Value = (int)e.TotalPercentage;
                    break;

                #endregion

                #region Downloading File Events

                case TransferState.StartDownloadingFile:
                    // Source file (remote file) is about to be downloaded.
                    // To skip uploading this file, simply set e.Skip = true.
                    status.Text = string.Format("Downloading file {0}...", System.IO.Path.GetFileName(e.SourcePath));
                    progressBar.Value = (int)e.Percentage;
                    progressBarTotal.Value = (int)e.TotalPercentage;
                    break;

                case TransferState.Downloading:
                    // Source file is being downloaded to the local disk.
#if SHOWSPEED
                    status.Text =
                            string.Format("Downloading file {0}... {1} {2} remaining",
                                          System.IO.Path.GetFileName(e.SourcePath), Util.FormatSize(e.BytesPerSecond),
                                          e.RemainingTime);
                    progressBar.Value = (int)e.Percentage;
                    progressBarTotal.Value = (int)e.TotalPercentage;
#endif
                    break;

                case TransferState.FileDownloaded:
                    // Remote file has been downloaded.
                    progressBar.Value = (int)e.Percentage;
                    progressBarTotal.Value = (int)e.TotalPercentage;
                    break;

                case TransferState.MultiFileOperationCompleted:
                    if (_multiThreadTransfer != 0)
                    {
                        EnableProgress(false);

                        if (_multiThreadTransfer == 1) // Transfer direction is Disk -> FTP
                        {
                            ShowStatistics(e.TransferStatistics, "upload");
                            RefreshRemoteList(); // Then refresh the remote list.
                        }
                        else
                        {
                            ShowStatistics(e.TransferStatistics, "download");
                            RefreshLocalList(); // Otherwise refresh the local list.
                        }
                        _multiThreadTransfer = 0;
                        _multiThreadTransferStatistics = null;
                    }
                    break;

                #endregion
            }      
        }

        #region Keep Alive

        /// <summary>
        /// Handles the timer's Tick event.
        /// </summary>
        /// <param name="sender">The timer object.</param>
        /// <param name="e">The event arguments.</param>
        private void keepAliveTimer_Tick(object sender, EventArgs e)
        {
            if (_state == ConnectionState.Ready)
            {
                try
                {
                    client.KeepAlive();
                }
                catch (Exception exc)
                {
                    HandleException(exc, false);
                }
            }
        }

        #endregion



        #endregion

        #region Local List

        private int _lastLocalColumnToSort; // last sort action on this column.
        private SortOrder _lastLocalSortOrder = SortOrder.Ascending; // last sort order.

        private const string MessageDeleteFolder = "Are you sure you want to delete entire folder '{0}'?";
        private const string MessageDeleteFile = "Are you sure you want to delete file '{0}'?";
        private const string MessageDeleteItems = "Are you sure you want to delete {0} items?";

        #region Local File List

        /// <summary>
        /// Refreshes the local list view.
        /// </summary>
        private void RefreshLocalList()
        {
            try
            {
                ListViewItem item;

                // Create the list.
                lvwLocal.Items.Clear();

                DirectoryInfo directory = new DirectoryInfo(txtLocalDir.Text);
                FileInfoCollection list = DiskFileSystem.Default.ListDirectory(directory.FullName);

                foreach (AbstractFileInfo info in list)
                {
                    item = lvwLocal.Items.Add(info.Name, info.IsDirectory ? _folderIconIndex : SetFileIcon(info.FullName));
                    item.SubItems.Add(info.IsFile ? Util.FormatSize(info.Length) : "");
                    item.SubItems.Add(info.LastWriteTime.ToString());

                    item.Tag = info;
                }

                UpdateLocalListViewSorter();

                if (directory.FullName.Length > 3) // Not root dir?
                {
                    // Add Cdup list item.
                    ListViewItem cdup = new ListViewItem("..", 0);
                    lvwLocal.Items.Insert(0, cdup);
                }

                // Update local dir textbox's value.
                txtLocalDir.Text = directory.FullName;
                if (lvwLocal.Items.Count > 0)
                    lvwLocal.Items[0].Selected = true;
            }
            catch (Exception exc)
            {
                Util.ShowError(exc);
                // Create the list.
                lvwLocal.Items.Clear();
            }
        }

        /// <summary>
        /// Starts notepad.exe for viewing the selected file.
        /// </summary>
        /// <param name="item">The selected item.</param>
        private void DoLocalView(ListViewItem item)
        {
            try
            {
                // If the selected item is not a folder or a symlink?
                if (item.ImageIndex != UpFolderImageIndex && item.ImageIndex != _folderIconIndex)
                {
                    // Start notepad.exe.
                    Process.Start("Notepad.exe", GetItemFullName(item));
                }
            }
            catch (Exception exc)
            {
                Util.ShowError(exc);
            }
        }

        /// <summary>
        /// Changes local directory.
        /// </summary>
        /// <param name="dir">The new target path.</param>
        private void DoLocalChangeDirectory(string dir)
        {
            dir = Path.Combine(txtLocalDir.Text, dir);
            txtLocalDir.Text = dir;
            RefreshLocalList();
        }

        /// <summary>
        /// Deletes local files and/or folders.
        /// </summary>
        /// <returns>true if success; otherwise is false.</returns>
        public bool DoLocalDelete()
        {
            EnableProgress(true);

            if (lvwLocal.SelectedItems.Count == 1)
            {
                ListViewItem item = lvwLocal.SelectedItems[0];
                if (item.ImageIndex > UpFolderImageIndex)
                    if (item.ImageIndex == _folderIconIndex) // is a folder
                    {
                        if (
                            MessageBox.Show(
                                string.Format(MessageDeleteFolder, item.SubItems[0].Text),
                                "Ftp Client Demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                            System.Windows.Forms.DialogResult.Yes)
                        {
                            try
                            {
                                System.IO.Directory.Delete(GetItemFullName(item), true);
                                lvwLocal.Items.Remove(item);
                            }
                            catch (Exception exc)
                            {
                                Util.ShowError(exc);
                            }
                        }
                        else
                        {
                            EnableProgress(false);
                            return false;
                        }
                    }
                    else if (item.ImageIndex > UpFolderImageIndex)
                    {
                        if (
                            MessageBox.Show(
                                string.Format(MessageDeleteFile, item.SubItems[0].Text),
                                "Ftp Client Demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                            System.Windows.Forms.DialogResult.Yes)
                        {
                            try
                            {
                                System.IO.File.Delete(GetItemFullName(item));
                                lvwLocal.Items.Remove(item);
                            }
                            catch (Exception exc)
                            {
                                Util.ShowError(exc);
                            }
                        }
                        else
                        {
                            EnableProgress(false);
                            return false;
                        }
                    }
            }
            else
            {
                // Delete multiple files/folders.
                if (MessageBox.Show(
                            string.Format(MessageDeleteItems, lvwLocal.SelectedItems.Count),
                            "Ftp Client Demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        foreach (ListViewItem item in lvwLocal.SelectedItems)
                        {
                            if (_aborting)
                                break;

                            if (item.ImageIndex == _folderIconIndex) // Folder
                                System.IO.Directory.Delete(GetItemFullName(item), true);
                            else if (item.ImageIndex > UpFolderImageIndex)
                                System.IO.File.Delete(GetItemFullName(item));
                        }
                    }
                    catch (Exception exc)
                    {
                        Util.ShowError(exc);
                    }

                    RefreshLocalList();
                }
                else
                {
                    EnableProgress(false);
                    return false;
                }
            }

            EnableProgress(false);

            return true;
        }

        /// <summary>
        /// Resumes upload the selected item.
        /// </summary>
        private void DoLocalResumeUpload()
        {
            DoLocalResumeUpload(lvwLocal.SelectedItems);
        }

        /// <summary>
        /// Resumes upload a list of list view item.
        /// </summary>
        /// <param name="items">The list of list view item.</param>
        private void DoLocalResumeUpload(IList items)
        {
            try
            {
                EnableProgress(true);
                foreach (ListViewItem item in items)
                {
                    if (_aborting)
                        break;

                    string localFile = Path.Combine(txtLocalDir.Text, item.Text);
                    //string remoteFile = GetFullPath(item.Text);

                    if (item.ImageIndex != _folderIconIndex && item.ImageIndex != UpFolderImageIndex)
                    {
                        // Reset the progress bar's value.
                        progressBar.Value = 0;
                        // Upload a single file.
                        long result = client.ResumeUploadFile(localFile, item.Text);

                        if (items.Count == 1)
                        {
                            if (result == -1)
                                MessageBox.Show("Remote file size is greater than the local file size", "Ftp Demo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            else if (result == 0)
                                MessageBox.Show("Remote file size is equal to the local file size", "Ftp Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                if (!HandleException(exc))
                    return;
            }

            EnableProgress(false);
            RefreshRemoteList();
        }

        /// <summary>
        /// Uploads a list of list view item.
        /// </summary>
        /// <param name="items">The list of list view item.</param>
        private void DoLocalUpload(IList items)
        {
            if (_ftpSettings.AdvancedTransfer)
                DoLocalUploadAdvanced(items);
            else
                DoLocalUploadBasic(items);
        }

        FileSystemTransferStatistics _multiThreadTransferStatistics;

        /// <summary>
        /// Uploads a list of list view item - Basic mode.
        /// </summary>
        /// <param name="items">The list of list view item.</param>
        private void DoLocalUploadBasic(IList items)
        {
            try
            {
                EnableProgress(true);
                foreach (ListViewItem item in items)
                {
                    if (_aborting)
                        break;

                    string localFile = Path.Combine(txtLocalDir.Text, item.Text);
                    //string remoteFile = GetFullPath(item.Text);

                    if (item.ImageIndex == _folderIconIndex) // Directory
                    {
                        TransferOptions opt = new TransferOptions(false, RecursionMode.Recursive, true, (SearchCondition)null, FileExistsResolveAction.Overwrite, SymlinksResolveAction.Skip);

                        client.UploadFiles(Path.Combine(txtLocalDir.Text, item.Text), FileSystemPath.Combine(_currentDirectory, item.Text), opt);
                    }
                    else if (item.ImageIndex > UpFolderImageIndex)
                    {
                        // Reset the progress bar's value.
                        progressBar.Value = 0;
                        // Upload a single file.
                        //client.UploadFile(localFile, remoteFile);
                        client.UploadFile(localFile, item.Text);
                    }
                }
            }
            catch (Exception exc)
            {
                if (!HandleException(exc))
                    return;
            }

            EnableProgress(false);
            RefreshRemoteList();
        }

        void DoLocalUploadAdvanced(IList items)
        {
            List<AbstractFileInfo> files = BuildFileList(items);

            EnableProgress(true);

            TransferOptions opt = new TransferOptions(true, RecursionMode.Recursive, true, (SearchCondition)null, FileExistsResolveAction.Confirm, SymlinksResolveAction.Confirm);

            if (_ftpSettings.Transfers > 1) // Are we using more than 1 thread for uploading? If so multi-thread method is used
            {
                try
                {
                    _multiThreadTransfer = 1;
                    _multiThreadTransferStatistics = client.UploadFiles(txtLocalDir.Text, files, _currentDirectory, opt, _ftpSettings.Transfers, false);
                }
                catch (Exception ex)
                {
                    EnableProgress(false);
                    Util.ShowError(ex);
                }
                return;
            }

            // Asynchronously upload files. UploadFilesCompleted event will be fired when the operation completes.
            client.UploadFilesAsync(txtLocalDir.Text, files, _currentDirectory, opt);
        }

        void ShowStatistics(FileSystemTransferStatistics s, string direction)
        {
            if (s != null && s.FilesTransferred > 0)
            {
                WriteLine(string.Format("{0} file(s) {4}ed in {3} second(s) - total size: {1} - {2} / second",
                s.FilesTransferred, Util.FormatSize(s.TotalBytes),
                Util.FormatSize(s.BytesPerSecond), s.ElapsedTime.TotalSeconds, direction),
                RichTextBoxTraceListener.TextColorInfo);
                WriteLine(string.Format("Started time: {0}, ended time: {1}", s.Started.ToLocalTime(), s.Ended.ToLocalTime()), RichTextBoxTraceListener.TextColorInfo);
            }
        }

        /// <summary>
        /// Handles the client's UploadFilesCompleted event.
        /// </summary>
        /// <param name="sender">The client object.</param>
        /// <param name="e">The event arguments.</param>
        void client_UploadFilesCompleted(object sender, ExtendedAsyncCompletedEventArgs<FileSystemTransferStatistics> e)
        {
            ShowStatistics(e.Result, "upload");

            if (e.Error != null)
            {
				// If we encounter a fatal error we need to return immediately.                
                if (!HandleException(e.Error))
                    return;
            }

            EnableProgress(false);
            RefreshRemoteList(); // Then refresh the remote list.
        }

        /// <summary>
        /// Uploads selected items in the local list view.
        /// </summary>
        private void DoLocalUpload()
        {
            DoLocalUpload(lvwLocal.SelectedItems);
        }



        /// <summary>
        /// Shows System Properties dialog.
        /// </summary>
        /// <param name="item">The selected item.</param>
        private void DoLocalProperties(ListViewItem item)
        {
            string localFile = GetItemFullName(item);
            // Call Shell API Show File Properties method.
            ShellAPI.ShowFileProperties(localFile, Handle);
        }

        /// <summary>
        /// Renames the selected local file.
        /// </summary>
        /// <param name="newName">The new file name.</param>
        /// <returns>true if successful; otherwise is false.</returns>
        public bool DoLocalRenameFile(string newName)
        {
            if (!string.IsNullOrEmpty(newName))
            {
                ListViewItem item = lvwLocal.SelectedItems[0];

                try
                {
                    string newPath = Path.Combine(txtLocalDir.Text, newName);
                    if (item.ImageIndex == _folderIconIndex)
                        Directory.Move(GetItemFullName(item), newPath);
                    else if (item.ImageIndex != UpFolderImageIndex) // Not up dir.
                    {
                        File.Move(GetItemFullName(item), newPath);
                        item.ImageIndex = _iconManager.AddFileIcon(newName);
                    }
                    //item.Tag = newPath;
                    SetItemFullName(item, newPath);

                    return true;
                }
                catch (Exception exc)
                {
                    Util.ShowError(exc);
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a new directory and update the local list view.
        /// </summary>
        private void DoLocalMakeDir()
        {
            const string common = "New Folder";
            string dirname = common;
            int n = 0;
            string fullPath = Path.Combine(txtLocalDir.Text, dirname);

            try
            {
                while (Directory.Exists(fullPath)) // While folder with unique name exists.
                {
                    n = n + 1;
                    string unique = "(" + n + ")";
                    dirname = common + unique;
                    fullPath = Path.Combine(txtLocalDir.Text, dirname);
                }

                // Try to create a new folder with unique identifier.
                Directory.CreateDirectory(fullPath);

                ListViewItem item = lvwLocal.Items.Add(dirname, _folderIconIndex);

                item.Tag = DiskFileSystem.Default.CreateFileInfo(fullPath, FileAttributes.Directory, 0, DateTime.Now);

                item.BeginEdit();
            }
            catch (Exception exc)
            {
                Util.ShowError(exc);
            }
        }

        /// <summary>
        /// Moves local files.
        /// </summary>
        private void DoLocalMove()
        {
            try
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.SelectedPath = txtLocalDir.Text;
                dlg.Description = "Select destination folder";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (ListViewItem item in lvwLocal.SelectedItems)
                    {
                        if (_aborting)
                            break;

                        if (item.ImageIndex == _folderIconIndex) // Folder
                            Directory.Move(GetItemFullName(item), dlg.SelectedPath + "\\" + item.SubItems[0].Text);
                        else if (item.ImageIndex > UpFolderImageIndex)
                            File.Move(GetItemFullName(item), dlg.SelectedPath + "\\" + item.SubItems[0].Text);

                        lvwLocal.Items.Remove(item);
                    }

                    RefreshLocalList();
                }
            }
            catch (Exception exc)
            {
                Util.ShowError(exc);
            }
        }

        private void DoLocalExpandSelection(bool expand)
        {
            FileMask dlg = new FileMask(_mask, expand);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _mask = dlg.Mask;
                System.Text.RegularExpressions.Regex rg = FileSystemPath.MaskToRegex(_mask, false);

                foreach (ListViewItem item in lvwLocal.Items)
                {
                    if (rg.Match(item.Text).Success)
                    {
                        item.Selected = expand;
                    }
                }
            }
        }

        #region Event Handlers

        /// <summary>
        /// Handles the LocalDirBrowse's Click event.
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
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtLocalDir.Text = dlg.SelectedPath;
                    RefreshLocalList();
                }
            }
            catch (Exception exc)
            {
                Util.ShowError(exc);
            }
        }

        private void txtLocalDir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Enter
            {
                DoLocalChangeDirectory(txtLocalDir.Text);
            }
        }

        private void lvwLocal_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            e.CancelEdit = !DoLocalRenameFile(e.Label);
        }

        private void lvwLocal_DoubleClick(object sender, EventArgs e)
        {
            if (lvwLocal.SelectedItems.Count == 0)
                return;

            if (lvwLocal.SelectedItems[0].ImageIndex == UpFolderImageIndex) // Arrow up
                // Move one level.
                DoLocalChangeDirectory("..");
            else if (lvwLocal.SelectedItems[0].ImageIndex == _folderIconIndex) // Folder
                // Change directory.
                DoLocalChangeDirectory(lvwLocal.SelectedItems[0].Text);
            else
            {
                if (_state != ConnectionState.Ready)
                {
                    MessageBox.Show("Connection has not been established", "Ftp Client Demo");
                    return;
                }

                // Upload a single file.
                DoLocalUpload();
            }
        }

        private void UpdateLocalListViewSorter()
        {
            switch (_lastLocalColumnToSort)
            {
                case 0:
                    lvwLocal.ListViewItemSorter = new ListViewItemNameComparer(_lastLocalSortOrder, _folderIconIndex, FolderLinkImageIndex);
                    break;
                case 1:
                    lvwLocal.ListViewItemSorter = new ListViewItemSizeComparer(_lastLocalSortOrder, _folderIconIndex, FolderLinkImageIndex);
                    break;
                case 2:
                    lvwLocal.ListViewItemSorter = new ListViewItemDateComparer(_lastLocalSortOrder, _folderIconIndex, FolderLinkImageIndex);
                    break;
            }

            lvwLocal.ListViewItemSorter = null;
        }

        private void lvwLocal_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (lvwLocal.Items.Count == 0)
                return;

            ListViewItem cdup = lvwLocal.Items[0].ImageIndex == UpFolderImageIndex ? lvwLocal.Items[0] : null;
            if (cdup != null)
                lvwLocal.Items.Remove(cdup);

            SortOrder sortOrder;
            if (_lastLocalColumnToSort == e.Column)
            {
                sortOrder = _lastLocalSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
                sortOrder = SortOrder.Ascending;

            _lastLocalColumnToSort = e.Column;
            _lastLocalSortOrder = sortOrder;

            UpdateLocalListViewSorter();

            lvwLocal.ListViewItemSorter = null;
            if (cdup != null)
                lvwLocal.Items.Insert(0, cdup);
        }

        /// <summary>
        /// Handles the local list view's DragDrop event.
        /// </summary>
        /// <param name="sender">The list view object.</param>
        /// <param name="e">The event arguments.</param>
        private void lvwLocal_DragDrop(object sender, DragEventArgs e)
        {
            if (DragAndDropListView.IsValidDragItem(e))
            {
                DragItemData data = (DragItemData)e.Data.GetData(typeof(DragItemData));
                if (data.ListView == lvwRemote)
                    // Download dropped items from the remote list view.
                    DoRemoteDownload(data.DragItems);
            }
        }

        private void lvwLocal_KeyDown(object sender, KeyEventArgs e)
        {
            if (lvwLocal.SelectedItems.Count == 0)
                return;

            ListViewItem lvi;

            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Back:
                    DoLocalChangeDirectory("..");
                    break;

                case System.Windows.Forms.Keys.Delete:
                    // Delete files/folders
                    DoLocalDelete();
                    break;

                case Keys.F2:
                    if (lvwLocal.SelectedItems.Count > 0 && lvwLocal.SelectedItems[0].ImageIndex > UpFolderImageIndex) // Not Up dir
                    {
                        lvwLocal.SelectedItems[0].BeginEdit();
                    }
                    break;

                case Keys.Enter:
                    if (lvwLocal.SelectedItems.Count > 0)
                    {
                        lvi = lvwLocal.SelectedItems[0];
                        if (lvi.ImageIndex == _folderIconIndex || lvi.ImageIndex == UpFolderImageIndex) // Is selected item a directory
                        {
                            DoLocalChangeDirectory(lvwLocal.SelectedItems[0].Text);
                        }
                        else
                        {
                            if (_state != ConnectionState.Ready)
                            {
                                MessageBox.Show("Connection has not been established", "Ftp Client Demo");
                                return;
                            }

                            DoLocalUpload();
                        }
                    }
                    break;

                case Keys.A:
                    if (e.Control)
                        for (int i = 0; i < lvwLocal.Items.Count; i++)
                            lvwLocal.Items[i].Selected = true;
                    break;
            }
        }

        void lcmRefresh_Click(object sender, System.EventArgs e)
        {
            RefreshLocalList();
        }

        void lcmView_Click(object sender, System.EventArgs e)
        {
            DoLocalView(lvwLocal.SelectedItems[0]);
        }

        void lcmMove_Click(object sender, System.EventArgs e)
        {
            DoLocalMove();
        }

        void lcmDelete_Click(object sender, System.EventArgs e)
        {
            DoLocalDelete();
        }

        void lcmMakeDir_Click(object sender, System.EventArgs e)
        {
            DoLocalMakeDir();
        }

        void lcmRename_Click(object sender, System.EventArgs e)
        {
            lvwLocal.SelectedItems[0].BeginEdit();
        }



        void lcmUpload_Click(object sender, System.EventArgs e)
        {
            DoLocalUpload();
        }

        void lcmSynchronize_Click(object sender, System.EventArgs e)
        {
            DoSynchronize();
        }

        void lcmProperties_Click(object sender, EventArgs e)
        {
            DoLocalProperties(lvwLocal.SelectedItems[0]);
        }

        private void lcmResumeUpload_Click(object sender, EventArgs e)
        {
            DoLocalResumeUpload();
        }

        private void lcmSelectGroup_Click(object sender, EventArgs e)
        {
            DoLocalExpandSelection(true);
        }

        private void lcmUnselectGroup_Click(object sender, EventArgs e)
        {
            DoLocalExpandSelection(false);
        }

        private void localContextMenu_Popup(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Enable/disable controls.
            bool connected = _state == ConnectionState.Ready && Ready;

            ListViewItem selectedItem;

            if (lvwLocal.SelectedItems.Count > 0 &&
                                         lvwLocal.SelectedItems[0].ImageIndex != UpFolderImageIndex)
                selectedItem = lvwLocal.SelectedItems[0];
            else
                selectedItem = null;

            bool selected = lvwLocal.SelectedItems.Count > 1 || selectedItem != null;
            bool isFile = selectedItem != null && selectedItem.ImageIndex != _folderIconIndex;

            lcmRename.Enabled = selectedItem != null;
            lcmDelete.Enabled = selected;
            lcmMove.Enabled = selected;
            lcmMakeDir.Enabled = true;
            lcmUpload.Enabled = connected && selected;
            lcmUploadUnique.Enabled = connected && isFile;
            lcmView.Enabled = isFile;
            lcmRefresh.Enabled = true;
            lcmSynchronize.Enabled = connected;
            lcmProperties.Enabled = selected;
            lcmResumeUpload.Enabled = connected && (isFile || lvwLocal.SelectedItems.Count > 1);
            lcmSelectGroup.Enabled = true;
            lcmUnselectGroup.Enabled = true;
        }

        private void lvwLocal_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (lvwLocal.Items[e.Item].ImageIndex == UpFolderImageIndex)
                e.CancelEdit = true;
        }

        #endregion

        #endregion

        #endregion

        #region Remote List

        private int _lastRemoteColumnToSort; // last sort action on this column.
        private SortOrder _lastRemoteSortOrder = SortOrder.Ascending; // last sort order.

        #region Remote List File

        /// <summary>
        /// Resume download remote files.
        /// </summary>
        /// <param name="items">The list of ListViewItem.</param>
        public void DoRemoteResumeDownload(IList items)
        {
            try
            {
                EnableProgress(true);
                foreach (ListViewItem item in items)
                {
                    if (_aborting)
                        break;

                    string localFile = Path.Combine(txtLocalDir.Text, item.Text);
                    //string remoteFile = GetFullPath(item.Text);

                    if (item.ImageIndex != _folderIconIndex && item.ImageIndex != UpFolderImageIndex && item.ImageIndex != FolderLinkImageIndex)
                    {
                        progressBar.Value = 0;
                        long result = client.ResumeDownloadFile(item.Text, localFile);
                        if (items.Count == 1)
                        {
                            if (result == -1)
                                MessageBox.Show("Remote file size is greater than the local file size", "Ftp Demo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            else if (result == 0)
                                MessageBox.Show("Remote file size is equal to the local file size", "Ftp Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                if (!HandleException(exc))
                    return;
            }

            EnableProgress(false);
            RefreshLocalList();
        }

        #region Download

        /// <summary>
        /// Downloads remote files.
        /// </summary>
        /// <param name="items">The list of ListViewItem.</param>
        public void DoRemoteDownload(IList items)
        {
            if (_ftpSettings.AdvancedTransfer)
                DoRemoteDownloadAdvanced(items);
            else
                DoRemoteDownloadBasic(items);
        }

        /// <summary>
        /// Downloads remote files.
        /// </summary>
        /// <param name="items">The list of ListViewItem.</param>
        public void DoRemoteDownloadBasic(IList items)
        {
            try
            {
                EnableProgress(true);
                foreach (ListViewItem item in items)
                {
                    if (_aborting)
                        break;

                    string localFile = Path.Combine(txtLocalDir.Text, item.Text);

                    if (item.ImageIndex == _folderIconIndex) // Directory
                    {
                        client.DownloadFiles(FileSystemPath.Combine(_currentDirectory, item.Text), Path.Combine(txtLocalDir.Text, item.Text));
                    }
                    else if (item.ImageIndex != UpFolderImageIndex && item.ImageIndex != FolderLinkImageIndex)
                    {
                        progressBar.Value = 0;
                        client.DownloadFile(item.Text, localFile);
                    }
                }
            }
            catch (Exception exc)
            {
                if (!HandleException(exc))
                    return;
            }

            EnableProgress(false);
            RefreshLocalList();
        }

        public void DoRemoteDownloadAdvanced(IList items)
        {
            List<AbstractFileInfo> fileList = BuildFileList(items);
            if (fileList == null)
                return;

            EnableProgress(true);

            TransferOptions opt = new TransferOptions(true, RecursionMode.Recursive, true, (SearchCondition)null, FileExistsResolveAction.Confirm, SymlinksResolveAction.Confirm);

            if (_ftpSettings.Transfers > 1) // Are we using more than 1 thread for uploading? If so multi-thread method is used
            {
                try
                {
                    _multiThreadTransfer = 2;
                    _multiThreadTransferStatistics = client.DownloadFiles(_currentDirectory, fileList, txtLocalDir.Text, opt, _ftpSettings.Transfers, false);
                }
                catch (Exception ex)
                {
                    EnableProgress(false);
                    Util.ShowError(ex);
                }
                return;
            }

            // Asynchronously download files. DownloadFilesCompleted event will be fired when the operation completes.
            client.DownloadFilesAsync(_currentDirectory, fileList, txtLocalDir.Text, opt);
        }

        /// <summary>
        /// Handles the client's DownloadFilesCompleted event.
        /// </summary>
        /// <param name="sender">The client object.</param>
        /// <param name="e">The event arguments.</param>
        void client_DownloadFilesCompleted(object sender, ExtendedAsyncCompletedEventArgs<FileSystemTransferStatistics> e)
        {
            ShowStatistics(e.Result, "download");

            if (e.Error != null)
            {
                // If we encounter a fatal error we need to return immediately.                                
				if (!HandleException(e.Error))
                    return;
            }

            EnableProgress(false);
            RefreshLocalList(); // Then refresh the local list.
        }

        private void DoRemoteDownload()
        {
            DoRemoteDownload(lvwRemote.SelectedItems);
        }

        private void DoRemoteExpandSelection(bool expand)
        {
            FileMask dlg = new FileMask(_mask, expand);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _mask = dlg.Mask;
                System.Text.RegularExpressions.Regex rg = FileSystemPath.MaskToRegex(_mask, false);

                foreach (ListViewItem item in lvwRemote.Items)
                {
                    if (rg.Match(item.Text).Success)
                    {
                        item.Selected = expand;
                    }
                }
            }
        }

        #endregion

        #region List View Event Handlers

        private void lvwRemote_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            e.CancelEdit = !(DoRemoteRename(e.Label));
        }

        private void UpdateRemoteListViewSorter()
        {
            switch (_lastRemoteColumnToSort)
            {
                case 0:
                    lvwRemote.ListViewItemSorter = new ListViewItemNameComparer(_lastRemoteSortOrder, _folderIconIndex, FolderLinkImageIndex);
                    break;
                case 1:
                    lvwRemote.ListViewItemSorter = new ListViewItemSizeComparer(_lastRemoteSortOrder, _folderIconIndex, FolderLinkImageIndex);
                    break;
                case 2:
                    lvwRemote.ListViewItemSorter = new ListViewItemDateComparer(_lastRemoteSortOrder, _folderIconIndex, FolderLinkImageIndex);
                    break;
                case 3:
                    lvwRemote.ListViewItemSorter = new ListViewItemPermissionsComparer(_lastRemoteSortOrder, _folderIconIndex, FolderLinkImageIndex);
                    break;
            }

            lvwRemote.ListViewItemSorter = null;
        }

        private void lvwRemote_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (lvwRemote.Items.Count == 0)
                return;

            ListViewItem cdup = lvwRemote.Items[0].ImageIndex == UpFolderImageIndex ? lvwRemote.Items[0] : null;
            if (cdup != null)
                lvwRemote.Items.Remove(cdup);

            SortOrder sortOrder;
            if (_lastRemoteColumnToSort == e.Column)
            {
                sortOrder = _lastRemoteSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
                sortOrder = SortOrder.Ascending;

            _lastRemoteColumnToSort = e.Column;
            _lastRemoteSortOrder = sortOrder;

            UpdateRemoteListViewSorter();

            lvwRemote.ListViewItemSorter = null;
            if (cdup != null)
                lvwRemote.Items.Insert(0, cdup);
        }

        private void lvwRemote_DoubleClick(object sender, EventArgs e)
        {
            if (_state != ConnectionState.Ready)
                return;

            if (lvwRemote.SelectedItems.Count == 0)
                return;

            if (lvwRemote.SelectedItems[0].ImageIndex == UpFolderImageIndex) // Arrow up
                DoRemoteChangeDirectory("..");
            else if (lvwRemote.SelectedItems[0].ImageIndex == _folderIconIndex) // Folder
                DoRemoteChangeDirectory(FileSystemPath.Combine(_currentDirectory, lvwRemote.SelectedItems[0].Text));
            else if (lvwRemote.SelectedItems[0].ImageIndex == FolderLinkImageIndex) // Folder Link
                DoRemoteChangeDirectory(GetItemFullName(lvwRemote.SelectedItems[0]));
            else
                DoRemoteDownload();
        }

        private void lvwRemote_KeyDown(object sender, KeyEventArgs e)
        {
            if (_state != ConnectionState.Ready)
                return;

            if (lvwRemote.SelectedItems.Count == 0)
                return;

            ListViewItem lvi;

            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Back:
                    DoRemoteChangeDirectory("..");
                    break;

                case System.Windows.Forms.Keys.Delete:
                    DoRemoteDelete(null, null);
                    break;

                case Keys.F2:
                    if (lvwRemote.SelectedItems.Count > 0 && lvwRemote.SelectedItems[0].ImageIndex > UpFolderImageIndex)
                    {
                        lvwRemote.SelectedItems[0].BeginEdit();
                    }
                    break;



                case Keys.Enter:
                    if (lvwRemote.SelectedItems.Count > 0)
                    {
                        lvi = lvwRemote.SelectedItems[0];
                        if (lvi.ImageIndex == _folderIconIndex || lvi.ImageIndex == UpFolderImageIndex || lvi.ImageIndex == FolderLinkImageIndex)
                        {
                            string dir = lvwRemote.SelectedItems[0].Text;
                            DoRemoteChangeDirectory(dir);
                        }
                        else
                        {
                            DoRemoteDownload();
                        }
                    }
                    break;

                case Keys.A:
                    if (e.Control)
                        for (int i = 0; i < lvwRemote.Items.Count; i++)
                            lvwRemote.Items[i].Selected = true;
                    break;
            }
        }

        private void txtRemoteDir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _state == ConnectionState.Ready) // Enter
            {
                DoRemoteChangeDirectory(txtRemoteDir.Text);
            }
        }

        private void lvwRemote_DragDrop(object sender, DragEventArgs e)
        {
            if (_state != ConnectionState.Ready)
                MessageBox.Show("Connection has not been established", "Ftp Client Demo");
            else if (DragAndDropListView.IsValidDragItem(e))
            {
                DragItemData data = (DragItemData)e.Data.GetData(typeof(DragItemData));
                if (data.ListView == lvwLocal)
                    DoLocalUpload(data.DragItems);
            }
        }

        #endregion

        #region Context Menu Event handlers

        private void mnuPopRename_Click(object sender, EventArgs e)
        {
            lvwRemote.SelectedItems[0].BeginEdit();
        }

        private void mnuPopMkdir_Click(object sender, EventArgs e)
        {
            DoRemoteMakeDir();
        }

        private void mnuPopDelete_Click(object sender, EventArgs e)
        {
            DoRemoteDelete(null, null);
        }

        private void mnuPopRetrieve_Click(object sender, EventArgs e)
        {
            DoRemoteDownload();
        }

        void mnuPopMove_Click(object sender, System.EventArgs e)
        {
            DoRemoteMove();
        }

        private void mnuPopView_Click(object sender, EventArgs e)
        {
            DoRemoteView(lvwRemote.SelectedItems[0]);
        }

        private void mnuPopRefresh_Click(object sender, EventArgs e)
        {
            RefreshRemoteList();
        }

        private void mnuSynchronize_Click(object sender, EventArgs e)
        {
            DoSynchronize();
        }



        private void mnuPopGetTimeDiff_Click(object sender, EventArgs e)
        {
            try
            {
                EnableProgress(true);

                // Get the time difference.
                TimeSpan ts = client.GetServerTimeDifference();

                MessageBox.Show(string.Format("The time difference between the client and server: {0}", ts), "Ftp Client Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exc)
            {
                HandleException(exc);
            }
            finally
            {
                EnableProgress(false);
            }
        }

        private void mnuPopResumeDownload_Click(object sender, EventArgs e)
        {
            DoRemoteResumeDownload(lvwRemote.SelectedItems);
        }

        private void mnuPopCalcTotalSize_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the time difference.
                long total = 0;
                foreach (ListViewItem item in lvwRemote.SelectedItems)
                {
                    if (item.ImageIndex == _folderIconIndex)
                        total += client.GetDirectorySize(GetItemFullName(item), true);
                    else
                        total += ((AbstractFileInfo)item.Tag).Length;
                }

                MessageBox.Show(string.Format("Total size: {0}", Util.FormatSize(total)), "Ftp Client Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exc)
            {
                HandleException(exc);
            }
        }

        private void mnuPopDeleteMultipleFiles_Click(object sender, EventArgs e)
        {
            DoRemoteDeleteMultipleFiles();
        }

        private void remoteContextMenu_Popup(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool connected = _state == ConnectionState.Ready && Ready;

            ListViewItem selectedItem;
            if (connected && lvwRemote.SelectedItems.Count > 0 &&
                                         lvwRemote.SelectedItems[0].ImageIndex != UpFolderImageIndex)
                selectedItem = lvwRemote.SelectedItems[0];
            else
                selectedItem = null;

            bool selected = connected && (lvwRemote.SelectedItems.Count > 1 || selectedItem != null);
            bool isFile = selectedItem != null && selectedItem.ImageIndex != _folderIconIndex && selectedItem.ImageIndex != FolderLinkImageIndex;

            mnuPopRename.Enabled = selectedItem != null;
            mnuPopDelete.Enabled = selected;
            mnuPopDeleteMultipleFiles.Enabled = selected;
            mnuPopMove.Enabled = selected;
            mnuPopMkdir.Enabled = connected;
            mnuPopRetrieve.Enabled = selected;
            mnuPopView.Enabled = isFile;
            mnuPopRefresh.Enabled = connected;
            mnuPopSynchronize.Enabled = connected;
            mnuPopProperties.Enabled = selectedItem != null;
            mnuPopCalcTotalSize.Enabled = selected;
            mnuPopExecuteCommand.Enabled = connected;
            mnuPopGetTimeDiff.Enabled = connected;
            mnuPopResumeDownload.Enabled = isFile || lvwRemote.SelectedItems.Count > 1;
            mnuPopSelectGroup.Enabled = connected;
            mnuPopUnselectGroup.Enabled = connected;
        }

        private void mnuPopProperties_Click(object sender, EventArgs e)
        {
            propertiesToolStripMenuItem_Click(null, null);
        }

        private void mnuPopSelectGroup_Click(object sender, EventArgs e)
        {
            DoRemoteExpandSelection(true);
        }

        private void mnuPopUnselectGroup_Click(object sender, EventArgs e)
        {
            DoRemoteExpandSelection(false);
        }

        private void lvwRemote_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (lvwRemote.Items[e.Item].ImageIndex == UpFolderImageIndex)
                e.CancelEdit = true;
        }

        #endregion

        #endregion

        #endregion
    }
}

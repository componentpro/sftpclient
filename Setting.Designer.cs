namespace SftpClient
{
    partial class Setting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkSmartPath = new System.Windows.Forms.CheckBox();
            this.chkChDirBeforeTransfer = new System.Windows.Forms.CheckBox();
            this.chkChDirBeforeListing = new System.Windows.Forms.CheckBox();
            this.chkAdvancedTransfer = new System.Windows.Forms.CheckBox();
            this.chkSendABOR = new System.Windows.Forms.CheckBox();
            this.chkSendSignals = new System.Windows.Forms.CheckBox();
            this.lblKAS = new System.Windows.Forms.Label();
            this.lblKeepAlive = new System.Windows.Forms.Label();
            this.txtKeepAliveInterval = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCompress = new System.Windows.Forms.CheckBox();
            this.rbtBinary = new System.Windows.Forms.RadioButton();
            this.rbtAscii = new System.Windows.Forms.RadioButton();
            this.tabControlExt = new System.Windows.Forms.TabControl();
            this.generalPage = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.numTransfers = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.txtOffsetSec = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtOffsetMin = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtOffsetHour = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtOffsetDay = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.chkRestoreFileLastWriteTime = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtUploadBuffer = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtConnectionRetries = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtReconnectDelay = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblProgressSlow = new System.Windows.Forms.Label();
            this.lblThrottle = new System.Windows.Forms.Label();
            this.lblProgressFast = new System.Windows.Forms.Label();
            this.txtThrottle = new System.Windows.Forms.TextBox();
            this.tbProgress = new System.Windows.Forms.TrackBar();
            this.lblKBPS = new System.Windows.Forms.Label();
            this.lblProgressUpdate = new System.Windows.Forms.Label();
            this.ftpPage = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.sftpPage = new System.Windows.Forms.TabPage();
            this.lblServer = new System.Windows.Forms.Label();
            this.cbxServerOs = new System.Windows.Forms.ComboBox();
            this.tabControlExt.SuspendLayout();
            this.generalPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTransfers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConnectionRetries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbProgress)).BeginInit();
            this.ftpPage.SuspendLayout();
            this.sftpPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkSmartPath
            // 
            this.chkSmartPath.BackColor = System.Drawing.SystemColors.Control;
            this.chkSmartPath.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkSmartPath.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSmartPath.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkSmartPath.Location = new System.Drawing.Point(11, 67);
            this.chkSmartPath.Name = "chkSmartPath";
            this.chkSmartPath.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkSmartPath.Size = new System.Drawing.Size(129, 22);
            this.chkSmartPath.TabIndex = 4;
            this.chkSmartPath.Text = "Smart Path Resolving";
            this.chkSmartPath.UseVisualStyleBackColor = false;
            // 
            // chkChDirBeforeTransfer
            // 
            this.chkChDirBeforeTransfer.BackColor = System.Drawing.SystemColors.Control;
            this.chkChDirBeforeTransfer.Checked = true;
            this.chkChDirBeforeTransfer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkChDirBeforeTransfer.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkChDirBeforeTransfer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkChDirBeforeTransfer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkChDirBeforeTransfer.Location = new System.Drawing.Point(11, 117);
            this.chkChDirBeforeTransfer.Name = "chkChDirBeforeTransfer";
            this.chkChDirBeforeTransfer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkChDirBeforeTransfer.Size = new System.Drawing.Size(287, 22);
            this.chkChDirBeforeTransfer.TabIndex = 7;
            this.chkChDirBeforeTransfer.Text = "Change Dir before transferring files (slow but reliable)";
            this.chkChDirBeforeTransfer.UseVisualStyleBackColor = false;
            // 
            // chkChDirBeforeListing
            // 
            this.chkChDirBeforeListing.BackColor = System.Drawing.SystemColors.Control;
            this.chkChDirBeforeListing.Checked = true;
            this.chkChDirBeforeListing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkChDirBeforeListing.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkChDirBeforeListing.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkChDirBeforeListing.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkChDirBeforeListing.Location = new System.Drawing.Point(11, 95);
            this.chkChDirBeforeListing.Name = "chkChDirBeforeListing";
            this.chkChDirBeforeListing.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkChDirBeforeListing.Size = new System.Drawing.Size(234, 22);
            this.chkChDirBeforeListing.TabIndex = 6;
            this.chkChDirBeforeListing.Text = "Change Dir before listing (slow but reliable)";
            this.chkChDirBeforeListing.UseVisualStyleBackColor = false;
            // 
            // chkAdvancedTransfer
            // 
            this.chkAdvancedTransfer.BackColor = System.Drawing.SystemColors.Control;
            this.chkAdvancedTransfer.Checked = true;
            this.chkAdvancedTransfer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAdvancedTransfer.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkAdvancedTransfer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAdvancedTransfer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkAdvancedTransfer.Location = new System.Drawing.Point(160, 210);
            this.chkAdvancedTransfer.Name = "chkAdvancedTransfer";
            this.chkAdvancedTransfer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkAdvancedTransfer.Size = new System.Drawing.Size(184, 22);
            this.chkAdvancedTransfer.TabIndex = 10;
            this.chkAdvancedTransfer.Text = "Advanced Multiple File Transfer";
            this.chkAdvancedTransfer.UseVisualStyleBackColor = false;
            this.chkAdvancedTransfer.CheckedChanged += new System.EventHandler(this.chkAdvancedTransfer_CheckedChanged);
            // 
            // chkSendABOR
            // 
            this.chkSendABOR.BackColor = System.Drawing.SystemColors.Control;
            this.chkSendABOR.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkSendABOR.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSendABOR.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkSendABOR.Location = new System.Drawing.Point(11, 31);
            this.chkSendABOR.Name = "chkSendABOR";
            this.chkSendABOR.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkSendABOR.Size = new System.Drawing.Size(257, 18);
            this.chkSendABOR.TabIndex = 2;
            this.chkSendABOR.Text = "Send ABOR command when aborting download";
            this.chkSendABOR.UseVisualStyleBackColor = false;
            // 
            // chkSendSignals
            // 
            this.chkSendSignals.BackColor = System.Drawing.SystemColors.Control;
            this.chkSendSignals.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkSendSignals.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSendSignals.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkSendSignals.Location = new System.Drawing.Point(11, 49);
            this.chkSendSignals.Name = "chkSendSignals";
            this.chkSendSignals.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkSendSignals.Size = new System.Drawing.Size(212, 20);
            this.chkSendSignals.TabIndex = 3;
            this.chkSendSignals.Text = "Send signals when aborting download";
            this.chkSendSignals.UseVisualStyleBackColor = false;
            // 
            // lblKAS
            // 
            this.lblKAS.BackColor = System.Drawing.SystemColors.Control;
            this.lblKAS.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblKAS.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKAS.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblKAS.Location = new System.Drawing.Point(156, 18);
            this.lblKAS.Name = "lblKAS";
            this.lblKAS.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblKAS.Size = new System.Drawing.Size(10, 14);
            this.lblKAS.TabIndex = 87;
            this.lblKAS.Text = "s";
            // 
            // lblKeepAlive
            // 
            this.lblKeepAlive.BackColor = System.Drawing.SystemColors.Control;
            this.lblKeepAlive.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblKeepAlive.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKeepAlive.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblKeepAlive.Location = new System.Drawing.Point(11, 12);
            this.lblKeepAlive.Name = "lblKeepAlive";
            this.lblKeepAlive.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblKeepAlive.Size = new System.Drawing.Size(70, 28);
            this.lblKeepAlive.TabIndex = 86;
            this.lblKeepAlive.Text = "Keep Alive Interval:";
            // 
            // txtKeepAliveInterval
            // 
            this.txtKeepAliveInterval.AcceptsReturn = true;
            this.txtKeepAliveInterval.BackColor = System.Drawing.SystemColors.Window;
            this.txtKeepAliveInterval.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtKeepAliveInterval.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKeepAliveInterval.Location = new System.Drawing.Point(85, 14);
            this.txtKeepAliveInterval.MaxLength = 3;
            this.txtKeepAliveInterval.Name = "txtKeepAliveInterval";
            this.txtKeepAliveInterval.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtKeepAliveInterval.Size = new System.Drawing.Size(62, 20);
            this.txtKeepAliveInterval.TabIndex = 1;
            this.txtKeepAliveInterval.Text = "60";
            this.txtKeepAliveInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(326, 17);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(10, 14);
            this.label3.TabIndex = 83;
            this.label3.Text = "s";
            // 
            // lblTimeout
            // 
            this.lblTimeout.BackColor = System.Drawing.SystemColors.Control;
            this.lblTimeout.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTimeout.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeout.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTimeout.Location = new System.Drawing.Point(209, 17);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTimeout.Size = new System.Drawing.Size(48, 14);
            this.lblTimeout.TabIndex = 82;
            this.lblTimeout.Text = "Timeout:";
            // 
            // txtTimeout
            // 
            this.txtTimeout.AcceptsReturn = true;
            this.txtTimeout.BackColor = System.Drawing.SystemColors.Window;
            this.txtTimeout.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTimeout.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimeout.Location = new System.Drawing.Point(286, 14);
            this.txtTimeout.MaxLength = 3;
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtTimeout.Size = new System.Drawing.Size(36, 20);
            this.txtTimeout.TabIndex = 2;
            this.txtTimeout.Text = "30";
            this.txtTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(252, 346);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 40;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(333, 346);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 41;
            this.btnCancel.Text = "Cancel";
            // 
            // chkCompress
            // 
            this.chkCompress.BackColor = System.Drawing.SystemColors.Control;
            this.chkCompress.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkCompress.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCompress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkCompress.Location = new System.Drawing.Point(11, 10);
            this.chkCompress.Name = "chkCompress";
            this.chkCompress.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkCompress.Size = new System.Drawing.Size(180, 17);
            this.chkCompress.TabIndex = 1;
            this.chkCompress.Text = "Enable Compression (MODE Z)";
            this.chkCompress.UseVisualStyleBackColor = false;
            // 
            // rbtBinary
            // 
            this.rbtBinary.Location = new System.Drawing.Point(15, 218);
            this.rbtBinary.Name = "rbtBinary";
            this.rbtBinary.Size = new System.Drawing.Size(96, 26);
            this.rbtBinary.TabIndex = 9;
            this.rbtBinary.Text = "Binary Transfer";
            // 
            // rbtAscii
            // 
            this.rbtAscii.Location = new System.Drawing.Point(15, 200);
            this.rbtAscii.Name = "rbtAscii";
            this.rbtAscii.Size = new System.Drawing.Size(94, 20);
            this.rbtAscii.TabIndex = 8;
            this.rbtAscii.Text = "ASCII Transfer";
            // 
            // tabControlExt
            // 
            this.tabControlExt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlExt.Controls.Add(this.generalPage);
                        this.tabControlExt.Controls.Add(this.sftpPage);
            this.tabControlExt.Location = new System.Drawing.Point(7, 6);
            this.tabControlExt.Name = "tabControlExt";
            this.tabControlExt.SelectedIndex = 0;
            this.tabControlExt.Size = new System.Drawing.Size(402, 333);
            this.tabControlExt.TabIndex = 114;
            // 
            // generalPage
            // 
            this.generalPage.Controls.Add(this.label17);
            this.generalPage.Controls.Add(this.numTransfers);
            this.generalPage.Controls.Add(this.label18);
            this.generalPage.Controls.Add(this.txtOffsetSec);
            this.generalPage.Controls.Add(this.label16);
            this.generalPage.Controls.Add(this.txtOffsetMin);
            this.generalPage.Controls.Add(this.label15);
            this.generalPage.Controls.Add(this.txtOffsetHour);
            this.generalPage.Controls.Add(this.label14);
            this.generalPage.Controls.Add(this.label12);
            this.generalPage.Controls.Add(this.txtOffsetDay);
            this.generalPage.Controls.Add(this.label13);
            this.generalPage.Controls.Add(this.chkRestoreFileLastWriteTime);
            this.generalPage.Controls.Add(this.label11);
            this.generalPage.Controls.Add(this.label9);
            this.generalPage.Controls.Add(this.txtUploadBuffer);
            this.generalPage.Controls.Add(this.label10);
            this.generalPage.Controls.Add(this.label8);
            this.generalPage.Controls.Add(this.txtConnectionRetries);
            this.generalPage.Controls.Add(this.label4);
            this.generalPage.Controls.Add(this.txtReconnectDelay);
            this.generalPage.Controls.Add(this.label6);
            this.generalPage.Controls.Add(this.label7);
            this.generalPage.Controls.Add(this.label5);
            this.generalPage.Controls.Add(this.label1);
            this.generalPage.Controls.Add(this.lblProgressSlow);
            this.generalPage.Controls.Add(this.lblThrottle);
            this.generalPage.Controls.Add(this.lblProgressFast);
            this.generalPage.Controls.Add(this.rbtAscii);
            this.generalPage.Controls.Add(this.chkAdvancedTransfer);
            this.generalPage.Controls.Add(this.rbtBinary);
            this.generalPage.Controls.Add(this.txtThrottle);
            this.generalPage.Controls.Add(this.tbProgress);
            this.generalPage.Controls.Add(this.lblKBPS);
            this.generalPage.Controls.Add(this.lblKAS);
            this.generalPage.Controls.Add(this.lblProgressUpdate);
            this.generalPage.Controls.Add(this.lblKeepAlive);
            this.generalPage.Controls.Add(this.txtKeepAliveInterval);
            this.generalPage.Controls.Add(this.txtTimeout);
            this.generalPage.Controls.Add(this.label3);
            this.generalPage.Controls.Add(this.lblTimeout);
            this.generalPage.Location = new System.Drawing.Point(4, 22);
            this.generalPage.Name = "generalPage";
            this.generalPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalPage.Size = new System.Drawing.Size(394, 307);
            this.generalPage.TabIndex = 0;
            this.generalPage.Text = "General Settings";
            this.generalPage.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.SystemColors.Control;
            this.label17.Cursor = System.Windows.Forms.Cursors.Default;
            this.label17.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label17.Location = new System.Drawing.Point(263, 69);
            this.label17.Name = "label17";
            this.label17.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label17.Size = new System.Drawing.Size(43, 14);
            this.label17.TabIndex = 121;
            this.label17.Text = "(1 - 10)";
            // 
            // numTransfers
            // 
            this.numTransfers.Location = new System.Drawing.Point(197, 67);
            this.numTransfers.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numTransfers.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTransfers.Name = "numTransfers";
            this.numTransfers.Size = new System.Drawing.Size(63, 20);
            this.numTransfers.TabIndex = 5;
            this.numTransfers.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.SystemColors.Control;
            this.label18.Cursor = System.Windows.Forms.Cursors.Default;
            this.label18.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label18.Location = new System.Drawing.Point(13, 71);
            this.label18.Name = "label18";
            this.label18.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label18.Size = new System.Drawing.Size(184, 14);
            this.label18.TabIndex = 120;
            this.label18.Text = "Maximum simultaneous transfers:";
            // 
            // txtOffsetSec
            // 
            this.txtOffsetSec.AcceptsReturn = true;
            this.txtOffsetSec.BackColor = System.Drawing.SystemColors.Window;
            this.txtOffsetSec.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtOffsetSec.Enabled = false;
            this.txtOffsetSec.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOffsetSec.Location = new System.Drawing.Point(288, 279);
            this.txtOffsetSec.MaxLength = 0;
            this.txtOffsetSec.Name = "txtOffsetSec";
            this.txtOffsetSec.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtOffsetSec.Size = new System.Drawing.Size(26, 20);
            this.txtOffsetSec.TabIndex = 25;
            this.txtOffsetSec.Text = "0";
            this.txtOffsetSec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.SystemColors.Control;
            this.label16.Cursor = System.Windows.Forms.Cursors.Default;
            this.label16.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label16.Location = new System.Drawing.Point(319, 282);
            this.label16.Name = "label16";
            this.label16.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label16.Size = new System.Drawing.Size(13, 14);
            this.label16.TabIndex = 118;
            this.label16.Text = "s";
            // 
            // txtOffsetMin
            // 
            this.txtOffsetMin.AcceptsReturn = true;
            this.txtOffsetMin.BackColor = System.Drawing.SystemColors.Window;
            this.txtOffsetMin.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtOffsetMin.Enabled = false;
            this.txtOffsetMin.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOffsetMin.Location = new System.Drawing.Point(243, 279);
            this.txtOffsetMin.MaxLength = 0;
            this.txtOffsetMin.Name = "txtOffsetMin";
            this.txtOffsetMin.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtOffsetMin.Size = new System.Drawing.Size(26, 20);
            this.txtOffsetMin.TabIndex = 24;
            this.txtOffsetMin.Text = "0";
            this.txtOffsetMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.SystemColors.Control;
            this.label15.Cursor = System.Windows.Forms.Cursors.Default;
            this.label15.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label15.Location = new System.Drawing.Point(274, 282);
            this.label15.Name = "label15";
            this.label15.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label15.Size = new System.Drawing.Size(15, 14);
            this.label15.TabIndex = 116;
            this.label15.Text = "m";
            // 
            // txtOffsetHour
            // 
            this.txtOffsetHour.AcceptsReturn = true;
            this.txtOffsetHour.BackColor = System.Drawing.SystemColors.Window;
            this.txtOffsetHour.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtOffsetHour.Enabled = false;
            this.txtOffsetHour.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOffsetHour.Location = new System.Drawing.Point(199, 279);
            this.txtOffsetHour.MaxLength = 0;
            this.txtOffsetHour.Name = "txtOffsetHour";
            this.txtOffsetHour.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtOffsetHour.Size = new System.Drawing.Size(26, 20);
            this.txtOffsetHour.TabIndex = 23;
            this.txtOffsetHour.Text = "0";
            this.txtOffsetHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.SystemColors.Control;
            this.label14.Cursor = System.Windows.Forms.Cursors.Default;
            this.label14.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.Location = new System.Drawing.Point(230, 282);
            this.label14.Name = "label14";
            this.label14.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label14.Size = new System.Drawing.Size(13, 14);
            this.label14.TabIndex = 114;
            this.label14.Text = "h";
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.SystemColors.Control;
            this.label12.Cursor = System.Windows.Forms.Cursors.Default;
            this.label12.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label12.Location = new System.Drawing.Point(13, 280);
            this.label12.Name = "label12";
            this.label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label12.Size = new System.Drawing.Size(137, 18);
            this.label12.TabIndex = 111;
            this.label12.Text = "Server Time Zone Offset:";
            // 
            // txtOffsetDay
            // 
            this.txtOffsetDay.AcceptsReturn = true;
            this.txtOffsetDay.BackColor = System.Drawing.SystemColors.Window;
            this.txtOffsetDay.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtOffsetDay.Enabled = false;
            this.txtOffsetDay.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOffsetDay.Location = new System.Drawing.Point(155, 279);
            this.txtOffsetDay.MaxLength = 0;
            this.txtOffsetDay.Name = "txtOffsetDay";
            this.txtOffsetDay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtOffsetDay.Size = new System.Drawing.Size(26, 20);
            this.txtOffsetDay.TabIndex = 22;
            this.txtOffsetDay.Text = "0";
            this.txtOffsetDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.SystemColors.Control;
            this.label13.Cursor = System.Windows.Forms.Cursors.Default;
            this.label13.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label13.Location = new System.Drawing.Point(186, 282);
            this.label13.Name = "label13";
            this.label13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label13.Size = new System.Drawing.Size(13, 14);
            this.label13.TabIndex = 112;
            this.label13.Text = "d";
            // 
            // chkRestoreFileLastWriteTime
            // 
            this.chkRestoreFileLastWriteTime.BackColor = System.Drawing.SystemColors.Control;
            this.chkRestoreFileLastWriteTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkRestoreFileLastWriteTime.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRestoreFileLastWriteTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkRestoreFileLastWriteTime.Location = new System.Drawing.Point(15, 251);
            this.chkRestoreFileLastWriteTime.Name = "chkRestoreFileLastWriteTime";
            this.chkRestoreFileLastWriteTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkRestoreFileLastWriteTime.Size = new System.Drawing.Size(333, 22);
            this.chkRestoreFileLastWriteTime.TabIndex = 21;
            this.chkRestoreFileLastWriteTime.Text = "Restore file last write time after transfer";
            this.chkRestoreFileLastWriteTime.UseVisualStyleBackColor = false;
            this.chkRestoreFileLastWriteTime.CheckedChanged += new System.EventHandler(this.chkRestoreFileLastWriteTime_CheckedChanged);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label11.Cursor = System.Windows.Forms.Cursors.Default;
            this.label11.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(10, 246);
            this.label11.Name = "label11";
            this.label11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label11.Size = new System.Drawing.Size(375, 2);
            this.label11.TabIndex = 108;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.Cursor = System.Windows.Forms.Cursors.Default;
            this.label9.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Location = new System.Drawing.Point(209, 44);
            this.label9.Name = "label9";
            this.label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label9.Size = new System.Drawing.Size(77, 14);
            this.label9.TabIndex = 106;
            this.label9.Text = "Upload Buffer:";
            // 
            // txtUploadBuffer
            // 
            this.txtUploadBuffer.AcceptsReturn = true;
            this.txtUploadBuffer.BackColor = System.Drawing.SystemColors.Window;
            this.txtUploadBuffer.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUploadBuffer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUploadBuffer.Location = new System.Drawing.Point(286, 41);
            this.txtUploadBuffer.MaxLength = 0;
            this.txtUploadBuffer.Name = "txtUploadBuffer";
            this.txtUploadBuffer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtUploadBuffer.Size = new System.Drawing.Size(62, 20);
            this.txtUploadBuffer.TabIndex = 4;
            this.txtUploadBuffer.Text = "64";
            this.txtUploadBuffer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Cursor = System.Windows.Forms.Cursors.Default;
            this.label10.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(351, 44);
            this.label10.Name = "label10";
            this.label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label10.Size = new System.Drawing.Size(22, 14);
            this.label10.TabIndex = 107;
            this.label10.Text = "Kb/s";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.Control;
            this.label8.Cursor = System.Windows.Forms.Cursors.Default;
            this.label8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(263, 101);
            this.label8.Name = "label8";
            this.label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label8.Size = new System.Drawing.Size(118, 14);
            this.label8.TabIndex = 104;
            this.label8.Text = "0 to disable the feature";
            // 
            // txtConnectionRetries
            // 
            this.txtConnectionRetries.Location = new System.Drawing.Point(197, 99);
            this.txtConnectionRetries.Name = "txtConnectionRetries";
            this.txtConnectionRetries.Size = new System.Drawing.Size(63, 20);
            this.txtConnectionRetries.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(13, 103);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(184, 14);
            this.label4.TabIndex = 102;
            this.label4.Text = "Maximum number of retries:";
            // 
            // txtReconnectDelay
            // 
            this.txtReconnectDelay.AcceptsReturn = true;
            this.txtReconnectDelay.BackColor = System.Drawing.SystemColors.Window;
            this.txtReconnectDelay.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtReconnectDelay.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReconnectDelay.Location = new System.Drawing.Point(197, 128);
            this.txtReconnectDelay.MaxLength = 3;
            this.txtReconnectDelay.Name = "txtReconnectDelay";
            this.txtReconnectDelay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtReconnectDelay.Size = new System.Drawing.Size(63, 20);
            this.txtReconnectDelay.TabIndex = 6;
            this.txtReconnectDelay.Text = "5";
            this.txtReconnectDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Control;
            this.label6.Cursor = System.Windows.Forms.Cursors.Default;
            this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(264, 130);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label6.Size = new System.Drawing.Size(10, 14);
            this.label6.TabIndex = 101;
            this.label6.Text = "s";
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.SystemColors.Control;
            this.label7.Cursor = System.Windows.Forms.Cursors.Default;
            this.label7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(13, 131);
            this.label7.Name = "label7";
            this.label7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label7.Size = new System.Drawing.Size(184, 14);
            this.label7.TabIndex = 100;
            this.label7.Text = "Delay between failed login attempts:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Cursor = System.Windows.Forms.Cursors.Default;
            this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(9, 154);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label5.Size = new System.Drawing.Size(375, 2);
            this.label5.TabIndex = 98;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(9, 91);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(375, 2);
            this.label1.TabIndex = 96;
            // 
            // lblProgressSlow
            // 
            this.lblProgressSlow.BackColor = System.Drawing.SystemColors.Control;
            this.lblProgressSlow.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblProgressSlow.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgressSlow.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblProgressSlow.Location = new System.Drawing.Point(345, 169);
            this.lblProgressSlow.Name = "lblProgressSlow";
            this.lblProgressSlow.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblProgressSlow.Size = new System.Drawing.Size(35, 14);
            this.lblProgressSlow.TabIndex = 95;
            this.lblProgressSlow.Text = "Slow";
            // 
            // lblThrottle
            // 
            this.lblThrottle.BackColor = System.Drawing.SystemColors.Control;
            this.lblThrottle.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblThrottle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThrottle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblThrottle.Location = new System.Drawing.Point(10, 45);
            this.lblThrottle.Name = "lblThrottle";
            this.lblThrottle.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblThrottle.Size = new System.Drawing.Size(46, 14);
            this.lblThrottle.TabIndex = 90;
            this.lblThrottle.Text = "Throttle:";
            // 
            // lblProgressFast
            // 
            this.lblProgressFast.BackColor = System.Drawing.SystemColors.Control;
            this.lblProgressFast.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblProgressFast.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgressFast.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblProgressFast.Location = new System.Drawing.Point(75, 170);
            this.lblProgressFast.Name = "lblProgressFast";
            this.lblProgressFast.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblProgressFast.Size = new System.Drawing.Size(28, 14);
            this.lblProgressFast.TabIndex = 94;
            this.lblProgressFast.Text = "Fast";
            // 
            // txtThrottle
            // 
            this.txtThrottle.AcceptsReturn = true;
            this.txtThrottle.BackColor = System.Drawing.SystemColors.Window;
            this.txtThrottle.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtThrottle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtThrottle.Location = new System.Drawing.Point(85, 42);
            this.txtThrottle.MaxLength = 0;
            this.txtThrottle.Name = "txtThrottle";
            this.txtThrottle.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtThrottle.Size = new System.Drawing.Size(62, 20);
            this.txtThrottle.TabIndex = 3;
            this.txtThrottle.Text = "0";
            this.txtThrottle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbProgress
            // 
            this.tbProgress.Location = new System.Drawing.Point(103, 164);
            this.tbProgress.Maximum = 20;
            this.tbProgress.Minimum = 1;
            this.tbProgress.Name = "tbProgress";
            this.tbProgress.Size = new System.Drawing.Size(236, 42);
            this.tbProgress.TabIndex = 7;
            this.tbProgress.Value = 10;
            // 
            // lblKBPS
            // 
            this.lblKBPS.BackColor = System.Drawing.SystemColors.Control;
            this.lblKBPS.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblKBPS.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKBPS.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblKBPS.Location = new System.Drawing.Point(150, 45);
            this.lblKBPS.Name = "lblKBPS";
            this.lblKBPS.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblKBPS.Size = new System.Drawing.Size(22, 14);
            this.lblKBPS.TabIndex = 91;
            this.lblKBPS.Text = "Kb/s";
            // 
            // lblProgressUpdate
            // 
            this.lblProgressUpdate.BackColor = System.Drawing.SystemColors.Control;
            this.lblProgressUpdate.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblProgressUpdate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgressUpdate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblProgressUpdate.Location = new System.Drawing.Point(13, 163);
            this.lblProgressUpdate.Name = "lblProgressUpdate";
            this.lblProgressUpdate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblProgressUpdate.Size = new System.Drawing.Size(60, 32);
            this.lblProgressUpdate.TabIndex = 92;
            this.lblProgressUpdate.Text = "Progress Update:";
            // 
            // ftpPage
            // 
            this.ftpPage.Controls.Add(this.label2);
            this.ftpPage.Controls.Add(this.chkSmartPath);
            this.ftpPage.Controls.Add(this.chkCompress);
            this.ftpPage.Controls.Add(this.chkChDirBeforeTransfer);
            this.ftpPage.Controls.Add(this.chkChDirBeforeListing);
            this.ftpPage.Controls.Add(this.chkSendABOR);
            this.ftpPage.Controls.Add(this.chkSendSignals);
            this.ftpPage.Location = new System.Drawing.Point(4, 22);
            this.ftpPage.Name = "ftpPage";
            this.ftpPage.Padding = new System.Windows.Forms.Padding(3);
            this.ftpPage.Size = new System.Drawing.Size(394, 307);
            this.ftpPage.TabIndex = 1;
            this.ftpPage.Text = "FTP Settings";
            this.ftpPage.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(9, 91);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(376, 2);
            this.label2.TabIndex = 97;
            // 
            // sftpPage
            // 
            this.sftpPage.Controls.Add(this.lblServer);
            this.sftpPage.Controls.Add(this.cbxServerOs);
            this.sftpPage.Location = new System.Drawing.Point(4, 22);
            this.sftpPage.Name = "sftpPage";
            this.sftpPage.Size = new System.Drawing.Size(394, 307);
            this.sftpPage.TabIndex = 2;
            this.sftpPage.Text = "SFTP Settings";
            this.sftpPage.UseVisualStyleBackColor = true;
            // 
            // lblServer
            // 
            this.lblServer.Location = new System.Drawing.Point(10, 9);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(59, 13);
            this.lblServer.TabIndex = 45;
            this.lblServer.Text = "Server OS:";
            // 
            // cbxServerOs
            // 
            this.cbxServerOs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxServerOs.FormattingEnabled = true;
            this.cbxServerOs.Items.AddRange(new object[] {
            "Auto Detect (Default)",
            "Unknown",
            "Windows",
            "Linux"});
            this.cbxServerOs.Location = new System.Drawing.Point(74, 5);
            this.cbxServerOs.Name = "cbxServerOs";
            this.cbxServerOs.Size = new System.Drawing.Size(144, 21);
            this.cbxServerOs.TabIndex = 4;
            // 
            // Setting
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(416, 377);
            this.Controls.Add(this.tabControlExt);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Setting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.tabControlExt.ResumeLayout(false);
            this.generalPage.ResumeLayout(false);
            this.generalPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTransfers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConnectionRetries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbProgress)).EndInit();
            this.ftpPage.ResumeLayout(false);
            this.sftpPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblKAS;
        public System.Windows.Forms.Label lblKeepAlive;
        public System.Windows.Forms.TextBox txtKeepAliveInterval;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label lblTimeout;
        public System.Windows.Forms.TextBox txtTimeout;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbtBinary;
        private System.Windows.Forms.RadioButton rbtAscii;
        public System.Windows.Forms.CheckBox chkSendABOR;
        public System.Windows.Forms.CheckBox chkSendSignals;
        public System.Windows.Forms.CheckBox chkAdvancedTransfer;
        public System.Windows.Forms.CheckBox chkChDirBeforeTransfer;
        public System.Windows.Forms.CheckBox chkChDirBeforeListing;
        public System.Windows.Forms.CheckBox chkCompress;
        public System.Windows.Forms.CheckBox chkSmartPath;
        private System.Windows.Forms.TabControl tabControlExt;
        private System.Windows.Forms.TabPage generalPage;
        private System.Windows.Forms.TabPage ftpPage;
        private System.Windows.Forms.TabPage sftpPage;
        public System.Windows.Forms.Label lblProgressSlow;
        public System.Windows.Forms.Label lblThrottle;
        public System.Windows.Forms.Label lblProgressFast;
        public System.Windows.Forms.TextBox txtThrottle;
        private System.Windows.Forms.TrackBar tbProgress;
        public System.Windows.Forms.Label lblKBPS;
        public System.Windows.Forms.Label lblProgressUpdate;
        private System.Windows.Forms.ComboBox cbxServerOs;
        private System.Windows.Forms.Label lblServer;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtReconnectDelay;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown txtConnectionRetries;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox txtUploadBuffer;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.CheckBox chkRestoreFileLastWriteTime;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.TextBox txtOffsetSec;
        public System.Windows.Forms.Label label16;
        public System.Windows.Forms.TextBox txtOffsetMin;
        public System.Windows.Forms.Label label15;
        public System.Windows.Forms.TextBox txtOffsetHour;
        public System.Windows.Forms.Label label14;
        public System.Windows.Forms.Label label12;
        public System.Windows.Forms.TextBox txtOffsetDay;
        public System.Windows.Forms.Label label13;
        public System.Windows.Forms.Label label17;
        private System.Windows.Forms.NumericUpDown numTransfers;
        public System.Windows.Forms.Label label18;
    }
}
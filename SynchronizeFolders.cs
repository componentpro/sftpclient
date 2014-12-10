﻿using System.Windows.Forms;
using ComponentPro.IO;

namespace SftpClient
{
    public partial class SynchronizeFolders : Form
    {
        public SynchronizeFolders()
        {
            InitializeComponent();
        }

        public SynchronizeFolders(bool localIsMaster, RecursionMode recursive, bool syncDateTime, int comparisonMethod, bool checkForResumability, string searchPattern)
        {
            InitializeComponent();

            rbtLocalMaster.Checked = localIsMaster;
            rbtRemoteMaster.Checked = !localIsMaster;
            chkRecursive.Checked = recursive == RecursionMode.Recursive;
            chkSyncDateTime.Checked = syncDateTime;
            cbxComparisonType.SelectedIndex = comparisonMethod;
            chkResumability.Checked = checkForResumability;
            txtSearchPattern.Text = searchPattern;
        }

        /// <summary>
        /// Gets the boolean value indicating whether the local folder is the master.
        /// </summary>
        public bool RemoteIsMaster
        {
            get { return rbtRemoteMaster.Checked; }
        }

        public RecursionMode Recursive
        {
            get { return chkRecursive.Checked ? RecursionMode.Recursive : RecursionMode.None; }
        }

        public bool SyncDateTime
        {
            get { return chkSyncDateTime.Checked; }
        }

        public int ComparisonMethod
        {
            get { return cbxComparisonType.SelectedIndex; }
        }

        public string SearchPattern
        {
            get { return txtSearchPattern.Text; }
        }

        public bool CheckForResumability
        {
            get { return chkResumability.Checked; }
        }

        private void cbxComparisonType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            chkResumability.Visible = cbxComparisonType.SelectedIndex == 1;
        }
    }
}
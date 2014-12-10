using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ComponentPro.IO;

namespace SftpClient
{
    public partial class FileOperation : Form
    {
        private Dictionary<System.Windows.Forms.Button, TransferConfirmNextActions> _btns;
        private readonly Dictionary<TransferConfirmReason, object> _skipTypes = new Dictionary<TransferConfirmReason, object>();
        private TransferConfirmEventArgs _args;

        private bool _overwriteAll;
        private bool _overwriteOlder;
        private bool _overwriteDifferentSize;
		private bool _cancelAll;

        private bool _resumeAll;
        private bool _followAllLinks;

        public FileOperation()
        {
            InitializeComponent();
        }

        public void Init()
        {
            if (_btns == null)
            {
                _btns = new Dictionary<System.Windows.Forms.Button, TransferConfirmNextActions>();
                _btns.Add(btnOverwrite, TransferConfirmNextActions.Overwrite);
                _btns.Add(btnOverwriteAll, TransferConfirmNextActions.Overwrite);
                _btns.Add(btnOverwriteDiffSize, TransferConfirmNextActions.CheckAndOverwriteFilesWithDifferentSizes);
                _btns.Add(btnOverwriteOlder, TransferConfirmNextActions.CheckAndOverwriteOlderFiles);
                _btns.Add(btnRename, TransferConfirmNextActions.Rename);
                _btns.Add(btnSkip, TransferConfirmNextActions.Skip);
                _btns.Add(btnSkipAll, TransferConfirmNextActions.Skip);
                _btns.Add(btnFollowLink, TransferConfirmNextActions.FollowLink);
                _btns.Add(btnFollowLinkAll, TransferConfirmNextActions.FollowLink);
                _btns.Add(btnRetry, TransferConfirmNextActions.Retry);
                _btns.Add(btnCancel, TransferConfirmNextActions.Cancel);
                _btns.Add(btnResume, TransferConfirmNextActions.ResumeFileTransfer);
                _btns.Add(btnResumeAll, TransferConfirmNextActions.ResumeFileTransfer);
            }

            _skipTypes.Clear();
            _overwriteAll = false;
            _overwriteOlder = false;
            _overwriteDifferentSize = false;
			_cancelAll = false;

            _resumeAll = false;

            _followAllLinks = false;
        }

        public void Show(Form parent, TransferConfirmEventArgs evt, FileSystem sys)
        {
            if (_cancelAll)
            {
                evt.NextAction = TransferConfirmNextActions.Cancel;
                return;
            }
            if (_skipTypes.ContainsKey(evt.ConfirmReason))
            {
                evt.NextAction = TransferConfirmNextActions.Skip;
                return;
            }

            if (evt.ConfirmReason == TransferConfirmReason.FileAlreadyExists)
            {
                if (_overwriteAll)
                {
                    evt.NextAction = TransferConfirmNextActions.Overwrite;
                    return;
                }

                if (_overwriteDifferentSize)
                {
                    evt.NextAction = TransferConfirmNextActions.CheckAndOverwriteFilesWithDifferentSizes;
                    return;
                }

                if (_overwriteOlder)
                {
                    evt.NextAction = TransferConfirmNextActions.CheckAndOverwriteOlderFiles;
                    return;
                }

                if (_resumeAll && (evt.PossibleNextActions & TransferConfirmNextActions.ResumeFileTransfer) != 0)
                {
                    evt.NextAction = TransferConfirmNextActions.ResumeFileTransfer;
                    return;
                }

                // format the text according to TransferState (Downloading or Uploading)
                const string messageFormat = "Are you sure you want to overwrite file: {0}\r\n{1} Bytes, {2}\r\n\r\nWith file: {3}\r\n{4} Bytes, {5}";

                lblMessage.Text = string.Format(messageFormat,
                                                evt.DestinationFileInfo.FullName, evt.DestinationFileInfo.Length,
                                                evt.DestinationFileInfo.LastWriteTime,
                                                evt.SourceFileInfo.FullName, evt.SourceFileInfo.Length,
                                                evt.SourceFileInfo.LastWriteTime);

                Text = "File already exists";
            }
            else
            {
                if (evt.Exception != null)
                {
                    Exception ex = evt.Exception;
					while (ex.InnerException != null)
						ex = ex.InnerException;						
					
                    // Show the exception message.
					lblMessage.Text = evt.Message + "\r\nReason: " + ex.Message;
                }
                else
                {
                    if (_followAllLinks && (evt.PossibleNextActions & TransferConfirmNextActions.FollowLink) != 0)
                    {
                        evt.NextAction = TransferConfirmNextActions.FollowLink;
                        return;
                    }

                    lblMessage.Text = evt.Message;
                }

                Text = "An error occurred";
            }

            _args = evt;

            ArrangeButtons(evt);

            this.ShowDialog(parent);
        }

        private void ArrangeButtons(TransferConfirmEventArgs evt)
        {
            const int height = 24;
            const int width = 128;
            const int gap = 3;


            int buttons = 0;
            foreach (KeyValuePair<System.Windows.Forms.Button, TransferConfirmNextActions> en in _btns)
            {
                bool b = evt.CanPerform(en.Value);
                en.Key.Visible = b;
                if (b)
                {
                    buttons++;
                }
            }

            int count = this.ClientSize.Width / (width + gap);
            int y = this.ClientSize.Height - (height + gap) * ((buttons / count) + ((buttons % count) == 0 ? 0 : 1)) - 4;
            int x = (this.ClientSize.Width - count * width - gap) / 2;
            
            int i = 0;
            foreach (KeyValuePair<System.Windows.Forms.Button, TransferConfirmNextActions> en in _btns)
            {
                bool b = evt.CanPerform(en.Value);
                en.Key.Visible = b;
                if (b)
                {
                    en.Key.Left = x + (width + gap) * (i % count);
                    en.Key.Top = y + (height + gap) * (i / count);
                    i++;
                }
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.Cancel;
			_cancelAll = true;
            _args = null; // Release mem
            Close();
        }

        private void btnSkip_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.Skip;
            _args = null; // Release mem
            Close();
        }

        private void btnSkipAll_Click(object sender, System.EventArgs e)
        {
            _skipTypes.Add(_args.ConfirmReason, null);

            _args.NextAction = TransferConfirmNextActions.Skip;
            _args = null; // Release mem
            Close();
        }

        private void btnRetry_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.Retry;
            _args = null; // Release mem
            Close();
        }

        private void btnRename_Click(object sender, System.EventArgs e)
        {
            string oldName = _args.DestinationFileSystem.GetFileName(_args.DestinationFileInfo.Name);
            NewNamePrompt formNewName = new NewNamePrompt(oldName);

            DialogResult result = formNewName.ShowDialog(this);

            string newName = formNewName.NewName;

            if (result != DialogResult.OK || newName.Length == 0 || newName == oldName)
                return;

            _args.NextAction = TransferConfirmNextActions.Rename;
            _args.NewName = newName;
            _args = null; // Release mem
            Close();
        }

        private void btnOverwrite_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.Overwrite;
            _args = null; // Release mem
            Close();
        }

        private void btnOverwriteAll_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.Overwrite;
            _overwriteAll = true;
            _args = null; // Release mem
            Close();
        }

        private void btnOverwriteOlder_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.CheckAndOverwriteOlderFiles;
            _overwriteOlder = true;
            _args = null; // Release mem
            Close();
        }

        private void btnOverwriteDiffSize_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.CheckAndOverwriteFilesWithDifferentSizes;
            _overwriteDifferentSize = true;
            _args = null; // Release mem
            Close();
        }

        private void btnFollowLink_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.FollowLink;
            _args = null; // Release mem
            Close();
        }

        private void btnResume_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.ResumeFileTransfer;
            _args = null; // Release mem
            Close();
        }

        private void btnResumeAll_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.ResumeFileTransfer;
            _resumeAll = true;
            _args = null; // Release mem
            Close();
        }

        private void btnFollowLinkAll_Click(object sender, System.EventArgs e)
        {
            _args.NextAction = TransferConfirmNextActions.FollowLink;
            _followAllLinks = true;
            _args = null; // Release mem
            Close();
        }
    }
}
using System;
using ComponentPro.IO;
using ComponentPro.Net;
using ComponentPro;

namespace SftpClient
{
    public class SettingInfo
    {
        int _throttle;
        public int Throttle
        {
            get { return _throttle; }
            set { _throttle = value; }
        }

        int _transfers;
        public int Transfers
        {
            get { return _transfers; }
            set { _transfers = value; }
        }

        int _timeout;
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        int _keepAlive;
        public int KeepAlive
        {
            get { return _keepAlive; }
            set { _keepAlive = value; }
        }

        bool _asciiTransfer;
        public bool AsciiTransfer
        {
            get { return _asciiTransfer; }
            set { _asciiTransfer = value; }
        }

        int _progressUpdateInterval;
        public int ProgressUpdateInterval
        {
            get { return _progressUpdateInterval; }
            set { _progressUpdateInterval = value; }
        }

        int _reconnectDelay;
        public int ReconnectionFailureDelay
        {
            get { return _reconnectDelay; }
            set { _reconnectDelay = value; }
        }

        int _connectionRetries;
        public int ReconnectionMaxRetries
        {
            get { return _connectionRetries; }
            set { _connectionRetries = value; }
        }

        bool _restoreFileProperties;
        public bool RestoreFileProperties
        {
            get { return _restoreFileProperties; }
            set { _restoreFileProperties = value; }
        }

        TimeSpan _timeZoneOffset;
        public TimeSpan TimeZoneOffset
        {
            get { return _timeZoneOffset; }
            set { _timeZoneOffset = value; }
        }

        #region FTP
        int _uploadBufferSize;
        public int UploadBufferSize
        {
            get { return _uploadBufferSize; }
            set { _uploadBufferSize = value; }
        }

        bool _sendSignals;
        public bool SendSignals
        {
            get { return _sendSignals; }
            set { _sendSignals = value; }
        }

        OptionValue _sendAbor;
        public OptionValue SendAbor
        {
            get { return _sendAbor; }
            set { _sendAbor = value; }
        }

        bool _advancedFileTransfer;
        public bool AdvancedTransfer
        {
            get { return _advancedFileTransfer; }
            set { _advancedFileTransfer = value; }
        }

        bool _changeDirBeforeListing;
        public bool ChangeDirBeforeListing
        {
            get { return _changeDirBeforeListing; }
            set { _changeDirBeforeListing = value; }
        }

        bool _changeDirBeforeTransfer;
        public bool ChangeDirBeforeTransfer
        {
            get { return _changeDirBeforeTransfer; }
            set { _changeDirBeforeTransfer = value; }
        }

        bool _compress;
        public bool Compress
        {
            get { return _compress; }
            set { _compress = value; }
        }

        bool _smartPath;
        public bool SmartPath
        {
            get { return _smartPath; }
            set { _smartPath = value; }
        }
        #endregion

        #region SFTP

        int _serverOs;
        public int ServerOs
        {
            get { return _serverOs; }
            set { _serverOs = value; }
        }

        #endregion

        #region Methods

        public void SaveConfig()
        {
            // Save settings.
            Util.SaveProperty("Transfers", Transfers);
            Util.SaveProperty("Throttle", Throttle);
            Util.SaveProperty("Timeout", Timeout);
            Util.SaveProperty("KeepAlive", KeepAlive);
            Util.SaveProperty("AsciiTransfer", AsciiTransfer);
            Util.SaveProperty("ProgressUpdateInterval", ProgressUpdateInterval);
            Util.SaveProperty("AdvancedTransfer", AdvancedTransfer);
            Util.SaveProperty("ConnectionRetries", ReconnectionMaxRetries);
            Util.SaveProperty("ReconnectDelay", ReconnectionFailureDelay);
            Util.SaveProperty("TimeZoneOffset", TimeZoneOffset.TotalSeconds);
            Util.SaveProperty("RestoreFileProperties", RestoreFileProperties);
            
            #region Ftp
            Util.SaveProperty("UploadBuffer", UploadBufferSize);
            Util.SaveProperty("SendSignals", SendSignals);
            Util.SaveProperty("SendABOR", (int)SendAbor);
            Util.SaveProperty("ChangeDirBeforeListing", ChangeDirBeforeListing);
            Util.SaveProperty("ChangeDirBeforeTransfer", ChangeDirBeforeTransfer);
            Util.SaveProperty("Compress", Compress);
            Util.SaveProperty("SmartPathSolving", SmartPath);
            #endregion

            #region SFTP
            Util.SaveProperty("ServerOs", ServerOs);
            #endregion
        }

        public static SettingInfo LoadConfig()
        {
            // Load settings.
            SettingInfo s = new SettingInfo();

            // Load settings.
            s.Transfers = Util.GetIntProperty("Transfers", 1);
            s.Throttle = Util.GetIntProperty("Throttle", 0);
            s.Timeout = Util.GetIntProperty("Timeout", 30);
            s.KeepAlive = Util.GetIntProperty("KeepAlive", 60);
            s.AsciiTransfer = Util.GetProperty("AsciiTransfer", "False").ToString() == "True";
            s.ProgressUpdateInterval = Util.GetIntProperty("ProgressUpdateInterval", 500);
            s.AdvancedTransfer = Util.GetProperty("AdvancedTransfer", "True").ToString() == "True";
            s.ReconnectionFailureDelay = Util.GetIntProperty("ReconnectDelay", 5000);
            s.ReconnectionMaxRetries = Util.GetIntProperty("ConnectionRetries", 2);
            s.TimeZoneOffset = new TimeSpan(0, 0, Util.GetIntProperty("TimeZoneOffset", 0));
            s.RestoreFileProperties = Util.GetProperty("RestoreFileProperties", "False").ToString() == "True";
            
            #region FTP
            s.UploadBufferSize = Util.GetIntProperty("UploadBuffer", 64);
            s.SendSignals = Util.GetProperty("SendSignals", "True").ToString() == "True";
            s.SendAbor = (OptionValue)Util.GetIntProperty("SendABOR", (int)OptionValue.Auto);
            s.ChangeDirBeforeListing = Util.GetProperty("ChangeDirBeforeListing", "True").ToString() == "True";
            s.ChangeDirBeforeTransfer = Util.GetProperty("ChangeDirBeforeTransfer", "False").ToString() == "True";
            s.Compress = Util.GetProperty("Compress", "False").ToString() == "True";
            s.SmartPath = Util.GetProperty("SmartPathSolving", "True").ToString() == "True";
            #endregion

            #region SFTP
            s.ServerOs = Util.GetIntProperty("ServerOs", 0);
            #endregion

            return s;
        }

        #endregion
    }    
}

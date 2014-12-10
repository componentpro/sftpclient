#define TRANSACTION
//#define CUTTOPLINES

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ComponentPro.Diagnostics;

namespace SftpClient
{
    public class RichTextBoxTraceListener : UltimateTextWriterTraceListener
    {
        private const int MaxLines = 30000;

#if CUTTOPLINES
        class LineInfo
        {
            public int Pos;
            public int Length;
        }

        private List<LineInfo> LineHistory = new List<LineInfo>(MaxLines);
#else
        private int _lines;
#endif

        private static readonly Color TextColorCommand = Color.White; // Text color for command texts.
        private static readonly Color TextColorError = Color.FromArgb(0xff, 0x50, 0x50); // Text color for error texts.
        internal static readonly Color TextColorInfo = Color.FromArgb(0x72, 0xff, 0x7c); // Text color for information texts.
        private static readonly Color TextColorResponse = Color.FromArgb(0xa0, 0xa0, 0xa0); // Text color for response texts.
        private static readonly Color TextColorSecure = Color.FromArgb(0x8b, 0xf5, 0xfc); // Text color for security information texts.
        private readonly RichTextBox _textbox;

        public RichTextBoxTraceListener(RichTextBox textbox)
        {
            _textbox = textbox;
        }

        public override void TraceData(object source, TraceEventType level, string category, string message)
        {
            Color color = TextColorInfo;

            string prefix = string.Format("[{0:HH:mm:ss.fff}] {1}", DateTime.Now, source);
            string body;

            // If it's showing an error?
            if (level <= TraceEventType.Error)
            {
                color = TextColorError;
            }
            else
            {
                switch (category.ToUpper())
                {
                    case "COMMAND":
                        // command log.
                        color = TextColorCommand;
                        //message = string.Format("[{0:HH:mm:ss.fff}] {1} - COMMAND>   {2}\r\n", DateTime.Now, level,
                        //                        message);
                        body = string.Format(" {0} - COMMAND>   ", level);
                        goto Invoke;

                    case "RESPONSE":
                        // response log.
                        color = TextColorResponse;
                        //message = string.Format("[{0:HH:mm:ss.fff}] {1} -        <   {2}\r\n", DateTime.Now, level,
                        //                        message);
                        body = string.Format(" {0} -        <   ", level);
                        goto Invoke;

                    case "SECURESHELL":
                    case "SECURESOCKET":
                        color = TextColorSecure;
                        break;
                }
            }
            //message = string.Format("[{0:HH:mm:ss.fff}] {3} {1} - {2}: ", DateTime.Now, level, category, source) + message + "\r\n";
            body = string.Format(" {0} - {1}: ", level, category);

        Invoke:
            _textbox.Invoke(new OnLogHandler(OnLog), new object[] {prefix + body + message + "\r\n", color });
        }

#if TRANSACTION
        public void BeginUpdate()
        {
            SendMessage(_textbox.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);
        }
        public void EndUpdate()
        {
            SendMessage(_textbox.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            _textbox.Invalidate();
        }
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int WM_SETREDRAW = 0x0b;
#endif

        private void OnLog(string message, Color color)
        {
#if TRANSACTION
            BeginUpdate();
#endif

#if CUTTOPLINES
            LineInfo line;
            
            if (LineHistory.Count > MaxLines)
            {
                if (_textbox.Text.Length == 0)
                    LineHistory.Clear();
                else
                {
                    line = LineHistory[0];
                    int len = line.Length + 1;

                    _textbox.ReadOnly = false;
                    _textbox.Select(line.Pos, len);
                    _textbox.SelectedText = "";
                    _textbox.ReadOnly = true;
                    LineHistory.RemoveAt(0);

                    for (int i = 0; i < LineHistory.Count; i++)
                    {
                        line = LineHistory[i];
                        line.Pos -= len;

                        if (line.Pos < 0)
                        {
                            line.Length += line.Pos;
                            line.Pos = 0;
                        }
                    }
                }
            }

            line = new LineInfo();
            line.Pos = _textbox.Text.Length;
            line.Length = message.Length - 2;
            LineHistory.Add(line);
#else
            if (_lines > MaxLines)
            {
                _textbox.Clear();
                _lines = 0;
            }
            else if (_textbox.Text.Length == 0)
                _lines = 0;
            _lines++;
#endif
            // Write log message to the text box.
            _textbox.SelectionColor = color;
            _textbox.SelectionStart = _textbox.Text.Length;
            _textbox.SelectedText = message;
            _textbox.ScrollToCaret();

#if TRANSACTION
            EndUpdate();
#endif
        }

        #region Nested type: OnLogHandler

        private delegate void OnLogHandler(string message, Color color);

        #endregion
    }
}
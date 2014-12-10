﻿using System;
using ComponentPro.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SftpClient
{
    public static class Util
    {
        /// <summary>
        /// Saves a key value pair to the registry.
        /// </summary>
        /// <param name="keyName">The key name.</param>
        /// <param name="value">The value.</param>
        public static void SaveProperty(string keyName, object value)
        {
            try
            {
                RegistryKey Key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\ComponentPro\\Samples\\SftpClient");
                Key.SetValue(keyName, value);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Gets the value for the given key name from the registry.
        /// </summary>
        /// <param name="keyName">The key name.</param>
        /// <param name="defaultValue">The default value that is used when the key is not found.</param>
        public static object GetProperty(string keyName, object defaultValue)
        {
            try
            {
                RegistryKey Key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\ComponentPro\\Samples\\SftpClient");
                return Key.GetValue(keyName, defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static object GetProperty(string keyName)
        {
            return GetProperty(keyName, null);
        }

        public static int GetIntProperty(string keyName, int defaultValue)
        {
            try
            {
                RegistryKey Key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\ComponentPro\\Samples\\SftpClient");
                return int.Parse(Key.GetValue(keyName, defaultValue).ToString());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static long GetLongProperty(string keyName, long defaultValue)
        {
            try
            {
                RegistryKey Key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\ComponentPro\\Samples\\SftpClient");
                return long.Parse(Key.GetValue(keyName, defaultValue).ToString());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static void ShowError(Exception exc)
        {
            while (exc.InnerException != null)
                exc = exc.InnerException;

            FileSystemOperationCancelledException fc = exc as FileSystemOperationCancelledException;
            if (fc != null)
                return;

            string str = string.Format(null, "An error occurred: {0}", exc.Message);

            MessageBox.Show(str, "Error");
        }

        public static void ShowError(Exception exc, string msg)
        {
            while (exc.InnerException != null)
                exc = exc.InnerException;

            string str = string.Format(null, "{0}. An error occurred: {1}", msg, exc.Message);

            MessageBox.Show(str, "Error");
        }

        const int MF_BYCOMMAND = 0;
        const int MF_ENABLED = 0x00000000;
        const int MF_GRAYED = 0x00000001;

        [DllImport("User32")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32")]
        private static extern bool EnableMenuItem(IntPtr hMenu, IntPtr hMenuItem, int nEnable);

        [DllImport("User32")]
        private static extern IntPtr GetMenuItemID(IntPtr hMenu, int nPos);

        [DllImport("User32")]
        private static extern int GetMenuItemCount(IntPtr hWnd);

        static readonly Dictionary<string, bool> _map = new Dictionary<string, bool>();

        /// <summary>
        /// Disables Close(X) button.
        /// </summary>
        /// <param name="form">Form object.</param>
        /// <param name="enable">Indicates whether the close button is enabled.</param>
        static void EnableCloseButtonInt(Form form, bool enable)
        {
            IntPtr hMenu = GetSystemMenu(form.Handle, false);
            int menuItemCount = GetMenuItemCount(hMenu);
            IntPtr hItem = GetMenuItemID(hMenu, menuItemCount - 1);
            EnableMenuItem(hMenu, hItem, MF_BYCOMMAND | (enable ? MF_ENABLED : MF_GRAYED));
            form.Refresh();
        }

        /// <summary>
        /// Disables Close(X) button.
        /// </summary>
        /// <param name="form">Form object.</param>
        /// <param name="enable">Indicates whether the close button is enabled.</param>
        public static void EnableCloseButton(Form form, bool enable)
        {
            EnableCloseButtonInt(form, enable);

            if (!_map.ContainsKey(form.Name))
            {
                lock (_map)
                {
                    _map.Add(form.Name, enable);
                    form.Resize += form_Resize;
                }
            }
            else
                _map[form.Name] = enable;
        }

        static void form_Resize(object sender, EventArgs e)
        {
            Form form = (Form)sender;

            if (!_map[form.Name])
                EnableCloseButtonInt(form, false);
        }

        /// <summary>
        /// Returns a formatted file size in bytes, kbytes, or mbytes.
        /// </summary>
        /// <param name="size">The input file size.</param>
        /// <returns>The formatted file size.</returns>
        public static string FormatSize(long size)
        {
            if (size < 1024)
                return size + " B";
            if (size < 1024 * 1024)
                return string.Format("{0:#.#} KB", size / 1024.0f);
            return size < 1024 * 1024 * 1024 ? string.Format("{0:#.#} MB", size / 1024.0f / 1024.0f) : string.Format("{0:#.#} GB", size / 1024.0f / 1024.0f / 1024.0f);
        }
    }
}
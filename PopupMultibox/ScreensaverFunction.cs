﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PopupMultibox
{
    public class ScreensaverFunction : AbstractFunction
    {
        #region MultiboxFunction Members

        public override bool Triggers(MultiboxFunctionParam args)
        {
            return (args.MultiboxText != null && args.MultiboxText.Length > 0 && args.MultiboxText.Equals("scr"));
        }

        public override string RunSingle(MultiboxFunctionParam args)
        {
            return "Start Screensaver";
        }

        public override bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return true;
        }

        public override void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            LockDesktop.SetScreenSaverRunning();
        }

        #endregion
    }
    public static class LockDesktop
    {
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "LockWorkStation")]
        private static extern IntPtr LockWorkStation();

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

        private const int SC_SCREENSAVE = 0xF140;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SPI_GETSCREENSAVESECURE = 0x76;

        public static void SetScreenSaverRunning()
        {
            if (ScreensaverLocks())
                LockWorkStation();
            SendMessage(GetDesktopWindow(), WM_SYSCOMMAND, SC_SCREENSAVE, 0);
        }

        public static bool ScreensaverLocks()
        {
            uint result = 0;
            SystemParametersInfo(SPI_GETSCREENSAVESECURE, 0, ref result, 0);
            return (result == 1);
        }
    }
}
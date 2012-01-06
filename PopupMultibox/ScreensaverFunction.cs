using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PopupMultibox
{
    public class ScreensaverFunction : MultiboxFunction
    {
        #region MultiboxFunction Members

        public bool Triggers(MultiboxFunctionParam args)
        {
            return (args.MultiboxText != null && args.MultiboxText.Length > 0 && args.MultiboxText.Equals("scr"));
        }

        public bool IsMulti(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool IsBackgroundStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool ShouldRun(MultiboxFunctionParam args)
        {
            return true;
        }

        public string RunSingle(MultiboxFunctionParam args)
        {
            return "Start Screensaver";
        }

        public List<ResultItem> RunMulti(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public void RunSingleBackgroundStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public void RunMultiBackgroundStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasDetails(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool IsBackgroundDetailsStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public string GetDetails(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public void GetBackgroundDetailsStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasActions(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool IsBackgroundActionsStream(MultiboxFunctionParam args)
        {
            return false;
        }

        public List<ResultItem> GetActions(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public void GetBackgroundActionsStream(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasAction(MultiboxFunctionParam args)
        {
            return false;
        }

        public void RunAction(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool SupressKeyPress(MultiboxFunctionParam args)
        {
            return false;
        }

        public bool HasKeyDownAction(MultiboxFunctionParam args)
        {
            return false;
        }

        public void RunKeyDownAction(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasActionKeyEvent(MultiboxFunctionParam args)
        {
            return true;
        }

        public void RunActionKeyEvent(MultiboxFunctionParam args)
        {
            LockDesktop.SetScreenSaverRunning();
        }

        public bool HasSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            return false;
        }

        public string RunSpecialDisplayCopyHandling(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
        }

        public bool HasSpecialInputCopyHandling(MultiboxFunctionParam args)
        {
            return false;
        }

        public string RunSpecialInputCopyHandling(MultiboxFunctionParam args)
        {
            throw new InvalidOperationException();
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
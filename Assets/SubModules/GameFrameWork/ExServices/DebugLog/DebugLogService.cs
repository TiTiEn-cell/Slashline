using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine;

namespace GFramework.Services
{
    public class DebugLogService : SingletonMono<DebugLogService>
    {
        public Text contentText;
        public bool Enable = false;

        private bool isEnable;
        private string contentStr;

        private void Start()
        {
            EnableDebugLog();
        }

        public void EnableDebugLog()
        {
            var debug = GameService.instance.IsDebug;
            if (!debug) return;
            this.gameObject.SetActive(debug);
            isEnable = contentText != null;
            contentStr = "";
            if (isEnable) contentText.text = "";
        }

        public void DisableDebuLog()
        {
            this.gameObject.SetActive(false);
        }

        void SetContent(string str)
        {
            contentStr = "\n" + contentStr;
            contentStr = str + contentStr;
            contentText.text = contentStr;
        }

        public static void Log(string str)
        {
            if (DebugLogService.instance == null) return;
            if (!DebugLogService.instance.isEnable) return;
            DebugLogService.instance.SetContent($"<color=white>{str}</color>");
        }

        public static void LogError(string str)
        {
            if (DebugLogService.instance == null) return;
            if (!DebugLogService.instance.isEnable) return;
            DebugLogService.instance.SetContent($"<color=red>{str}</color>");
        }

        public static void LogWarning(string str)
        {
            if (DebugLogService.instance == null) return;
            if (!DebugLogService.instance.isEnable) return;
            DebugLogService.instance.SetContent($"<color=yellow>{str}</color>");
        }
    }
}

#if USE_LOCALIZATION
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace GFramework.Services
{
    [InitializeOnLoad]
    internal static class LocalizedMenuItem 
    {
        [MenuItem("CONTEXT/Text/Add Localized Text & Font")]
        static void LocalizeUIText(MenuCommand command)
        {
            var target = command.context as Text;
            SetupForLocalization(target);
        }

        [MenuItem("CONTEXT/Text/Add Localized Font")]
        static void LocalizeUITextFont(MenuCommand command)
        {
            var target = command.context as Text;
            if(target.gameObject.GetComponent<LocalizedFontEvent>() == null) {
                target.gameObject.AddComponent<LocalizedFontEvent>();
            }
        }

        public static void SetupForLocalization(Text target)
        {
            if(target.gameObject.GetComponent<LocalizedTextEvent>() == null) {
                target.gameObject.AddComponent<LocalizedTextEvent>();
            }
            
            if(target.gameObject.GetComponent<LocalizedFontEvent>() == null) {
                target.gameObject.AddComponent<LocalizedFontEvent>();
            }
        }
    }
}
#endif

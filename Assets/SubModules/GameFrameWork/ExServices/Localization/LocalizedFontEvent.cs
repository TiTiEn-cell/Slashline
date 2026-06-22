#if USE_LOCALIZATION

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace GFramework.Services
{
    [Serializable]
    public class LocalizedFont : LocalizedAsset<Font> {}

    [RequireComponent(typeof(Text))]
    public class LocalizedFontEvent : LocalizedAssetBehaviour<Font, LocalizedFont>
    {
        private Text text;

        void Start() {
            text = GetComponent<Text>();
        }

        protected override void UpdateAsset(Font localizedFont)
        {
            if(text == null) {
                 text = GetComponent<Text>();
            }

            text.font = localizedFont;
        }
    }
}

#endif

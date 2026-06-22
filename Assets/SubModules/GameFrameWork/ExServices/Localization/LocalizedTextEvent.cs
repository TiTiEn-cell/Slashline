#if USE_LOCALIZATION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace GFramework.Services
{
    [RequireComponent(typeof(Text))]
    public class LocalizedTextEvent : LocalizeStringEvent
    {
        private Text text;

        void Start() {
            text = GetComponent<Text>();
        }

        protected override void UpdateString(string value)
        {
            base.UpdateString(value);

            if(text == null) text = GetComponent<Text>();
            text.text = value;
        }
    }
}

#endif

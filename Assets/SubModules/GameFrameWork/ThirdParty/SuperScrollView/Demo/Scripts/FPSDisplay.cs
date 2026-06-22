using UnityEngine;

namespace SuperScrollView
{

    public class FPSDisplay : MonoBehaviour
    {
        float deltaTime = 0.0f;

        GUIStyle mStyle;
        void Awake()
        {
            mStyle = new GUIStyle();
            mStyle.alignment = TextAnchor.UpperLeft;
            mStyle.normal.background = null;
            mStyle.fontSize = 25;
            mStyle.normal.textColor = new Color(1f, 0f, 0f, 1.0f);
        }

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int w = Screen.width;
            int h = Screen.height;
            Rect rect = new Rect(10, 10, w, h * 2 / 100);
            float fps = 1.0f / deltaTime;
            string text = string.Format("   {0:0.} FPS", fps);
            GUI.Label(rect, text, mStyle);
        }
    }
}

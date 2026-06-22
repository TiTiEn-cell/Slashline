using UnityEngine;
using UnityEngine.UI;
using System;

namespace GFramework.GUI
{
    public class GUIBase : MonoBehaviour
    {
        public GUIHandlerBase handler;

        public Animation animController;
        public bool isCheckScale = true;
        public bool selfHide = false;

        public Action onBackKeyAndroid;

        //Minh.ho add
        private void CheckScale()  //Suitable for all device resolution
        {
            if (!isCheckScale) return;
            float ratio = (float)Screen.width / (float)Screen.height;
            if (ratio > 0.5625f)     //Minh.ho: 0.5625 is the ratio of FullHD resolution: 1080*1920
            {
                this.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
            }
            else
            {
                this.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
            }
        }
        //Minh.ho end

        protected void AddActionToAllButton(UnityEngine.Events.UnityAction action)
        {
            Button[] listButton = gameObject.GetComponentsInChildren<Button>(true);
            foreach (Button button in listButton)
            {
                button.onClick.AddListener(action);
            }
        }

        public virtual bool Show(params object[] @parameter)
        {
            if (handler == null)
                return false;
            selfHide = false;
            // CheckScale();   //Minh.ho add

            onBackKeyAndroid += OnBackKeyAndroid;

            return handler.Show(@parameter);
        }

        public virtual void Hide(params object[] @parameter)
        {
            if (handler == null)
                return;
            selfHide = true;

            onBackKeyAndroid -= OnBackKeyAndroid;

            handler.Hide(@parameter);
        }

        public virtual void OnBeginShowing() { }
        public virtual void OnEndShowing() { }
        public virtual void OnBeginHidding() { }
        public virtual void OnEndHidding() { }
        public virtual void OnBackKeyAndroid() { }

        public virtual void Update()
        {
            CheckBackKeyAndroid();
        }

        private void CheckBackKeyAndroid()
        {
            //if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (GUIManagerBase.instance.IsGUIOnTop(handler.guiName) == false)
                    {
                        return;
                    }

                    if (onBackKeyAndroid != null)
                    {
                        onBackKeyAndroid.Invoke();
                    }
                }
            }
        }
    }
}
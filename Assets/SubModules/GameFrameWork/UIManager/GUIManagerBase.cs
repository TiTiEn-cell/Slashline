using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

namespace GFramework.GUI
{
    public abstract class GUIManagerBase : SingletonMono<GUIManagerBase>
    {
        public enum EScreenType
        {
            Portrait, Landscape
        }

        public class PopupQueueData
        {
            public int Index;
            public object[] Datas;
        }

        public Camera guiCamera = null;

        public EventSystem eventSystem;

        public EScreenType ScreenType;

        public List<GUIHandlerBase> listHandler = new List<GUIHandlerBase>();

        [HideInInspector]
        public string prefabPath = "Prefabs/GUI";

        [HideInInspector]
        public string destPrefabPath = "Resources/Prefabs/GUI";

        [HideInInspector]
        public string destSourcePath = "Scripts/GUI";

        [HideInInspector]
        public string newGuiName = "";

        [HideInInspector]
        public string destPath = "Scripts";

        [HideInInspector]
        public string destName = "GUIName.cs";

#if UNITY_EDITOR
        //Create GUI with template
        [HideInInspector]
        public bool createWithTemplate = false;

        [HideInInspector]
        public GUIBase template = null;
#endif

        private Queue<PopupQueueData> popupQueues = new Queue<PopupQueueData>();
        private List<int> popupQueueShowed = new List<int>();

        public bool HasGUIShowed => listHandler.Any(x => x.IsShowed());

        public T GetHandler<T>(int index) where T : GUIHandlerBase
        {
            if (listHandler.Count <= index || index < 0)
                return null;
            if (listHandler[index] is T)
                return (T)listHandler[index];
            return null;
        }

        public T GetGUI<T>(int index) where T : GUIBase
        {
            if (listHandler.Count <= index || index < 0)
                return null;
            if (listHandler[index] == null)
                return null;
            return listHandler[index].GetGUI<T>();
        }

        public void ShowGUI(int index, params object[] @parameter)
        {
            if (listHandler.Count <= index || index < 0)
                return;
            if (listHandler[index] == null)
                return;

            listHandler[index].Show(@parameter);
        }

        public void HideGUI(int index, params object[] @parameter)
        {
            if (popupQueueShowed.Contains(index))
            {
                popupQueueShowed.Remove(index);
            }

            if (listHandler.Count <= index || index < 0)
                return;

            if (listHandler[index] == null)
                return;

            listHandler[index].Hide(@parameter);
        }

        public void ShowGUIQueue(int index, params object[] @parameter)
        {
            if (popupQueueShowed.Count > 0)
            {
                popupQueues.Enqueue(new PopupQueueData()
                {
                    Index = index,
                    Datas = @parameter
                });
            }
            else
            {
                popupQueueShowed.Add(index);
                ShowGUI(index, @parameter);
            }
        }

        public void HideGUIQueue(int index, params object[] @parameter)
        {

            HideGUI(index, @parameter);

            if (popupQueues.Count > 0)
            {
                var nextPopup = popupQueues.Dequeue();
                if (nextPopup != null)
                {
                    popupQueueShowed.Add(nextPopup.Index);
                    ShowGUI(nextPopup.Index, nextPopup.Datas);
                }
            }
        }

        public virtual void CloseAll()
        {
            popupQueues.Clear();
            popupQueueShowed.Clear();

            if (listHandler.Count == 0) return;

            for (int i = 0; i < listHandler.Count; i++)
            {
                var handler = listHandler[i];
                if ((handler.status != GUIStatus.Showed) && (handler.status != GUIStatus.Showing)) continue;

                handler.Hide();
            }
        }

        public virtual void CloseAll(List<int> listExceptIndex)
        {
            popupQueues.Clear();
            popupQueueShowed.Clear();

            if (listHandler.Count == 0) return;

            for (int i = 0; i < listHandler.Count; i++)
            {
                if (listExceptIndex.Contains(i)) continue;

                var handler = listHandler[i];
                if ((handler.status != GUIStatus.Showed) && (handler.status != GUIStatus.Showing)) continue;

                handler.Hide();
            }
        }


        public virtual void CloseAll(int exceptIndex)
        {
            popupQueues.Clear();
            popupQueueShowed.Clear();

            if (listHandler.Count == 0) return;

            for (int i = 0; i < listHandler.Count; i++)
            {
                if (i == exceptIndex) continue;

                var handler = listHandler[i];
                if ((handler.status != GUIStatus.Showed) && (handler.status != GUIStatus.Showing)) continue;

                handler.Hide();
            }
        }

        public bool IsShowed(int index)
        {
            if (listHandler.Count <= index || index < 0)
                return false;
            if (listHandler[index] == null)
                return false;
            return listHandler[index].IsShowed();
        }

        public bool IsGUI(int index, GUIBase gUIBase)
        {
            if (listHandler.Count <= index || index < 0)
                return false;
            if (listHandler[index] == null)
                return false;
            return listHandler[index] == gUIBase.handler;
        }

        public bool IsGUIOnTop(string guiName) //note: need check show hide.
        {
            if (string.IsNullOrEmpty(guiName))
            {
                return false;
            }

            int layerThis = 0;
            int highedLayer = 0;

            for (int i = 0; i < listHandler.Count; i++)
            {
                if (listHandler[i].guiName == guiName)
                {
                    layerThis = listHandler[i].guiCanvas.sortingOrder;
                }
            }

            for (int i = 0; i < listHandler.Count; i++)
            {
                if (IsShowed(i) == false)
                {
                    continue;
                }

                if (listHandler[i].guiCanvas == null)
                {
                    continue;
                }

                if (listHandler[i].processBackKey == false)
                {
                    continue;
                }

                int layer = listHandler[i].guiCanvas.sortingOrder;
                if (layer > highedLayer)
                {
                    highedLayer = layer;
                }
            }

            if (layerThis == highedLayer)
            {
                Debug.Log("--layer gui on top : " + layerThis);
                return true;
            }

            return false;
        }
    }
}
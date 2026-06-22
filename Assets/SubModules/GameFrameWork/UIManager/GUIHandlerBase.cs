using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GFramework.GUI
{
    [System.Serializable]
    public enum GUIHideAction
    {
        Disable = 0,
        Destroy = 1,
    }

    [System.Serializable]
    public enum GUIAnimationType
    {
        None,
        AnimationClip,
        Scripting
    }

    [System.Serializable]
    public enum GUIStatus
    {
        Invalid = 0,
        Ready = 1,
        Showing = 2,
        Showed = 3,
        Hiding = 4,
        Hidden = 5,
    }

    [System.Serializable]
    public enum GUIAnimShowHide
    {
        Direct = 0,
        MoveHorizontal = 1,
        MoveVertical = 2,
        RotateX = 3,
        RotateY = 4,
        RotateZ = 5,
        Zoom = 6,
    }

    public class GUIHandlerBase : MonoBehaviour
    {
        public GameObject guiPrefabObj;

        public bool processBackKey = true;

        public string guiName = "";

        public Camera guiCamera = null;

        [Space(15)]
        [Header("Transistion animation")]
        public GUIAnimationType guiAnimType;

        public GUIAnimShowHide showAnim = GUIAnimShowHide.Direct;

        public AnimationCurve showCurve;

        public GUIAnimShowHide hideAnim = GUIAnimShowHide.Direct;

        public AnimationCurve hideCurve;

        public AnimationClip showAnimClip;

        public AnimationClip hideAnimClip;
        [Space(15)]

        public GUIHideAction hideAction = GUIHideAction.Disable;

        //[HideInInspector]
        public GUIStatus status = GUIStatus.Invalid;

        [HideInInspector]
        public GameObject guiObject;

        public GUIBase gui;

        [ReadOnly]
        public Canvas guiCanvas;

        [ReadOnly]
        public GameObject rootGUI = null;

        private IEnumerator showingGUI = null;

        private IEnumerator hiddingGUI = null;

        private List<Vector3> oPosition = null;

        private List<Vector3> oEulerAngles = null;

        private List<Vector3> oScale = null;

        public event Action onBeginShowing = null;

        public event Action onEndShowing = null;

        public event Action onBeginHidding = null;

        public event Action onEndHidding = null;

        void OnEnable()
        {
            //Debug.Log("<color=yellow>Status " + name + " : " + status + "</color>");
            if (gui != null)
            {
                if (gui.handler != null)
                    gui.handler = null;

                if (onBeginShowing != null)
                    onBeginShowing = null;
                if (onEndShowing != null)
                    onEndShowing = null;
                if (onBeginHidding != null)
                    onBeginHidding = null;
                if (onEndHidding != null)
                    onEndHidding = null;

                gui.handler = this;
                onBeginShowing += gui.OnBeginShowing;
                onEndShowing += gui.OnEndShowing;
                onBeginHidding += gui.OnBeginHidding;
                onEndHidding += gui.OnEndHidding;
            }
        }

        public T GetGUI<T>() where T : GUIBase
        {
            if (gui != null)
                return (T)gui;

            return null;
        }

        public virtual GameObject InitExc()
        {
            try
            {
                if (rootGUI == null)
                {
					rootGUI = GameObject.Find("GUIRoot");
                }

                if (rootGUI == null)
                {
                    rootGUI = new GameObject();
                    rootGUI.name = "GUIRoot";
                }

                //Debug.Log("Init GUI " + prefabPath + "/" + guiPrefab);
#if UNITY_EDITOR
                GameObject obj = PrefabUtility.InstantiatePrefab(guiPrefabObj) as GameObject;
#else
                GameObject obj = GameObject.Instantiate(guiPrefabObj) as GameObject;
#endif

                obj.transform.SetParent(rootGUI.transform);

                gui = obj.GetComponentInChildren<GUIBase>();
                if (gui != null)
                {
                    if (gui.handler != null)
                        gui.handler = null;

                    if (onBeginShowing != null)
                        onBeginShowing = null;
                    if (onEndShowing != null)
                        onEndShowing = null;
                    if (onBeginHidding != null)
                        onBeginHidding = null;
                    if (onEndHidding != null)
                        onEndHidding = null;

                    gui.handler = this;
                    onBeginShowing += gui.OnBeginShowing;
                    onEndShowing += gui.OnEndShowing;
                    onBeginHidding += gui.OnBeginHidding;
                    onEndHidding += gui.OnEndHidding;
                }

                guiCanvas = obj.GetComponentInChildren<Canvas>();

                if (guiCanvas != null)
                {
                    guiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    guiCanvas.worldCamera = guiCamera;
                }
                return obj;
            }
            catch (Exception ex)
            {
                Debug.LogError("Error:" + ex.Message);
            }
            return null;
        }

        public bool Init()
        {
            if (status >= GUIStatus.Ready &&  gui != null)
                return true;

            status = GUIStatus.Invalid;

            try
            {
                guiObject = InitExc();
                if (guiObject != null)
                {
                    status = GUIStatus.Ready;

                    //Get old rectTransform
                    if (oPosition != null)
                        oPosition.Clear();
                    if (oEulerAngles != null)
                        oEulerAngles.Clear();
                    if (oScale != null)
                        oScale.Clear();

                    oPosition = new List<Vector3>();
                    oEulerAngles = new List<Vector3>();
                    oScale = new List<Vector3>();

                    foreach (Transform tran in guiCanvas.transform)
                    {
                        RectTransform rectTrans = tran.gameObject.GetComponent<RectTransform>();
                        if (rectTrans != null)
                        {
                            oPosition.Add(new Vector3(rectTrans.localPosition.x, rectTrans.localPosition.y, rectTrans.localPosition.z));
                            oEulerAngles.Add(new Vector3(rectTrans.localEulerAngles.x, rectTrans.localEulerAngles.y, rectTrans.localEulerAngles.z));
                            oScale.Add(new Vector3(rectTrans.localScale.x, rectTrans.localScale.y, rectTrans.localScale.z));
                        }
                    }
                }

                if (guiAnimType == GUIAnimationType.AnimationClip)
                {
                    if (showAnimClip != null)
                    {
                        gui.animController.AddClip(showAnimClip, "showAnimClip");
                    }

                    if (hideAnimClip != null)
                    {
                        gui.animController.AddClip(hideAnimClip, "hideAnimClip");
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Init GUI panel - Init " + this.GetType().Name + "Exception - " + ex);
            }

            return false;
        }

        public virtual bool Show(params object[] @parameter)
        {
            if (status == GUIStatus.Invalid || gui== null)
            {
                Init();
            }

            switch (status)
            {
                case GUIStatus.Ready:
                case GUIStatus.Hidden:
                    if (guiAnimType == GUIAnimationType.None)
                    {
                        status = GUIStatus.Showed;
                        guiObject.SetActive(true);
                        gui.Show(@parameter);
                        if (onBeginShowing != null)
                            onBeginShowing();

                        if (onEndShowing != null)
                            onEndShowing();
                    }
                    else
                    {
                        status = GUIStatus.Showing;

                        showingGUI = RunGUIAnimation(true, showAnim, showCurve, @parameter);
                        StartCoroutine(showingGUI);
                    }
                    return true;
                case GUIStatus.Showing:
                case GUIStatus.Showed:
                    return true;
                case GUIStatus.Hiding:
                    status = GUIStatus.Showing;

                    if (hiddingGUI != null)
                    {
                        StopCoroutine(hiddingGUI);
                        hiddingGUI = null;
                    }
                    showingGUI = RunGUIAnimation(true, showAnim, showCurve, @parameter);
                    StartCoroutine(showingGUI);
                    return true;
                default:
                    Debug.Log("Invalid status!");
                    return false;
            }
        }

        public virtual void Hide(params object[] @parameter)
        {
            if (guiObject == null)
                return;

            switch (status)
            {
                case GUIStatus.Ready:
                case GUIStatus.Showed:
                    status = GUIStatus.Hiding;

                    hiddingGUI = RunGUIAnimation(false, hideAnim, hideCurve, @parameter);
                    StartCoroutine(hiddingGUI);
                    return;
                case GUIStatus.Hiding:
                case GUIStatus.Hidden:
                    return;
                case GUIStatus.Showing:
                    status = GUIStatus.Hiding;

                    if (showingGUI != null)
                    {
                        StopCoroutine(showingGUI);
                        showingGUI = null;
                    }
                    hiddingGUI = RunGUIAnimation(false, hideAnim, hideCurve, @parameter);
                    StartCoroutine(hiddingGUI);
                    return;
                default:
                    Debug.Log("Invalid status!");
                    return;
            }
        }

        private IEnumerator RunGUIAnimation(bool isShowing, GUIAnimShowHide guiAnim, AnimationCurve animCurve, params object[] @parameter)
        {
            if (guiObject == null)
                yield break;

            if (isShowing)
            {
                guiObject.SetActive(true);
                gui.Show(@parameter);
                if (onBeginShowing != null)
                    onBeginShowing();

                GUIAnimation[] subAnims = guiObject.GetComponentsInChildren<GUIAnimation>();
                foreach (GUIAnimation anim in subAnims)
                {
                    if (anim.autoPlay)
                        anim.PlayGUIAnimation();
                }
            }
            else
            {
                if (onBeginHidding != null)
                    onBeginHidding();
            }

            if (Application.isPlaying)
            {
                float curveTime = 0.0f;
                List<RectTransform> listRectTrans = new List<RectTransform>();

                if (guiAnim != GUIAnimShowHide.Direct)
                {
                    if (animCurve.length < 2)
                        yield break;

                    //Get total time of curve
                    curveTime = animCurve[animCurve.length - 1].time - animCurve[0].time;

                    //Get all current rectTransform
                    foreach (Transform tran in guiCanvas.transform)
                    {
                        RectTransform rectTrans = tran.gameObject.GetComponent<RectTransform>();
                        if (rectTrans != null)
                            listRectTrans.Add(rectTrans);
                    }
                }

                float startTime = Time.time;
                float runTime = 0.0f;

                //do animation with clip
                if (guiAnimType == GUIAnimationType.AnimationClip)
                {
                    #region Animation Clip
                    if (isShowing)
                    {
                        if (gui.animController != null && showAnimClip != null)
                        {
                            gui.animController.clip = showAnimClip;
                            gui.animController.Play("showAnimClip");

                            yield return new WaitForSeconds(showAnimClip.length);
                        }
                    }
                    else
                    {
                        if (gui.animController != null && hideAnimClip != null)
                        {
                            gui.animController.clip = hideAnimClip;
                            gui.animController.Play("hideAnimClip");

                            yield return new WaitForSeconds(hideAnimClip.length);
                        }
                    }
                    #endregion
                }
                else if (guiAnimType == GUIAnimationType.Scripting)
                {
                    #region Animation scripting
                    //Animation scripting
                    switch (guiAnim)
                    {
                        case GUIAnimShowHide.Direct:
                            break;
                        case GUIAnimShowHide.MoveHorizontal:
                            if (oPosition != null && oPosition.Count == listRectTrans.Count)
                            {
                                Vector3 moveHorizontal = new Vector3(Screen.width, 0, 0);
                                while (runTime < curveTime)
                                {
                                    for (int i = 0; i < listRectTrans.Count; i++)
                                    {
                                        listRectTrans[i].localPosition = oPosition[i] + moveHorizontal * animCurve.Evaluate(runTime);
                                    }
                                    runTime = Time.time - startTime;
                                    yield return new WaitForSeconds(0.02f);
                                }

                                for (int i = 0; i < listRectTrans.Count; i++)
                                {
                                    listRectTrans[i].localPosition = oPosition[i];
                                }
                            }
                            break;
                        case GUIAnimShowHide.MoveVertical:
                            if (oPosition != null && oPosition.Count == listRectTrans.Count)
                            {
                                Vector3 moveVertical = new Vector3(0, Screen.height, 0);
                                while (runTime < curveTime)
                                {
                                    for (int i = 0; i < listRectTrans.Count; i++)
                                    {
                                        listRectTrans[i].localPosition = oPosition[i] + moveVertical * animCurve.Evaluate(runTime);
                                    }
                                    runTime = Time.time - startTime;
                                    yield return new WaitForSeconds(0.02f);
                                }

                                for (int i = 0; i < listRectTrans.Count; i++)
                                {
                                    listRectTrans[i].localPosition = oPosition[i];
                                }
                            }
                            break;
                        case GUIAnimShowHide.RotateX:
                            if (oEulerAngles != null && oEulerAngles.Count == listRectTrans.Count)
                            {
                                Vector3 rotX = new Vector3(90, 0, 0);
                                while (runTime < curveTime)
                                {
                                    for (int i = 0; i < listRectTrans.Count; i++)
                                    {
                                        listRectTrans[i].localEulerAngles = oEulerAngles[i] + rotX * animCurve.Evaluate(runTime);
                                    }
                                    runTime = Time.time - startTime;
                                    yield return new WaitForSeconds(0.02f);
                                }

                                for (int i = 0; i < listRectTrans.Count; i++)
                                {
                                    listRectTrans[i].localEulerAngles = oEulerAngles[i];
                                }
                            }
                            break;
                        case GUIAnimShowHide.RotateY:
                            if (oEulerAngles != null && oEulerAngles.Count == listRectTrans.Count)
                            {
                                Vector3 rotY = new Vector3(0, 90, 0);
                                while (runTime < curveTime)
                                {
                                    for (int i = 0; i < listRectTrans.Count; i++)
                                    {
                                        listRectTrans[i].localEulerAngles = oEulerAngles[i] + rotY * animCurve.Evaluate(runTime);
                                    }
                                    runTime = Time.time - startTime;
                                    yield return new WaitForSeconds(0.02f);
                                }

                                for (int i = 0; i < listRectTrans.Count; i++)
                                {
                                    listRectTrans[i].localEulerAngles = oEulerAngles[i];
                                }
                            }
                            break;
                        case GUIAnimShowHide.RotateZ:
                            if (oEulerAngles != null && oEulerAngles.Count == listRectTrans.Count)
                            {
                                Vector3 rotZ = new Vector3(0, 0, 90);
                                while (runTime < curveTime)
                                {
                                    for (int i = 0; i < listRectTrans.Count; i++)
                                    {
                                        listRectTrans[i].localEulerAngles = oEulerAngles[i] + rotZ * animCurve.Evaluate(runTime);
                                    }
                                    runTime = Time.time - startTime;
                                    yield return new WaitForSeconds(0.02f);
                                }

                                for (int i = 0; i < listRectTrans.Count; i++)
                                {
                                    listRectTrans[i].localEulerAngles = oEulerAngles[i];
                                }
                            }
                            break;
                        case GUIAnimShowHide.Zoom:
                            if (oScale != null && oScale.Count == listRectTrans.Count)
                            {
                                while (runTime < curveTime)
                                {
                                    for (int i = 0; i < listRectTrans.Count; i++)
                                    {
                                        listRectTrans[i].localScale = oScale[i] * animCurve.Evaluate(runTime);
                                    }
                                    runTime = Time.time - startTime;
                                    yield return new WaitForSeconds(0.02f);
                                }

                                for (int i = 0; i < listRectTrans.Count; i++)
                                {
                                    listRectTrans[i].localScale = oScale[i];
                                }
                            }
                            break;
                    }
                    #endregion
                }
                
                if (isShowing)
                {
                    if (onEndShowing != null)
                        onEndShowing();

                    status = GUIStatus.Showed;
                }
                else
                {
                    if (onEndHidding != null)
                        onEndHidding();
                }
            }

            if (!isShowing)
            {
                GUIAnimation[] subAnims = guiObject.GetComponentsInChildren<GUIAnimation>();
                foreach (GUIAnimation anim in subAnims)
                {
                    anim.ForceEndAnim();
                }

                switch (hideAction)
                {
                    case GUIHideAction.Disable:
                        if (!gui.selfHide)
                        {
                            gui.Hide(@parameter);
                        }

                        guiObject.SetActive(false);
                        status = GUIStatus.Hidden;
                        break;

                    case GUIHideAction.Destroy:
                        if (!gui.selfHide)
                        {
                            gui.Hide(@parameter);
                        }

                        guiObject.SetActive(false);
                        status = GUIStatus.Hidden;
                        StartCoroutine(DoDestroy());
                        break;
                }
            }
        }

        private IEnumerator DoDestroy()
        {
            if (guiObject == null)
                yield break;

            yield return new WaitForSeconds(0.2f);
            Destroy(guiObject);
            status = GUIStatus.Invalid;
        }

        public bool IsShowed()
        {
            return status == GUIStatus.Showed;
        }

#region For Editor
#if UNITY_EDITOR
        public void Load()
        {
            if (status == GUIStatus.Invalid)
            {
                if (gameObject.transform.Find(guiName) == null
                    || gameObject.transform.Find(guiName).gameObject.GetComponentInChildren<GUIHandlerBase>() == null)
                {
                    guiObject = InitExc();
                    status = GUIStatus.Ready;
                }
            }
        }

        public void UnLoad()
        {
            if (status != GUIStatus.Invalid)
            {
                if (guiObject != null)
                {
                    GameObject.DestroyImmediate(guiObject);
                    status = GUIStatus.Invalid;
                }
            }
        }

        public void ShowEditor()
        {
            if (guiObject == null)
            {
                Load();
            }

            if (guiObject != null)
            {
                guiObject.SetActive(true);
                status = GUIStatus.Showed;
            }
        }

        public void HideEditor()
        {
            if (guiObject == null)
                return;

            if (status != GUIStatus.Invalid)
            {
                switch (hideAction)
                {
                    case GUIHideAction.Disable:
                        guiObject.SetActive(false);
                        status = GUIStatus.Hidden;
                        break;

                    case GUIHideAction.Destroy:
                        guiObject.SetActive(false);
                        GameObject.DestroyImmediate(guiObject);
                        status = GUIStatus.Invalid;
                        break;
                }
            }
        }

        public void Reset()
        {
            if (guiObject != null)
            {
                GameObject.DestroyImmediate(guiObject);
            }
            status = GUIStatus.Invalid;
        }
#endif
#endregion
    }
}

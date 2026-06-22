using UnityEngine;
using UnityEngine.UI;

public class UIFitResolutionHelper : MonoBehaviour
{
    [SerializeField]
    private CanvasScaler rootCanvas;
    
    [SerializeField]
    private Vector2 screenSizeDefault = new Vector2(1080f,1920f);

    private float curWidth, curHeight;
    private float curRatio;
    private Vector3 firstScale;
    private RectTransform rootRect;

    public float CurRatio => curRatio;

    void Awake() 
    {
        if(rootCanvas == null) {
            rootCanvas = GetComponentInParent<CanvasScaler>();
        }
        
        rootRect = rootCanvas.gameObject.GetComponent<RectTransform>();
        firstScale = transform.localScale;
        CalScreenScale();
    }

    void Update()
    {
        if(curWidth != rootRect.sizeDelta.x || curHeight != rootRect.sizeDelta.y) {
            CalScreenScale();
        }
    }

    public void CalScreenScale() {

        curWidth = rootRect.sizeDelta.x;
        curHeight = rootRect.sizeDelta.y;
        curRatio = (curWidth/screenSizeDefault.x)/(curHeight/screenSizeDefault.y);
        transform.localScale = firstScale * Mathf.Max(1, curRatio);
    }
}

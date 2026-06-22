using UnityEngine;

public class FitResolutionHelper : MonoBehaviour
{
    [SerializeField]
    private Vector2 screenSizeDefault = new Vector2(1080f, 1920f);

    [SerializeField]
    private bool isMatchWidth = true;

    private float curWidth, curHeight;
    private float curRatio;

    public float CurRatio => curRatio;

    void Awake()
    {
        CalScreenScale();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (curWidth != Screen.width || curHeight != Screen.height)
        {
            CalScreenScale();
        }
    }
#endif

    public void CalScreenScale()
    {

        curWidth = Screen.width;
        curHeight = Screen.height;

        var isLandScape = curWidth > curHeight;
        var sizeScene = (float)curWidth / curHeight;

        if (isLandScape)
        {
            var size3_2 = 3 / 2f;
            isMatchWidth = sizeScene < size3_2;
        }
        else
        {
            var size2_3 = (2 / 3f);
            isMatchWidth = ((sizeScene + 0.1f) < size2_3);
        }

        curRatio = isMatchWidth ? (curWidth / screenSizeDefault.x) / (curHeight / screenSizeDefault.y) : 1f;
        transform.localScale = Vector3.one * curRatio;
    }
}

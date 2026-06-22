using UnityEngine;
using UnityEngine.UI;

public class CanvasHelper : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_WEBGL
    int _preWidth, _preHeight;
#endif

    void Awake()
    {
        var isLandScape = Screen.width > Screen.height;
		var sizeScene = (float)Screen.width / Screen.height;

		if (isLandScape)
		{
			var size3_2 = 3 / 2f;
			var isMatchHeight = sizeScene >= size3_2;
			var canvas = GetComponent<CanvasScaler>();
			canvas.matchWidthOrHeight = isMatchHeight ? 1f : 0f;
		}
        else
		{
			var size2_3 = (2 / 3f);
			var isMatchHeight = (sizeScene >= size2_3);
			var canvas = GetComponent<CanvasScaler>();
			canvas.matchWidthOrHeight = isMatchHeight ? 1f : 0f;
		}
    }

#if UNITY_EDITOR || UNITY_WEBGL
    void Update()
    {
        if (_preWidth != Screen.width || _preHeight != Screen.height)
        {
            _preWidth = Screen.width;
            _preHeight = Screen.height;
            Awake();
        }
    }
#endif
}


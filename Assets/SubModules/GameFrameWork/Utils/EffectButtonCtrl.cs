using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class EffectButtonCtrl : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler
{


    [SerializeField] List<Transform> objBtnScales = new List<Transform>();
    [SerializeField] List<Graphic> graphicColors = new List<Graphic>();
    [SerializeField] float duration = 0.2f;

    public ScaleOption scaleOption;
    public Vector3 scaleAdd = new Vector3(-0.1f, -0.1f, 0);
    [SerializeField] bool useConstantAlpha = false;
    [SerializeField] bool playSound = true;
    [SerializeField] float currentA = 1f;

    private bool useAlpha = false;
    private bool isBlock = false;

    private Vector3 currentScale;

    private Color targetColor = new Color(0.4622642f, 0.4622642f, 0.4622642f, 1f);
    private Color defaultColor = Color.white;

    void Start()
    {
        Init();
    }

    public void OnMyPress()
    {
        Vector3 nextScale = currentScale + scaleAdd;

        foreach (var item in objBtnScales)
        {
            item.DOKill();
            item.DOScale(nextScale, duration).SetEase(Ease.OutBounce).PlayForward();
        }

        OnMyEnter();
        PlayBtnPress();
    }

    private void PlayBtnPress()
    {
        if (!playSound) return;
        AudioController.instance.PlaySFX(SoundName.BtnPress);
    }

    public void OnMyReturn()
    {
        foreach (var item in objBtnScales)
        {
            item.DOKill();
            item.DOScale(currentScale, duration).PlayForward();
        }

        if (isBlock) return;
        OnMyExit();
    }

    public void OnMyEnter()
    {
        if (!useAlpha) return;

        if (graphicColors == null || graphicColors.Count == 0) return;

        foreach (var item in graphicColors)
        {
            item.DOKill();
            item.DOColor(targetColor, duration).PlayForward();
        }
    }

    public void OnMyExit()
    {
        if (!useAlpha) return;

        if (graphicColors == null || graphicColors.Count == 0) return;

        foreach (var item in graphicColors)
        {
            item.DOKill();
            item.DOColor(defaultColor, duration).PlayForward();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isBlock) return;
        OnMyPress();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnMyReturn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isBlock) return;
        OnMyReturn();
    }

    private void Reset()
    {
#if UNITY_EDITOR
        GetElement();
#endif
    }

    public void Init()
    {
        if (objBtnScales == null || objBtnScales.Count == 0)
        {
            objBtnScales = new List<Transform>() { transform };
        }

        if (scaleOption != null && scaleOption.useFixedValue)
        {
            currentScale = scaleOption.fixedValue;
        }
        else
        {
            if (objBtnScales.Count > 0)
            {
                currentScale = objBtnScales[0].localScale;
            }
        }

        if (graphicColors == null)
        {
            graphicColors = new List<Graphic>() { transform.GetComponent<Graphic>() };
        }

        if (!useConstantAlpha && graphicColors != null && graphicColors.Count > 0)
        {
            currentA = graphicColors[0].color.a;
        }

    }

    public void GetElement()
    {
        var list1 = GetComponentsInChildren<Graphic>(true);
        graphicColors.Clear();
        objBtnScales.Clear();
        if (list1.Count() > 0)
        {
            objBtnScales.Add(list1[0].transform);

        }
        foreach (var obj in list1)
        {
            graphicColors.Add(obj);
        }
    }


    public void UpdateCurrentAlpha(float alpha)
    {
        currentA = alpha;
    }

    public void SetBlockBtn(bool _isBlock)
    {
        isBlock = _isBlock;

        if (_isBlock)
        {
            OnMyExit();
            OnMyEnter();
        }
        else
        {
            OnMyExit();
        }

    }

    [Serializable]
    public class ScaleOption
    {
        public Vector3 fixedValue;
        public bool useFixedValue;
    }
}


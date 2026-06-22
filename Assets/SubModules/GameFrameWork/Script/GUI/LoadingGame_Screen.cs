using UnityEngine;
using GFramework.GUI;
using UnityEngine.UI;
using System;
using TMPro;

public class LoadingGame_Screen : GUIBase
{
    public Slider progressSlider;
    public TextMeshProUGUI VersionText;
    public TextMeshProUGUI LoadingText;
    public TextMeshProUGUI LoadingPercent;

    private Data data;
    private float currentTime = 0;

    public override bool Show(params object[] @parameter)
    {
        OnProgressChanged(0f);

        if (parameter != null && parameter.Length > 0)
        {
            data = parameter[0] as Data;
        }
        if (VersionText != null) VersionText.text = "v." + Application.version;
        currentTime = 0;

        return base.Show(@parameter);
    }

    public override void Hide(params object[] @parameter)
    {
        base.Hide(@parameter);
    }

    public override void Update()
    {
        base.Update();

        if (data != null && data.GetProgress != null)
        {
            var pc = data.GetProgress();
            pc = Mathf.Clamp01(pc);
            OnProgressChanged(pc);
        }

        currentTime += Time.deltaTime * 2f;
        AnimTitle((int)currentTime);
        if (currentTime > 3)
        {
            currentTime = 0f;
        }
    }

    private void OnProgressChanged(float value)
    {
        var percent = (int)(value * 100);
        if (LoadingPercent != null) LoadingPercent.text = $"{percent}%";
        if (progressSlider != null) progressSlider.value = value;
    }

    private void AnimTitle(int count)
    {
        var tt = "Loading.";

        for (int i = 0; i < count; i++)
        {
            tt += ".";
        }

        if (LoadingText != null) LoadingText.text = tt;
    }

    public class Data
    {
        public Func<float> GetProgress;
    }
}

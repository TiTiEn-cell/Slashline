using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimControl : MonoBehaviour
{
    [SerializeField] AnimClipSO ShowAnim;
    [SerializeField] AnimClipSO HideAnim;

    [SerializeField] List<Animator> listAnimControl;

    public void Show()
    {
        foreach (var anim in listAnimControl)
        {
            anim.Rebind();
            anim.Play(ShowAnim.GetAnimHashCode());
        }
    }

    public void Hide()
    {
        foreach (var anim in listAnimControl)
        {
            anim.Rebind();
            anim.Play(HideAnim.GetAnimHashCode());
        }
    }
}

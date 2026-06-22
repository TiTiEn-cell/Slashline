using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayLevelUI : MonoBehaviour
{
    [SerializeField] Transform hardLevelIconTrans;
    [SerializeField] Animator animator;
    [SerializeField] AnimClipSO normalLevelAnim;
    [SerializeField] AnimClipSO hardLevelAnim;

    public Transform GetHardLevelIconTrans()
    {
        return hardLevelIconTrans;
    }

    public void PlayAnim(bool isHard = false)
    {
        animator.Rebind();
        animator.Play(isHard ? hardLevelAnim.GetAnimHashCode() : normalLevelAnim.GetAnimHashCode());
    }
}

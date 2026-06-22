using UnityEngine;

public class AnimClipSO : ScriptableObject
{
    [SerializeField] AnimationClip animationClip;
    [SerializeField] private int animHash;
    [SerializeField] private float duration;

    public virtual int GetAnimHashCode()
    {
        return animHash;
    }

    public virtual float GetAnimDuration()
    {
        return duration;
    }

    private void OnValidate()
    {
        animHash = Animator.StringToHash(animationClip.name);
        duration = animationClip.length;
    }
}


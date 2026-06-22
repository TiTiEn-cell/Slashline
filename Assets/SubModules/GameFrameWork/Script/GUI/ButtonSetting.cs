using UnityEngine;

public class ButtonSetting : MonoBehaviour
{
    public Animator Anim;
    private int currentState = -1;
    public void SetState(int state)
    {
        if (currentState == state) return;
        currentState = state;
        if (state == 1)
        {
            Anim.Play(AnimName.ButtonSettingsOn);
        }
        else
        {
            Anim.Play(AnimName.ButtonSettingsOff);
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerCallback : MonoBehaviour
{
    [Serializable]
    public class Collider2DEvent : UnityEvent<Collider2D>
    {
    }

    public Collider2DEvent OnTriggerEnter2DEvent;
    public Collider2DEvent OnTriggerExit2DEvent;
    public Collider2DEvent OnTriggerStay2DEvent;

    void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnter2DEvent?.Invoke(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerExit2DEvent?.Invoke(collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerStay2DEvent?.Invoke(collision);
    }
}

using System;
using UnityEngine;

public class MatchableSurround_Dettector : MonoBehaviour
{
    public Action<Collider2D> OnTriggerEnter2D_Action;
    public Action<Collider2D> OnTriggerStay2D_Action;
    public Action<Collider2D> OnTriggerExit2D_Action;

    // Unused
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    OnTriggerEnter2D_Action?.Invoke(collision);
    //}

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerStay2D_Action?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerExit2D_Action?.Invoke(collision);
    }

}

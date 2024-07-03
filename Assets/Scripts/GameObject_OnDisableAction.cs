using System;
using UnityEngine;

/// <summary>
/// Class used on GameObjects to dettect when it disables himself
/// </summary>
public class GameObject_OnDisableAction : MonoBehaviour
{
    public Action<GameObject> onGameObject_Disables;

    private void OnDisable()
    {
        onGameObject_Disables?.Invoke(this.gameObject);
    }
}

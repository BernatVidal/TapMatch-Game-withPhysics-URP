using System;
using UnityEngine;

/// <summary>
/// Class used on objects with unlooped ParticleSystes, to dettect when it stops
/// </summary>
public class ParticleSystem_OnStopAction : MonoBehaviour
{
    public Action<GameObject> onParticleStystem_Stops;

    void Start()
    {
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    public void OnParticleSystemStopped()
    {
        onParticleStystem_Stops?.Invoke(this.gameObject);
    }
}
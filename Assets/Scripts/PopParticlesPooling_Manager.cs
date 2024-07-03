using UnityEngine;

public class PopParticlesPooling_Manager : MonoBehaviour
{
    #region Variables
    
    [SerializeField] GameObject _popParticleSystemPrefab;

    #endregion

    #region Unity Methods

    private void Start()
    {
        /// Subscribe to events
        GameEvents.Instance.onMatchablePop += SpawnParticle;

        SetPool();
    }

    private void OnDisable()
    {
        /// UnSubscribe to events
        GameEvents.Instance.onMatchablePop -= SpawnParticle;
    }
    #endregion

    #region Methods

    void SetPool()
    {
        PoolInfo popParticlesPool = new PoolInfo(PoolObjectType.MatchablePopPartycleSystem, 5, _popParticleSystemPrefab, this.transform);
        PoolsManager.Instance.FillPool(popParticlesPool);
    }


    /// Spawns a Particle at the matchable position and scale, set it's color, and start listening for the end of the Particle System
    private void SpawnParticle(MatchableController matchable)
    {
        GameObject popParticleObj = PoolsManager.Instance.UnPoolObject(PoolObjectType.MatchablePopPartycleSystem);
        popParticleObj.transform.position = matchable.transform.position;
        popParticleObj.transform.localScale = matchable.transform.localScale;
        ParticleSystem.MainModule mainPS = popParticleObj.GetComponent<ParticleSystem>().main;
        mainPS.startColor = matchable.GetComponent<SpriteRenderer>().color;
        popParticleObj.GetComponent<ParticleSystem_OnStopAction>().onParticleStystem_Stops += DespawnParticle;
    }


    /// Called from Particle system callback action event
    private void DespawnParticle(GameObject particleObject)
    {
        particleObject.GetComponent<ParticleSystem_OnStopAction>().onParticleStystem_Stops -= DespawnParticle;
        PoolsManager.Instance.PoolObject(particleObject, PoolObjectType.MatchablePopPartycleSystem);
    }
    #endregion

}

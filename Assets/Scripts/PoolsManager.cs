using System.Collections.Generic;
using UnityEngine;


public enum PoolObjectType
{
    Matchable,
    MatchablePopPartycleSystem,
    PointsAnimText
}

public struct PoolInfo
{
    public PoolObjectType type;
    public int amount;
    public GameObject prefab;
    public Transform parentContainer;

    public Stack<GameObject> pool;

    public PoolInfo(PoolObjectType type, int amount, GameObject prefab, Transform parentContainer)
    {
        this.type = type;
        this.amount = amount;
        this.prefab = prefab;
        this.parentContainer = parentContainer;
        pool = new Stack<GameObject>();
    }
}


public class PoolsManager : MonoBehaviour
{
    #region Variables
    public static PoolsManager Instance { get; private set; }

    HashSet<PoolInfo> _listOfPools = new HashSet<PoolInfo>();

    #endregion

    #region Unity Methods
    private void Awake()
    {
        /// Singleton Pattern
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// Given the Pool info or pool parameters, it will generate a new Pool or fill an existing one
    /// </summary>
    public void FillPool(PoolInfo info)
    {
        _listOfPools.Add(info);
        for(int i = 0; i < info.amount; i++)
        {
            GameObject objInstance = Instantiate(info.prefab, info.parentContainer);
            PoolObject(objInstance, info.type);
        }
    }


    /// <summary>
    /// Give an object to be pooled
    /// </summary>
    public void PoolObject(GameObject go, PoolObjectType type)
    {
        go.SetActive(false);

        PoolInfo selectedPool = GetPoolByType(type);

        if (!selectedPool.pool.Contains(go))
            selectedPool.pool.Push(go);
    }

    /// <summary>
    /// Returns a object unpooled or creates a new one if required
    /// </summary>
    public GameObject UnPoolObject(PoolObjectType type)
    {
        PoolInfo selectedPool = GetPoolByType(type);

        GameObject objInstance = null;
        /// Unpool object if pool is not empty
        if (selectedPool.pool.Count > 0)
        {
            objInstance = selectedPool.pool.Pop();
            objInstance.SetActive(true);
        }    
        /// Else Instantiate a new Object 
        else
            objInstance = Instantiate(selectedPool.prefab, selectedPool.parentContainer);

        return objInstance;
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Returns a Pool by the type requested
    /// </summary>
    private PoolInfo GetPoolByType(PoolObjectType type)
    {
        foreach(PoolInfo pool in _listOfPools)
        {
            if (pool.type == type)
                return pool;
        }
        return default(PoolInfo);
    }

    #endregion
}

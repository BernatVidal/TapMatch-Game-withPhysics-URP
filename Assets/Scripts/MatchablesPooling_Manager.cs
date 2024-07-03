using System.Collections;
using UnityEngine;

public class MatchablesPooling_Manager : MonoBehaviour
{
    #region Variables

    int _amountOfMatchables = 0;
    int _matchablesOnPosition = 0;

    [SerializeField] Transform _matchablesSpawnZone;
    [SerializeField] GameObject _matchablePrefab;
    [SerializeField] MatchablePreset[] _matchablePresets;
    MatchablePreset[] _matchablesUsedThisGame;

    #endregion


    #region Unity Methods

    private void Start()
    {
        /// Subscribe to events
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        /// Unsubscribe to events
        UnSubscribeToEvents();
    }

    #endregion


    #region Public Methods

    /// <summary>
    /// According to varietyOfMatchables, this method will set MatchablesUsedThisGame array from the randomized Matchable available Presets
    /// </summary>
    public void SetMatchablesForThisGame(int varietyOfMatchables)
    {
        /// Shuffle the available Matchable presets for random purposes
        for (int t = 0; t < _matchablePresets.Length; t++)
        {
            MatchablePreset tmp = _matchablePresets[t];
            int r = Random.Range(t, _matchablePresets.Length);
            _matchablePresets[t] = _matchablePresets[r];
            _matchablePresets[r] = tmp;
        }

        /// Get the amount matchables required
        _matchablesUsedThisGame = new MatchablePreset[varietyOfMatchables];
        for (int i = 0; i < _matchablesUsedThisGame.Length; i++)
        {
            _matchablesUsedThisGame[i] = _matchablePresets[i];
        }
    }


    /// <summary>
    /// Spawn Matchables according to the Grid already created
    /// </summary>
    public void SpawnMatchables(Transform[,] gridPositions, int cellSize)
    {
        /// Set the amount of matchables
        _amountOfMatchables = gridPositions.Length;

        /// Generate PoolInfo and start a Pool
        PoolInfo matchablesPool = new PoolInfo(PoolObjectType.Matchable, _amountOfMatchables, _matchablePrefab, this.transform);
        PoolsManager.Instance.FillPool(matchablesPool);

        /// Spawn Matchables
        for (int col = 0; col < gridPositions.GetLength(0); col++)
        {
            for (int row = 0; row < gridPositions.GetLength(1); row++)
            {
                /// Get a Matchable object from pool, and set its values with a random matchable from the used on this game
                GameObject matchableObj = PoolsManager.Instance.UnPoolObject(PoolObjectType.Matchable);
                matchableObj.transform.localScale *= (float)cellSize / (100 * 2);
                SpawnRandomMatchableAtDesiredPosition(matchableObj, gridPositions[col, row].position);
            }
        }
    }

    #endregion


    #region PrivateMethods

    /// <summary>
    /// Will Spawn the desired MatchableObject at the relative position of the Matchable's Spawn Zone
    /// </summary>
    void SpawnRandomMatchableAtDesiredPosition(GameObject matchable, Vector2 spawnPosition)
    {
        matchable.GetComponent<MatchableController>().SetMatchablePreset(GetARandomMatchablePreset());
        matchable.transform.position = new Vector2(spawnPosition.x, spawnPosition.y + _matchablesSpawnZone.position.y);
    }

    /// <summary>
    /// Returns a Random Matchable Preset from the ones used on the Game
    /// </summary>
    MatchablePreset GetARandomMatchablePreset()
    {
        return _matchablesUsedThisGame[Random.Range(0, _matchablesUsedThisGame.Length)];
    }

    /// <summary>
    /// All Matchables will turn on their surrounding dettectors to know the Matchables thei are surrounded by, until they are turned off by theirselfs
    /// </summary>
    void ActivateMatchablesSurroundDettection()
    {
        MatchableController[] matchables = GetComponentsInChildren<MatchableController>();
        for(int i = 0; i< matchables.Length; i++)
        {
            matchables[i].EnableSurroundDettectors(true);
        }
    }

    #endregion


    #region Events Methods


    /// If a Matchable pops, respawn it
    void OnMatchablePops(MatchableController matchable)
    {
        --_matchablesOnPosition;
        StartCoroutine(RespawnMatchable(matchable));
    }


    /// Despawn, wait and re-spawn Matchable
    IEnumerator RespawnMatchable(MatchableController matchable)
    {
        /// Despawn Object
        PoolsManager.Instance.PoolObject(matchable.gameObject, PoolObjectType.Matchable);

        /// Wait for pop animations to happen
        yield return new WaitForEndOfFrame();

        /// Spawn Matchable with a different preset
        GameObject matchableObj = PoolsManager.Instance.UnPoolObject(PoolObjectType.Matchable);
        SpawnRandomMatchableAtDesiredPosition(matchableObj, matchableObj.transform.position);
    }

    /// Checks if all are in position, if so, call the event OnAllMatchablesOnPosition, and temporary activate matchables surround dettectors
    void OnMatchableFindsPosition()
    {
        if (++_matchablesOnPosition == _amountOfMatchables)
        {
            GameEvents.Instance.OnAllMatchablesOnPosition();
            ActivateMatchablesSurroundDettection();
        }
    }

    void OnPositionedMatchableMoves()
    {
        --_matchablesOnPosition;
    }


    void SubscribeToEvents()
    {
        GameEvents.Instance.onMatchablePop += OnMatchablePops;
        GameEvents.Instance.onMatchableInPosition += OnMatchableFindsPosition;
        GameEvents.Instance.onPositionedMatchableMoves += OnPositionedMatchableMoves;
    }

    void UnSubscribeToEvents()
    {
        GameEvents.Instance.onMatchablePop -= OnMatchablePops;
        GameEvents.Instance.onMatchableInPosition -= OnMatchableFindsPosition;
        GameEvents.Instance.onPositionedMatchableMoves -= OnPositionedMatchableMoves;
    }

    #endregion

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables

    public const int MATCHABLES_LAYER = 0;
    public const int GRID_LAYER = 5;
    public const float MAX_VERTICAL_VELOCITY = 4.5f;

    public static bool CanUserInteract;

    [Header("Game Configuration")]
    [Range(5, 20)] [SerializeField] int _cols = 5;
    [Range(5, 20)] [SerializeField] int _rows = 5;
    [Range(3, 6)] [SerializeField] int _varietyOfMatchables = 3;

    [Header("UI Settings")]
    [SerializeField] UIManager _uiManager;

    [Header("Matchable Settings")]
    [SerializeField] MatchablesPooling_Manager _matchablesManager;

    [Header("Grid Settings")]
    [SerializeField] GridController _gridController;

    #endregion


    #region Unity Methods

    private void Start()
    {
        /// Subscribe to Events
        SubscribeToEvents();

        /// Populate the game (Grid and Matchables)
        StartCoroutine(PopulateGame());
    }


    private void OnDisable()
    {
        /// Unsubscribe to Events
        UnSubscribeToEvents();
    }

    #endregion


    #region Startup Methods

    IEnumerator PopulateGame()
    {
        /// Generate the Grid (And wait for the Canvas to set its Grid layout elements)
        _gridController.GenerateGrid(_cols, _rows);
        yield return new WaitForEndOfFrame();

        /// Set the matchables this game will be using
        _matchablesManager.SetMatchablesForThisGame(_varietyOfMatchables);

        /// SPAWN!
        _matchablesManager.SpawnMatchables(_gridController.GridPositions, _gridController.CellSize);
    }

    void SubscribeToEvents()
    {
        GameEvents.Instance.onUserTouchMatchable += OnUserTouchesMatchable;
        GameEvents.Instance.onMatchablePop += OnMatchablePop;
        GameEvents.Instance.onAllMatchablesOnPosition += TempPlayerCanMove;
    }

    void UnSubscribeToEvents()
    {
        GameEvents.Instance.onUserTouchMatchable -= OnUserTouchesMatchable;
        GameEvents.Instance.onMatchablePop -= OnMatchablePop;
        GameEvents.Instance.onAllMatchablesOnPosition -= TempPlayerCanMove;
    }
    #endregion


    #region Event Methods

    void OnUserTouchesMatchable(MatchableController matchable)
    {
        HashSet<MatchableController> matches = DFS_MatchablesFounder.GetSelectedSurroundMatches(matchable);
        int points = 0;

        foreach(MatchableController m in matches)
        {
            m.PopMatchable();
            points += m.Points;
        }
        _uiManager.AddPoints(points, matches.Count);
    }

    void OnMatchablePop(MatchableController matchable)
    {
        CanUserInteract = false;
    }

    /// Triggered when all Matchables are in position
    void TempPlayerCanMove()
    {
        CanUserInteract = true;
    }

    #endregion
}

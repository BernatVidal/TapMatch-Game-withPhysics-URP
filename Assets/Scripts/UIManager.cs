using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Variables

    const int _POINTS_ANIM_TEXT_SIZE = 30;
    const int _MULTIPLIER_ANIM_TEXT_SIZE = 65;

    int _touches;
    [SerializeField] TextMeshProUGUI _touchesTxt;
    int _points;
    [SerializeField] TextMeshProUGUI _pointsTxt;

    [SerializeField] Transform _pointsAnim_Parent;
    [SerializeField] GameObject _pointsAnim_Prefab;

    private Queue<IEnumerator> _showPointsAndMultiplier_CoroutineQueue = new Queue<IEnumerator>();
    
    #endregion

    #region Unity Methods

    void Start()
    {
        /// Set touches and points
        _touches = -1;
        AddTouches();
        _points = 0;
        AddPoints(0,0);

        /// Set Pool
        SetPointsAndMultipliersFeedbackPool();
        /// Subscribe Matchable touch event
        GameEvents.Instance.onUserTouchMatchable += AddTouches;
    }

    private void OnDisable()
    {
        /// Unsubscribe to Matchable touch event
        GameEvents.Instance.onUserTouchMatchable -= AddTouches;
    }

    #endregion

    #region Methods

    /// Add user touches to UI
    void AddTouches(MatchableController m = null)
    {
        ++_touches;
        _touchesTxt.text = _touches.ToString();
    }

    /// <summary>
    /// Add Points to UI. It required amount of points and amount of Matchables popped. 
    /// Queues the new received points on an animation to handle multiple consecutively.
    /// </summary>
    public void AddPoints(int points, int multiplier)
    {
        _points += points * multiplier;
        if (multiplier > 0)
            AddToQueue(ShowPointsAndMultiplier(points/multiplier, multiplier));
    }

    /// <summary>
    /// Show the Matchable points getted one by one, and then the multiplier in a bigger format.
    /// </summary>
    IEnumerator ShowPointsAndMultiplier(int pointsValue, int multValue)
    {
        int currentPoints = int.Parse(_pointsTxt.text);
        GameObject multiplier;
        /// Show points 1 by 1
        for (int i = 0; i < multValue; i++)
        {
            multiplier = PoolsManager.Instance.UnPoolObject(PoolObjectType.PointsAnimText);
            multiplier.GetComponent<TextMeshProUGUI>().text = $"+{pointsValue}";
            multiplier.GetComponent<TextMeshProUGUI>().fontSize = _POINTS_ANIM_TEXT_SIZE;
            multiplier.GetComponent<GameObject_OnDisableAction>().onGameObject_Disables += HideMultiplier;
            yield return new WaitForSeconds(0.75f/multValue);
            currentPoints += pointsValue;
            _pointsTxt.text = currentPoints.ToString();
        }

        yield return new WaitForSeconds(0.25f);

        /// Show multiplier, the bigger the value, the bigger size
        if (multValue > 1)
        {
            multiplier = PoolsManager.Instance.UnPoolObject(PoolObjectType.PointsAnimText);
            multiplier.GetComponent<TextMeshProUGUI>().text = $"x{multValue}";
            multiplier.GetComponent<TextMeshProUGUI>().fontSize = _MULTIPLIER_ANIM_TEXT_SIZE + (multValue/2);
            multiplier.GetComponent<GameObject_OnDisableAction>().onGameObject_Disables += HideMultiplier;

            /// Add it to points and text
            yield return new WaitForSeconds(1f);
            _pointsTxt.text = _points.ToString();
        }
    }

    /// Pool Points/Multiply feedback UI
    void HideMultiplier(GameObject multiplier)
    {
        multiplier.GetComponent<GameObject_OnDisableAction>().onGameObject_Disables -= HideMultiplier;
        PoolsManager.Instance.PoolObject(multiplier, PoolObjectType.PointsAnimText);
    }

    /// SetUp Pool
    void SetPointsAndMultipliersFeedbackPool()
    {
        /// Generate the PoolInfo
        PoolInfo multipliersPoolInfo = new PoolInfo(PoolObjectType.PointsAnimText, 10, _pointsAnim_Prefab, this.transform);
        PoolsManager.Instance.FillPool(multipliersPoolInfo);
    }

    /// <summary>
    /// Used to queue showpoints coroutines to avoid overlappings
    /// </summary>
    public void AddToQueue(IEnumerator coroutine)
    {
        _showPointsAndMultiplier_CoroutineQueue.Enqueue(coroutine);
        if (_showPointsAndMultiplier_CoroutineQueue.Count == 1) //no previous items in queue
            StartCoroutine(CoroutineCoordinator());
    }

    /// Coordinates Animation coroutines consecutively
    private IEnumerator CoroutineCoordinator()
    {
        while (_showPointsAndMultiplier_CoroutineQueue.Count > 0)
        {
            yield return StartCoroutine(_showPointsAndMultiplier_CoroutineQueue.Peek());
            _showPointsAndMultiplier_CoroutineQueue.Dequeue();
        }
    }
    #endregion
}

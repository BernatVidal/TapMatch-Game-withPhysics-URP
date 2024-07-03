using System.Collections.Generic;
using UnityEngine;

public class MatchableController : MonoBehaviour
{
    #region Variables

    Rigidbody2D _rb;
    FixedJoint2D _joint;

    enum matchableStatus
    {
        firstFall,
        firstBounce,
        secondFall,
        secondBounce,
        onPosition
    }
    matchableStatus _mStatus;

    string _matchableNameID;
    SpriteRenderer _matchableSprite;
    int _points;

    [SerializeField] MatchableSurround_Dettector _belowMatchableDettector;
    [SerializeField] MatchableSurround_Dettector _allSurroundsDettector;
    HashSet<MatchableController> _surroundMatchables;

    bool _positionedThisSpawn;
    public bool isCollidingAnotherMatchable;

    #endregion


    #region Properties
    public string MatchableNameID => _matchableNameID;
    public int Points => _points;
    #endregion


    #region Unity Methods
    private void Awake()
    {
        _matchableSprite = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _joint = GetComponent<FixedJoint2D>();
    }


    private void FixedUpdate()
    {
        /// Clamp RB velocity to avoid physics issues with too many bodies at the same time
        _rb.velocity = new Vector2(0, Mathf.Clamp(_rb.velocity.y, -GameManager.MAX_VERTICAL_VELOCITY, Mathf.Infinity));
        /// Decide Matchable State
        SetMatchableStatus();
    }

    private void OnEnable()
    {
        /// Subscribe to dettector events
        _belowMatchableDettector.OnTriggerExit2D_Action += OnBelowTriggerExit;
        _allSurroundsDettector.OnTriggerStay2D_Action += OnSurroundsTriggerStay;
        ResetMatchable();
    }

    private void OnDisable()
    {
        /// UnSubscribe to dettector events
        _belowMatchableDettector.OnTriggerExit2D_Action -= OnBelowTriggerExit;
        _allSurroundsDettector.OnTriggerStay2D_Action -= OnSurroundsTriggerStay;
    }
    #endregion



    #region Public Methods

    /// <summary>
    /// Sets the Matchable properties and display settings
    /// </summary>
    public void SetMatchablePreset(MatchablePreset matchable)
    {
        _matchableNameID = matchable.matchableNameID;
        _matchableSprite.sprite = matchable.matchableSprite;
        _matchableSprite.color = matchable.matchableColor;
        _points = matchable.points;
    }


    public void PopMatchable()
    {
        GameEvents.Instance.OnMatchablePop(this);
        ResetMatchable();
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Restarts all variables for pooling purposes
    /// </summary>
    void ResetMatchable()
    {
        _mStatus = matchableStatus.firstFall;
        _joint.enabled = false;
        _surroundMatchables = new HashSet<MatchableController>();
        _positionedThisSpawn = false;
        EnableSurroundDettectors(false);
    }


    /// Called when Matchable finds its position
    void FixMatchablePosition()
    {
        _mStatus = matchableStatus.onPosition;
        _joint.enabled = true;
        if (!_positionedThisSpawn)
            GameEvents.Instance.OnMatchableFindsPosition();
        _positionedThisSpawn = true;
    }

    /// Called when matchable leaves from its previous fixed position (due to below matchable movements)
    void UnFixMatchablePosition()
    {
        _mStatus = matchableStatus.secondFall;
        _joint.enabled = false;
        _rb.velocity = new Vector2(0, -1f);
    }

    /// Sets Matchable status according to Physics related conditions
    void SetMatchableStatus()
    {
        switch (_mStatus)
        {
            case matchableStatus.firstFall:
                if (_rb.velocity.y > 0)
                    _mStatus = matchableStatus.firstBounce;
                break;
            case matchableStatus.firstBounce:
                if (_rb.velocity.y < 0)
                    _mStatus = matchableStatus.secondFall;
                break;
            case matchableStatus.secondFall:
                if (_rb.velocity.y > 0)
                    _mStatus = matchableStatus.secondBounce;
                break;
            default:
                break;
        }
    }


    #endregion


    #region Collider Methods

    /// Triggers with grid cells, used to set Matchables on position after the second bounce
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (_mStatus == matchableStatus.secondBounce)
        {
            if (collider.gameObject.layer == GameManager.GRID_LAYER)
            {
                transform.position = new Vector3(collider.transform.position.x, collider.transform.position.y);
                FixMatchablePosition();
            }
        }        
    }


    /// Called When Below dettectors dettects movement (due to pop or moves from below matchable)
    void OnBelowTriggerExit(Collider2D collider)
    {
        UnFixMatchablePosition();
    }


    /// Dettects surrounds Matchables and adds them to the HashSet, when Hashset is completed, it stops dettectors for optimization purposes
    void OnSurroundsTriggerStay(Collider2D collider)
    {
        MatchableController matchable = collider.GetComponent<MatchableController>();
        if(!_surroundMatchables.Add(matchable))
            EnableSurroundDettectors(false);
    }

    /// <summary>
    /// Enables or disables the surround dettectors triggers (for Bot, Top, Left and Right Matchables)
    /// </summary>
    public void EnableSurroundDettectors(bool var)
    {
        if(var)
            _surroundMatchables = new HashSet<MatchableController>();
        _allSurroundsDettector.gameObject.SetActive(var);
    }

    /// <summary>
    /// Returns matched surround matchables
    /// </summary>
    public Stack<MatchableController> GetSurroundMatches()
    {
        Stack<MatchableController> matches = new Stack<MatchableController>();
        foreach (MatchableController matchable in _surroundMatchables)
            if (_matchableNameID.Equals(matchable.MatchableNameID))
                matches.Push(matchable);

        return matches;
    }


    /// When User taps the Matchable
    private void OnMouseDown()
    {
        if (GameManager.CanUserInteract)
            GameEvents.Instance.OnUserTouchMatchable(this);
    }

    #endregion
}

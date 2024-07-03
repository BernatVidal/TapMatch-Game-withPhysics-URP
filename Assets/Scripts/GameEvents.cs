using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
	#region Fields

	public static GameEvents Instance;
	public event Action<MatchableController> onUserTouchMatchable;
	public event Action<MatchableController> onMatchablePop;
	public event Action onMatchableInPosition;
	public event Action onPositionedMatchableMoves;
	public event Action onAllMatchablesOnPosition;

	#endregion



	#region Unity Methods

	void Awake()
	{
		Instance = this;
	}
	#endregion



	#region Public Methods
	/// Triggered when user touches a Matchable
	public void OnUserTouchMatchable(MatchableController matchable)
    {
		onUserTouchMatchable?.Invoke(matchable);
    }

	/// Triggered when a Matchables Pop
	public void OnMatchablePop(MatchableController matchable)
	{
		onMatchablePop?.Invoke(matchable);
	}

	/// Triggered when a Matchables stablish it's new position
	public void OnMatchableFindsPosition()
    {
		onMatchableInPosition?.Invoke();
	}

	/// Triggered when a Matchable with stablished position moves (due to pop below it)
	public void OnPositionedMatchableMoves()
    {
		onPositionedMatchableMoves?.Invoke();
	}


	///  Triggered when all Matchables have a stablished Position
	public void OnAllMatchablesOnPosition()
	{
		onAllMatchablesOnPosition?.Invoke();
	}

	#endregion
}

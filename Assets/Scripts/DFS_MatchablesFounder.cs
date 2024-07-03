using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DFS_MatchablesFounder
{
    /// <summary>
    /// Returns a HashSet with the connected Matches of the one sended
    /// </summary>
	public static HashSet<MatchableController> GetSelectedSurroundMatches(MatchableController selected)
	{
		HashSet<MatchableController> matchedTiles = new();
		return GetMatches_DFS(selected, matchedTiles);
	}


    /// <summary>
    /// Recursive Depth First Search of same Matches
    /// </summary>
	static HashSet<MatchableController> GetMatches_DFS(MatchableController current, HashSet<MatchableController> historicMatches)
    {
        historicMatches.Add(current);        
        foreach (MatchableController matchable in current.GetSurroundMatches())
        {
            if (!historicMatches.Contains(matchable))
            {
                foreach (MatchableController match in GetMatches_DFS(matchable, historicMatches))
                {
                    historicMatches.Add(match);
                }
            }
        }
        
        return historicMatches;
    }


}

using UnityEngine;

[CreateAssetMenu (fileName ="New Matchable", menuName ="Matchable")]
public class MatchablePreset : ScriptableObject
{
    public string matchableNameID;
    public Sprite matchableSprite;
    public Color matchableColor;
    public int points = 1;
}

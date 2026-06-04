using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile;
[CreateAssetMenu(fileName = "New Custom Tile", menuName = "2D/Tiles/Sibling Rule Tile")]
//https://gamedev.stackexchange.com/questions/193974/how-to-make-different-rule-tiles-interact-with-each-other
public class SiblingRuleTile : RuleTile
{

    public enum SiblingGroup
    {
        Sand,
        Grass,
    }
    public SiblingGroup siblingGroup;

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        switch (neighbor)
        {
            case TilingRule.Neighbor.This:
                {
                    return other is SiblingRuleTile
                        && (other as SiblingRuleTile).siblingGroup == this.siblingGroup;
                }
            case TilingRule.Neighbor.NotThis:
                {
                    return !(other is SiblingRuleTile
                        && (other as SiblingRuleTile).siblingGroup == this.siblingGroup);
                }
        }

        return base.RuleMatch(neighbor, other);
    }
}
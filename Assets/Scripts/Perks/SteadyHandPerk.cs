using UnityEngine;

[CreateAssetMenu(fileName = "SteadyHandPerk", menuName = "Rogue Dice/Perks/SteadyHand", order = 11)]
public class SteadyHandPerk : Perk
{
    // Steady Hand: If your rolls are within 1 point of each other, gain 3 bonus points for that roll.
    public override int ModifyScore(int rawRollScore, int[] individualDiceResults)
    {
        if (individualDiceResults.Length == 2 && Mathf.Abs(individualDiceResults[0] - individualDiceResults[1]) == 1)
        {
            Debug.Log("[SteadyHandPerk] Dice within 1 point! +3 bonus");
            return rawRollScore + 3;
        }
        return rawRollScore;
    }
}

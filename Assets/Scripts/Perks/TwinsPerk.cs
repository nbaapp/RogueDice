using UnityEngine;

[CreateAssetMenu(fileName = "TwinsPerk", menuName = "Rogue Dice/Perks/Twins", order = 10)]
public class TwinsPerk : Perk
{
    // Twins: doubles are worth twice as much points (add the value of both dice again if they match)
    public override int ModifyScore(int rawRollScore, int[] individualDiceResults)
    {
        if (individualDiceResults.Length == 2 && individualDiceResults[0] == individualDiceResults[1])
        {
            int bonus = individualDiceResults[0] + individualDiceResults[1];
            Debug.Log($"[TwinsPerk] Doubles rolled! Bonus: {bonus}");
            return rawRollScore + bonus;
        }
        return rawRollScore;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Lucky7Perk", menuName = "Rogue Dice/Perks/Lucky7", order = 12)]
public class Lucky7Perk : Perk
{
    // Lucky 7: If the dice add to 7, double your score for that roll.
    public override int ModifyScore(int rawRollScore, int[] individualDiceResults)
    {
        if (individualDiceResults.Length == 2 && (individualDiceResults[0] + individualDiceResults[1]) == 7)
        {
            Debug.Log("[Lucky7Perk] Lucky 7! Score doubled.");
            return rawRollScore * 2;
        }
        return rawRollScore;
    }
}

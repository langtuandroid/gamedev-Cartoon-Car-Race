using UnityEngine;

[CreateAssetMenu(fileName = "RaceRewardConfig", menuName = "Configs/RaceRewardConfig")]
public class RaceRewardConfig : ScriptableObject
{
    [SerializeField] private int[] raceRewards;

    public int GetReward(int place)
    {
        return place < raceRewards.Length ? raceRewards[place] : 0;
    }
}

using UnityEngine;

public class GameData
{
    public float PlayTime { get; set; }
    public int AmountTime { get; set; }

    public GameData(float playTime, int amountTime)
    {
        PlayTime = playTime;
        AmountTime = amountTime;
    }
}

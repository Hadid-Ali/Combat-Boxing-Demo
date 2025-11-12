using System.Collections.Generic;

[System.Serializable]
public class PlayerScoreObject
{
    public int UserID;
    public int Score;
    public int SelectedCard;
    
    public List<int> WinningHandsIndexes;

    public PlayerScoreObject(int userID, int selectedCard)
    {
        UserID = userID;
        SelectedCard = selectedCard;
        Score = 0;
        WinningHandsIndexes = new();
    }

    public void AddScore(int score, int handIndex)
    {
        Score += score;
        WinningHandsIndexes.Add(handIndex);
    }
}

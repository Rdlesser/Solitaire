public class Statistics
{
    public int Moves { get; private set; }
    public float TimePlayed { get; private set; }
    public int Score { get; private set; }

    public void IncrementMoves() { Moves++; }
    public void AddTime(float timeToAdd) { TimePlayed += timeToAdd; }
    public void UpdateScore(int points) { Score += points; }
    public void Reset() { Moves = 0; TimePlayed = 0; Score = 0; }
}
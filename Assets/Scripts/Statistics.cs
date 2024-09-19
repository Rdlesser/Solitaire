using System;

public class Statistics
{
    public int Moves { get; private set; }
    public float TimePlayed
    {
        get
        {
            if (EndTime < StartTime)
            {
                return (float)(DateTime.UtcNow - StartTime).TotalSeconds;
            }

            return (float) (EndTime - StartTime).TotalSeconds;
        }
    }
    public int CardsDrawn { get; private set; }

    public int Undos { get; private set; }
    public DateTime StartTime { get; private set; } = DateTime.MinValue;
    public DateTime EndTime { get; private set; } = DateTime.MinValue;

    public void IncrementMoves() { Moves++; }
    public void IncrementUndos() {Undos++;}

    public void IncrementCardsDrawn()
    {
        CardsDrawn++;
    }

    public void StartTimer()
    {
        StartTime = DateTime.UtcNow;
    }

    public void EndTimer()
    {
        EndTime = DateTime.UtcNow;
    }

    public void Reset() { Moves = 0; StartTime = DateTime.MinValue; EndTime = DateTime.MinValue;}
}
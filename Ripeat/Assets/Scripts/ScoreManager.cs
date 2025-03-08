using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Point;

    public void AddPoints(int amount)
    {
        Point += amount;
        Debug.Log("Punti totali: " + Point);
        ControlWin(Point);
    }

    public void ControlWin(int Point)
    {
        if (Point >= 10)
        {
            Debug.Log("Hai vinto!");
        }
    }
}

using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Point;

    public void AddPoints(int amount)
    {
        Point += amount;
        Debug.Log("Punti totali: " + Point);
    }
}

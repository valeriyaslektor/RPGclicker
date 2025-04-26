// Assets/Scripts/TimeCheat.cs
using UnityEngine;

public class TimeCheat : MonoBehaviour
{
    public GameManager gameManager;
    public void AddHour()    => gameManager.AddHour();
    public void AdvanceDay() => gameManager.AdvanceDayOnly();
}












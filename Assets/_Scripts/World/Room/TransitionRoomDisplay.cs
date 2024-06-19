using TMPro;
using UnityEngine;

public class TransitionRoomDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomsClearedText;
    [SerializeField] private TMP_Text _enemiesKilledText;
    [SerializeField] private TMP_Text _shotsFired;

    public void DisplayStatistics()
    {
        _roomsClearedText.text = $"Rooms Cleared: {StatisticManager.Instance.GetStatistics().RoomsCleared}";
        _enemiesKilledText.text = $"Enemies Killed: {StatisticManager.Instance.GetStatistics().EnemiesKilled}";
        _shotsFired.text = $"Shots Fired: {StatisticManager.Instance.GetStatistics().ShotsFired}";
    }
}

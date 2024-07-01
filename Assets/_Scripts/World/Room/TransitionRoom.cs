using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransitionRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomsClearedText;
    [SerializeField] private TMP_Text _enemiesKilledText;
    [SerializeField] private TMP_Text _shotsFired;
    [SerializeField] private TMP_Text _highestStreak;

    [SerializeField] private GameObject _downEntrance;
    [SerializeField] private GameObject _leftEntrance;
    [SerializeField] private GameObject _rightEntrance;

    private Dictionary<LevelController.Direction, GameObject> _entrances = new Dictionary<LevelController.Direction, GameObject>();

    private void Awake()
    {
        _entrances.Add(LevelController.Direction.Down, _downEntrance);
        _entrances.Add(LevelController.Direction.Left, _leftEntrance);
        _entrances.Add(LevelController.Direction.Right, _rightEntrance);
        LevelAttributes levelAttributes = GetComponent<LevelAttributes>();
       
        _entrances[ levelAttributes.GetEntranceDirection()].GetComponent<EntranceTrigger>().Setup(levelAttributes.GetEntranceDirection());
    }

    public GameObject GetDoor(LevelController.Direction direction)
    {
        return _entrances[direction];
    }

    public void DisplayStatistics()
    {
        _roomsClearedText.text = $"Rooms Cleared: {StatisticManager.Instance.GetStatistics().RoomsCleared}";
        _enemiesKilledText.text = $"Enemies Killed: {StatisticManager.Instance.GetStatistics().EnemiesKilled}";
        _shotsFired.text = $"Shots Fired: {StatisticManager.Instance.GetStatistics().ShotsFired}";
        _highestStreak.text = $"Highest Streak: {StatisticManager.Instance.GetStatistics().HighestStreak}";
    }
}

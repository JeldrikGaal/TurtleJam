using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransitionRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text _shotAccuracy;
    [SerializeField] private TMP_Text _highestStreak;
    [SerializeField] private TMP_Text _bounceKillAmount;

    private List<GameObject> _textObjects = new List<GameObject>();
    
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
        
        _textObjects.AddRange(new List<GameObject> { _shotAccuracy.gameObject, _highestStreak.gameObject, _bounceKillAmount.gameObject});
       
        _entrances[ levelAttributes.GetEntranceDirection()].GetComponent<EntranceTrigger>().Setup(levelAttributes.GetEntranceDirection());
    }

    public GameObject GetDoor(LevelController.Direction direction)
    {
        return _entrances[direction];
    }
    
    /*public List<float> GetTransitionRoomStatistics()
    {
        StatisticManager.Statistics totalStats = StatisticManager.Instance.GetStatistics();
        StatisticManager.Statistics stageStartStats = StatisticManager.Instance.GetCurrentStageStatistics();

        statistics.Add((totalStats.GetShotAccuracy() * 100));
        statistics.Add(totalStats.HighestStreak);
        statistics.Add(totalStats.BounceKills);
        return statistics;
    }*/

    public void DisplayStatistics()
    {
        StatisticManager.Statistics totalStats = StatisticManager.Instance.GetStatistics();
        _shotAccuracy.text = "Shot Accuracy: " + (totalStats.GetShotAccuracy() * 100).ToString("0.00") + " %";
        _highestStreak.text = $"Highest Streak: {totalStats.HighestStreak}";
        _bounceKillAmount.text = $"Bounce Kills: {totalStats.BounceKills}";

        Invoke(nameof(SpawnMedals), 0.5f);
    }
    
    private void SpawnMedals()
    {
        var position = _shotAccuracy.transform.position + ( Vector3.right * 1f);
        float distance = position.y - _highestStreak.transform.position.y;   
        StartCoroutine(MedalProvider.Instance.SpawnMedalList(position, 0.35f, distance, _textObjects));
    }
}

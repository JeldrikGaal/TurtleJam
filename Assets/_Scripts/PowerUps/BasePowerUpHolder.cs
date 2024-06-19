using UnityEngine;

public class BasePowerUpHolder : ScriptableObject
{
    public float InitialDuration = 10;
    public float DurationAddedEnemyKill = 2;
    public float DurationAddedLevelCompleted = 1;
    public string TutorialText;
    public float TutorialTextDuration = 3;
    public string DisplayName;
}

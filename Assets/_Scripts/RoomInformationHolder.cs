using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomInformationHolder", menuName = "Custom/RoomInformationHolder")]
public class DirectionsHolder : ScriptableObject
{
    public List<LevelController.Direction> Directions;
}



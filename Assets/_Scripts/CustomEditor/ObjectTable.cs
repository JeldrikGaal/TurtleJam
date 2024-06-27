using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectTable
{
    [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector, PreviewField]
    private List<GameObject> _allRooms;

    [PreviewField, ShowInInspector]
    private GameObject _oneRoom;

    private static string _path;


    public ObjectTable(string path)
    {
        _path = path;
        _allRooms = FetchAllObjects();
        _oneRoom = _allRooms[0];
    }
    private static List<GameObject> FetchAllObjects()
    {
        return Resources.LoadAll<GameObject>(_path).ToList();
    }

    public void UpdateObjects()
    {
        _allRooms = FetchAllObjects();
    }
}
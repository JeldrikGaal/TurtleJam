#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class LevelEditor : OdinMenuEditorWindow
{
    [MenuItem("Tools/Romanesco/LevelEditor")]
    private static void Open()
    {
        var window = GetWindow<LevelEditor>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(true);
            
        tree.Add("Rooms" , new ObjectTable("Rooms"));
        
            
        return tree;
    }
}
#endif
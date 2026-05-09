#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathBuilder : EditorWindow
{
    WaypointPath _target;

    [MenuItem("TowerDefence/Bake Path Waypoints")]
    static void Open() => GetWindow<PathBuilder>("Path Baker");

    void OnGUI()
    {
        _target = (WaypointPath)EditorGUILayout.ObjectField("WaypointPath", _target, typeof(WaypointPath), true);

        if (GUILayout.Button("Auto-collect child transforms as waypoints"))
            BakeWaypoints();
    }

    void BakeWaypoints()
    {
        if (_target == null)
        {
            EditorUtility.DisplayDialog("Error", "Assign a WaypointPath first", "OK");
            return;
        }

        var children = new List<Transform>();
        foreach (Transform child in _target.transform)
            children.Add(child);

        children.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        _target.points = children.ToArray();

        EditorUtility.SetDirty(_target);
        Debug.Log($"Baked {children.Count} waypoints into WaypointPath.");
    }
}
#endif

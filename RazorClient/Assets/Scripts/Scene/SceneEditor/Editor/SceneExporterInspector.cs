using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(SceneExporter))]
public class SceneExporterInspector : Editor
{
    SceneExporter sceneExporter => target as SceneExporter;

    private Grid _grid;

    private Grid grid
    {
        get
        {
            if (_grid == null)
                _grid = sceneExporter.GetComponent<Grid>();
            return _grid;
        }
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("保存场景"))
        {
            SceneUtils.saveMap(grid);
        }

        if (GUILayout.Button("导入场景Json"))
        {
            SceneUtils.loadMap(SceneManager.GetActiveScene().name);
        }
    }
}
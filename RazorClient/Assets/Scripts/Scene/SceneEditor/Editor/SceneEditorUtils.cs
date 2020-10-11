using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using XLua;

public class SceneEditorUtils
{

    [LuaCallCSharp]
    public static void saveMap(Grid grid)
    {
        var tileSceneJson = new TileSceneJson { sceneName = SceneManager.GetActiveScene().name };
        var tilemaps = grid.GetComponentsInChildren<Tilemap>();

        foreach (var tilemap in tilemaps)
        {
            var tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
            TilemapJson tilemapJson = new TilemapJson();
            tileSceneJson.tilemaps.Add(tilemapJson);
            tilemapJson.name = tilemap.name;
            tilemapJson.order = tilemapRenderer.sortingOrder;
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(pos);
                if (tile != null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(tile);
                    var assetBundleName = AssetImporter.GetAtPath(assetPath).assetBundleName;
                    var tileJson = new TileJson
                    { x = pos.x, y = pos.y, bundle = assetBundleName, asset = tile.name };
                    tilemapJson.tiles.Add(tileJson);
                }
            }
        }

        var jsonStr = JsonUtility.ToJson(tileSceneJson, true);
        PlayerPrefs.SetString($"scene_{tileSceneJson.sceneName}", jsonStr);
        Debug.Log(jsonStr);
    }
}

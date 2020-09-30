using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using XLua;

public class SceneUtils
{
    [LuaCallCSharp]
    public static void loadScene(String sceneName, Action done)
    {
        Boot.inst.StartCoroutine(LoadSceneCoroutine(sceneName, done));
    }

    private static IEnumerator LoadSceneCoroutine(string sceneName, Action done)
    {
        var oper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (oper.isDone == false)
        {
            yield return null;
        }

        done?.Invoke();
    }

    public static void unloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    [LuaCallCSharp]
    public static void listenSceneUnload(Action<string> action)
    {
        SceneManager.sceneUnloaded += scene => action?.Invoke(scene.name);
    }

    [LuaCallCSharp]
    public static void listenSceneLoaded(Action<string> action)
    {
        SceneManager.sceneLoaded += (scene, loadMode) => action?.Invoke(scene.name);
    }

    [LuaCallCSharp]
    public static void saveMap(Grid grid)
    {
        var tileSceneJson = new TileSceneJson {sceneName = SceneManager.GetActiveScene().name};
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
                        {x = pos.x, y = pos.y, bundle = assetBundleName, asset = tile.name};
                    tilemapJson.tiles.Add(tileJson);
                }
            }
        }

        var jsonStr = JsonUtility.ToJson(tileSceneJson, true);
        PlayerPrefs.SetString($"scene_{tileSceneJson.sceneName}", jsonStr);
        Debug.Log(jsonStr);
    }

    [LuaCallCSharp]
    public static void loadMap(string sceneName)
    {
        var jsonStr = PlayerPrefs.GetString($"scene_{sceneName}", null);
        if (jsonStr == null)
            return;
        TileSceneJson tileSceneJson = JsonUtility.FromJson<TileSceneJson>(jsonStr);
        if (tileSceneJson == null)
            return;
        GameObject gridGo = new GameObject("TileGrid");
        var grid = gridGo.AddComponent<Grid>();
        grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;

        foreach (TilemapJson tilemapJson in tileSceneJson.tilemaps)
        {
            GameObject tilemapGo = new GameObject(tilemapJson.name);
            tilemapGo.transform.SetParent(gridGo.transform);
            var tilemap = tilemapGo.AddComponent<Tilemap>();
            var renderer = tilemapGo.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = tilemapJson.order;

            List<Vector3Int> posList = new List<Vector3Int>(tilemapJson.tiles.Count);
            List<TileBase> tiles = new List<TileBase>(tilemapJson.tiles.Count);

            foreach (var tilemapJsonTile in tilemapJson.tiles)
            {
                posList.Add(new Vector3Int(tilemapJsonTile.x, tilemapJsonTile.y, 0));
            }

            tilemap.SetTiles(posList.ToArray(), tiles.ToArray());
        }
    }
}
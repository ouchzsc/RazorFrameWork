using System;
using System.Collections.Generic;

[Serializable]
public class TileSceneJson
{
    public string sceneName;
    public List<TilemapJson> tilemaps=new List<TilemapJson>();
}
using System;
using System.Collections.Generic;

[Serializable]
public class TilemapJson
{
    public string name;
    public int order;
    public List<TileJson> tiles=new List<TileJson>();
}
using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class VectorUtils
{
    public static void v2Normalized(float x,float y,out float ox,out float oy)
    {
        Vector2 vector2 = new Vector2(x, y);
        vector2.Normalize();
        ox = vector2.x;
        oy = vector2.y;
    }
}

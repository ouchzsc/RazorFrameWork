using UnityEngine;
using System.Collections;
using XLua;

[LuaCallCSharp]
public static class CameraUtils
{
    public static void getWorldPosFromScreen(float x, float y, float z, out float ox, out float oy, out float oz)
    {
        var wPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, z));
        ox = wPos.x;
        oy = wPos.y;
        oz = wPos.z;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class TransformUtils
{
    public static void movePosition(Transform transform, float deltaX, float deltaY, float deltaZ,out float ox, out float oy, out float oz)
    {
        var oldPos = transform.position;
        oldPos.x += deltaX;
        oldPos.y += deltaY;
        oldPos.z += deltaZ;
        transform.position = oldPos;
        ox = oldPos.x;
        oy = oldPos.y;
        oz = oldPos.z;
    }
}

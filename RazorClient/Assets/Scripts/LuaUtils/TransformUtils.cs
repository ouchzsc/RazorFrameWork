using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class TransformUtils
{
    public static void movePosition(Transform transform, float deltaX, float deltaY, float deltaZ)
    {
        var oldPos = transform.position;
        oldPos.x += deltaX;
        oldPos.y += deltaY;
        oldPos.z += deltaZ;
        transform.position = oldPos;
    }
}

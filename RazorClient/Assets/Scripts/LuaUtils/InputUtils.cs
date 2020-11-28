using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class InputUtils
{
    public static void getMousePosition(out float x, out float y)
    {
        var pos = Input.mousePosition;
        x = pos.x;
        y = pos.y;
    }
}

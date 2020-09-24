using UnityEngine;
using XLua;

public class Boot : MonoBehaviour
{
    private LuaTable luaMain;
    private LuaFunction luaStart;
    private LuaFunction luaUpdate;

    public static Boot inst;

    private void Awake()
    {
        inst = this;
        csModules.init();

        luaMain = csModules.luaManager.luaRequire("main");
        luaStart = luaMain.Get<LuaFunction>("onStart");
        luaUpdate = luaMain.Get<LuaFunction>("onUpdate");
    }

    void Start()
    {
        luaStart?.Call();
    }

    private void Update()
    {
        luaUpdate?.Call();
    }
}
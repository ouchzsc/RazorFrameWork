using System.IO;
using XLua;

public class LuaManager
{
    public string luaPath = "Lua";

    [CSharpCallLua]
    public delegate LuaTable LuaRequire(string moudle);

    public readonly LuaRequire luaRequire;

    public readonly LuaEnv LuaEnv;

    public LuaManager()
    {
        LuaEnv = new LuaEnv();
        LuaEnv.AddLoader(loader);
        luaRequire = LuaEnv.Global.GetInPath<LuaRequire>("require");
    }


    public byte[] loader(ref string filePath)
    {
        return File.ReadAllBytes(Path.Combine(luaPath, filePath.Replace('.', Path.DirectorySeparatorChar)) + ".lua");
    }
}
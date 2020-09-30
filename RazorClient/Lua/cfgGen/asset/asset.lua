local util = require("cfgGen.util")
local asset = {}

---@class asset.asset
local name2index = {
    assetPath = 1,
    bundleName = 2,
    assetName = 3,
    type = 4,
}

local name2ref = {}

local data = {
    ["Assets/Res/Boot/LuaBoot.prefab"] = { "Assets/Res/Boot/LuaBoot.prefab", "boot", "LuaBoot", ".prefab", },
    ["Assets/Res/Scene1/s1.unity"] = { "Assets/Res/Scene1/s1.unity", "s1", "s1", ".unity", },
    ["Assets/Res/TestGO.prefab"] = { "Assets/Res/TestGO.prefab", "testgo", "TestGO", ".prefab", },
    ["Assets/Res/Scene1/img/grass.asset"] = { "Assets/Res/Scene1/img/grass.asset", "tile", "grass", ".asset", },
    ["Assets/Res/Scene1/img/grass.png"] = { "Assets/Res/Scene1/img/grass.png", "tile", "grass", ".png", },
    ["Assets/Res/Scene1/img/ground.asset"] = { "Assets/Res/Scene1/img/ground.asset", "tile", "ground", ".asset", },
    ["Assets/Res/Scene1/img/ground.png"] = { "Assets/Res/Scene1/img/ground.png", "tile", "ground", ".png", },
    ["Assets/Res/Scene1/img/stone.asset"] = { "Assets/Res/Scene1/img/stone.asset", "tile", "stone", ".asset", },
    ["Assets/Res/Scene1/img/stone.png"] = { "Assets/Res/Scene1/img/stone.png", "tile", "stone", ".png", },
}

---@return asset.asset
function asset.get(id)
    return data[id]
end

util.setMetaGet(data, name2index, name2ref)

return asset
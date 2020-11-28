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
    ["Assets/Res/Common/bullet.prefab"] = { "Assets/Res/Common/bullet.prefab", "common", "bullet", ".prefab", },
    ["Assets/Res/Common/go_player.prefab"] = { "Assets/Res/Common/go_player.prefab", "common", "go_player", ".prefab", },
    ["Assets/Res/Scene1/s1.unity"] = { "Assets/Res/Scene1/s1.unity", "s1", "s1", ".unity", },
}

---@return asset.asset
function asset.get(id)
    return data[id]
end

util.setMetaGet(data, name2index, name2ref)

return asset
local module = require("module")
local resUtils = require("res.resUtils")
local ASyncGameObject = require("obj.ASyncGameObject")
local Player = require("player.Player")
local test = {}

function test.f5()
    --local s = resUtils.loadAssetByPath("Assets/Res/TestGO.prefab", function(asset, f)
    --    CS.UnityEngine.GameObject.Instantiate(asset)
    --    f()
    --end)
    local player = Player:new()
    player:setAssetInfo("Assets/Res/TestGO.prefab")
    player:show()

end

function test.f6()
    package.loaded["player.PlayerInfo"] = nil
    --goObj2 = ASyncGameObject:new()
    --goObj2:setAssetInfo("Assets/Res/TestGO.prefab")
    --goObj2:show()
    --module.sceneMgr.switch("s1", "s1")
end

function test.f7()
    local t = { a = 1, b = 2, "a", "b", "c", "d" }
    for k, v in pairs(t) do
        print(k, v)
    end
    print("clear")
    for k, v in pairs(t) do
        print(k)
        t[k] = nil
    end
    print("left")
    for k, v in pairs(t) do
        print(k, v)
    end

end

function test.f8()
    goObj2:hide()
    --resUtils.dump()
end

return test
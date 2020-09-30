local module = require("module")
local resUtils = require("res.resUtils")
local test = {}

function test.f5()
    resUtils.loadAssetByPath("Assets/Res/TestGO.prefab", function(asset)
        CS.UnityEngine.GameObject.Instantiate(asset)
    end)
end

function test.f6()
    module.sceneMgr.switch("s1", "s1")
end

function test.f7()

end

function test.f8()
    resUtils.dump()
end

return test
local assetMgr = CS.Res.AssetMgr.Instance
local bundleMgr = CS.Res.BundleMgr.Instance
local bundleDepMgr = CS.Res.BundleDepMgr.Instance

local test = {}

function test.f5()
    assetMgr:loadAsset("cube", "Cube", function(asset)
        CS.UnityEngine.GameObject.Instantiate(asset)
    end)
end

function test.f6()

end

function test.f7()

end

function test.f8()
    bundleMgr:dump()
end

return test